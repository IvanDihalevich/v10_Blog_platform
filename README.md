# Блог-платформа (ASP.NET Core + PostgreSQL)

Публічний репозиторій навчального завдання: Web API для постів, коментарів і тегів.

## Вимоги завдання (чеклист)

| Вимога | Статус |
|--------|--------|
| Сутності Post, Comment, Tag, PostTag | ✅ `src/BlogPlatform.Domain/Entities` |
| Ендпоінти з таблиці завдання | ✅ `src/BlogPlatform.Api/Controllers` |
| Бізнес-правила (slug, лише опубліковані у списку, перегляди, модерація, публікація без порожнього контенту) | ✅ |
| PostgreSQL + EF Core | ✅ |
| Testcontainers.PostgreSql для тестів БД | ✅ |
| Модульні тести (slug, перегляди, схвалення коментарів) | ✅ `tests/BlogPlatform.Tests.Unit` |
| Інтеграційні тести (WebApplicationFactory) | ✅ `tests/BlogPlatform.Tests.Integration` |
| Тести БД (унікальність slug, конкурентність ViewCount, M:N теги) | ✅ `tests/BlogPlatform.Tests.Database` |
| k6 (список з пагінацією + стрес переглядів) | ✅ `perf/` |
| Наповнення ≥10 000 записів (Bogus; AutoFixture — у тестах/білдерах) | ✅ `LargeDatasetSeeder` + AutoFixture у тестах |
| GitHub Actions CI (push / pull_request) | ✅ `.github/workflows/ci.yml` |

### Автогенерація даних у тестах

- **AutoFixture**: поля з завдання (Title, Content, AuthorName, PublishedAt для Post; AuthorName, Content, CreatedAt для Comment; Name для Tag) — див. `tests/BlogPlatform.Tests.Unit/EntityTestData/`.
- **Явно в тестах**: Slug, IsPublished, ViewCount, IsApproved, PostId (див. ті самі файли та сидер).
- **Bogus**: масове наповнення БД у `LargeDatasetSeeder` (допускається завданням як альтернатива).

### Pull Request

Кожну логічну частину роботи варто оформлювати **окремим Pull Request** у `main` (гілка `feature/...` або `task/...`). CI автоматично запускає тести на кожен push і PR.

## Запуск локально

1. PostgreSQL (наприклад Docker):

   `docker run -d --name blogplatform-pg -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=blogplatform -p 5432:5432 postgres:16`

2. API:

   `dotnet run --project src/BlogPlatform.Api`

3. Swagger (у Development): `http://localhost:5039/swagger` (порт з `launchSettings.json`).

4. Тести: `dotnet test` (потрібен **Docker** для Testcontainers).

5. k6: `k6 run perf/load_posts.js` (змінна `BASE_URL`), після сиду — `perf/stress_views.js` з `POST_SLUG`.

## Ліцензія

Навчальний проєкт.
