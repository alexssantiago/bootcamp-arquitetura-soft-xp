# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY XPE.ArquiteturaSoftware.DesafioFinal.Api/*.csproj ./XPE.ArquiteturaSoftware.DesafioFinal.Api/
COPY XPE.ArquiteturaSoftware.DesafioFinal.Application/*.csproj ./XPE.ArquiteturaSoftware.DesafioFinal.Application/

RUN dotnet restore ./XPE.ArquiteturaSoftware.DesafioFinal.Api/XPE.ArquiteturaSoftware.DesafioFinal.Api.csproj

COPY . .

WORKDIR /src/XPE.ArquiteturaSoftware.DesafioFinal.Api
RUN dotnet publish -c Release -o /app /p:UseAppHost=false

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "XPE.ArquiteturaSoftware.DesafioFinal.Api.dll"]