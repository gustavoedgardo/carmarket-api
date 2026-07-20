# CarMarket API (.NET 8 / C#)

Backend de administración para el sitio de venta de autos: API REST con
autenticación JWT para el panel admin, y endpoints públicos de solo lectura
para el catálogo (listado con orden/filtro y detalle con hasta 15 imágenes).

## Requisitos

- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- No requiere SQL Server instalado: usa **SQLite** por defecto (archivo
  `carmarket.db`, se crea solo). Para producción real, cambiar a SQL Server
  o PostgreSQL (ver notas en `CarMarket.Api.csproj` y `Program.cs`).

## Correr en local

```bash
cd CarMarket.Api
dotnet restore
dotnet run
```

La API queda en `https://localhost:7xxx` (el puerto exacto se imprime en
consola). Swagger UI disponible en `/swagger` para probar todos los
endpoints (incluye login con JWT integrado: click en "Authorize" y pegar
`Bearer {token}`).

Al arrancar por primera vez se crea la base con datos de ejemplo y un
usuario administrador:

- **Email:** admin@concesionaria.com
- **Password:** admin123

⚠️ Cambiar esta contraseña (y la clave `Jwt:Key` de `appsettings.json`)
antes de ir a producción.

## Endpoints principales

### Públicos (catálogo)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/cars?q=&estado=&orderBy=&page=&pageSize=` | Listado con búsqueda, filtro por estado y orden (`precio_asc`, `precio_desc`, `anio_asc`, `anio_desc`, `km_asc`, `km_desc`, `recientes`) |
| GET | `/api/cars/{id}` | Detalle completo, hasta 15 imágenes |

### Autenticación

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/login` | `{ "email": "...", "password": "..." }` → devuelve JWT (válido 8hs) |

### Administración (requiere header `Authorization: Bearer {token}`)

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/cars` | Alta de vehículo |
| PUT | `/api/cars/{id}` | Modificación (incluye reemplazo de imágenes) |
| DELETE | `/api/cars/{id}` | Baja |

## Conectar con el frontend React

El origen del frontend debe estar en `Cors:AllowedOrigins` de
`appsettings.json` (por defecto habilita `localhost:5173` y `localhost:3000`,
los puertos típicos de Vite/CRA).

Desde React, ejemplo de consumo:

```js
// Login
const res = await fetch("https://localhost:7xxx/api/auth/login", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({ email, password }),
});
const { token } = await res.json();

// Alta protegida
await fetch("https://localhost:7xxx/api/cars", {
  method: "POST",
  headers: {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  },
  body: JSON.stringify(nuevoAuto),
});
```

## Para subir imágenes reales (en vez de URLs externas)

Este proyecto recibe las imágenes como **URLs** (pensado para un storage
externo tipo Azure Blob Storage, AWS S3 o Cloudinary). Para permitir subida
directa de archivos desde el panel admin, se puede agregar un
`UploadController` con `IFormFile` que suba a ese storage y devuelva la URL
pública a guardar en `CarUpsertDto.Imagenes`. Puedo agregarlo si querés que
las imágenes se carguen como archivos en vez de URLs.

## Desplegar para que el cliente lo vea online

El proyecto ya incluye `Dockerfile`, `render.yaml` y `railway.json`, listos
para desplegar sin tocar nada.

### Opción A — Render (recomendado, capa free)

1. Subir esta carpeta (`CarMarket.Api`) a un repo de GitHub.
2. En [render.com](https://render.com) → **New > Blueprint** → conectar el
   repo. Render lee `render.yaml` solo y crea el servicio con Docker.
3. Editar la env var `Cors__AllowedOrigins__0` en el dashboard de Render
   y poner ahí la URL real del frontend (Vercel/Netlify) cuando la tengas.
4. Render te da una URL pública tipo `https://carmarket-api.onrender.com`.
   Probarla en `/api/cars` o `/swagger` (Swagger queda habilitado por la
   env var `ENABLE_SWAGGER=true` que ya viene seteada).

⚠️ En el plan free, Render "duerme" el servicio tras 15 min sin uso (el
primer request después tarda ~30s en responder). Para la demo al cliente
alcanza; para producción conviene un plan pago o Railway.

### Opción B — Railway

1. Subir el repo a GitHub.
2. En [railway.app](https://railway.app) → **New Project > Deploy from
   GitHub repo**. Railway detecta el `Dockerfile` y `railway.json` solo.
3. En **Variables** cargar `Jwt__Key` (una clave larga aleatoria),
   `Jwt__Issuer`, `Jwt__Audience`, `Cors__AllowedOrigins__0` (URL del
   frontend), `ENABLE_SWAGGER=true`.
4. Agregar un **Volume** montado en `/app/data` (Settings > Volumes) para
   que la base SQLite no se borre en cada deploy.
5. Railway asigna dominio público automáticamente (Settings > Networking >
   Generate Domain).

### Opción C — Azure App Service

Para equipos que ya trabajan en Azure: `az webapp up` desde la carpeta del
proyecto, o publicar directo desde Visual Studio. Recomendado migrar de
SQLite a **Azure SQL** en este caso (cambiar el paquete NuGet en el
`.csproj` de `Sqlite` a `SqlServer` y `UseSqlite` → `UseSqlServer` en
`Program.cs`).

### Después de desplegar el backend

1. Copiar la URL pública que te dio Render/Railway (ej.
   `https://carmarket-api.onrender.com`).
2. Actualizar el frontend React para apuntar ahí en vez de a los datos
   simulados (reemplazar el array `SEED_CARS` por `fetch` a `/api/cars`).
3. Desplegar el frontend en Vercel o Netlify → esa es la URL final para
   el cliente.

Si querés, hago ese último paso: conectar el frontend React ya entregado
a este backend real (fetch + manejo de token JWT en el panel admin) y
dejar todo listo para ese deploy.
