
# Printing Job Tracker (Blazor Server, .NET 8)

Aplicación de prueba técnica estilo Evertec para gestionar trabajos de impresión y su progreso.

## ✅ Funcionalidades implementadas
- **Lista** de trabajos con filtro por estado y orden por fecha (desc).
- **Crear trabajo**: estado inicial `Received` y registro de historial.
- **Detalle**: información completa + **historial** cronológico.
- **Avanzar estado**: `Received → Printing → Inserting → Mailed → Delivered`.
- **Marcar Exception** (requiere nota) + historial.
- **Dashboard** con conteo por estado.
- **Seed data**: 12 trabajos con carriers y estados variados.
- **EF Core (SQL Server)** con `DbContext` y `Migrate()` en arranque.

## 🧱 Modelo de Datos
- `Job` (Id, ClientName, JobName, Quantity, Carrier, CurrentStatus, CreatedAt, SLA_MailBy, History)
- `JobStatusHistory` (Id, JobId, Status, Note, ChangedAt)
- `JobStatus` enum: Received, Printing, Inserting, Mailed, Delivered, Exception

## 🛠️ Tecnologías
- .NET 8, Blazor Server
- Entity Framework Core 8 (SqlServer)
- C# 12

## 🚀 Cómo ejecutar
1. **Requisitos**: .NET 8 SDK y SQL Server LocalDB (o SQL Express).
2. Clona el repo o extrae el ZIP.
3. Edita `appsettings.json` si usas otra instancia SQL.
4. En la carpeta del proyecto:
   ```bash
   dotnet restore
   dotnet ef migrations add Initial
   dotnet ef database update
   dotnet run
   ```
5. Abre `https://localhost:5001/` o el puerto que indique la consola.

> **Nota**: `Program.cs` llama a `db.Database.Migrate()` y `DataSeeder.Seed(db)` en arranque; tras crear la BD, insertará datos de ejemplo automáticamente si la tabla está vacía.

## 📌 Decisiones técnicas
- **EF Core** (no Dapper) porque la consigna pide explícitamente EF Core para evaluar modelado y migraciones.
- **Historial automático** se registra en `WorkOrdersService` al **crear**, **avanzar** y **marcar excepción**.
- **Orden** por `CreatedAt desc` desde el servicio.

## 🔎 Validaciones y errores
- DataAnnotations: requeridos, longitudes y rango.
- Al marcar Exception se **exige nota** (throw si vacío).

## 🧭 Mejoras futuras (si hay tiempo)
- Paginación, búsqueda por texto.
- Concurrency token (`RowVersion`) para evitar overwrites.
- Pruebas unitarias de `WorkOrdersService`.
- Autenticación/Autorización básica.

---

© 2025-11-03 – Entrega lista para evaluación técnica.
