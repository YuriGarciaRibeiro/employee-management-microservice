import http from 'k6/http';
import { check, sleep } from 'k6';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5001';

export const options = {
  vus: __ENV.VUS ? parseInt(__ENV.VUS) : 50,
  duration: __ENV.DURATION || '20s',
  thresholds: {
    http_req_duration: ['p(95)<500'],
  },
};

export default function () {
  const payload = JSON.stringify({
    email: 'admin@example.com',
    password: 'Admin@123',
  });

  const params = { headers: { 'Content-Type': 'application/json' } };

  const res = http.post(`${BASE_URL}/api/auth/login`, payload, params);

  check(res, {
    'login succeeded': (r) => r.status === 200 && r.json('token') !== undefined,
  });

  sleep(1);
}
