# Teste - 4 Tech

Solução desenvolvida para o desafio de backend júnior descrito em [backend-jr.md](./backend-jr.md).

## Sobre o Projeto

Esta API foi desenvolvida seguindo os princípios de Clean Architecture e SOLID, implementando um sistema de CRUD de
beneficiários e planos de saúde com validações robustas.

## Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **MediatR** - Padrão CQRS
- **FluentValidation** - Validações
- **Docker** - Containerização

## Padrões Utilizados

- **Result Pattern** - Padronização de retorno de dados
- **CQRS** - Separação de comandos e consultas
- **Clean Architecture** - Organização do código em camadas
- **Domain Driven Design** - Utilizado somente a parte essencial como Entities e ValueObjects
- **Repository Pattern** - Abstração para acesso aos dados
- **Unit of Work Pattern** - Unificando todos os repositórios em uma injeção e controlando transações

---

## Decisões Técnicas

- **Scalar**: Escolhido pela interface moderna e exemplos de código em múltiplas linguagens
- **Soft Delete**: Dados não são removidos fisicamente, apenas marcados como excluídos
- **Result Pattern**: Padronização consistente de respostas (sucesso e erro)
- **GlobalExceptionHandler**: Tratamento centralizado de exceções
- **Keyset Pagination**: Melhor performance em grandes volumes de dados comparado a offset
---

## Estrutura do Projeto
```
src/
├── Health.Api/              # Camada de apresentação (Controllers)
├── Health.Application/      # Casos de uso (Commands, Queries, Handlers)
├── Health.Domain/           # Entidades e regras de negócio
└── Health.Infrastructure/   # Acesso a dados

tests/
├── Health.Application.Tests/      # Testes dos casos de uso
├── Health.Domain.Tests/           # Testes das entidades e value objects
```

---

## Executando os Testes

### Todos os testes
```bash
dotnet test
```

---

## Executando o Projeto

Há duas formas de executar o projeto: via Docker Compose ou localmente com banco em container.

### Executando Localmente

**Pré-requisitos:**

- .NET 9 SDK instalado
- Docker instalado (para o banco de dados)

A connection string no arquivo `appsettings.json` já está configurada para rodar localmente.

1. Suba o container do PostgreSQL:

```bash
docker run --name postgres \
  -e POSTGRES_DB=health_db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=4tech \
  -p 5432:5432 \
  -d postgres:16
```

2. Execute as migrations:

```bash
dotnet ef database update --project src/Health.Infrastructure
```

3. Execute a aplicação:

```bash
dotnet run --project src/Health.Api
```

A API estará disponível em `http://localhost:5178`

### Executando via Docker Compose

**Pré-requisitos:**

- Docker Compose instalado

1. Execute o comando do Docker Compose:

```bash
docker compose up -d
```

A API estará disponível em `http://localhost:8080`

---

## Documentação da API

A API utiliza **Scalar** para documentação interativa com exemplos em múltiplas linguagens.

Acesse em:
- Local: `http://localhost:5178/scalar`
- Docker: `http://localhost:8080/scalar`

---

## Endpoints da API

### Beneficiários

- `POST /api/beneficiaries` - Criar um novo beneficiário
- `PUT /api/beneficiaries/{id}` - Atualizar beneficiário existente
- `DELETE /api/beneficiaries/{id}` - Deletar beneficiário (soft delete)
- `GET /api/beneficiaries/{id}` - Buscar beneficiário por ID
- `GET /api/beneficiaries` - Listar beneficiários com filtros (nome, CPF, status, plano, data de nascimento) e paginação

### Planos de Saúde

- `POST /api/health-plans` - Criar um novo plano de saúde
- `PUT /api/health-plans/{id}` - Atualizar plano de saúde existente
- `DELETE /api/health-plans/{id}` - Deletar plano de saúde (soft delete)
- `GET /api/health-plans/{id}` - Buscar plano de saúde por ID
- `GET /api/health-plans` - Listar planos de saúde com filtros (nome, código ANS) e paginação

### Health Check

- `GET /` - Verificar status da aplicação e conexão com banco de dados