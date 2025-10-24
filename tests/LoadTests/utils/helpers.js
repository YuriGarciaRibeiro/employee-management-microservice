import http from 'k6/http';
import { check } from 'k6';
import {
  BASE_URL,
  TEST_USERS,
  ENDPOINTS,
  DEFAULT_HEADERS,
  TEST_DATA,
  RETRY_CONFIG
} from './config.js';

// ===========================
// AUTENTICAÇÃO
// ===========================

/**
 * Obtém token JWT para autenticação
 * @param {string} userType - Tipo de usuário ('admin' ou 'demo')
 * @returns {string|null} Token JWT ou null se falhar
 */
export function getAuthToken(userType = 'admin') {
  const user = TEST_USERS[userType];

  if (!user) {
    console.error(`User type '${userType}' not found in TEST_USERS`);
    return null;
  }

  const payload = JSON.stringify({
    email: user.email,
    password: user.password,
  });

  const params = {
    headers: DEFAULT_HEADERS,
    tags: { name: 'GetAuthToken' },
  };

  const response = http.post(
    `${BASE_URL}${ENDPOINTS.LOGIN}`,
    payload,
    params
  );

  const success = check(response, {
    'authentication successful': (r) => r.status === 200,
    'token received': (r) => {
      try {
        return r.json('token') !== undefined;
      } catch {
        return false;
      }
    },
  });

  if (success && response.status === 200) {
    try {
      return response.json('token');
    } catch (e) {
      console.error('Failed to parse auth token:', e);
      return null;
    }
  }

  console.error(`Authentication failed for ${userType}:`, response.status, response.body);
  return null;
}

/**
 * Registra um novo usuário
 * @param {string} email - Email do usuário
 * @param {string} password - Senha do usuário
 * @returns {object} Response do registro
 */
export function registerUser(email, password) {
  const payload = JSON.stringify({
    email: email,
    password: password,
  });

  const params = {
    headers: DEFAULT_HEADERS,
    tags: { name: 'RegisterUser' },
  };

  return http.post(
    `${BASE_URL}${ENDPOINTS.REGISTER}`,
    payload,
    params
  );
}

// ===========================
// HTTP HELPERS
// ===========================

/**
 * GET request autenticado
 * @param {string} endpoint - Endpoint da API
 * @param {string} token - Token JWT
 * @param {object} additionalParams - Parâmetros adicionais
 * @returns {object} HTTP Response
 */
export function authenticatedGet(endpoint, token, additionalParams = {}) {
  const params = {
    headers: {
      ...DEFAULT_HEADERS,
      'Authorization': `Bearer ${token}`,
      ...additionalParams.headers,
    },
    tags: additionalParams.tags || {},
  };

  return http.get(`${BASE_URL}${endpoint}`, params);
}

/**
 * POST request autenticado
 * @param {string} endpoint - Endpoint da API
 * @param {object} payload - Dados para envio
 * @param {string} token - Token JWT
 * @param {object} additionalParams - Parâmetros adicionais
 * @returns {object} HTTP Response
 */
export function authenticatedPost(endpoint, payload, token, additionalParams = {}) {
  const params = {
    headers: {
      ...DEFAULT_HEADERS,
      'Authorization': `Bearer ${token}`,
      ...additionalParams.headers,
    },
    tags: additionalParams.tags || {},
  };

  return http.post(
    `${BASE_URL}${endpoint}`,
    JSON.stringify(payload),
    params
  );
}

/**
 * PUT request autenticado
 * @param {string} endpoint - Endpoint da API
 * @param {object} payload - Dados para atualização
 * @param {string} token - Token JWT
 * @param {object} additionalParams - Parâmetros adicionais
 * @returns {object} HTTP Response
 */
export function authenticatedPut(endpoint, payload, token, additionalParams = {}) {
  const params = {
    headers: {
      ...DEFAULT_HEADERS,
      'Authorization': `Bearer ${token}`,
      ...additionalParams.headers,
    },
    tags: additionalParams.tags || {},
  };

  return http.put(
    `${BASE_URL}${endpoint}`,
    JSON.stringify(payload),
    params
  );
}

/**
 * DELETE request autenticado
 * @param {string} endpoint - Endpoint da API
 * @param {string} token - Token JWT
 * @param {object} additionalParams - Parâmetros adicionais
 * @returns {object} HTTP Response
 */
export function authenticatedDelete(endpoint, token, additionalParams = {}) {
  const params = {
    headers: {
      ...DEFAULT_HEADERS,
      'Authorization': `Bearer ${token}`,
      ...additionalParams.headers,
    },
    tags: additionalParams.tags || {},
  };

  return http.del(`${BASE_URL}${endpoint}`, null, params);
}

// ===========================
// GERAÇÃO DE DADOS
// ===========================

/**
 * Gera um payload de funcionário aleatório
 * @param {object} overrides - Valores para sobrescrever
 * @returns {object} Employee payload
 */
export function generateEmployeePayload(overrides = {}) {
  const firstName = generateRandomFirstName();
  const lastName = generateRandomLastName();
  const department = generateRandomDepartment();
  const phone = generatePhoneNumber();
  const startDate = generateFutureStartDate();

  return {
    name: `${firstName} ${lastName}`,
    phone: phone,
    department: department,
    startDate: startDate.toISOString(),
    ...overrides,
  };
}

/**
 * Gera um nome aleatório
 * @returns {string} Nome completo
 */
export function generateRandomName() {
  const firstName = generateRandomFirstName();
  const lastName = generateRandomLastName();
  return `${firstName} ${lastName}`;
}

/**
 * Gera um primeiro nome aleatório
 * @returns {string} Primeiro nome
 */
export function generateRandomFirstName() {
  return TEST_DATA.firstNames[Math.floor(Math.random() * TEST_DATA.firstNames.length)];
}

/**
 * Gera um sobrenome aleatório
 * @returns {string} Sobrenome
 */
export function generateRandomLastName() {
  return TEST_DATA.lastNames[Math.floor(Math.random() * TEST_DATA.lastNames.length)];
}

/**
 * Gera um departamento aleatório
 * @returns {string} Departamento
 */
export function generateRandomDepartment() {
  return TEST_DATA.departments[Math.floor(Math.random() * TEST_DATA.departments.length)];
}

/**
 * Gera um número de telefone brasileiro válido
 * @returns {string} Número de telefone
 */
export function generatePhoneNumber() {
  const number = Math.floor(Math.random() * 900000000 + 100000000);
  return `${TEST_DATA.phonePrefix}${number}`;
}

/**
 * Gera uma data de início futura (entre 1 e 90 dias)
 * @param {number} minDays - Mínimo de dias no futuro (padrão: 1)
 * @param {number} maxDays - Máximo de dias no futuro (padrão: 90)
 * @returns {Date} Data de início
 */
export function generateFutureStartDate(minDays = 1, maxDays = 90) {
  const today = new Date();
  const daysToAdd = Math.floor(Math.random() * (maxDays - minDays + 1)) + minDays;
  const futureDate = new Date(today);
  futureDate.setDate(today.getDate() + daysToAdd);
  return futureDate;
}

/**
 * Gera uma data no passado (entre 1 e 90 dias atrás)
 * @param {number} minDays - Mínimo de dias no passado (padrão: 1)
 * @param {number} maxDays - Máximo de dias no passado (padrão: 90)
 * @returns {Date} Data passada
 */
export function generatePastDate(minDays = 1, maxDays = 90) {
  const today = new Date();
  const daysToSubtract = Math.floor(Math.random() * (maxDays - minDays + 1)) + minDays;
  const pastDate = new Date(today);
  pastDate.setDate(today.getDate() - daysToSubtract);
  return pastDate;
}

/**
 * Gera um email único para testes
 * @param {string} prefix - Prefixo do email (padrão: 'test')
 * @returns {string} Email único
 */
export function generateUniqueEmail(prefix = 'test') {
  const timestamp = Date.now();
  const random = Math.floor(Math.random() * 10000);
  return `${prefix}.${timestamp}.${random}@example.com`;
}

// ===========================
// UTILIDADES
// ===========================

/**
 * Formata uma data para ISO string sem milissegundos
 * @param {Date} date - Data para formatar
 * @returns {string} Data formatada
 */
export function formatDateISO(date) {
  return date.toISOString().split('.')[0] + 'Z';
}

/**
 * Sleep com variação aleatória
 * @param {number} baseSeconds - Segundos base
 * @param {number} variationPercent - Percentual de variação (0-100)
 * @returns {number} Segundos ajustados
 */
export function randomSleep(baseSeconds, variationPercent = 20) {
  const variation = (variationPercent / 100) * baseSeconds;
  const randomFactor = (Math.random() * 2 - 1) * variation;
  return Math.max(0.1, baseSeconds + randomFactor);
}

/**
 * Extrai ID de um endpoint
 * @param {string} endpoint - Endpoint com {id}
 * @param {string} id - ID para substituir
 * @returns {string} Endpoint com ID
 */
export function replaceEndpointId(endpoint, id) {
  return endpoint.replace('{id}', id);
}

/**
 * Valida response JSON
 * @param {object} response - HTTP Response
 * @returns {boolean} True se JSON válido
 */
export function isValidJson(response) {
  try {
    JSON.parse(response.body);
    return true;
  } catch {
    return false;
  }
}

/**
 * Extrai valor JSON do response
 * @param {object} response - HTTP Response
 * @param {string} key - Chave para extrair (opcional)
 * @returns {*} Valor extraído ou null
 */
export function safeJsonParse(response, key = null) {
  try {
    const data = JSON.parse(response.body);
    return key ? data[key] : data;
  } catch (e) {
    console.error('Failed to parse JSON:', e);
    return null;
  }
}

// ===========================
// VALIDAÇÕES
// ===========================

/**
 * Valida estrutura de Employee
 * @param {object} employee - Objeto employee
 * @returns {boolean} True se válido
 */
export function isValidEmployee(employee) {
  return (
    employee &&
    typeof employee.id === 'string' &&
    typeof employee.name === 'string' &&
    typeof employee.phone === 'string' &&
    typeof employee.department === 'string' &&
    employee.name.length > 0 &&
    employee.phone.length > 0 &&
    employee.department.length > 0
  );
}

/**
 * Valida token JWT básico
 * @param {string} token - Token JWT
 * @returns {boolean} True se formato válido
 */
export function isValidJwt(token) {
  if (!token || typeof token !== 'string') {
    return false;
  }

  const parts = token.split('.');
  return parts.length === 3;
}

// Exporta tudo como default também
export default {
  // Auth
  getAuthToken,
  registerUser,

  // HTTP
  authenticatedGet,
  authenticatedPost,
  authenticatedPut,
  authenticatedDelete,

  // Data Generation
  generateEmployeePayload,
  generateRandomName,
  generateRandomFirstName,
  generateRandomLastName,
  generateRandomDepartment,
  generatePhoneNumber,
  generateFutureStartDate,
  generatePastDate,
  generateUniqueEmail,

  // Utils
  formatDateISO,
  randomSleep,
  replaceEndpointId,
  isValidJson,
  safeJsonParse,

  // Validations
  isValidEmployee,
  isValidJwt,
};
