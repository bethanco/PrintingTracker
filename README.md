# PrintingJobTracker

Aplicación **.NET 8 Blazor Server** para la gestión y seguimiento de órdenes de impresión.

## Descripción
Permite registrar, consultar y monitorear el estado de las órdenes de trabajo (jobs), con historial por estado y panel de resumen. Arquitectura modular con inyección de dependencias (DI), separación de capas y componentes reutilizables.

## Tecnologías
- .NET 8 (Blazor Server)
- C# 12
- Entity Framework Core
- SQL Server
- Bootstrap 5

## Estructura
```
PrintingJobTracker/
├─ Data/        # Contexto y migraciones
├─ Models/      # Entidades de dominio
├─ Services/    # Servicios y lógica de negocio
├─ Pages/       # Páginas (.razor)
├─ Shared/      # Componentes compartidos (StatusFilter, SearchBox, Pager)
├─ wwwroot/     # Estáticos
└─ Program.cs   # Configuración y DI
```

## Ejecución local
1. Visual Studio 2022: abrir solución y restaurar paquetes.
2. Verificar conexión en `appsettings.json` (SQL Server/LocalDB).
3. (Si usas EF) aplicar migraciones:
   ```bash
   dotnet ef database update
   ```
4. Ejecutar con **F5**.


