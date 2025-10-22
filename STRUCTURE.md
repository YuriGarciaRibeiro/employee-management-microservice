# Estrutura do Projeto

## Arquitetura Clean Architecture

Cada microsserviço segue a organização em camadas:

### Domain (Núcleo)
- **Entities**: Modelos de negócio
- **Enums**: Enumeradores
- **Events**: Eventos de domínio
- **Interfaces**: Contratos/abstrações

### Application (Casos de Uso)
- **UseCases**: Commands e Queries
- **DTOs**: Data Transfer Objects
- **Validators**: FluentValidation
- **Mappings**: AutoMapper profiles
- **Interfaces**: Contratos da camada

### Infrastructure (Implementação)
- **Data**: DbContext, Configurations, Migrations
- **Repositories**: Implementações
- **Services**: Email, JWT, etc
- **Messaging**: RabbitMQ Publisher/Consumer
- **Jobs**: Hangfire jobs

### Presentation (API/Worker)
- **Controllers**: Endpoints REST
- **Configurations**: DI, Swagger, etc
- **Middleware**: Error handling, etc

## Microsserviços

### 1. Cadastro (Registration)
Responsável por gerenciar o cadastro de funcionários.

**Endpoints principais:**
- POST /api/employees
- GET /api/employees
- GET /api/employees/{id}
- PUT /api/employees/{id}
- DELETE /api/employees/{id}
- GET /api/employees/by-date-range
- GET /api/employees/grouped-by-department

### 2. Ativação (Activation)
Worker que processa ativações agendadas.

**Responsabilidades:**
- Consumir fila RabbitMQ
- Ativar funcionários na data de início
- Processar em lotes de 100

### 3. Notificações (Notifications)
Hub SignalR para notificações em tempo real.

**Responsabilidades:**
- Receber eventos de outros microsserviços
- Notificar setores via SignalR
- Distribuir eventos em tempo real

## Comunicação entre Microsserviços

```
Cadastro API 
    ↓ (publica evento)
RabbitMQ
    ↓ (consome)
Ativação Worker → Processa ativações
    ↓ (publica evento)
RabbitMQ
    ↓ (consome)
Notificações API → SignalR → Clientes
```

## Bancos de Dados

- **PostgreSQL**: Banco compartilhado
  - Schema `cadastro` - Dados de funcionários
  - Schema `ativacao` - Batches de ativação
  - Schema `identity` - Usuários e roles

## Tecnologias por Camada

**Domain:**
- Nenhuma dependência externa

**Application:**
- FluentValidation
- AutoMapper

**Infrastructure:**
- Entity Framework Core
- Npgsql (PostgreSQL)
- MassTransit (RabbitMQ)
- Hangfire
- MailKit (Email)

**API:**
- ASP.NET Core
- Swashbuckle (Swagger)
- Identity
- JWT Bearer

**Tests:**
- xUnit
- Moq
- FluentAssertions
- EF InMemory
