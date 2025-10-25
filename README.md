# Employee Management Microservices

Sistema distribuído para cadastro e ativação de funcionários utilizando .NET 9 (C#) e Clean Architecture.

## 🏗️ Arquitetura

O sistema é composto por 3 microsserviços principais:

### 1. Microsserviço de Cadastro
- API REST para CRUD de funcionários
- Persistência de dados
- Envio de e-mails de notificação
- Publicação de eventos para o barramento de mensagens

### 2. Microsserviço de Ativação
- Consumo de fila RabbitMQ
- Ativação automática de funcionários na data de início
- Processamento em lotes (100 funcionários por vez)
- Agendamento via Hangfire

### 3. Microsserviço de Notificações
- Hub SignalR para notificações em tempo real
- Distribuição de eventos para setores
- Notificações sobre novos funcionários e alterações

## 🚀 Stack Tecnológica

- **.NET 9.0** - Framework principal
- **Clean Architecture** - Padrão arquitetural
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **RabbitMQ** - Message broker
- **Hangfire** - Agendamento de jobs
- **SignalR** - Comunicação em tempo real
- **ASP.NET Identity** - Autenticação e autorização
- **JWT** - Tokens de autenticação com refresh token
- **Docker** - Containerização
- **xUnit + Moq** - Testes unitários
- **k6** - Testes de carga
- **SonarQube** - Análise de código

## 📁 Estrutura do Projeto

```
employee-management-microservice/
├── src/Services/
│   ├── Cadastro/
│   │   ├── EmployeeManagement.Cadastro.Domain/
│   │   ├── EmployeeManagement.Cadastro.Application/
│   │   ├── EmployeeManagement.Cadastro.Infrastructure/
│   │   └── EmployeeManagement.Cadastro.API/
│   ├── Ativacao/
│   │   ├── EmployeeManagement.Ativacao.Domain/
│   │   ├── EmployeeManagement.Ativacao.Application/
│   │   ├── EmployeeManagement.Ativacao.Infrastructure/
│   │   └── EmployeeManagement.Ativacao.Worker/
│   └── Notificacoes/
│       ├── EmployeeManagement.Notificacoes.Domain/
│       ├── EmployeeManagement.Notificacoes.Application/
│       ├── EmployeeManagement.Notificacoes.Infrastructure/
│       └── EmployeeManagement.Notificacoes.API/
├── tests/
│   ├── Cadastro.Tests/
│   ├── Ativacao.Tests/
│   └── Notificacoes.Tests/
└── docker-compose.yml
```

## 📋 Requisitos Funcionais

### Cadastro de Funcionários
- Campos: Nome, Telefone, Data de Início, Setor, Situação (Ativo/Inativo)
- CRUD completo via API REST
- Validações de dados

### Listagem e Agrupamento
- Filtro por período (range de datas)
- Agrupamento por setor
- Paginação de resultados

### Notificação por E-mail
- Envio automático ao cadastrar funcionário
- Conteúdo: dados do funcionário e data de início

### Eventos em Tempo Real
- SignalR para notificação de setores
- Eventos: novo funcionário, alteração de data de início

### Ativação Automática
- Agendamento via Hangfire
- Ativação na data de início
- Processamento em lotes de 100 para volume > 1000

## ⚡ Quick Start

### Pré-requisitos
- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker** e **Docker Compose** - [Download](https://www.docker.com/products/docker-desktop)
- **Git** - [Download](https://git-scm.com/)

### Opção 1: Executar com Docker Compose (Recomendado)

Esta é a forma mais rápida de rodar toda a aplicação.

**1. Clone o repositório:**
```bash
git clone https://github.com/seu-usuario/employee-management-microservice.git
cd employee-management-microservice
```

**2. Inicie todos os serviços:**
```bash
docker-compose up --build
```

**3. Aguarde os serviços estarem prontos (2-3 minutos):**

| Serviço | URL | Credenciais |
|---------|-----|-------------|
| **Cadastro API** | http://localhost:5001 | - |
| **Notificações API** | http://localhost:5002 | - |
| **Swagger (Cadastro)** | http://localhost:5001/swagger | - |
| **Swagger (Notificações)** | http://localhost:5002/swagger | - |
| **PostgreSQL** | localhost:5432 | user: `postgres` / pwd: `postgres` |
| **RabbitMQ Management** | http://localhost:15672 | user: `guest` / pwd: `guest` |
| **Hangfire Dashboard** | http://localhost:5001/hangfire | - |

**4. Teste a API:**

```bash
# Criar um usuário
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123"
  }'

# Fazer login
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123"
  }'

# Copie o token retornado e use para criar um funcionário
curl -X POST http://localhost:5001/api/employees \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "name": "João Silva",
    "phone": "+5511987654321",
    "department": "TI",
    "startDate": "2025-12-01T08:00:00Z"
  }'
```

**5. Parar os serviços:**
```bash
docker-compose down
```

**6. Limpar volumes (reset completo):**
```bash
docker-compose down -v
```

---

## 📣 Visualizando logs (Dozzle)

Adicionei o Dozzle para visualização simples de logs via web. Ele é leve, imediado e ideal para desenvolvimento.

Como ver os logs rapidamente:

- Dozzle (UI muito simples e imediata):
  - URL: http://localhost:8088
  - Dozzle lista todos os containers e permite seguir (tail) os logs em tempo real. Ele lê diretamente do Docker socket.

Passos rápidos (terminal zsh):
```bash
# Subir todos os serviços (inclui dozzle)
docker compose up -d

# Verificar containers
docker compose ps
```

Dozzle é a forma mais direta: abra http://localhost:8088 e clique no container que deseja inspecionar.

Observação:
- Removi o Loki/Promtail da configuração por pedido — caso queira histórico e buscas avançadas, posso readicionar Loki+Promtail no futuro.


### Opção 2: Executar Localmente (Desenvolvimento)

Para desenvolvimento local sem Docker.

**1. Inicie as dependências (PostgreSQL, RabbitMQ):**
```bash
docker-compose up -d postgres rabbitmq
```

**2. Configure as connection strings (opcional):**

Edite `src/Services/Cadastro/EmployeeManagement.Cadastro.API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=EmployeeManagement;Username=postgres;Password=postgres"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

**3. Restaurar e compilar:**
```bash
dotnet restore
dotnet build
```

**4. Executar migrations (primeira vez):**
```bash
cd src/Services/Cadastro/EmployeeManagement.Cadastro.API
dotnet ef database update
```

**5. Executar os serviços (em terminais separados):**

**Terminal 1 - Cadastro API:**
```bash
dotnet run --project src/Services/Cadastro/EmployeeManagement.Cadastro.API
# Rodando em http://localhost:5001
```

**Terminal 2 - Notificações API:**
```bash
dotnet run --project src/Services/Notificacoes/EmployeeManagement.Notificacoes.API
# Rodando em http://localhost:5002
```

**Terminal 3 - Worker de Ativação:**
```bash
dotnet run --project src/Services/Ativacao/EmployeeManagement.Ativacao.Worker
# Worker rodando em background
```

**6. Acesse:**
- Swagger Cadastro: http://localhost:5001/swagger
- Swagger Notificações: http://localhost:5002/swagger
- Hangfire: http://localhost:5001/hangfire

---

### Opção 3: Executar Testes

**Testes Unitários (99 testes):**
```bash
# Rodar todos os testes
dotnet test

# Rodar testes com coverage
dotnet test /p:CollectCoverage=true

# Rodar testes de um projeto específico
dotnet test tests/EmployeeManagement.Cadastro.Application.Tests
```

**Testes de Carga K6:**
```bash
# Certifique-se que a API está rodando primeiro
docker-compose up -d

# Teste de autenticação
k6 run tests/LoadTests/auth-load-test.js

# Teste CRUD de funcionários
k6 run tests/LoadTests/employee-load-test.js

# Teste de ativação em lote
k6 run tests/LoadTests/activation-batch-test.js

# Cenário customizado
k6 run --env VUS=100 --env DURATION=60s tests/LoadTests/employee-load-test.js
```

Veja mais detalhes em [tests/LoadTests/README.md](tests/LoadTests/README.md)

---

### 🔧 Troubleshooting

**Problema: Porta já em uso**
```bash
# Verificar portas em uso
netstat -ano | findstr :5001
netstat -ano | findstr :5432

# Matar processo (Windows - substitua PID)
taskkill /PID <PID> /F

# Ou altere as portas no docker-compose.yml
```

**Problema: Migrations não aplicadas**
```bash
# Aplicar migrations manualmente
cd src/Services/Cadastro/EmployeeManagement.Cadastro.API
dotnet ef database update
```

**Problema: RabbitMQ não conecta**
```bash
# Verificar se RabbitMQ está rodando
docker ps | grep rabbitmq

# Ver logs do RabbitMQ
docker logs <rabbitmq-container-id>

# Reiniciar RabbitMQ
docker-compose restart rabbitmq
```

**Problema: Erro de autenticação**
```bash
# Certifique-se de criar um usuário primeiro
# Use o endpoint /api/auth/register antes de /api/auth/login
```

**Problema: Docker build falha**
```bash
# Limpar cache do Docker
docker system prune -a

# Rebuild sem cache
docker-compose build --no-cache
docker-compose up
```

## 📝 Endpoints da API

### Base URLs
- **Cadastro API**: `http://localhost:5001/api`
- **Notificações API**: `http://localhost:5002/api`
- **Swagger UI**: `http://localhost:5001/swagger` (Cadastro) e `http://localhost:5002/swagger` (Notificações)

---

## 🔐 Autenticação

Todos os endpoints (exceto `/auth/register` e `/auth/login`) requerem autenticação via JWT Bearer token.

**Header obrigatório:**
```
Authorization: Bearer {seu_token_jwt}
```

### 1. Registrar Usuário

**POST** `/api/auth/register`

Cria uma nova conta de usuário.

**Request Body:**
```json
{
  "email": "usuario@example.com",
  "password": "SenhaForte@123"
}
```

**Response (201 Created):**
```json
{
  "message": "User registered successfully"
}
```

**Validações:**
- Email deve ser válido
- Senha: mínimo 8 caracteres, 1 maiúscula, 1 minúscula, 1 número, 1 caractere especial

---

### 2. Login

**POST** `/api/auth/login`

Autentica um usuário e retorna tokens JWT.

**Request Body:**
```json
{
  "email": "usuario@example.com",
  "password": "SenhaForte@123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0...",
  "expiration": "2025-10-23T15:30:00Z"
}
```

**Campos:**
- `token`: JWT token para autenticação (válido por 1 hora)
- `refreshToken`: Token para renovação (válido por 7 dias)
- `expiration`: Data/hora de expiração do token

---

### 3. Refresh Token

**POST** `/api/auth/refresh`

Renova o token JWT usando o refresh token.

**Request Body:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0..."
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "k1l2m3n4o5p6q7r8s9t0...",
  "expiration": "2025-10-23T16:30:00Z"
}
```

---

## 👥 Funcionários (Employees)

### 4. Criar Funcionário

**POST** `/api/employees`

Cria um novo funcionário no sistema.

**Request Body:**
```json
{
  "name": "João Silva Santos",
  "phone": "+5511987654321",
  "department": "TI",
  "startDate": "2025-11-15T08:00:00Z"
}
```

**Response (201 Created):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva Santos",
  "phone": "+5511987654321",
  "department": "TI",
  "startDate": "2025-11-15T08:00:00Z",
  "isActive": false,
  "createdAt": "2025-10-23T10:30:00Z",
  "updatedAt": "2025-10-23T10:30:00Z"
}
```

**Validações:**
- `name`: obrigatório, 3-200 caracteres
- `phone`: obrigatório, 10-15 dígitos (formato: +5511987654321)
- `department`: obrigatório, 2-100 caracteres
- `startDate`: obrigatório, deve ser data futura

**Eventos gerados:**
- 📧 Email de notificação enviado
- 📨 Mensagem publicada no RabbitMQ
- 🔔 Notificação SignalR enviada para o departamento

---

### 5. Listar Todos os Funcionários

**GET** `/api/employees?pageNumber=1&pageSize=10`

Lista todos os funcionários com paginação.

**Query Parameters:**
- `pageNumber` (opcional, padrão: 1): Número da página
- `pageSize` (opcional, padrão: 10, máx: 100): Itens por página

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "João Silva Santos",
      "phone": "+5511987654321",
      "department": "TI",
      "startDate": "2025-11-15T08:00:00Z",
      "isActive": false,
      "createdAt": "2025-10-23T10:30:00Z",
      "updatedAt": "2025-10-23T10:30:00Z"
    },
    {
      "id": "7bc92a41-8d15-4f2e-b9c7-1e4f5a6b8c9d",
      "name": "Maria Oliveira Costa",
      "phone": "+5511912345678",
      "department": "RH",
      "startDate": "2025-10-30T08:00:00Z",
      "isActive": false,
      "createdAt": "2025-10-23T11:00:00Z",
      "updatedAt": "2025-10-23T11:00:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "totalCount": 47,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

### 6. Buscar Funcionário por ID

**GET** `/api/employees/{id}`

Retorna os detalhes de um funcionário específico.

**Path Parameter:**
- `id`: UUID do funcionário

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva Santos",
  "phone": "+5511987654321",
  "department": "TI",
  "startDate": "2025-11-15T08:00:00Z",
  "isActive": false,
  "createdAt": "2025-10-23T10:30:00Z",
  "updatedAt": "2025-10-23T10:30:00Z"
}
```

---

### 7. Atualizar Funcionário

**PUT** `/api/employees/{id}`

Atualiza todos os dados de um funcionário.

**Path Parameter:**
- `id`: UUID do funcionário

**Request Body:**
```json
{
  "name": "João Silva Santos Junior",
  "phone": "+5511999887766",
  "department": "Desenvolvimento",
  "startDate": "2025-11-20T08:00:00Z"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva Santos Junior",
  "phone": "+5511999887766",
  "department": "Desenvolvimento",
  "startDate": "2025-11-20T08:00:00Z",
  "isActive": false,
  "createdAt": "2025-10-23T10:30:00Z",
  "updatedAt": "2025-10-23T14:45:00Z"
}
```

---

### 8. Atualizar Data de Início

**PUT** `/api/employees/{id}/start-date`

Atualiza apenas a data de início de um funcionário.

**Path Parameter:**
- `id`: UUID do funcionário

**Request Body:**
```json
{
  "startDate": "2025-12-01T08:00:00Z"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva Santos",
  "phone": "+5511987654321",
  "department": "TI",
  "startDate": "2025-12-01T08:00:00Z",
  "isActive": false,
  "createdAt": "2025-10-23T10:30:00Z",
  "updatedAt": "2025-10-23T15:00:00Z"
}
```

**Eventos gerados:**
- 🔔 Notificação SignalR sobre alteração de data
- 📨 Mensagem atualizada no RabbitMQ

---

### 9. Deletar Funcionário

**DELETE** `/api/employees/{id}`

Remove um funcionário do sistema (soft delete).

**Path Parameter:**
- `id`: UUID do funcionário

**Response (204 No Content)**

---

## 📊 Consultas e Relatórios

### 10. Funcionários por Período

**GET** `/api/employees/by-date-range?startDate={start}&endDate={end}`

Retorna funcionários com data de início dentro do período especificado.

**Query Parameters:**
- `startDate` (obrigatório): Data inicial (formato ISO 8601)
- `endDate` (obrigatório): Data final (formato ISO 8601)

**Exemplo:**
```
GET /api/employees/by-date-range?startDate=2025-11-01T00:00:00Z&endDate=2025-11-30T23:59:59Z
```

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "João Silva Santos",
    "phone": "+5511987654321",
    "department": "TI",
    "startDate": "2025-11-15T08:00:00Z",
    "isActive": false,
    "createdAt": "2025-10-23T10:30:00Z",
    "updatedAt": "2025-10-23T10:30:00Z"
  },
  {
    "id": "8cd43b52-9e26-5g3f-c0d8-2f5g6b7c9e0a",
    "name": "Pedro Souza Lima",
    "phone": "+5511923456789",
    "department": "Vendas",
    "startDate": "2025-11-22T08:00:00Z",
    "isActive": false,
    "createdAt": "2025-10-23T12:15:00Z",
    "updatedAt": "2025-10-23T12:15:00Z"
  }
]
```

---

### 11. Funcionários Agrupados por Departamento

**GET** `/api/employees/grouped-by-department`

Retorna funcionários agrupados por departamento.

**Response (200 OK):**
```json
{
  "TI": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "João Silva Santos",
      "phone": "+5511987654321",
      "department": "TI",
      "startDate": "2025-11-15T08:00:00Z",
      "isActive": false,
      "createdAt": "2025-10-23T10:30:00Z",
      "updatedAt": "2025-10-23T10:30:00Z"
    },
    {
      "id": "4gb96c75-6828-5673-c4gd-3d074g77bgb7",
      "name": "Carlos Eduardo Ferreira",
      "phone": "+5511934567890",
      "department": "TI",
      "startDate": "2025-11-18T08:00:00Z",
      "isActive": false,
      "createdAt": "2025-10-23T13:00:00Z",
      "updatedAt": "2025-10-23T13:00:00Z"
    }
  ],
  "RH": [
    {
      "id": "7bc92a41-8d15-4f2e-b9c7-1e4f5a6b8c9d",
      "name": "Maria Oliveira Costa",
      "phone": "+5511912345678",
      "department": "RH",
      "startDate": "2025-10-30T08:00:00Z",
      "isActive": false,
      "createdAt": "2025-10-23T11:00:00Z",
      "updatedAt": "2025-10-23T11:00:00Z"
    }
  ],
  "Vendas": [
    {
      "id": "8cd43b52-9e26-5g3f-c0d8-2f5g6b7c9e0a",
      "name": "Pedro Souza Lima",
      "phone": "+5511923456789",
      "department": "Vendas",
      "startDate": "2025-11-22T08:00:00Z",
      "isActive": false,
      "createdAt": "2025-10-23T12:15:00Z",
      "updatedAt": "2025-10-23T12:15:00Z"
    }
  ]
}
```

---

## ⚠️ Tratamento de Erros

Todos os endpoints retornam respostas estruturadas em caso de erro.

### 400 Bad Request - Validação

Ocorre quando os dados enviados não passam nas validações.

**Exemplo - Campo obrigatório faltando:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "The Name field is required."
    ],
    "StartDate": [
      "The StartDate field must be a future date."
    ]
  }
}
```

**Exemplo - Dados inválidos:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Phone": [
      "Phone must be between 10 and 15 characters.",
      "Phone must contain only digits and optional '+' prefix."
    ],
    "Department": [
      "Department must be at least 2 characters long."
    ]
  }
}
```

---

### 401 Unauthorized - Não Autenticado

Ocorre quando o token JWT não é fornecido ou é inválido.

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authorization header is missing or invalid token"
}
```

**Causas comuns:**
- Header `Authorization` não enviado
- Token JWT expirado
- Token JWT malformado ou inválido
- Refresh token inválido ou expirado

**Solução:**
1. Fazer login novamente (`POST /api/auth/login`)
2. Ou usar refresh token (`POST /api/auth/refresh`)

---

### 404 Not Found - Recurso Não Encontrado

Ocorre quando o recurso solicitado não existe.

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "detail": "Employee with id '3fa85f64-5717-4562-b3fc-2c963f66afa6' not found"
}
```

**Causas comuns:**
- ID do funcionário inválido ou não existe
- Funcionário foi deletado
- Endpoint incorreto

---

### 409 Conflict - Conflito de Dados

Ocorre quando há conflito com dados existentes.

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflict",
  "status": 409,
  "detail": "User with email 'usuario@example.com' already exists"
}
```

**Causas comuns:**
- Email já registrado no sistema
- Dados duplicados

---

### 500 Internal Server Error - Erro Interno

Ocorre quando há um erro inesperado no servidor.

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred. Please try again later."
}
```

**Causas comuns:**
- Erro de conexão com banco de dados
- Erro no RabbitMQ
- Erro ao enviar email
- Exceção não tratada

**Ação recomendada:**
- Verificar logs do servidor
- Verificar se todos os serviços (PostgreSQL, RabbitMQ) estão rodando
- Tentar novamente após alguns segundos

---

## 🔔 Notificações em Tempo Real (SignalR)

### Conectar ao Hub

**URL:** `ws://localhost:5002/hubs/employee`

**Conexão via JavaScript:**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5002/hubs/employee")
    .build();

await connection.start();
```

### Juntar-se a um Grupo (Departamento)

```javascript
await connection.invoke("JoinDepartmentGroup", "TI");
```

### Eventos Recebidos

#### EmployeeCreated
Disparado quando um novo funcionário é criado.

```javascript
connection.on("ReceiveEmployeeCreated", (notification) => {
    console.log("Novo funcionário:", notification);
});
```

**Payload:**
```json
{
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "employeeName": "João Silva Santos",
  "department": "TI",
  "startDate": "2025-11-15T08:00:00Z",
  "eventType": "EmployeeCreated",
  "timestamp": "2025-10-23T10:30:00Z"
}
```

#### EmployeeActivated
Disparado quando um funcionário é ativado (na data de início).

```javascript
connection.on("ReceiveEmployeeActivated", (notification) => {
    console.log("Funcionário ativado:", notification);
});
```

**Payload:**
```json
{
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "employeeName": "João Silva Santos",
  "department": "TI",
  "startDate": null,
  "eventType": "EmployeeActivated",
  "timestamp": "2025-11-15T08:00:00Z"
}
```

#### StartDateUpdated
Disparado quando a data de início é alterada.

```javascript
connection.on("ReceiveStartDateUpdated", (notification) => {
    console.log("Data de início atualizada:", notification);
});
```

**Payload:**
```json
{
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "employeeName": "João Silva Santos",
  "department": "TI",
  "startDate": "2025-12-01T08:00:00Z",
  "eventType": "StartDateUpdated",
  "timestamp": "2025-10-23T15:00:00Z"
}
```

## 🧪 Testes

### Cobertura de Testes

O projeto possui **99 testes unitários** cobrindo todas as camadas da aplicação:

| Projeto | Testes | Cobertura |
|---------|--------|-----------|
| Cadastro.Application.Tests | 88 | Services, Handlers, Validators |
| Notificacoes.Application.Tests | 7 | NotificationService, SignalR |
| Ativacao.Infrastructure.Tests | 4 | EmployeeActivationJob |
| **TOTAL** | **99** | - |

### Executar Testes Unitários

```bash
# Rodar todos os testes
dotnet test

# Rodar com detalhes
dotnet test --verbosity normal

# Rodar testes de um projeto específico
dotnet test tests/EmployeeManagement.Cadastro.Application.Tests

# Rodar com coverage
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover
```

### Testes de Carga com K6

O projeto inclui 3 suites de testes de carga:

**1. auth-load-test.js** - Testes de autenticação
- 50 VUs simultâneos
- Duração: 20s
- Threshold: 95% < 500ms

**2. employee-load-test.js** - CRUD completo
- Stages: 10 → 50 → 100 → 0 VUs
- Testa: Create, Read, Update, Delete
- Threshold: 95% < 2s, falhas < 10%

**3. activation-batch-test.js** - Ativação em lote
- Simula processamento batch
- Consultas por período e departamento
- Peak: 100 VUs

```bash
# Executar testes K6
k6 run tests/LoadTests/auth-load-test.js
k6 run tests/LoadTests/employee-load-test.js
k6 run tests/LoadTests/activation-batch-test.js

# Com parâmetros customizados
k6 run --env VUS=100 --env DURATION=60s tests/LoadTests/employee-load-test.js
```

Documentação completa: [tests/LoadTests/README.md](tests/LoadTests/README.md)

---

## 🔄 CI/CD Pipeline

Pipeline automatizada com **GitHub Actions** implementando as melhores práticas:

### Workflows

**1. deploy.yml** - Pipeline Principal
- ✅ Build e compilação (.NET 9.0)
- ✅ Testes unitários (99 testes)
- ✅ Análise de código SonarCloud
- ✅ Análise de segurança (Trivy)
- ✅ Build de imagens Docker
- ✅ Push para Docker Hub
- ✅ Deploy automático

**Triggers:**
- Push para `main` branch
- Pull requests
- Manual (workflow_dispatch)

**Secrets necessários:**
```
SONAR_TOKEN          # Token do SonarCloud
DOCKER_USERNAME      # Usuario Docker Hub
DOCKER_PASSWORD      # Senha Docker Hub
```

### SonarCloud

**Quality Gates:**
- Cobertura de código > 80%
- Code Smells: Rating A
- Bugs: 0
- Vulnerabilidades: 0
- Duplicação < 3%

**Métricas analisadas:**
- Reliability (Bugs)
- Security (Vulnerabilities)
- Maintainability (Code Smells)
- Coverage (Testes)
- Duplications

### Segurança

**Trivy Scanning:**
- Scan de vulnerabilidades em imagens Docker
- Severidade: CRITICAL, HIGH
- Relatórios em SARIF format
- Upload para GitHub Security

**Análise de dependências:**
- Scan automático de pacotes NuGet
- Detecção de CVEs
- Alertas de segurança

---

## 🏛️ Arquitetura e Fluxos

### Clean Architecture

Cada microsserviço segue Clean Architecture com 4 camadas:

```
┌─────────────────────────────────────────┐
│           API / Presentation            │  ← Controllers, Middlewares
├─────────────────────────────────────────┤
│            Application                  │  ← Use Cases, DTOs, Handlers
├─────────────────────────────────────────┤
│              Domain                     │  ← Entities, Interfaces
├─────────────────────────────────────────┤
│          Infrastructure                 │  ← Data Access, External Services
└─────────────────────────────────────────┘
```

**Princípios:**
- Dependency Inversion (DI)
- Separation of Concerns
- Single Responsibility
- Testabilidade

### Fluxo de Criação de Funcionário

```
┌──────────┐       ┌─────────────┐       ┌──────────────┐
│  Client  │──1──>│ Cadastro API│──2──>│  PostgreSQL  │
└──────────┘       └─────────────┘       └──────────────┘
                          │
                          ├──3──> Email Service
                          │
                          ├──4──> RabbitMQ ──5──> Ativação Worker
                          │
                          └──6──> SignalR Hub ──7──> Frontend (Dept)
```

**Passos:**
1. Cliente envia POST /api/employees
2. Dados salvos no PostgreSQL
3. Email de notificação enviado
4. Mensagem publicada no RabbitMQ
5. Worker consome mensagem e agenda job Hangfire
6. Notificação SignalR enviada
7. Frontend do departamento recebe notificação em tempo real

### Fluxo de Ativação Automática

```
┌──────────────┐     ┌────────────┐     ┌──────────────┐
│ Hangfire Job │────>│  Database  │────>│  RabbitMQ    │
│  (Cron 1h)   │     │   Query    │     │   Publish    │
└──────────────┘     └────────────┘     └──────────────┘
                                               │
                                               v
                                        ┌──────────────┐
                                        │ SignalR Hub  │
                                        │  Notifica    │
                                        └──────────────┘
```

**Lógica:**
1. Job Hangfire executa a cada 1 hora
2. Busca funcionários com StartDate <= hoje e IsActive = false
3. Processa em lotes de 100 (se > 1000 funcionários)
4. Atualiza IsActive = true
5. Publica evento EmployeeActivated no RabbitMQ
6. Envia notificação SignalR para departamento

### Comunicação entre Microsserviços

```
┌─────────────┐                    ┌──────────────┐
│  Cadastro   │◄──────────────────►│  PostgreSQL  │
└──────┬──────┘                    └──────────────┘
       │
       │ Publish Events
       ▼
┌─────────────┐
│  RabbitMQ   │
│   Exchange  │
└──────┬──────┘
       │
       │ Subscribe
       ▼
┌─────────────┐                    ┌──────────────┐
│  Ativação   │◄──────────────────►│  PostgreSQL  │
│   Worker    │                    │   (Cadastro) │
└─────────────┘                    └──────────────┘

┌─────────────┐
│Notificações │◄─── SignalR ───────► Frontend
└─────────────┘
```

**Padrões utilizados:**
- Event-Driven Architecture
- Message Queue (RabbitMQ)
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Unit of Work
- Dependency Injection

---

## 📚 Documentação Adicional

### Arquivos de Referência

- **[FAST_TRACK_PLAN.md](FAST_TRACK_PLAN.md)** - Plano de implementação rápida (6.5h)
- **[tests/LoadTests/README.md](tests/LoadTests/README.md)** - Documentação completa dos testes K6
- **[.github/workflows/README.md](.github/workflows/README.md)** - Documentação da pipeline CI/CD
- **[.github/workflows/deploy.yml](.github/workflows/deploy.yml)** - Workflow principal

### Swagger UI

Após iniciar a aplicação, acesse a documentação interativa:

- **Cadastro API**: http://localhost:5001/swagger
- **Notificações API**: http://localhost:5002/swagger

### Tecnologias e Bibliotecas

**Backend:**
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- MassTransit (RabbitMQ)
- Hangfire (Background Jobs)
- SignalR (Real-time)
- FluentValidation
- AutoMapper
- Serilog

**Testes:**
- xUnit
- Moq
- FluentAssertions
- K6 (Load Testing)

**Infraestrutura:**
- PostgreSQL 16
- RabbitMQ 3.12
- Docker & Docker Compose
- GitHub Actions

**Qualidade:**
- SonarCloud
- Trivy Security Scanner

---

## 🎯 Requisitos Atendidos

### Funcionalidades Implementadas

✅ **Cadastro de Funcionários**
- CRUD completo
- Validações de dados
- Paginação

✅ **Consultas e Relatórios**
- Filtro por período (date range)
- Agrupamento por departamento
- Busca por ID

✅ **Notificações**
- Email automático na criação
- SignalR para notificações em tempo real
- Eventos por departamento

✅ **Ativação Automática**
- Job Hangfire executando a cada 1h
- Processamento em lotes (100 itens)
- Ativação na data de início

✅ **Autenticação e Segurança**
- JWT com Refresh Token
- ASP.NET Identity
- Proteção de endpoints

✅ **Arquitetura**
- Clean Architecture
- Microsserviços
- Event-Driven
- Docker/Docker Compose

✅ **Testes**
- 99 testes unitários
- Testes de carga K6
- Cobertura > 80%

✅ **CI/CD**
- Pipeline GitHub Actions
- SonarCloud
- Security Scanning
- Deploy automático

---

## 🤝 Contribuindo

Este projeto foi desenvolvido como parte de um desafio técnico.

Para contribuir:
1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -m 'feat: adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

**Padrões de Commit:**
- `feat:` Nova funcionalidade
- `fix:` Correção de bug
- `docs:` Documentação
- `test:` Testes
- `refactor:` Refatoração
- `chore:` Manutenção

---

## 📞 Suporte

Para dúvidas ou problemas:
- Abra uma [Issue](https://github.com/seu-usuario/employee-management-microservice/issues)
- Consulte a documentação do Swagger
- Veja os exemplos em [tests/LoadTests/](tests/LoadTests/)

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

**Desenvolvido com .NET 9.0 | Clean Architecture | Microservices**
