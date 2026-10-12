# Reglas para agentes de CompraCerca

## Proyecto
- CompraCerca es un marketplace para Colombia.
- Backend: ASP.NET Core con C# y Entity Framework Core.
- Frontend: React, TypeScript y Vite.
- Base de datos de producción: Azure SQL.

## Reglas obligatorias
- Analizar el código antes de recomendar modificaciones.
- Revisar errores de compilación, seguridad, lógica y rendimiento.
- Verificar autenticación, autorización, validación de datos y manejo de errores.
- Comprobar la compatibilidad entre el frontend y la API.
- No exponer contraseñas, tokens, claves ni cadenas de conexión.
- No modificar configuraciones de producción ni ejecutar migraciones sin autorización.
- No eliminar archivos ni cambiar funcionalidades fuera del alcance solicitado.
- No modificar archivos automáticamente durante una revisión.
- No crear commits ni subir cambios a GitHub sin autorización explícita.
- Indicar el archivo, la ubicación, la gravedad y la explicación de cada problema.
- No inventar errores. Si no hay evidencia suficiente, indicarlo.
- Priorizar errores reales sobre sugerencias de estilo.

## Resultado esperado
- Resumen general de la revisión.
- Problemas encontrados, clasificados por gravedad.
- Evidencias concretas de cada problema.
- Recomendaciones para corregirlos.
- Indicar qué aspectos no pudieron verificarse.
