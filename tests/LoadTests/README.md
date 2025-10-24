# Testes de Carga K6 - Employee Management

Este diretório contém os testes de carga usando [K6](https://k6.io/) para o sistema de gerenciamento de funcionários.

## Estrutura

```
LoadTests/
├── auth-load-test.js           # Testes de autenticação
├── employee-load-test.js       # Testes CRUD de funcionários
├── activation-batch-test.js    # Testes de ativação em lote
├── utils/
│   ├── config.js              # Configurações centralizadas
│   └── helpers.js             # Funções auxiliares
└── README.md                  # Este arquivo
```

## Pré-requisitos

### Instalação do K6

**Windows (Chocolatey):**
```bash
choco install k6
```

**macOS (Homebrew):**
```bash
brew install k6
```

**Linux:**
```bash
sudo gpg -k
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6
```

Ou via Docker:
```bash
docker pull grafana/k6
```

## Testes Disponíveis

### 1. auth-load-test.js
Testa a API de autenticação (login).

**Cenários:**
- Login simultâneo de múltiplos usuários
- Validação de token JWT
- Performance de autenticação

**Configuração:**
- VUs padrão: 50
- Duração padrão: 20s
- Threshold: 95% das requests abaixo de 500ms

**Executar:**
```bash
# Execução básica
k6 run tests/LoadTests/auth-load-test.js

# Com variáveis de ambiente
k6 run --env BASE_URL=http://localhost:5001 tests/LoadTests/auth-load-test.js

# Customizar VUs e duração
k6 run --env VUS=100 --env DURATION=30s tests/LoadTests/auth-load-test.js
```

### 2. employee-load-test.js
Testa operações CRUD de funcionários.

**Cenários:**
- Criar funcionários
- Listar funcionários
- Buscar por ID
- Atualizar data de início
- Deletar funcionários

**Configuração:**
- Stages:
  - Warm-up: 30s → 10 VUs
  - Carga normal: 1m → 50 VUs
  - Pico: 1m → 100 VUs
  - Cool-down: 30s → 0 VUs

**Thresholds:**
- 95% requests < 2s
- Taxa de falha < 10%

**Executar:**
```bash
# Execução básica
k6 run tests/LoadTests/employee-load-test.js

# Com URL customizada
k6 run --env BASE_URL=http://localhost:5001 tests/LoadTests/employee-load-test.js

# Com credenciais customizadas
k6 run --env ADMIN_EMAIL=admin@test.com --env ADMIN_PASSWORD=Test@123 tests/LoadTests/employee-load-test.js
```

### 3. activation-batch-test.js
Testa cenários de ativação em lote.

**Cenários:**
- Consulta por intervalo de datas
- Funcionários agrupados por departamento
- Criação em lote (batch)

**Configuração:**
- Stages:
  - Warm-up: 30s → 10 VUs
  - Carga normal: 1m → 50 VUs
  - Pico: 1m → 100 VUs (simula ativação em lote)
  - Cool-down: 30s → 0 VUs

**Executar:**
```bash
k6 run tests/LoadTests/activation-batch-test.js
```

## Cenários de Teste Pré-Configurados

Os cenários estão definidos em `utils/config.js`:

### Smoke Test
Teste básico com 1 VU por 30 segundos.
```bash
k6 run --env SCENARIO=smoke tests/LoadTests/employee-load-test.js
```

### Average Load
Carga média: 10 → 50 → 0 VUs em 2 minutos.
```bash
k6 run --env SCENARIO=average tests/LoadTests/employee-load-test.js
```

### Spike Test
Teste de pico: 10 → 200 → 10 VUs em 50 segundos.
```bash
k6 run --env SCENARIO=spike tests/LoadTests/employee-load-test.js
```

### Stress Test
Teste de stress: 50 → 100 → 200 → 0 VUs em 6 minutos.
```bash
k6 run --env SCENARIO=stress tests/LoadTests/employee-load-test.js
```

### Soak Test
Teste de longa duração: 50 VUs por 10 minutos.
```bash
k6 run --env SCENARIO=soak tests/LoadTests/employee-load-test.js
```

## Variáveis de Ambiente

Todas as variáveis podem ser customizadas via `--env`:

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| `BASE_URL` | URL base da API de Cadastro | `http://localhost:5001/api` |
| `NOTIFICATIONS_URL` | URL da API de Notificações | `http://localhost:5002/api` |
| `ADMIN_EMAIL` | Email do usuário admin | `admin@example.com` |
| `ADMIN_PASSWORD` | Senha do usuário admin | `Admin@123` |
| `DEMO_EMAIL` | Email do usuário demo | `demo@example.com` |
| `DEMO_PASSWORD` | Senha do usuário demo | `Demo@123` |
| `VUS` | Número de VUs (apenas auth-load-test) | `50` |
| `DURATION` | Duração do teste (apenas auth-load-test) | `20s` |

**Exemplo com múltiplas variáveis:**
```bash
k6 run \
  --env BASE_URL=http://production-api.com \
  --env ADMIN_EMAIL=admin@prod.com \
  --env ADMIN_PASSWORD=SecurePass@123 \
  tests/LoadTests/employee-load-test.js
```

## Funções Auxiliares (utils/helpers.js)

### Autenticação
- `getAuthToken(userType)` - Obtém token JWT
- `registerUser(email, password)` - Registra novo usuário

### HTTP Helpers
- `authenticatedGet(endpoint, token)` - GET autenticado
- `authenticatedPost(endpoint, payload, token)` - POST autenticado
- `authenticatedPut(endpoint, payload, token)` - PUT autenticado
- `authenticatedDelete(endpoint, token)` - DELETE autenticado

### Geração de Dados
- `generateEmployeePayload()` - Gera payload de funcionário
- `generateRandomName()` - Gera nome aleatório
- `generatePhoneNumber()` - Gera telefone brasileiro
- `generateFutureStartDate(minDays, maxDays)` - Gera data futura
- `generateUniqueEmail(prefix)` - Gera email único

### Validações
- `isValidEmployee(employee)` - Valida estrutura de Employee
- `isValidJwt(token)` - Valida formato JWT
- `isValidJson(response)` - Valida JSON do response

### Utilidades
- `replaceEndpointId(endpoint, id)` - Substitui {id} em endpoints
- `safeJsonParse(response, key)` - Parse JSON seguro
- `randomSleep(baseSeconds, variationPercent)` - Sleep com variação

**Exemplo de uso:**
```javascript
import { getAuthToken, generateEmployeePayload, authenticatedPost } from './utils/helpers.js';

export default function() {
  // Autentica
  const token = getAuthToken('admin');

  // Gera funcionário aleatório
  const employee = generateEmployeePayload();

  // Cria funcionário
  const response = authenticatedPost('/employees', employee, token);
}
```

## Configurações (utils/config.js)

### Endpoints
Todos os endpoints da API estão mapeados:
```javascript
import { ENDPOINTS } from './utils/config.js';

// Autenticação
ENDPOINTS.LOGIN        // /auth/login
ENDPOINTS.REGISTER     // /auth/register
ENDPOINTS.REFRESH      // /auth/refresh

// Funcionários
ENDPOINTS.EMPLOYEES                  // /employees
ENDPOINTS.EMPLOYEES_BY_ID            // /employees/{id}
ENDPOINTS.EMPLOYEES_BY_DATE_RANGE    // /employees/by-date-range
ENDPOINTS.EMPLOYEES_GROUPED          // /employees/grouped-by-department
ENDPOINTS.UPDATE_START_DATE          // /employees/{id}/start-date

// Notificações
ENDPOINTS.NOTIFICATIONS  // /notifications
```

### Performance Thresholds
```javascript
import { PERFORMANCE_THRESHOLDS } from './utils/config.js';

export const options = {
  thresholds: PERFORMANCE_THRESHOLDS
};
```

### Dados de Teste
```javascript
import { TEST_DATA } from './utils/config.js';

TEST_DATA.departments   // ['TI', 'RH', 'Vendas', ...]
TEST_DATA.firstNames    // ['João', 'Maria', 'Pedro', ...]
TEST_DATA.lastNames     // ['Silva', 'Santos', 'Oliveira', ...]
TEST_DATA.phonePrefix   // '+5511'
```

## Análise de Resultados

### Executar com Saída JSON
```bash
k6 run --out json=results.json tests/LoadTests/employee-load-test.js
```

### Executar com Dashboard Web
```bash
k6 run --out web-dashboard tests/LoadTests/employee-load-test.js
```

### Métricas Importantes

**HTTP Request Duration:**
- `p(50)`: 50% das requests abaixo deste tempo
- `p(90)`: 90% das requests abaixo deste tempo
- `p(95)`: 95% das requests abaixo deste tempo
- `p(99)`: 99% das requests abaixo deste tempo

**HTTP Request Failed:**
- Taxa de falhas das requisições
- Threshold: < 10%

**HTTP Requests:**
- Requisições por segundo
- Threshold: > 50 req/s

**Exemplo de saída:**
```
     ✓ login succeeded
     ✓ employee created
     ✓ employee retrieved

     checks.........................: 100.00% ✓ 15000      ✗ 0
     data_received..................: 45 MB   750 kB/s
     data_sent......................: 30 MB   500 kB/s
     http_req_duration..............: avg=120ms min=50ms med=100ms max=500ms p(90)=200ms p(95)=300ms
     http_req_failed................: 0.00%   ✓ 0          ✗ 15000
     http_reqs......................: 15000   250/s
     iteration_duration.............: avg=1.2s  min=1s med=1.1s max=2s
     vus............................: 50      min=10       max=100
```

## Executar com Docker

Se preferir usar K6 via Docker:

```bash
# auth-load-test
docker run --rm -i \
  --network host \
  -v ${PWD}/tests/LoadTests:/scripts \
  grafana/k6 run /scripts/auth-load-test.js

# employee-load-test
docker run --rm -i \
  --network host \
  -v ${PWD}/tests/LoadTests:/scripts \
  grafana/k6 run /scripts/employee-load-test.js

# activation-batch-test
docker run --rm -i \
  --network host \
  -v ${PWD}/tests/LoadTests:/scripts \
  grafana/k6 run /scripts/activation-batch-test.js
```

## Boas Práticas

### 1. Warm-up e Cool-down
Sempre inclua stages de warm-up e cool-down para evitar picos abruptos:
```javascript
stages: [
  { duration: '30s', target: 10 },   // Warm-up
  { duration: '1m', target: 50 },    // Carga real
  { duration: '30s', target: 0 },    // Cool-down
]
```

### 2. Think Time
Use `sleep()` entre requisições para simular comportamento real:
```javascript
import { sleep } from 'k6';

export default function() {
  // ... fazer requisição
  sleep(1);  // Usuário "pensando" por 1 segundo
}
```

### 3. Checks vs Thresholds
- **Checks**: Validações individuais (não param o teste)
- **Thresholds**: Critérios de sucesso/falha (param o teste se não atingidos)

### 4. Tags
Use tags para organizar métricas:
```javascript
const params = {
  tags: { name: 'CreateEmployee', operation: 'POST' }
};
```

### 5. Dados Realistas
Use os helpers para gerar dados realistas:
```javascript
const employee = generateEmployeePayload({
  department: 'TI'  // Customizar apenas o necessário
});
```

## Troubleshooting

### Erro: "connection refused"
- Verifique se a API está rodando
- Verifique a URL configurada em `BASE_URL`

### Erro: "401 Unauthorized"
- Verifique as credenciais em `TEST_USERS`
- Verifique se o usuário foi seedado no banco

### Performance ruim
- Reduza número de VUs
- Aumente duração dos stages
- Verifique recursos do servidor

### Muitas falhas
- Ajuste os thresholds
- Verifique logs da API
- Reduza carga (VUs)

## Integração CI/CD

### GitHub Actions
Exemplo de step para executar K6 no pipeline:

```yaml
- name: Run Load Tests
  run: |
    k6 run --out json=results.json tests/LoadTests/employee-load-test.js
  env:
    BASE_URL: http://localhost:5001/api
```

### Threshold como Gate de Qualidade
Use thresholds para falhar o pipeline se performance não for aceitável:
```javascript
export const options = {
  thresholds: {
    'http_req_duration': ['p(95)<2000'],  // Falha se p95 > 2s
    'http_req_failed': ['rate<0.1'],      // Falha se > 10% de erros
  }
};
```

## Referências

- [Documentação K6](https://k6.io/docs/)
- [K6 Examples](https://k6.io/docs/examples/)
- [HTTP API Testing](https://k6.io/docs/using-k6/http-requests/)
- [Thresholds](https://k6.io/docs/using-k6/thresholds/)
- [Checks](https://k6.io/docs/using-k6/checks/)
