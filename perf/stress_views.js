import http from "k6/http";
import { check } from "k6";

const baseUrl = __ENV.BASE_URL || "http://localhost:5000";
const slug = __ENV.POST_SLUG || "seed-post-0";

export const options = {
  vus: 80,
  duration: "45s",
  thresholds: {
    http_req_failed: ["rate<0.02"],
  },
};

export default function () {
  const res = http.get(`${baseUrl}/api/posts/${slug}`);
  check(res, {
    "status 200": (r) => r.status === 200,
  });
}
