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
│  ├─ XPE.ArquiteturaSoftware.DesafioFinal.Api/            # Apresentação (Web API)
│  ├─ XPE.ArquiteturaSoftware.DesafioFinal.Application/    # Camada de aplicação
│  ├─ XPE.ArquiteturaSoftware.DesafioFinal.Domain/         # Domínio (entidades)
│  └─ XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data/     # Infraestrutura (dados)
│  └─ XPE.ArquiteturaSoftware.DesafioFinal.Infra.IoC/      # Infraestrutura (resolução de dependências e middlewares)
├─ devops/                                                 # Docker Compose e initdb
└─ README.md
```

---

## 🧩 Papéis dos Componentes

- **Controllers**: recebem requests HTTP, chamam Services, retornam `Result`.  
- **Services**: orquestram casos de uso, chamam repositórios, validam dados, aplicam cache.  
- **Domain Models**: entidades de negócio, encapsulam estado e comportamento (`Product`).  
- **Repositories**: persistência com Dapper + MySqlConnector.  
- **Mappers**: centralizam conversões entre DTOs e domínio.  
- **Validators**: validações de entrada com FluentValidation.  
- **Caching**: controle de chaves/versões no Redis para leituras rápidas.  
- **Filters**:  
  - `UseExceptionHandler + ProblemDetails`: tratamento global de exceções.  
  - `ResultHttpExtensions`: converte `Result` → IActionResult sem poluir controllers.  

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
