using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Caching;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Services;
using XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Tests.Extensions;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Tests.Services;

public sealed class ProductServiceTests
{
    private readonly Mock<ILogger<ProductService>> _logger = new();
    private readonly Mock<IProductRepository> _repository = new();
    private readonly Mock<IDistributedCache> _cache = new();
    private readonly Faker _faker = new();

    private ProductService CreateService() => new(_logger.Object, _repository.Object, _cache.Object);

    #region Create

    [Fact]
    public async Task CreateAsync_Should_Return_Success_And_Bump_Version()
    {
        // Arrange
        var service = CreateService();
        var request = new CreateProductRequest(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), _faker.Random.Decimal(1, 999));

        _repository.Setup(mock => mock.CreateAsync(It.IsAny<Product>())).ReturnsAsync(123);

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));
        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(123);

        _cache.Verify(mock => mock.SetAsync("products:version",
            It.IsAny<byte[]>(),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow.HasValue),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Invalid_Id()
    {
        // Arrange
        var service = CreateService();
        var request = new CreateProductRequest(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), 0.1m);

        _repository.Setup(mock => mock.CreateAsync(It.IsAny<Product>())).ReturnsAsync(0);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("invalid id");
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_On_Exception()
    {
        var service = CreateService();
        var request = new CreateProductRequest("p", "d", 10);

        _repository.Setup(mock => mock.CreateAsync(It.IsAny<Product>())).ThrowsAsync(new Exception("boom"));

        var result = await service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Unexpected error");
    }

    #endregion

    #region GetById

    [Fact]
    public async Task GetByIdAsync_Should_Return_From_Cache_When_Present()
    {
        // Arrange
        var service = CreateService();
        const int id = 10;

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var key = ProductCache.KeyProductById("v1", id);

        var cached = new ProductResponse(id, "cached", "desc", 9.9m, true, DateTime.UtcNow, null);
        _cache.Setup(mock => mock.GetAsync(key, It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.ToBytes(cached));

        // Act
        var result = await service.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("cached");
        _repository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Fetch_And_Cache_When_Miss()
    {
        var service = CreateService();
        const int id = 11;

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var key = ProductCache.KeyProductById("v1", id);

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var entity = new Product("name", "desc", 12.3m);
        entity.SetActive(true);

        _repository.Setup(mock => mock.GetByIdAsync(id)).ReturnsAsync(entity);

        var result = await service.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("name");

        _cache.Verify(mock => mock.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow.HasValue),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_NotFound_When_Repo_Returns_Null()
    {
        // Arrange
        var service = CreateService();
        const int id = 999;

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var key = ProductCache.KeyProductById("v1", id);

        _cache.Setup(mock => mock.GetAsync(key, It.IsAny<CancellationToken>()))
              .ReturnsAsync((byte[]?)null);

        _repository.Setup(mock => mock.GetByIdAsync(id)).ReturnsAsync((Product?)null);

        // Act
        var result = await service.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Not found");
    }

    #endregion

    #region GetAll / FindByName / Count

    [Fact]
    public async Task GetAllAsync_Should_Return_From_Cache_When_Present()
    {
        // arrange
        var service = CreateService();

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var key = ProductCache.KeyAll("v1");

        var list = new[]
        {
            new ProductResponse(1, "A", "d", 1m,  true,  DateTime.UtcNow, null),
            new ProductResponse(2, "B", "d", 2m,  false, DateTime.UtcNow, null),
        };

        _cache.Setup(mock => mock.GetAsync(key, It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.ToBytes<IEnumerable<ProductResponse>>(list));

        // act
        var result = await service.GetAllAsync();

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2).And.ContainSingle(p => p.Id == 1);
        _repository.Verify(mock => mock.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_Should_Fetch_And_Cache_When_Miss()
    {
        var service = CreateService();

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var key = ProductCache.KeyAll("v1");

        _cache.Setup(mock => mock.GetAsync(key, It.IsAny<CancellationToken>()))
              .ReturnsAsync((byte[]?)null);

        var entityList = new[] { new Product("n", "d", 1.2m) };
        _repository.Setup(mock => mock.GetAllAsync()).ReturnsAsync(entityList);

        var result = await service.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);

        _cache.Verify(mock => mock.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task FindByNameAsync_Should_Return_From_Cache_When_Present()
    {
        var service = CreateService();
        const string name = "abc";

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var key = ProductCache.KeyByName("v1", name);

        var list = new[] { new ProductResponse(7, "abc", "d", 1, true, DateTime.UtcNow, null) };
        _cache.Setup(mock => mock.GetAsync(key, It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.ToBytes<IEnumerable<ProductResponse>>(list));

        var result = await service.FindByNameAsync(name);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle(x => x.Name == "abc");
        _repository.Verify(mock => mock.FindByNameAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task FindByNameAsync_Should_Fetch_And_Cache_When_Miss()
    {
        var service = CreateService();
        const string name = "xyz";

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var key = ProductCache.KeyByName("v1", name);

        _cache.Setup(mock => mock.GetAsync(key, It.IsAny<CancellationToken>()))
              .ReturnsAsync((byte[]?)null);

        var entityList = new[] { new Product("xyz", "d", 2.3m) };
        _repository.Setup(mock => mock.FindByNameAsync(name)).ReturnsAsync(entityList);

        var result = await service.FindByNameAsync(name);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle(x => x.Name == "xyz");

        _cache.Verify(mock => mock.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CountAsync_Should_Return_From_Cache_When_Present()
    {
        var service = CreateService();

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var key = ProductCache.KeyCount("v1");

        _cache.Setup(mock => mock.GetAsync(key, It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.ToBytes<int?>(42));

        var result = await service.CountAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        _repository.Verify(mock => mock.CountAsync(), Times.Never);
    }

    [Fact]
    public async Task CountAsync_Should_Fetch_And_Cache_When_Miss()
    {
        var service = CreateService();

        _cache.Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v1"));

        var key = ProductCache.KeyCount("v1");

        _cache.Setup(mock => mock.GetAsync(key, It.IsAny<CancellationToken>()))
              .ReturnsAsync((byte[]?)null);

        _repository.Setup(mock => mock.CountAsync()).ReturnsAsync(7);

        var result = await service.CountAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(7);

        _cache.Verify(mock => mock.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Update / Delete

    [Fact]
    public async Task UpdateAsync_Should_Bump_Version_And_Remove_ItemCache_On_Success()
    {
        var service = CreateService();
        const int id = 5;
        var request = new UpdateProductRequest("n", "d", 1.1m, true);

        _repository.Setup(mock => mock.UpdateAsync(id, It.IsAny<Product>())).ReturnsAsync(true);

        var seq = new MockSequence();
        _cache.InSequence(seq)
              .Setup(mock => mock.SetAsync("products:version",
                                     It.IsAny<byte[]>(),
                                     It.IsAny<DistributedCacheEntryOptions>(),
                                     It.IsAny<CancellationToken>()))
              .Returns(Task.CompletedTask);
        _cache.InSequence(seq)
              .Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v2"));

        var result = await service.UpdateAsync(id, request);

        result.IsSuccess.Should().BeTrue();

        _cache.Verify(mock => mock.RemoveAsync(
            It.Is<string>(k => k.Contains($":id:{id}")),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_NotFound_When_Repo_False()
    {
        var service = CreateService();
        const int id = 404;
        var request = new UpdateProductRequest("n", "d", 1.1m, true);

        _repository.Setup(mock => mock.UpdateAsync(id, It.IsAny<Product>())).ReturnsAsync(false);

        var result = await service.UpdateAsync(id, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Not found");
    }

    [Fact]
    public async Task DeleteAsync_Should_Bump_Version_And_Remove_ItemCache_On_Success()
    {
        var service = CreateService();
        const int id = 6;

        _repository.Setup(mock => mock.DeleteAsync(id)).ReturnsAsync(true);

        var seq = new MockSequence();
        _cache.InSequence(seq)
              .Setup(mock => mock.SetAsync("products:version",
                                     It.IsAny<byte[]>(),
                                     It.IsAny<DistributedCacheEntryOptions>(),
                                     It.IsAny<CancellationToken>()))
              .Returns(Task.CompletedTask);
        _cache.InSequence(seq)
              .Setup(mock => mock.GetAsync("products:version", It.IsAny<CancellationToken>()))
              .ReturnsAsync(CacheExtensions.Bytes("v2"));

        var result = await service.DeleteAsync(id);

        result.IsSuccess.Should().BeTrue();

        _cache.Verify(mock => mock.RemoveAsync(
            It.Is<string>(k => k.Contains($":id:{id}")),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_NotFound_When_Repo_False()
    {
        var service = CreateService();
        const int id = 404;

        _repository.Setup(mock => mock.DeleteAsync(id)).ReturnsAsync(false);

        var result = await service.DeleteAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Not found");
    }

    #endregion
}