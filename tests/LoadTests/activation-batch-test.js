import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';
import { getAuthToken, BASE_URL, ENDPOINTS } from './utils/helpers.js';

// Configuração de carga
export const options = {
  stages: [
    { duration: '30s', target: 10 },  // Warm-up
    { duration: '1m', target: 50 },   // Carga normal
    { duration: '1m', target: 100 },  // Pico de carga (simula ativação em lote)
    { duration: '30s', target: 0 },   // Cool-down
  ],
  thresholds: {
    http_req_duration: ['p(95)<2000'], // 95% das requests abaixo de 2s
    http_req_failed: ['rate<0.1'],     // Menos de 10% de falhas
  },
};

// Dados compartilhados entre VUs
const departments = new SharedArray('departments', function () {
  return ['TI', 'RH', 'Vendas', 'Marketing', 'Financeiro'];
});

let authToken;

export function setup() {
  // Autentica uma vez antes de iniciar o teste
  const token = getAuthToken();
  console.log('Token de autenticação obtido para teste de ativação em lote');
  return { token };
}

export default function (data) {
  authToken = data.token;

  // Simula consulta de funcionários por intervalo de datas (preparação para ativação)
  testGetEmployeesByDateRange();
  sleep(1);

  // Simula consulta de funcionários agrupados por departamento
  testGetEmployeesGroupedByDepartment();
  sleep(1);

  // Simula criação de múltiplos funcionários (preparação para lote)
  if (__VU % 5 === 0) { // Apenas 20% dos VUs criam funcionários
    testBatchEmployeeCreation();
    sleep(2);
  }
}

function testGetEmployeesByDateRange() {
  const today = new Date();
  const startDate = new Date(today);
  startDate.setDate(today.getDate() - 30); // 30 dias atrás

  const endDate = new Date(today);
  endDate.setDate(today.getDate() + 30); // 30 dias à frente

  const url = `${BASE_URL}${ENDPOINTS.EMPLOYEES_BY_DATE_RANGE}?startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`;

  const params = {
    headers: {
      'Authorization': `Bearer ${authToken}`,
      'Content-Type': 'application/json',
    },
  };

  const response = http.get(url, params);

  check(response, {
    'get by date range status is 200': (r) => r.status === 200,
    'get by date range response is array': (r) => {
      try {
        return Array.isArray(JSON.parse(r.body));
      } catch {
        return false;
      }
    },
  });
}

function testGetEmployeesGroupedByDepartment() {
  const today = new Date();
  const startDate = new Date(today);
  startDate.setDate(today.getDate() - 60);

  const endDate = new Date(today);
  endDate.setDate(today.getDate() + 60);

  const url = `${BASE_URL}${ENDPOINTS.EMPLOYEES_GROUPED}?startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`;

  const params = {
    headers: {
      'Authorization': `Bearer ${authToken}`,
      'Content-Type': 'application/json',
    },
  };

  const response = http.get(url, params);

  check(response, {
    'grouped by department status is 200': (r) => r.status === 200,
    'grouped response has departments': (r) => {
      try {
        const data = JSON.parse(r.body);
        return Array.isArray(data) && data.length >= 0;
      } catch {
        return false;
      }
    },
  });
}

function testBatchEmployeeCreation() {
  // Cria 5 funcionários de uma vez (simula preparação para ativação em lote)
  const batchSize = 5;
  const today = new Date();

  for (let i = 0; i < batchSize; i++) {
    const startDate = new Date(today);
    startDate.setDate(today.getDate() + Math.floor(Math.random() * 30) + 1); // Entre 1-30 dias

    const employee = {
      name: `Employee Batch ${__VU}-${__ITER}-${i}`,
      phone: `+5511${Math.floor(Math.random() * 900000000 + 100000000)}`,
      department: departments[Math.floor(Math.random() * departments.length)],
      startDate: startDate.toISOString(),
    };

    const params = {
      headers: {
        'Authorization': `Bearer ${authToken}`,
        'Content-Type': 'application/json',
      },
    };

    const response = http.post(
      `${BASE_URL}${ENDPOINTS.EMPLOYEES}`,
      JSON.stringify(employee),
      params
    );

    check(response, {
      'batch create status is 201': (r) => r.status === 201,
      'batch create returns employee': (r) => {
        try {
          const data = JSON.parse(r.body);
          return data.id && data.name === employee.name;
        } catch {
          return false;
        }
      },
    });

    sleep(0.5); // Pequeno delay entre criações
  }
}

export function teardown(data) {
  console.log('Teste de ativação em lote concluído');
}
