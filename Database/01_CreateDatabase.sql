/* ============================================================
   Caso Práctico 1 - Programación Avanzada Web (SC-701)
   Script de creación de la base de datos EarthquakesDB (SQL Server)

   Análisis: el JSON de USGS trae un arreglo "features". Cada feature es
   un temblor con sus propiedades (properties) y su ubicación (geometry).
   Con una sola tabla alcanza: una fila por feature.

   JSON de USGS                    ->  Columna
   -------------------------------------------------------
   feature.id                      ->  UsgsId
   properties.mag                  ->  Magnitude
   properties.place                ->  Place
   properties.time (epoch ms)      ->  Time (UTC)
   properties.url                  ->  Url
   geometry.coordinates[0]         ->  Longitude
   geometry.coordinates[1]         ->  Latitude
   geometry.coordinates[2]         ->  Depth (km)
   ============================================================ */

IF DB_ID('EarthquakesDB') IS NULL
    CREATE DATABASE EarthquakesDB;
GO

USE EarthquakesDB;
GO

IF OBJECT_ID('dbo.Earthquakes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Earthquakes (
        Id         INT IDENTITY(1,1) NOT NULL,
        UsgsId     NVARCHAR(50)      NOT NULL,
        Magnitude  DECIMAL(4, 2)     NULL,
        Place      NVARCHAR(255)     NULL,
        [Time]     DATETIME          NOT NULL,
        Latitude   FLOAT             NULL,
        Longitude  FLOAT             NULL,
        Depth      FLOAT             NULL,
        Url        NVARCHAR(500)     NULL,
        CONSTRAINT PK_Earthquakes PRIMARY KEY CLUSTERED (Id)
    );

    CREATE INDEX IX_Earthquakes_UsgsId ON dbo.Earthquakes (UsgsId);
    CREATE INDEX IX_Earthquakes_Time   ON dbo.Earthquakes ([Time]);

    -- La búsqueda por país hace LIKE '%país%' sobre Place. Este índice es angosto
    -- (solo Place y Time), así SQL Server lee menos datos al recorrerlo.
    CREATE INDEX IX_Earthquakes_Place  ON dbo.Earthquakes (Place) INCLUDE ([Time]);
END
GO
