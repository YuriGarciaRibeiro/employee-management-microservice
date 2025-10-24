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

## ⚡ Como Executar

### Pré-requisitos
- .NET 9.0 SDK
- Docker e Docker Compose

### Executar com Docker Compose

1. Clone o repositório
```bash
git clone https://github.com/seu-usuario/employee-management-microservice.git
cd employee-management-microservice
```

2. Inicie os serviços de infraestrutura
```bash
docker-compose up -d
```

3. Aguarde os serviços estarem prontos
- PostgreSQL: http://localhost:5432
- RabbitMQ Management: http://localhost:15672 (guest/guest)
- SonarQube: http://localhost:9000 (admin/admin)

### Executar Localmente

```bash
# Restaurar pacotes
dotnet restore

# Compilar
dotnet build

# Executar microsserviço de Cadastro
dotnet run --project src/Services/Cadastro/EmployeeManagement.Cadastro.API

# Executar microsserviço de Notificações
dotnet run --project src/Services/Notificacoes/EmployeeManagement.Notificacoes.API

# Executar Worker de Ativação
dotnet run --project src/Services/Ativacao/EmployeeManagement.Ativacao.Worker
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

### Testes Unitários
```bash
dotnet test
```

### Testes de Carga
```bash
cd tests/LoadTests
k6 run employee-load-test.js
```

## 🔄 CI/CD

Pipeline configurada com GitHub Actions:
- Build e compilação
- Testes unitários
- Análise de código (SonarQube)
- Build de imagens Docker
- Deploy automático

## 📚 Documentação Adicional

- [STRUCTURE.md](STRUCTURE.md) - Detalhamento da estrutura do projeto
- [TODO.md](TODO.md) - Lista de tarefas e roadmap

## 🤝 Contribuindo

Este projeto é parte de um desafio técnico para processo seletivo.

## 📄 Licença

MIT
