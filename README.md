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

> EM DESENVOLVIMENTO - Ver documentação Swagger em /swagger

### Cadastro API (Port 5001)
- `POST /api/employees` - Criar funcionário
- `GET /api/employees` - Listar funcionários
- `GET /api/employees/{id}` - Buscar funcionário
- `PUT /api/employees/{id}` - Atualizar funcionário
- `DELETE /api/employees/{id}` - Remover funcionário
- `GET /api/employees/by-date-range` - Filtrar por período
- `GET /api/employees/grouped-by-department` - Agrupar por setor

### Autenticação
- `POST /api/auth/register` - Registrar usuário
- `POST /api/auth/login` - Login
- `POST /api/auth/refresh` - Refresh token

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
