# Caso Práctico 1 - Buscador de Temblores

Programación Avanzada Web (SC-701)

Es una app donde uno escribe el nombre de un país y le salen los temblores de ese país más la info del país.

## Qué trae

- **Earthquakes.API**: el Web API. Llama a USGS (temblores) y a REST Countries (info del país).
- **Earthquakes.DataAccess**: Entity Framework y el repository.
- **Earthquakes.Dtos**: los DTOs.
- **Earthquakes.Web**: el MVC con el buscador.
- **Database**: el script para crear la base de datos.

## Cómo correrlo

1. Correr `Database/01_CreateDatabase.sql` en SQL Server para crear la base y la tabla.
2. Si no usan `.\SQLEXPRESS`, cambiar el connection string en `Earthquakes.API/appsettings.json`.
3. La API key de REST Countries va en ese mismo `appsettings.json` (`RestCountries:ApiKey`).
4. Correr primero el API (perfil `http`) y después el Web. En Visual Studio se pueden poner los dos como proyectos de inicio.
5. Abrir `http://localhost:5167/` y buscar un país.

## Ojo con esto

- La primera vez que corre el API la base está vacía, entonces baja todos los temblores de USGS (como 140 mil). Dura unos minutos y hasta que termine el API no responde. Las siguientes veces ya no vuelve a llamar a USGS.
- USGS solo deja traer 20 000 por consulta, por eso los pide mes por mes.
- Si quieren traer solo los temblores nuevos, está el `POST /Earthquake/refresh`.
- El país se puede buscar en inglés o español (Japan, Japón, Canadá...).
- Todas las llamadas a APIs usan memory cache.
- En el MVC no se aceptan números ni caracteres especiales, y máximo 100 caracteres. Los errores salen con SweetAlert2.
