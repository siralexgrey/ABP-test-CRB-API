# Conference Room Booking API

A REST API for managing conference rooms, checking availability, booking rooms with
time-based pricing, and reporting revenue over a period.

Built with ASP.NET Core (controllers) on .NET 10, EF Core + SQLite, xUnit.

---

## Running

```bash
dotnet run --project ConferenceRooms.Api
```

- HTTP: `http://localhost:5006`
- Swagger UI: `http://localhost:5006/swagger`
- The SQLite database (`conferencerooms.db`) is created, migrated, and seeded on
  startup — no manual `dotnet ef database update` needed. Seed data: rooms **A**, **B**,
  **C**, each with Projector / Wi-Fi / Sound services.

```bash
dotnet test        # run the test suite
```

---

## Endpoints

### Rooms

| Method | Route | Notes |
| --- | --- | --- |
| `GET` | `/api/rooms` | List all rooms with their services. |
| `GET` | `/api/rooms/{id}` | Single room. `404` if missing. |
| `GET` | `/api/rooms/available?start=&end=&minCapacity=` | Rooms with capacity ≥ `minCapacity` and no booking overlapping `[start, end)`. |
| `POST` | `/api/rooms` | Create a room. Services can be supplied inline. |
| `POST` | `/api/rooms/{id}/services` | Add one service to a room. `404` if the room is missing. |
| `PUT` | `/api/rooms/{id}` | Update a room's scalar fields. `404` if missing. |
| `DELETE` | `/api/rooms/{id}` | Delete a room. |

### Bookings

| Method | Route | Notes |
| --- | --- | --- |
| `POST` | `/api/bookings` | Create a booking. `404` room / unknown service, `409` time overlap, `400` invalid input. |
| `GET` | `/api/bookings/{id}` | Single booking with its price breakdown. `404` if missing. |

`POST /api/bookings` body:

```json
{
  "roomId": 1,
  "startTime": "2026-09-02T11:00:00",
  "duration": "02:00:00",
  "serviceIds": [1, 3]
}
```

`duration` is a `TimeSpan` serialized as `hh:mm:ss` (not ISO-8601 `PT2H`).

### Reports

| Method | Route | Notes |
| --- | --- | --- |
| `GET` | `/api/reports/revenue?from=&to=` | Total revenue plus breakdown by room and by service for bookings whose `StartTime` falls in `[from, to)`. |

---

## Architecture

```
Controllers/   HTTP surface. Thin: bind, call a service, translate the result.
Services/      *AppService — orchestration, validation flow, DTO mapping.
Repositories/  EF Core data access, one interface per aggregate.
Data/          AppDbContext, DbSeeder.
Domain/        Pure, dependency-free rules — unit-testable without the app.
Dtos/          Request/response contracts. Entities never cross the controller boundary.
Models/        EF Core entities.
Migrations/    EF Core migrations.
```

- **Controller → AppService → Repository.** DTOs on every path so the storage schema
  doesn't dictate the API contract and internal columns don't leak.
- **`Domain/`** holds `BookingOverlap.Overlaps` (the overlap predicate) and
  `Domain/Pricing/` (`IPricingService` / `PricingService` / `PriceBreakdown`). No DI, no
  EF, no HTTP — priced and tested in isolation.
- **`AppService` suffix** on the service layer (`RoomAppService`, `BookingAppService`,
  `ReportAppService`) because the entity names `RoomService` / `BookingService` were
  already taken. Convention borrowed from ABP Framework.
- **`GlobalExceptionHandler`** (`IExceptionHandler`) maps domain exceptions to status
  codes and RFC 7807 `ProblemDetails`: `NotFoundException → 404`,
  `OverlapException → 409`, everything else → `500` with a generic body (logged
  server-side).
- **`IntSchemaFixTransformer`** works around a .NET 10 OpenAPI generator quirk where
  integer path parameters are emitted as a `["integer","string"]` union, which the
  Swagger UI parameter validator mishandles. It only touches the generated document,
  not request binding.
- **EF Core + SQLite.** `Restrict` delete behavior on `Booking → Room` and
  `BookingService → RoomService` so referenced rows can't be deleted out from under a
  booking.

---

## Pricing

The day is divided into non-overlapping tariff zones by wall-clock time. Multipliers
apply **only to room rental**; add-on services are charged at a flat price regardless
of time.

| Clock time | Zone | Multiplier |
| --- | --- | --- |
| 23:00 – 06:00 | Night | ×0.70 |
| 06:00 – 09:00 | Morning | ×0.90 |
| 09:00 – 12:00 | Standard | ×1.00 |
| 12:00 – 14:00 | Peak | ×1.15 |
| 14:00 – 18:00 | Standard | ×1.00 |
| 18:00 – 23:00 | Evening | ×0.80 |

A booking is split into sub-intervals at zone boundaries; each is priced
independently and summed. Bookings crossing midnight are split by clock time
(22:00–02:00 → 22:00–23:00 evening + 23:00–02:00 night).

**Example** — Room A (base 2000/h), 11:00–13:00, with Projector (500):

| Segment | Hours | Multiplier | Amount |
| --- | --- | --- | --- |
| 11:00–12:00 (Standard) | 1 | ×1.00 | 2000 |
| 12:00–13:00 (Peak) | 1 | ×1.15 | 2300 |
| **Rental** | | | **4300** |
| Projector | | | 500 |
| **Total** | | | **4800** |

The response carries the full segment-by-segment breakdown (`PriceBreakdown`).

**Availability / overlap rule.** `existingStart < requestEnd && requestStart < existingEnd`
— strict inequalities, so a booking ending at 12:00 and one starting at 12:00 do **not**
conflict.

**`PriceAtBooking`.** Each ordered service is snapshotted onto the `BookingService` row
at its price when the booking was made, so a later price change to the room's service
doesn't retroactively alter an existing booking.

---

## Validation & errors

- `CreateBookingRequest`: `RoomId ≥ 1`, `Duration` in `(0, 24h]`, `StartTime` provided
  (via `[Range]` + `IValidatableObject`).
- `AvailabilityQuery` / `RevenuePeriodQuery`: both dates required, `start < end`,
  `minCapacity ≥ 1`.
- Model-binding / DataAnnotations failures → automatic `400` `ProblemDetails` from
  `[ApiController]`.
- Domain failures → `404` / `409` `ProblemDetails` from `GlobalExceptionHandler`.

---

## Assumptions

1. **Night zone (23:00–06:00) at ×0.70** — the brief doesn't specify it. Chosen to
   continue the demand curve (lowest demand → lowest price).
2. **The brief's zones overlap** (peak 12–14 sits inside standard 09–18). Resolved by
   decomposing each booking into non-overlapping sub-intervals at the boundaries.
3. **Multipliers apply only to room rental.** Services are flat-priced — the brief
   states the discount/surcharge is "on room rental".
4. **Revenue is recognised by `Booking.StartTime`** (when the room is used), and the
   report period is half-open `[from, to)`.
5. **Only the grand total and per-service prices are snapshotted** on a booking; the
   room's base rate is not. `GET /api/bookings/{id}` recomputes the zone breakdown from
   the room's *current* base rate — if that rate changed after the booking, the
   recomputed segments may not sum to the stored `TotalPrice`. Persisting the breakdown
   is listed under Next steps.
6. **`PriceBreakdown` is serialized directly** from the domain result rather than mapped
   to a dedicated response DTO, to expose the full zone breakdown with no extra mapping.
7. **Error signalling is deliberately asymmetric.** Room mutations use nullable returns
   from the service (one failure mode: not found). Booking creation uses typed
   exceptions (three: room not found, unknown service, time overlap).
8. **`RoomService` is per-room**, not a global catalog — each room owns its service rows
   and prices. Services are supplied inline on create, or added one at a time via
   `POST /api/rooms/{id}/services`. Full collection reconciliation on `PUT` is out of
   scope.
9. **All timestamps are treated as wall-clock.** Incoming `DateTime` values are not
   normalized to a timezone.
10. **The database is created and seeded on startup** (`Migrate()` + an idempotent
    `DbSeeder`) so the app runs from a clean checkout with a bare `dotnet run`.

---

## Security

- Input validation on every write and query DTO; `[ApiController]` returns a structured
  `400` for malformed input.
- `exception.Message` is never returned to the client on the `500` path — a generic
  detail is sent and the exception is logged server-side. Domain exceptions (404/409)
  carry only safe, caller-oriented messages.
- Entities are never serialized directly; DTOs on every read path keep internal columns
  and navigation properties out of responses.
- HTTPS redirection enabled.
- **Out of scope:** authentication / authorization (no user model in the brief), rate
  limiting, CORS policy, secrets management (the connection string points at a local
  SQLite file).

---

## Tests

`ConferenceRooms.Tests` (xUnit):

- **`BookingOverlapTests`** — the pure overlap predicate: boundary, partial, and
  fully-contained cases.
- **`PricingTest`** — tariff-zone decomposition and totals, including intervals that
  cross zone boundaries and midnight, plus flat service charges.
- **`AvailabilityTest`** — integration against SQLite in-memory: capacity filter,
  overlap exclusion, adjacent-booking boundary.

---

## Angular admin (optional, not in the brief)

`ConferenceRooms.Admin/` is a small Angular 21 admin UI over this API — rooms CRUD,
availability search, booking creation with the price breakdown, and the revenue
report. It is outside the task scope; it's there to drive the API from a browser.

```bash
dotnet run --launch-profile https --project ConferenceRooms.Api   # terminal 1 — https://localhost:7265
cd ConferenceRooms.Admin && npm install && npm start              # terminal 2 — http://localhost:4200
```

The dev server proxies `/api/*` to the API (`ConferenceRooms.Admin/proxy.conf.json`,
target `https://localhost:7265`, `secure: false`), so no CORS policy is added to the
API. Run HTTP-only instead? Point the proxy at `http://localhost:5006`. Detail in
`ConferenceRooms.Admin/README.md`.

---

## Next steps

- Second report: room utilization (%) over a period.
- Swagger XML doc comments and `[ProducesResponseType]` for precise response schemas.
- `RoomService` reconciliation on `PUT` (add / update / remove by id) and `409` when
  removing a service referenced by a booking (`BookingService → RoomService` FK is
  `Restrict`).
- `409` on `DELETE /api/rooms/{id}` when the room has bookings (currently surfaces as an
  unhandled `500`).
- Persist the price breakdown (segments) so `GET /api/bookings/{id}` returns stored
  numbers instead of recomputing.
- Exhaustive pricing tests (current coverage is key cases, not exhaustive).
- Push report aggregation into SQL (`GROUP BY`) instead of in-memory grouping.
- Concurrency: two simultaneous overlapping booking requests can both pass the overlap
  check before either commits. Needs a transaction with a stricter isolation level or an
  application-level lock.
- Move the domain and service layers into a class library so tests don't reference the
  web host.
- Timezone handling: normalize incoming `DateTime` kinds / accept explicit offsets.
