import http from 'k6/http';
import { check, sleep } from 'k6';
import { randomString } from './utils/helpers.js';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5001';

export const options = {
  vus: __ENV.VUS ? parseInt(__ENV.VUS) : 100,
  duration: __ENV.DURATION || '30s',
  thresholds: {
    http_req_duration: ['p(95)<500'], // 95% requests should be < 500ms
    // keep http_req_failed threshold, but authentication will help avoid 401s
    'http_req_failed': ['rate<0.05'],
  },
};

// Run once to obtain an auth token that will be reused by all VUs
export function setup() {
  const payload = JSON.stringify({ email: 'admin@example.com', password: 'Admin@123' });
  const params = { headers: { 'Content-Type': 'application/json' } };
  const res = http.post(`${BASE_URL}/api/auth/login`, payload, params);
  if (res.status !== 200) {
    console.error('Auth setup failed, status:', res.status, 'body:', res.body);
    return { token: null };
  }
  // try common token key names
  const token = res.json('token') || res.json('access_token') || res.json().token;
  return { token };
}

export default function (data) {
  const token = data && data.token;
  if (!token) {
    // no token: abort the VU iteration early to avoid flooding with unauthorized requests
    return;
  }

  const payload = JSON.stringify({
    name: `Test User ${randomString(6)}`,
    email: `test+${randomString(6)}@example.com`,
    department: 'TI',
    position: 'Dev',
    // Validation requires phone and startDate. Provide valid formats.
    phone: `+1234567${Math.floor(1000 + Math.random() * 9000)}`,
    // startDate must not be in the past - use tomorrow in ISO format
    startDate: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  };

  const res = http.post(`${BASE_URL}/api/employees`, payload, params);

  check(res, {
    'create employee status is 201 or 200': (r) => r.status === 201 || r.status === 200,
  });

  // Small sleep to simulate user think time
  sleep(1);
}
