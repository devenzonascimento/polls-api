## PollsApp

Uma API de enquetes e votações desenvolvida em C# .NET 8, que permite criar enquetes, votar, comentar, buscar e listar resultados. Utiliza PostgreSQL para dados relacionais, Redis para cache e contadores em tempo real, e Dapper para acesso leve ao banco.

---

### 🏆 Principais Funcionalidades

* **Autenticação** (JWT): registro e login de usuários.
* **Gestão de Enquetes**

  * Criar, atualizar, consultar e deletar enquetes.
  * Buscar por texto e filtrar por status (ativas/encerradas).
  * Listar ranking das mais votadas.
* **Votação**

  * Votar em opções de enquete.
  * Contador de votos em tempo real (Redis Sorted Set).
* **Comentários**

  * Criar, responder, editar e remover comentários.
* **Migrations**: FluentMigrator para versionamento de esquema.
* **Background Jobs**: Hangfire para agendar fechamento automático de enquetes.
* **Indexação**: OpenSearch para busca full‑text.
* **Cache**: Redis para acelerar leituras e contadores.

---

### 🛠️ Tech Stack

* **.NET 8**
* **C#** com Dapper (micro‑ORM)
* **PostgreSQL** (dados principais)
* **Redis** (cache, counters)
* **Hangfire** (tarefas agendadas)
* **FluentMigrator** (migrations)
* **Docker / Docker Compose**
* **OpenAPI (Swagger)**

---

### 🚀 Pré‑requisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/get-started) & Docker Compose

---

### ⚙️ Configuração do Ambiente

1. **Clone o repositório**

   ```bash
   git clone https://github.com/devenzonascimento/polls-api.git
   cd polls-api
   ```

2. **Variáveis de ambiente**
   Renomeie `appsettings.json.example` para `appsettings.json` e ajuste:

   ```jsonc
   {
     "ConnectionStrings": {
       "PostgreSql":      "Host=postgres;Port=5432;Database=pollsdb;Username=postgres;Password=postgres",
       "Redis":           "redis:6379",
       "OpenSearch":      "http://opensearch:9200"
     },
     "Jwt": {
       "Key":             "<sua-chave-secreta-muito-longa>",
       "Issuer":          "PollsApp",
       "ExpireMinutes":   "60"
     }
   }
   ```

3. **Subir os containers**

   ```bash
   docker compose up -d
   ```

   Isso vai criar serviços:

   * **postgres** (5432)
   * **redis** (6379)
   * **opensearch** (9200) + **dashboards** (5601)
   * **api** (5232)

---

### ▶️ Executando a API

```bash
cd PollsApp.Api
dotnet build
dotnet run
```

Por padrão ouvirá em `http://localhost:5232`.

* **Swagger UI**: `http://localhost:5232/swagger`

---

### 📑 Endpoints Principais

| Método | Rota                       | Descrição                           |
| ------ | -------------------------- | ----------------------------------- |
| POST   | `/api/auth/register`       | Registra usuário                    |
| POST   | `/api/auth/login`          | Login (gera JWT)                    |
| GET    | `/api/polls`               | Lista todas enquetes (sem opções)   |
| POST   | `/api/polls`               | Cria nova enquete                   |
| PUT    | `/api/polls`               | Atualiza dados da enquete           |
| GET    | `/api/polls/{id}`          | Consulta enquete + opções           |
| DELETE | `/api/polls/{id}`          | Deleta enquete                      |
| GET    | `/api/polls/search`        | Buscar por texto/status             |
| GET    | `/api/polls/top-ranking`   | Top N enquetes mais votadas (Redis) |
| POST   | `/api/votes?optionId={id}` | Votar em opção                      |
| POST   | `/api/comments`            | Criar comentário                    |
| POST   | `/api/comments/reply`      | Responder comentário                |
| PUT    | `/api/comments/edit`       | Editar comentário                   |
| DELETE | `/api/comments/{id}`       | Deletar comentário                  |

---

### 🔄 Migrations

* Ao iniciar a API, o FluentMigrator roda automaticamente:

  ```csharp
  runner.MigrateUp();
  ```

* As migrations estão em
  `PollsApp.Infrastructure.Data.Migrations`

---

### 🔧 Hangfire

* Dashboard: `http://localhost:5232/hangfire`
* Jobs recorrentes: fechamento automático de enquetes (`Cron.Minutely`).

---

### 📂 Estrutura de Pastas

```
/PollsApp.Api              # Projeto ASP.NET Core (Controllers, DTOs, Configs, Middlewares, etc...)
/PollsApp.Application      # Services, Commands, Queries, Notifications, Handlers
/PollsApp.Domain           # Entidades, Agregados e Eventos de Domínio
/PollsApp.Infrastructure   # Repositórios, Dapper, Migrations, OpenSearch
/docker-compose.yml        # Orquestração dos serviços
```
