# Printing Job Tracker – Blazor Server (.NET 8)

**Proyecto técnico para evaluación de desarrollo Fullstack.**  
Interfaz en **Blazor Server** con **EF Core** y **SQL Server** para gestionar trabajos de impresión: listado, creación, avance de estado, excepciones y historial.

---

## 1. Tecnologías
- .NET 8, ASP.NET Core **Blazor Server**
- **Entity Framework Core** (Code-First + Migraciones + Seeding)
- **SQL Server** (LocalDB o SQLEXPRESS)
- C# 12
- Estilos **dark** con CSS moderno

---

## 2. Funcional
- **Lista de Jobs** con filtro por estado y orden por fecha.
- **Crear Job** (estado inicial **Received**).
- **Detalles** del Job con **historial** de cambios.
- **Avanzar estado** (Received → Printing → Inserting → Mailed → Delivered).
- **Marcar Exception** con nota y registro en historial.
- **Dashboard** con conteo por estado.
- **Home** con **navbar** y accesos (Jobs / Create / Dashboard).

---

## 3. Ejecución rápida

### Visual Studio
1. Abrir la solución/proyecto.
2. Seleccionar el perfil **Kestrel / Project** (no IIS Express).
3. **Ctrl+F5** o **F5**.
4. Navegar a `https://localhost:5xxx`.

### CLI
```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add Initial
dotnet ef database update
dotnet run
```

> Al arrancar, la app ejecuta **seeding** si `Jobs` está vacía.

---

## 4. Base de datos (`appsettings.json`)
- **LocalDB**
```
Server=(localdb)\MSSQLLocalDB;Database=PrintingJobTracker;Trusted_Connection=True;MultipleActiveResultSets=true
```
- **SQL Express**
```
Server=DESKTOP-NOMBRE\SQLEXPRESS;Database=PrintingJobTracker;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

---

## 5. Estructura
```
Domain/                 // Entidades: Job, JobStatusHistory, enums
Data/                   // OpsDbContext + DataSeeder
Services/               // IWorkOrdersService + WorkOrdersService
Pages/                  // Home, Index (jobs), WorkOrders/Create, Details, Overview
Shared/                 // HeaderNav (navbar), MainLayout, NavMenu
wwwroot/css/site.css    // Estilos dark
```

---

## 6. Navegación
- **Inicio**: `/` → `Pages/Home.razor` (navbar + accesos).
- **Listado**: `/jobs` → `Pages/Index.razor` (ruta actualizada).
- **Crear**: `/workorders/create`
- **Dashboard**: `/workorders/overview`

---

## 7. Buenas prácticas
- Capas separadas (Domain / Data / Services / UI).
- Inyección de dependencias.
- EF Core con migraciones + seeding.
- Validaciones en formularios Blazor.
- Tema dark accesible y consistente.

---

## 8. Troubleshooting
- **Herramienta EF**: `dotnet tool install --global dotnet-ef`
- **Certificado https local**: `dotnet dev-certs https --trust`
- **Archivo bloqueado al recompilar**: detener app (Stop/Shift+F5) o `taskkill /F /IM PrintingJobTracker.exe`, limpiar `bin/ obj`, recompilar.
- **No se crean datos seed**: la tabla `Jobs` ya tiene datos; use otra BD o vacíe la actual.

_Fecha: 2025-11-04_
