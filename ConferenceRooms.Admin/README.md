# Conference Rooms — Admin (Angular)

A small standalone admin UI for the Conference Room Booking API. **Not part of the
task brief** — added to exercise the API end to end from a browser.

Angular 21, standalone components, zoneless, signals, reactive forms. No UI library —
one shared stylesheet. Vitest for unit tests.

---

## Running

The API must be running first (from the repo root):

```bash
dotnet run --project ConferenceRooms.Api        # http://localhost:5006
```

Then, in this folder:

```bash
npm install
npm start                                        # http://localhost:4200
```

`npm start` serves with a dev proxy (`proxy.conf.json`) that forwards `/api/*` to the
API, so the browser only ever talks to `localhost:4200` and no CORS policy is needed.

The proxy targets **`https://localhost:7265`** with `secure: false` (the API's dev
cert is self-signed). This matches running the API from an IDE or with
`dotnet run --launch-profile https`. Two things to know:

- When the API is started with an HTTPS endpoint, plain `http://localhost:5006`
  answers with a `307` to `https://localhost:7265`; Vite's proxy won't follow that
  onto a self-signed cert, which is why the target is the HTTPS URL directly.
- If you run the API HTTP-only (`dotnet run` with the default `http` profile, port
  5006, no HTTPS), change the target in `proxy.conf.json` to
  `http://localhost:5006` — there's no redirect in that case.

```bash
npm test        # vitest, single run
npm run build   # production build -> dist/
```

---

## What it covers

| Screen | Route | API |
| --- | --- | --- |
| Rooms list | `/rooms` | `GET /api/rooms`, `DELETE /api/rooms/{id}` |
| Room create / edit | `/rooms/new`, `/rooms/:id` | `GET/POST/PUT /api/rooms`, `POST /api/rooms/{id}/services` |
| Availability | `/availability` | `GET /api/rooms/available` |
| New booking + lookup | `/bookings/new` | `POST /api/bookings`, `GET /api/bookings/{id}` |
| Revenue report | `/reports/revenue` | `GET /api/reports/revenue` |

Availability results link through to the booking form with the room and window
pre-filled via query params.

---

## Layout

```
src/app/
  app.ts / app.html / app.scss   Shell: sidebar nav + <router-outlet>
  app.routes.ts                  Lazy loadComponent per screen
  core/
    models/                      TS mirrors of the API DTOs
    services/                    One HttpClient wrapper per controller
    api-error.ts                 RFC 7807 ProblemDetails -> display string
    api-error.interceptor.ts     Rethrows failures as ApiError(message, status)
    duration.ts                  hours <-> TimeSpan "hh:mm:ss"
  features/<area>/<screen>/       One folder per screen (ts + html)
  shared/price-breakdown/        Reused by booking create + lookup
src/styles.scss                  The whole design system (~1 screen of CSS)
```

### Notes / assumptions

- **Duration** is entered in hours and converted to the `hh:mm:ss` `TimeSpan` string
  the API expects (`duration.ts`).
- **Timestamps** use `<input type="datetime-local">` / `type="date"`, whose values are
  wall-clock with no timezone — matching how the API treats incoming `DateTime`.
- **No remove-service endpoint** exists, so the room form only adds services.
- **No list-bookings endpoint** exists, so bookings are reached by creating one or
  looking one up by id.
- Errors are surfaced as inline banners; the interceptor turns `status 0` into a
  "can't reach the API" hint.
