import http from "k6/http";
import { check, sleep } from "k6";

const baseUrl = __ENV.BASE_URL || "http://localhost:5000";

export const options = {
  stages: [
    { duration: "30s", target: 20 },
    { duration: "1m", target: 50 },
    { duration: "30s", target: 0 },
  ],
  thresholds: {
    http_req_failed: ["rate<0.01"],
    http_req_duration: ["p(95)<800"],
  },
};

export default function () {
  const page = (__VU * __ITER) % 40 + 1;
  const res = http.get(`${baseUrl}/api/posts?page=${page}&pageSize=25`);
  check(res, {
    "status 200": (r) => r.status === 200,
    "has items array": (r) => {
      try {
        const body = JSON.parse(r.body);
        return Array.isArray(body.items);
      } catch {
        return false;
      }
    },
  });
  sleep(0.2);
}
