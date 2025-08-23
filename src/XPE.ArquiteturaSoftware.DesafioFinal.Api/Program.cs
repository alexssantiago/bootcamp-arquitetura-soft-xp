using XPE.ArquiteturaSoftware.DesafioFinal.Infra.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "Bootcamp: Arquiteto(a) de Software — Desafio Final";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.DisplayRequestDuration();
        c.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();