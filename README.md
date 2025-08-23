# XPE Bootcamp: Arquiteto(a) de Software — Desafio Final  
**Autor:** Alexsander Santiago Sillva Alves  

Projeto desenvolvido para fins acadêmicos no Bootcamp de Arquitetura de Software.  

---

## 🎯 Objetivo
O objetivo deste exercício é **aplicar os conhecimentos de arquitetura de software**,  
**focando na implementação de uma API RESTful**, seguindo o padrão **MVC**.  

A ideia é explorar:
- **Práticas de design e construção de APIs**  
- **Documentação de arquitetura**  
- **Organização e separação de responsabilidades no código**  

Para isso, foi construída uma **API em .NET 8** para gerenciamento de produtos (**CRUD + consultas**), utilizando:  
- **MySQL** (persistência via Dapper, sem ORM/EF)  
- **Redis** (cache distribuído)  
- **Swagger/OpenAPI** (documentação interativa)  
- **FluentValidation** (validação de requests)  
- **CSharpFunctionalExtensions** (result pattern)  
- **Docker Compose** para orquestração  

---

## 📂 Estrutura de Pastas

```text
XPE.ArquiteturaSoftware.DesafioFinal/
├─ src/
│  ├─ XPE.ArquiteturaSoftware.DesafioFinal.Api/            # Camada de Apresentação (Web API)
│  │  ├─ Controllers/
│  │  │  ├─ ProductsController.cs                          # Endpoints REST de produtos
│  │  │  └─ HealthController.cs                            # Health checks (DB/Redis)
│  │  ├─ Filters/
│  │  │  └─ ResultHttpExtensions.cs                        # Converte Result/Result<T,E> → IActionResult (global)
│  │  ├─ Program.cs                                        # Bootstrap; Swagger, UseExceptionHandler
│  │  └─ appsettings*.json                                 # Configurações (MySQL, Redis, logging)
│  │
│  ├─ XPE.ArquiteturaSoftware.DesafioFinal.Application/    # Camada de Aplicação (casos de uso)
│  │  ├─ Interfaces/
│  │  │  └─ IProductService.cs
│  │  │  └─ IHealthService.cs
│  │  ├─ Services/
│  │  │  ├─ ProductService.cs                              # Orquestra repositório, mapeia via Mapper, usa cache Redis, retorna Result
│  │  │  └─ HealthService.cs                               # Verifica DB e Redis (Result<T,E> quando necessário)
│  │  ├─ Requests/
│  │  │  ├─ CreateProductRequest.cs
│  │  │  └─ UpdateProductRequest.cs
│  │  ├─ Responses/
│  │  │  ├─ CreateProductResponse.cs
│  │  │  ├─ ProductResponse.cs
│  │  │  └─ HealthCheckResponse.cs
│  │  ├─ Validators/
│  │  │  └─ CreateProductRequestValidator.cs               # Regras FluentValidation
│  │  ├─ Mappers/
│  │  │  └─ ProductMapper.cs                               # Mapeamentos Request↔Domain↔Response centralizados
│  │  └─ Caching/
│  │     └─ ProductCache.cs                                # Chaves/versão de cache, TTLs, (de)serialização JSON
│  │
│  ├─ XPE.ArquiteturaSoftware.DesafioFinal.Domain/         # Domínio (Entidades/Agregados)
│  │  └─ Models/
│  │     └─ Product.cs
│  │
│  ├─ XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data/     # Infraestrutura (persistência)
│  │   ├─ Interfaces/
│  │   │  └─ IProductRepository.cs
│  │   ├─ Repositories/
│  │   │  └─ ProductRepository.cs                          # Dapper + MySqlConnector
│  │   └─ Queries/
│  │      └─ ProductQueries.cs                             # SQL (Create/Get/Find/Count/Update/Delete)
│  │
│  └─ XPE.ArquiteturaSoftware.DesafioFinal.Infra.IoC/      # Infraestrutura (injeção de dependências)
│     └─ ServiceCollectionExtensions.cs                    # Registro de DI (validators, repos, services, redis, dapper, swagger, filtros)
│
├─ devops/
│  ├─ docker-compose.yml                                   # API, MySQL, Redis
│  ├─ Dockerfile                                           # Build da API
│  ├─ initdb/
│  │  └─ 001_create_db_and_table.sql                       # Criação idempotente do DB/tabela/índices
└─ README.md                                               # Como rodar, links dos diagramas, etc.
```

---

## 🧩 Papéis dos Componentes (padrão MVC + camadas)

- **Controllers (API/Apresentação)**: recebem HTTP, delegam para Services e retornam respostas.  
  Com o filtro global, podem retornar `Result` diretamente.

- **Services (Aplicação)**: orquestram casos de uso; chamam Repositórios; aplicam regras de aplicação;  
  mapeiam DTOs via **Mappers**; fazem **cache Redis**; retornam `Result/Result<T,E>`.

- **Models (Domínio/Entidades)**: encapsulam estado e comportamento de negócio (`Product`, `SetActive` etc.).

- **Repositories (Infra/Data)**: persistência via **Dapper** e `IDbConnection` (**MySqlConnector**);  
  SQL isolado em `ProductQueries`.

- **Validators**: regras de entrada com **FluentValidation** (validação automática no pipeline MVC).

- **Mappers**: conversões **Request ↔ Domínio ↔ Response** centralizadas (evitam lógica de mapeamento nos Services).

- **Caching**: estratégia de chaves com **token de versão** para invalidar listas/contagens sem varrer Redis.

- **Cross-cutting**:  
  - **UseExceptionHandler + ProblemDetails**: tratamento global de exceções inesperadas.  
  - **Filtro Result→ActionResult**: mapeia `Result` para HTTP (200/400/404/503) sem poluir controllers. 

---

## 🚀 Como Rodar

1. Clone o repositório.  
2. Vá até a pasta `devops/`.  
3. Suba os containers:  
   ```bash
   docker compose up -d --build
   ```
4. Acesse:  
   - **API Swagger**: http://localhost:8080/swagger  
   - **MySQL**: localhost:3306 (user: root / pass: root)  
   - **Redis**: localhost:6379

---

## ✅ Observações Finais
- **Cache Redis**: leituras em cache, invalidação automática em Create/Update/Delete.  
- **Tratamento de erros**: global via `UseExceptionHandler` + ProblemDetails.  
- **Responsabilidades isoladas**: Controllers, Services, Models, Repositories, Mappers, Validators.  

---
