-- =====================================================================
--  SISTEMA ACADÉMICO - Versión simplificada para SQL Server
--  Ejecutar en SQL Server Management Studio (SSMS)
-- =====================================================================

CREATE DATABASE SistemaAcademico;
GO
USE SistemaAcademico;
GO

-- ---------------------------------------------------------------------
-- USUARIOS: login de alumnos y del encargado
-- activo = 0 -> dado de baja (no puede loguearse)
-- ---------------------------------------------------------------------
CREATE TABLE Usuarios (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Usuario      NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Rol          NVARCHAR(20)  NOT NULL CHECK (Rol IN ('ALUMNO', 'ENCARGADO')),
    Activo       BIT           NOT NULL DEFAULT 1
);
GO

-- ---------------------------------------------------------------------
-- ALUMNOS
-- ---------------------------------------------------------------------
CREATE TABLE Alumnos (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT           NOT NULL UNIQUE,
    Legajo    NVARCHAR(20)  NOT NULL UNIQUE,
    Dni       NVARCHAR(15)  NOT NULL UNIQUE,
    Nombre    NVARCHAR(60)  NOT NULL,
    Apellido  NVARCHAR(60)  NOT NULL,
    Email     NVARCHAR(120) NULL,
    CONSTRAINT FK_Alumnos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO

-- ---------------------------------------------------------------------
-- PROFESORES
-- ---------------------------------------------------------------------
CREATE TABLE Profesores (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Dni      NVARCHAR(15)  NOT NULL UNIQUE,
    Nombre   NVARCHAR(60)  NOT NULL,
    Apellido NVARCHAR(60)  NOT NULL,
    Email    NVARCHAR(120) NULL
);
GO

-- ---------------------------------------------------------------------
-- CARRERAS
-- ---------------------------------------------------------------------
CREATE TABLE Carreras (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        NVARCHAR(120) NOT NULL,
    DuracionAnios TINYINT       NOT NULL
);
GO

-- ---------------------------------------------------------------------
-- MATERIAS: cada materia es de una carrera y la dicta un profesor
-- ---------------------------------------------------------------------
CREATE TABLE Materias (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    CarreraId  INT           NOT NULL,
    ProfesorId INT           NULL,
    Nombre     NVARCHAR(120) NOT NULL,
    Anio       TINYINT       NOT NULL,   -- año de la carrera (1°, 2°...)
    CONSTRAINT FK_Materias_Carreras   FOREIGN KEY (CarreraId)  REFERENCES Carreras(Id),
    CONSTRAINT FK_Materias_Profesores FOREIGN KEY (ProfesorId) REFERENCES Profesores(Id)
);
GO

-- ---------------------------------------------------------------------
-- INSCRIPCIONES: alumno inscripto a una carrera
-- ---------------------------------------------------------------------
CREATE TABLE Inscripciones (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    AlumnoId  INT  NOT NULL,
    CarreraId INT  NOT NULL,
    Fecha     DATE NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Inscripcion UNIQUE (AlumnoId, CarreraId),
    CONSTRAINT FK_Inscripciones_Alumnos  FOREIGN KEY (AlumnoId)  REFERENCES Alumnos(Id),
    CONSTRAINT FK_Inscripciones_Carreras FOREIGN KEY (CarreraId) REFERENCES Carreras(Id)
);
GO

-- ---------------------------------------------------------------------
-- NOTAS: las carga el encargado
-- ---------------------------------------------------------------------
CREATE TABLE Notas (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    AlumnoId  INT          NOT NULL,
    MateriaId INT          NOT NULL,
    Tipo      NVARCHAR(20) NOT NULL CHECK (Tipo IN ('PARCIAL 1', 'PARCIAL 2', 'RECUPERATORIO', 'FINAL')),
    Nota      DECIMAL(4,2) NOT NULL CHECK (Nota BETWEEN 1 AND 10),
    Fecha     DATE         NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Notas_Alumnos  FOREIGN KEY (AlumnoId)  REFERENCES Alumnos(Id),
    CONSTRAINT FK_Notas_Materias FOREIGN KEY (MateriaId) REFERENCES Materias(Id)
);
GO

