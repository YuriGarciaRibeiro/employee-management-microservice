// Configuração centralizada para testes de carga K6

// URLs base dos serviços
export const BASE_URL = __ENV.BASE_URL || 'http://localhost:5001/api';
export const NOTIFICATIONS_URL = __ENV.NOTIFICATIONS_URL || 'http://localhost:5002/api';

// Credenciais de teste
export const TEST_USERS = {
  admin: {
    email: __ENV.ADMIN_EMAIL || 'admin@example.com',
    password: __ENV.ADMIN_PASSWORD || 'Admin@123',
  },
  demo: {
    email: __ENV.DEMO_EMAIL || 'demo@example.com',
    password: __ENV.DEMO_PASSWORD || 'Demo@123',
  },
};

// Endpoints da API
export const ENDPOINTS = {
  // Autenticação
  REGISTER: '/auth/register',
  LOGIN: '/auth/login',
  REFRESH: '/auth/refresh',

  // Employees
  EMPLOYEES: '/employees',
  EMPLOYEES_BY_ID: '/employees/{id}',
  EMPLOYEES_BY_DATE_RANGE: '/employees/by-date-range',
  EMPLOYEES_GROUPED: '/employees/grouped-by-department',
  UPDATE_START_DATE: '/employees/{id}/start-date',

  // Notificações
  NOTIFICATIONS: '/notifications',
};

// Configurações de performance
export const PERFORMANCE_THRESHOLDS = {
  // Duração de requisição
  http_req_duration: {
    p50: 500,   // 50% abaixo de 500ms
    p90: 1000,  // 90% abaixo de 1s
    p95: 2000,  // 95% abaixo de 2s
    p99: 5000,  // 99% abaixo de 5s
  },

  // Taxa de falha
  http_req_failed: {
    rate: 0.1,  // Máximo 10% de falhas
  },

  // Requisições por segundo
  http_reqs: {
    rate: 50,   // Mínimo 50 req/s
  },
};

// Cenários de carga pré-configurados
export const LOAD_SCENARIOS = {
  // Teste de fumaça (smoke test)
  smoke: {
    stages: [
      { duration: '30s', target: 1 },
    ],
  },

  // Teste de carga média
  average: {
    stages: [
      { duration: '30s', target: 10 },
      { duration: '1m', target: 50 },
      { duration: '30s', target: 0 },
    ],
  },

  // Teste de pico
  spike: {
    stages: [
      { duration: '10s', target: 10 },
      { duration: '30s', target: 200 },
      { duration: '10s', target: 10 },
    ],
  },

  // Teste de stress
  stress: {
    stages: [
      { duration: '1m', target: 50 },
      { duration: '2m', target: 100 },
      { duration: '2m', target: 200 },
      { duration: '1m', target: 0 },
    ],
  },

  // Teste de soak (longa duração)
  soak: {
    stages: [
      { duration: '2m', target: 50 },
      { duration: '10m', target: 50 },
      { duration: '2m', target: 0 },
    ],
  },
};

// Dados de teste
export const TEST_DATA = {
  departments: ['TI', 'RH', 'Vendas', 'Marketing', 'Financeiro', 'Operações', 'Logística'],

  phonePrefix: '+5511',

  // Nomes para geração de dados
  firstNames: [
    'João', 'Maria', 'Pedro', 'Ana', 'Carlos', 'Juliana', 'Rafael', 'Fernanda',
    'Lucas', 'Mariana', 'Gabriel', 'Beatriz', 'Felipe', 'Camila', 'Bruno', 'Amanda'
  ],

  lastNames: [
    'Silva', 'Santos', 'Oliveira', 'Souza', 'Costa', 'Ferreira', 'Rodrigues', 'Almeida',
    'Pereira', 'Lima', 'Gomes', 'Martins', 'Rocha', 'Ribeiro', 'Carvalho', 'Araújo'
  ],
};

// Configurações de delay
export const DELAYS = {
  think_time: 1,      // Tempo entre ações (segundos)
  between_requests: 0.5, // Delay entre requisições (segundos)
  after_error: 2,     // Delay após erro (segundos)
};

// Configurações de retry
export const RETRY_CONFIG = {
  max_retries: 3,
  backoff_ms: 1000,
};

// Headers padrão
export const DEFAULT_HEADERS = {
  'Content-Type': 'application/json',
  'Accept': 'application/json',
};

// Exporta tudo como default também para facilitar imports
export default {
  BASE_URL,
  NOTIFICATIONS_URL,
  TEST_USERS,
  ENDPOINTS,
  PERFORMANCE_THRESHOLDS,
  LOAD_SCENARIOS,
  TEST_DATA,
  DELAYS,
  RETRY_CONFIG,
  DEFAULT_HEADERS,
};
