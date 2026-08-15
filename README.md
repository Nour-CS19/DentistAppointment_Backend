# DentistAppointment API

ASP.NET Core (Clean Architecture: Domain / Application / Infrastructure / Api) backend
that replaces Supabase (Auth + Postgres + RLS + Edge Functions) for the dentist
appointment management app.

## Mapping from Supabase → here

| Supabase piece | Replacement |
|---|---|
| `auth.users` + `profiles` table | `AppUser : IdentityUser<Guid>` (ASP.NET Identity) |
| Supabase session / JWT | Custom JWT issued by `/api/auth/login` and `/register` |
| Row Level Security policies | Checked in code via `ICurrentUserService` in each command/query handler |
| `appointments` table | `Appointment` entity, same columns |
| Realtime channel subscription | Not implemented here — poll `GET /api/appointments` on an interval from the frontend (see frontend changes) |
| `create-payment` Edge Function | `POST /api/payments/create-checkout-session` |
| `verify-payment` Edge Function | `POST /api/payments/verify` |

## Run locally

1. Install the .NET 8 SDK.
2. Have SQL Server running (or `docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest`).
3. Update `src/DentistAppointment.Api/appsettings.json`:
   - `ConnectionStrings:DefaultConnection`
   - `Jwt:Key` (long random secret)
   - `Stripe:SecretKey`
   - `Cors:AllowedOrigins` (your Vite dev URL, e.g. `http://localhost:5173`)

   Better: keep real secrets out of git — copy these into `appsettings.Local.json`
   (already gitignored) instead of committing them, same pattern as Femora.

4. Create the initial migration and update the database:
   ```bash
   cd src/DentistAppointment.Api
   dotnet tool install --global dotnet-ef   # if not already installed
   dotnet ef migrations add InitialCreate --project ../DentistAppointment.Infrastructure --startup-project .
   dotnet ef database update --project ../DentistAppointment.Infrastructure --startup-project .
   ```
5. Run the API:
   ```bash
   dotnet run --project src/DentistAppointment.Api
   ```
   Swagger UI available at `/swagger` in Development.

## Making the first admin user

There's no signup flow for admins (by design — same as the original, where an admin
role had to be granted manually). After registering a normal user, promote them to
admin directly in the database:

```sql
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'admin@example.com' AND r.Name = 'admin';
```
(Make sure the `admin` role row exists in `AspNetRoles` first — it's created
automatically the first time any admin-role check runs, or you can insert it manually.)

## Endpoints

- `POST /api/auth/register` — { email, password, firstName, lastName }
- `POST /api/auth/login` — { email, password }
- `GET /api/appointments` — current user's appointments (auth required)
- `PUT /api/appointments/{id}` — update own appointment
- `DELETE /api/appointments/{id}` — delete own appointment
- `GET /api/admin/appointments` — all appointments (admin only)
- `PUT /api/admin/appointments/{id}/status` — update status/paymentStatus (admin only)
- `POST /api/payments/create-checkout-session` — creates Stripe session + pending appointment
- `POST /api/payments/verify` — { sessionId } → marks appointment paid/confirmed
