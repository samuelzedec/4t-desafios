# Desafio Técnico — Cadastro de Beneficiários (Backend Júnior)

## Objetivo

Construir uma **API REST** para gerenciar **beneficiários** de um plano de saúde. O sistema deve permitir **CRUD**, aplicar **regras de negócio básicas**, persistir os dados em **banco relacional**, possuir **testes de unidade** e **documentação** (Swagger/OpenAPI).

> Liberdade Tecnológica: Este desafio é **completamente agnóstico** de linguagem e framework. Você tem **total liberdade** para escolher qualquer stack tecnológico com o qual se sinta mais confortável e produtivo.

---

## Escopo Funcional

### 1) Entidades

* **Plano**

  * `id` (UUID/auto)
  * `nome` (string, obrigatório, único)
  * `codigo_registro_ans` (string, obrigatório, único, formato livre)
* **Beneficiário**

  * `id` (UUID/auto)
  * `nome_completo` (string, obrigatório)
  * `cpf` (string, obrigatório, único, 11 dígitos numéricos)
  * `data_nascimento` (date, obrigatório)
  * `status` (enum: `ATIVO` | `INATIVO`, padrão `ATIVO`)
  * `plano_id` (FK para Plano, obrigatório)
  * `data_cadastro` (datetime, default now)

> Observação: você pode adicionar campos auditáveis (`created_at`, `updated_at`) se desejar.

### 2) Regras de Negócio

* **CPF único** no sistema e **válido** (apenas formato: 11 dígitos, checagem dos dígitos verificadores conta como bônus).
* Todo **beneficiário deve estar vinculado a um plano existente**.
* **Exclusão** de beneficiário:

  * Hard delete permitido, **ou** soft delete (bônus).
* **Atualização de plano**:

  * Permitida; manter integridade referencial.
* **Status**:

  * Criar como `ATIVO` por padrão.
  * Permitir mudança para `INATIVO` via endpoint de atualização.

### 3) Operações REST (mínimo)

#### Planos

* `POST /api/planos` — cria plano
* `GET /api/planos` — lista todos
* `GET /api/planos/{id}` — detalhe
* `PUT /api/planos/{id}` — atualiza
* `DELETE /api/planos/{id}` — remove (se preferir soft delete, documente)

#### Beneficiários

* `POST /api/beneficiarios` — cria beneficiário
* `GET /api/beneficiarios` — listar; filtros: `status`, `plano_id`
* `GET /api/beneficiarios/{id}` — detalhe
* `PUT /api/beneficiarios/{id}` — atualiza (inclusive `status` e a ligação com `plano_id`)
* `DELETE /api/beneficiarios/{id}` — remove

#### Exemplos de payloads

```json
// POST /api/planos
{ "nome": "Plano Ouro", "codigo_registro_ans": "ANS-123456" }

// POST /api/beneficiarios
{
  "nome_completo": "Maria da Silva",
  "cpf": "12345678909",
  "data_nascimento": "1990-05-20",
  "plano_id": "uuid-do-plano"
}
```

### 4) Respostas & Erros

* Utilize **HTTP status codes** adequados:

  * 201 (Created), 200/204 (OK), 400 (Bad Request), 404 (Not Found), 409 (Conflict — CPF duplicado), 422 (Unprocessable Entity — validações), 500 (Internal Error).
* Estruture erros:

```json
{
  "error": "ValidationError",
  "message": "CPF inválido",
  "details": [{"field":"cpf","rule":"invalid"}]
}
```

---

## Requisitos Técnicos

### Banco de Dados

* **Relacional** (PostgreSQL, MySQL, SQL Server, SQLite, ou qualquer outro SGBD relacional de sua preferência).
* **Migrations(opcional)** versionadas (usando as ferramentas do ecossistema escolhido).

### API & Projeto

* **Linguagem e framework de sua escolha** - use o que você domina melhor! 
* **Estrutura organizada** seguindo as melhores práticas do ecossistema escolhido (ex.: camadas controller/route, service, repository/ORM, domain/models).
* **Validações** no nível da API e/ou domínio.
* **Configuração** (bônus): separada do código (arquivos .env, config files, etc. - não commitar segredos).
* **Docker** (bônus): compose com app + DB.

### Testes de Unidade (mínimo)

* Serviço de **criação de beneficiário** (CPF duplicado → erro 409).
* Validação de **CPF inválido**.
* **Vinculação ao plano** inexistente → erro 422/404.
* Atualização de **status** para `INATIVO`.
* Listagem com **filtros** (ex.: por `status` e `plano_id`).

### Documentação

* **Swagger/OpenAPI** acessível (ex.: `/swagger` ou `/docs`).
* README com:

  * Visão geral
  * Stack utilizada
  * Como rodar (local e via Docker, se houver)
  * Como rodar **testes**
  * Decisões de projeto (trade-offs)
  * Exemplos de requisições (curl ou HTTPie)

## Dados de Exemplo

Planos:

```json
[
  {"nome":"Plano Bronze","codigo_registro_ans":"ANS-100001"},
  {"nome":"Plano Prata","codigo_registro_ans":"ANS-100002"},
  {"nome":"Plano Ouro","codigo_registro_ans":"ANS-100003"},
  {"nome":"Plano Diamante","codigo_registro_ans":"ANS-100004"},
  {"nome":"Plano Executivo","codigo_registro_ans":"ANS-100005"}
]
```

Beneficiários:

```json
[
  {"nome_completo":"João Pereira","cpf":"11144477735","data_nascimento":"1988-01-10","status":"ATIVO","plano":"Plano Prata"},
  {"nome_completo":"Ana Souza","cpf":"98765432100","data_nascimento":"1995-09-03","status":"ATIVO","plano":"Plano Bronze"},
  {"nome_completo":"Carlos Silva","cpf":"12345678901","data_nascimento":"1985-03-15","status":"ATIVO","plano":"Plano Ouro"},
  {"nome_completo":"Maria Santos","cpf":"10987654321","data_nascimento":"1992-07-22","status":"INATIVO","plano":"Plano Diamante"},
  {"nome_completo":"Pedro Oliveira","cpf":"11122233344","data_nascimento":"1990-12-05","status":"ATIVO","plano":"Plano Executivo"}
]
```

---

## Casos de Teste Recomendados

1. **Criar beneficiário válido** → 201 + corpo com `id`.
2. **Criar beneficiário com CPF duplicado** → 409.
3. **Criar beneficiário com plano inexistente** → 422/404.
4. **Atualizar status para INATIVO** → 200 e `status` atualizado.
5. **Listar beneficiários filtrando por status=ATIVO** → retorna apenas ativos.
6. **Buscar beneficiário por id inexistente** → 404.
7. **Excluir beneficiário** → 204 (ou 200 com `deleted=true` se soft delete).

---

## Requisitos Não Funcionais

* Respostas em **JSON**.
* Logs simples de requisição/erro (não vazar dados sensíveis).
* Tratamento de erros centralizado.(opcional)

---

## Entrega

* Repositório público (GitHub/GitLab/Azure DevOps) contendo:

  * Código fonte
  * Migrations / scripts SQL
  * Testes
  * README
  * Swagger/OpenAPI
  * (Bônus) `docker-compose.yml`

### Como rodar (exemplo esperado no README)

* Comandos para subir a aplicação (ex.: `make up`, `docker compose up -d`, `npm start`, `dotnet run`, `mvn spring-boot:run`, `go run main.go`, etc.)
* Comandos para executar testes (ex.: `make test`, `npm test`, `dotnet test`, `mvn test`, `go test`, `pytest`, etc.)

---

## Observações & Limites

* **Não** é necessário front-end.
* **Sem** integração com APIs externas.
* **ORM/Query Builder** de sua escolha (ex.: JPA/Hibernate, Entity Framework, Sequelize, Prisma, SQLAlchemy, GORM, Eloquent, ActiveRecord, Diesel, etc.).
* Evite bibliotecas que “escondam” toda a lógica de validação de CPF (implemente a checagem mínima ou documente a escolha).
