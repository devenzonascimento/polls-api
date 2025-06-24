# 🚀 Roadmap e Checklist Prático

## 🥇 Prioritário para seu 1×1: OpenSearch & Redis

- [ ] **Configurar OpenSearch**
  - [ ] Instalar e subir container `opensearch` + dashboards  
  - [ ] Criar cliente NEST/`OpenSearch.Client` em `Infrastructure`  
  - [ ] Definir mapeamento de índice para `polls`:  
    - campos: `title` (text, analyzer), `description` (text), `status` (keyword), `createdAt` (date), `closesAt` (date)  
  - [ ] Indexar enquetes novas no evento de criação (`CreatePollCommandHandler`)  
  - [ ] Implementar endpoint `GET /api/polls/search?q=<termo>&status=<status>` que consulta o índice e retorna IDs + dados relacionais

- [ ] **Configurar Redis**
  - [ ] Subir container `redis`  
  - [ ] Criar cliente `ConnectionMultiplexer` em `Infrastructure`  
  - [ ] No momento do voto (`VoteCommandHandler`), incrementar contador em Sorted Set:  
    - chave: `poll:ranking` (score = votos totais por pollId)  
  - [ ] Criar endpoint `GET /api/polls/top?n=5` que lê top N do Sorted Set e retorna dados de polls

## ⏰ Background Task: Encerrar enquetes no `closesAt`

- [ ] **Scheduler básico (IHostedService)**
  - [ ] Criar classe `PollExpirationService : BackgroundService`  
  - [ ] Injetar `IPollRepository` e `IServiceScopeFactory`  
  - [ ] No `ExecuteAsync`:
    - a cada X minutos (ex.: 1 min),  
      - buscar no DB todas as polls com `status = Active` e `closesAt <= DateTime.UtcNow`  
      - para cada uma:  
        - atualizar `status = Closed`  
        - reindexar no OpenSearch  
        - (opcional) publicar evento Redis Pub/Sub `poll.closed`  
  - [ ] Registrar `PollExpirationService` em DI

## 🛡️ Qualidade & Segurança

- [ ] **Validações (FluentValidation)**
  - [ ] `CreatePollCommandValidator`  
  - [ ] `VoteCommandValidator` (não permitir voto duplo)  
  - [ ] `LoginCommandValidator` / `RegisterCommandValidator`

- [ ] **Testes Automatizados**
  - [ ] Unit tests para Handlers (Moq de repositórios)  
  - [ ] Integration tests contra PostgreSQL container (TestContainers)  
  - [ ] Testes de busca OpenSearch (usar índice de teste)  

## 📊 Observabilidade & Monitoramento

- [ ] **Logging**  
  - [ ] Configurar Serilog (console + arquivo)  
  - [ ] Log nos principais Handlers e no `PollExpirationService`

- [ ] **Health Checks**  
  - [ ] DB  
  - [ ] Redis  
  - [ ] OpenSearch  

- [ ] **Tracing (OpenTelemetry)**  
  - [ ] Instrumentar `PollExpirationService` e Handlers  
  - [ ] Exportador Console / Jaeger  

## 📦 Prepare para Deploy

- [ ] Dockerize a API (.NET Dockerfile)  
- [ ] Compor `docker-compose.yml` com todos os serviços  
- [ ] Testar orquestração localmente  

## 🏗️ Roadmap de Features Futuras

- [ ] Comentários em enquetes  
- [ ] Compartilhamento público por link  
- [ ] Dashboard de analytics (top enquetes, geolocalização, temas)  
- [ ] Real-time updates (SignalR ou Redis Pub/Sub + WebSocket)  
