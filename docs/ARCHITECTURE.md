# Arquitetura do Sistema

Este documento detalha a arquitetura do AgregaNews, incluindo decisões de design, padrões utilizados e estrutura dos componentes.

## Visão Geral

O AgregaNews é uma plataforma de agregação de notícias construída com arquitetura de microserviços, utilizando comunicação assíncrona via mensageria e processamento baseado em eventos.

## Serviços Principais

### CollectNews API (`:5100`)

**Responsabilidade:** Coleta notícias de APIs externas (NewsAPI)

**Funcionalidades:**
- Integra com a NewsAPI para buscar notícias em tempo real
- Salva notícias coletadas no MongoDB
- Publica eventos `NewsAnalyzeEvent` no RabbitMQ para processamento assíncrono

**Endpoints:**
- `GET /collect-news` - Coleta notícias com filtros (país, categoria, paginação)

### AnalyzeNews API (`:5200`)

**Responsabilidade:** Analisa e categoriza notícias usando IA

**Funcionalidades:**
- Consome eventos do RabbitMQ quando novas notícias são coletadas
- Utiliza OpenAI ChatGPT para categorizar notícias automaticamente
- Categorias disponíveis: Negócios, Entretenimento, Geral, Saúde, Ciência, Esportes, Tecnologia
- Armazena notícias categorizadas no MongoDB

**Endpoints:**
- `GET /analyze-news` - Lista notícias analisadas e categorizadas

### Log Service API (`:5300`)

**Responsabilidade:** Sistema centralizado de logging e auditoria

**Funcionalidades:**
- Consome eventos `LogEvent` do RabbitMQ
- Armazena logs estruturados no MongoDB
- Fornece API para consulta de logs com paginação

**Endpoints:**
- `GET /logs` - Lista logs com paginação

### Gateway API (`:5000`)

**Responsabilidade:** API Gateway que unifica o acesso aos serviços

**Funcionalidades:**
- Unifica o acesso aos serviços através de um único ponto de entrada
- Utiliza YARP (Yet Another Reverse Proxy) para roteamento
- Endpoints roteados:
  - `/collect-news-api/*` → CollectNews API
  - `/analyze-news-api/*` → AnalyzeNews API
  - `/log-api/*` → Log Service API

## Padrões Arquiteturais

### Clean Architecture

Cada microserviço segue os princípios da Clean Architecture com separação clara de responsabilidades:

```
[NomeServiço]/
├── Api/                    # Camada de apresentação
├── Application/            # Camada de aplicação (use cases)
├── Domain/                 # Camada de domínio (entidades, contratos)
└── Infrastructure/         # Camada de infraestrutura (repositórios, serviços externos)
```

**Benefícios:**
- Testabilidade: Camadas podem ser testadas independentemente
- Manutenibilidade: Mudanças em uma camada não afetam outras
- Flexibilidade: Fácil substituir implementações (ex: MongoDB por outro banco)

### CQRS (Command Query Responsibility Segregation)

Separação de comandos (escrita) e consultas (leitura) usando MediatR:

- **Commands/Queries**: Representam ações ou consultas
- **Handlers**: Implementam a lógica de processamento
- **Responses**: DTOs específicos para cada operação

**Exemplo:**
```csharp
public record CollectNewsQuery(...) : IRequest<List<CollectNewsResponse>>;

public class CollectNewsQueryHandler : IRequestHandler<CollectNewsQuery, List<CollectNewsResponse>>
{
    // Implementação
}
```

### Event-Driven Architecture

Comunicação assíncrona entre serviços via RabbitMQ:

- **Event Bus**: Abstração para publicação de eventos
- **Consumers**: Processam eventos de forma assíncrona
- **Eventos**: `NewsAnalyzeEvent`, `LogEvent`

**Vantagens:**
- Desacoplamento entre serviços
- Escalabilidade horizontal
- Tolerância a falhas (mensagens são persistidas)

### Domain-Driven Design (DDD)

Modelagem baseada no domínio de negócio:

- **Entities**: Representam objetos de negócio (News, AnalyzedNews, Log)
- **Value Objects**: Objetos imutáveis que representam conceitos do domínio
- **Domain Services**: Lógica de negócio que não pertence a uma entidade específica
- **Repositories**: Abstrações para persistência

## Tecnologias

### Backend
- **.NET 8** - Framework principal
- **Carter** - Framework para APIs modulares baseadas em endpoints
- **MediatR** - Implementação do padrão Mediator para CQRS
- **AutoMapper** - Mapeamento de objetos

### Banco de Dados
- **MongoDB** - Banco NoSQL para cada microserviço
  - CollectNewsDb: Notícias coletadas
  - AnalyzeNewsDb: Notícias analisadas
  - LogDb: Logs do sistema

### Mensageria
- **RabbitMQ** - Message broker para comunicação assíncrona
- **MassTransit** - Framework para aplicações distribuídas (.NET)

### API Gateway
- **YARP** - Reverse Proxy para roteamento de requisições

### Logging
- **Serilog** - Logging estruturado
- Logs em arquivo e console

### Documentação
- **Swagger/OpenAPI** - Documentação interativa de APIs

### Integrações Externas
- **NewsAPI** - Coleta de notícias
- **OpenAI ChatGPT** - Categorização inteligente

## Fluxo de Dados

### 1. Coleta de Notícias

```
Cliente → Gateway → CollectNews API → NewsAPI
                          ↓
                    MongoDB (CollectNews)
                          ↓
                    RabbitMQ (NewsAnalyzeEvent)
```

### 2. Análise e Categorização

```
RabbitMQ → AnalyzeNews API → OpenAI ChatGPT
                ↓
          MongoDB (AnalyzeNews)
```

### 3. Logging

```
Qualquer Serviço → RabbitMQ (LogEvent) → Log Service → MongoDB (Logs)
```

## Eventos

### NewsAnalyzeEvent

Publicado pelo CollectNews quando uma notícia é coletada.

```csharp
public record NewsAnalyzeEvent
{
    public Guid Id { get; set; }
    public string? Author { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? UrlToImage { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public string? Content { get; set; }
}
```

**Consumido por:** AnalyzeNews API

### LogEvent

Publicado por qualquer serviço para logging centralizado.

```csharp
public record LogEvent
{
    public required string Message { get; set; }
    public required string Service { get; set; }
    public required string Severity { get; set; }
    public required string Environment { get; set; }
    public string? StackTrace { get; set; }
    public string? ExceptionType { get; set; }
    public required DateTimeOffset OccurredIn { get; set; }
}
```

**Consumido por:** Log Service API

## Health Checks

Todos os serviços expõem endpoints de health check:

- `/health` - Status geral de saúde do serviço
- `/health/ready` - Readiness probe (inclui dependências como MongoDB)
- `/health/live` - Liveness probe (verifica se o serviço está vivo)

## Estrutura de Cada Microserviço

```
[NomeServiço]/
├── source/
│   └── AgregaNews.[NomeServiço].Api/        # Camada de API
│   │   ├── Modules/                         # Módulos Carter
│   │   ├── Middlewares/                     # Middlewares customizados
│   │   ├── Options/                         # Configurações
│   │   └── HealthChecks/                    # Health checks
│   ├── AgregaNews.[NomeServiço].Application/ # Camada de aplicação
│   │   ├── Queries/                         # Queries (CQRS)
│   │   ├── Consumers/                       # Consumidores de eventos
│   │   └── Mappers/                         # AutoMapper profiles
│   ├── AgregaNews.[NomeServiço].Domain/     # Camada de domínio
│   │   ├── Entities/                        # Entidades
│   │   ├── Contracts/                       # Interfaces e contratos
│   │   └── Exceptions/                      # Exceções de domínio
│   └── AgregaNews.[NomeServiço].Infrastructure/ # Camada de infraestrutura
│       ├── Data/                            # Repositórios e contexto
│       ├── Services/                        # Serviços externos
│       └── DependencyInjection.cs           # Configuração DI
└── tests/
    └── AgregaNews.[NomeServiço].Application.UnitTests/ # Testes unitários
```

## Escalabilidade

O sistema foi projetado para escalar horizontalmente:

- **Stateless Services**: Cada serviço pode ser escalado independentemente
- **Message Queue**: RabbitMQ permite processamento assíncrono distribuído
- **Database per Service**: Cada serviço tem seu próprio banco de dados
- **API Gateway**: Centraliza roteamento e pode ser escalado

## Observabilidade

- **Health Checks**: Monitoramento de saúde dos serviços
- **Structured Logging**: Logs estruturados com Serilog
- **Centralized Logging**: Log Service centralizado para auditoria
- **RabbitMQ Management**: Interface web para monitoramento de filas

## Segurança

- **CORS**: Configurável para produção
- **API Keys**: Necessárias para NewsAPI e OpenAI
- **HTTPS**: Recomendado para produção
- **Credenciais**: Configuráveis via appsettings.json ou variáveis de ambiente
