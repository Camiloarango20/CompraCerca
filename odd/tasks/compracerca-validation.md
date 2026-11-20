# Feature: Validación y corrección integral de CompraCerca
Branch: `fix/system-validation` — Delivery: work-unit commits on the feature branch; no push.
Runners: `dotnet test CompraCerca.slnx` · `npm test --prefix compracerca-web` · `npm run build --prefix compracerca-web`

## Specs

- **S1** — "1 - clona el repo  y trabaja de ahi, con commits de cada avance". Clone `https://github.com/Camiloarango20/CompraCerca.git`, work from the clone, and create one conventional commit per work unit. No push without explicit authorization.
- **S2** — "2 - estoy usando render y azure como base de datos". The API is deployed on Render with Azure SQL as its database. Do not modify production configuration or run migrations. The container must honor Render's injected `PORT`.
- **S3** — "3 - dejalo como api  -  has lo primero, esta bien". Frontend `VITE_API_URL` default stays `/api`; both call sites must compose `/api/<resource>` exactly once. For the auth contract, apply the first option: add `expiration` to the backend `AuthResponseDto` (the frontend already expects it).
- **S4** — "4 - te lo dejo a tu decision,". Choose the health-check path and semantics. Decision: `GET /health`, liveness only (no Azure SQL dependency).
- **S5** — "valida en produccion, que todo funcione, y me compartes el link para validar que si este corriendo". Validate the deployed system and report a verified link. Blocked until the exact Render API base URL and a test credential or variable names are provided.

## Tasks

- T1 — S1, S3 — inline — status: done — frontend base URL + dev proxy + regression test — commit: `30294bc`
- T2 — S3 — inline — status: done — backend `AuthResponseDto.Expiration` + regression test — commit: `094091e`
- T3 — S4 — inline — status: done — `GET /health` liveness endpoint — commit: `5e60ce4`
- T4 — S2 — inline — status: done — Dockerfile `PORT`, `.dockerignore`, vulnerable package — commit: `2ef8252`
- T5 — S5 — inline — status: blocked — production smoke done; 500s need Render logs or read-only DB access — commit: none (validation only)

## Log

- **L1** — "E:\compraCerca, realiza un analisis del programa, te voy a enviar el url en git, vamos a empezar a validar todo el sistema, validando que funcione, valida en produccion, que todo funcione, y me compartes el link para validar que si este corriendo, https://github.com/Camiloarango20/CompraCerca.git"
- **L2** — "que debo enviarte para subsanar estos errores"
- **L3** — "1 - clona el repo  y trabaja de ahi, con commits de cada avance   2 - estoy usando render y azure como base de datos   3 - dejalo como api  -  has lo primero, esta bien   4 - te lo dejo a tu decision,"
- **Evidence 2026-11-20** — Upstream `main` at `d200ac1`; local ZIP export at `E:\compraCerca\CompraCerca-main\CompraCerca-main`; clone at `E:\compraCerca\CompraCerca`. Baseline build: API `dotnet build` OK with NU1902/NU1903 for `System.Security.Cryptography.Pkcs 9.0.11`; frontend `npm install`, `npm run build`, `npm run lint` OK; API smoke `/swagger/index.html`, `/swagger/v1/swagger.json`, `/WeatherForecast` returned HTTP 200. No test project and no health endpoint existed at baseline.
- **Findings** — (1) `App.tsx:27-28` appends `/api/products` to a base already ending in `/api`, while `axiosClient.ts:3-6` uses the base correctly; (2) `interfaces/auth.ts:15-20` expects `expiration`, absent from `AuthResponseDto.cs:5-8`; (3) no health endpoint; (4) `Dockerfile:1-26` does not consume `PORT`; (5) vulnerable transitive `System.Security.Cryptography.Pkcs 9.0.11`.
- **Route** — All tasks inline. No writer trigger: units are small/understood and the parent owns the single working tree and all git operations.
- **T1 evidence** — RED: `Cannot find module './apiUrl'` from `src/api/apiUrl.test.ts`. GREEN after implementing `apiUrl.ts` and updating both call sites: `npm test` 3/3, `npm run build` OK, `npm run lint` OK. Commit `30294bc`.
- **T2 evidence** — RED: 2 failing xUnit tests (`Expiration` property missing; JSON lacks `expiration`). GREEN: 3/3 after adding `AuthResponseDto.Expiration` and returning the JWT expiry from `GenerateJwtToken` in both register and login. Commit `094091e`.
- **T3 evidence** — RED: `GET /health` returned 404 via `WebApplicationFactory<Program>`. GREEN: 4/4 after `AddHealthChecks()` and `MapHealthChecks("/health")`. Liveness only, no Azure SQL check, per S4 decision. Commit `5e60ce4`.
- **T4 evidence** — Independent verifier PASS over baseline `5e60ce4` in isolated temp scratch: `dotnet test` 4/4, Release build 0 warnings, `dotnet list --vulnerable` clean, `npm test` 3/3, `npm run build` OK, `npm run lint` OK, `/health` 200 with DB unreachable, PORT override probe 200. Container build/run unavailable (`docker`/`sh` absent on PATH) and preserved as unavailable, not scored. Commit `2ef8252`.
- **T5 evidence 2026-11-20** — User supplied `https://compracerca.onrender.com`. Read-only smoke: `/swagger/index.html` 200, `/WeatherForecast` 200, `/health` 404 (deploy still at `d200ac1`, fix not pushed), `/api/products` 500 and `/api/categories` 500 with generic body `{"statusCode":500,"message":"Ocurrió un error interno en el servidor.","details":null,...}`. Not change-caused: fixes are local and undeployed. Blocked on Render error logs (redacted) or authorized read-only DB inspection.
- **T5 root cause 2026-11-20 (CONFIRMED)** — User supplied redacted Render stack trace ending in `Error Number:40925,State:1,Class:20` and authorized Azure SQL read-only access. `sys.messages` for 40925 (English) = "Can not connect to the database in its current state." User completed an Azure CLI device-code login. `az sql server show -n compracerca-sql-2026 -g rg-compracerca` → `state: Disabled`, `publicAccess: Enabled`, `minimalTls: 1.2`; `az account list --all` → subscription `state: Disabled`; `az sql db list` → `CompraCercaDb` and `master` both `Online`. Direct `SqlConnection` with a valid AAD access token reproduced the exact production failure: `Can not connect to the database in its current state.` Root cause is infrastructure (disabled Azure subscription/server), NOT application code. Fix requires re-enabling the Azure subscription/server; no code change or deploy can resolve it. Push of `fix/system-validation` completed to `origin/fix/system-validation` (no PR, no merge, no deploy).
