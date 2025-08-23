namespace XPE.ArquiteturaSoftware.DesafioFinal.Tests.Models;

public sealed class ProductTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Ctor_With_Arguments_Should_Set_Properties()
    {
        // arrange
        var name = _faker.Commerce.ProductName();
        var desc = _faker.Commerce.ProductDescription();
        var price = _faker.Random.Decimal(0.01m, 10_000m);

        var before = DateTime.Now;

        // act
        var product = new Product(name, desc, price);

        // assert
        product.Id.Should().Be(0);
        product.Name.Should().Be(name);
        product.Description.Should().Be(desc);
        product.Price.Should().Be(price);
        product.Active.Should().BeFalse();
        product.UpdatedAt.Should().BeNull();

        product.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.Now);
    }

    [Fact]
    public void Parameterless_Ctor_Should_Exist_And_Set_Defaults()
    {
        // act
        var product = new Product();

        // assert
        product.Should().NotBeNull();
        product.Id.Should().Be(0);
        product.Name.Should().BeEmpty();
        product.Description.Should().BeEmpty();
        product.Price.Should().Be(0m);
        product.Active.Should().BeFalse();
        product.UpdatedAt.Should().BeNull();

        product.CreatedAt.Should().BeOnOrBefore(DateTime.Now);
    }

    [Fact]
    public async Task SetActive_Default_True_Should_Set_Active_And_Update_UpdatedAt()
    {
        var product = new Product(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), 123.45m);

        product.Active.Should().BeFalse();
        product.UpdatedAt.Should().BeNull();

        await Task.Delay(10);

        product.SetActive();

        product.Active.Should().BeTrue();
        product.UpdatedAt.Should().NotBeNull();

        product.UpdatedAt!.Value.Should().BeOnOrAfter(product.CreatedAt);
    }

    [Fact]
    public async Task SetActive_False_Should_Clear_Active_And_Update_UpdatedAt()
    {
        var product = new Product(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), 50m);
        product.SetActive(true);

        var prevUpdatedAt = product.UpdatedAt;
        product.Active.Should().BeTrue();

        await Task.Delay(10);

        product.SetActive(false);

        product.Active.Should().BeFalse();
        product.UpdatedAt.Should().NotBeNull();
        product.UpdatedAt.Should().NotBe(prevUpdatedAt);
    }

    [Fact]
    public void CreatedAt_Should_Not_Change_When_SetActive_Is_Called()
    {
        var product = new Product(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), 10m);
        var createdAt = product.CreatedAt;

        product.SetActive(true);
        product.SetActive(false);

        product.CreatedAt.Should().Be(createdAt);
    }
}