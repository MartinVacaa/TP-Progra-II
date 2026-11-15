-- =====================================================================
--  SISTEMA ACADÉMICO - Script COMPLETO para SQL Server
--  Alumnos con LEGAJO como Primary Key
--
--  ATENCIÓN: borra la base SistemaAcademico si existe y la crea de nuevo
--  con todas las tablas y datos de ejemplo. Reemplaza a los scripts
--  anteriores (creación + inserts).
-- =====================================================================
USE master;
GO

IF DB_ID('SistemaAcademico') IS NOT NULL
BEGIN
    ALTER DATABASE SistemaAcademico SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SistemaAcademico;
END
GO

CREATE DATABASE SistemaAcademico;
GO
USE SistemaAcademico;
GO

-- =====================================================================
-- TABLAS
-- =====================================================================

-- USUARIOS: login de alumnos y del encargado
CREATE TABLE Usuarios (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Usuario      NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Rol          NVARCHAR(20)  NOT NULL CHECK (Rol IN ('ALUMNO', 'ENCARGADO')),
    Activo       BIT           NOT NULL DEFAULT 1
);
GO

-- Solo puede existir UN encargado
CREATE UNIQUE INDEX UX_Usuarios_UnSoloEncargado
    ON Usuarios(Rol)
    WHERE Rol = 'ENCARGADO';
GO

-- ALUMNOS: el LEGAJO es la clave primaria
CREATE TABLE Alumnos (
    Legajo    NVARCHAR(20)  NOT NULL PRIMARY KEY,
    UsuarioId INT           NOT NULL UNIQUE,
    Dni       NVARCHAR(15)  NOT NULL UNIQUE,
    Nombre    NVARCHAR(60)  NOT NULL,
    Apellido  NVARCHAR(60)  NOT NULL,
    Email     NVARCHAR(120) NULL,
    CONSTRAINT FK_Alumnos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO

-- PROFESORES (no tienen usuario)
CREATE TABLE Profesores (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Dni      NVARCHAR(15)  NOT NULL UNIQUE,
    Nombre   NVARCHAR(60)  NOT NULL,
    Apellido NVARCHAR(60)  NOT NULL,
    Email    NVARCHAR(120) NULL
);
GO

-- CARRERAS
CREATE TABLE Carreras (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        NVARCHAR(120) NOT NULL,
    DuracionAnios TINYINT       NOT NULL
);
GO

-- MATERIAS
CREATE TABLE Materias (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    CarreraId  INT           NOT NULL,
    ProfesorId INT           NULL,
    Nombre     NVARCHAR(120) NOT NULL,
    Anio       TINYINT       NOT NULL,
    CONSTRAINT FK_Materias_Carreras   FOREIGN KEY (CarreraId)  REFERENCES Carreras(Id),
    CONSTRAINT FK_Materias_Profesores FOREIGN KEY (ProfesorId) REFERENCES Profesores(Id)
);
GO

-- INSCRIPCIONES: alumno (por legajo) inscripto a una carrera
CREATE TABLE Inscripciones (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Legajo    NVARCHAR(20) NOT NULL,
    CarreraId INT          NOT NULL,
    Fecha     DATE         NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Inscripcion UNIQUE (Legajo, CarreraId),
    CONSTRAINT FK_Inscripciones_Alumnos  FOREIGN KEY (Legajo)    REFERENCES Alumnos(Legajo),
    CONSTRAINT FK_Inscripciones_Carreras FOREIGN KEY (CarreraId) REFERENCES Carreras(Id)
);
GO

-- NOTAS: las carga el encargado
CREATE TABLE Notas (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Legajo    NVARCHAR(20) NOT NULL,
    MateriaId INT          NOT NULL,
    Tipo      NVARCHAR(20) NOT NULL CHECK (Tipo IN ('PARCIAL 1', 'PARCIAL 2', 'RECUPERATORIO', 'FINAL')),
    Nota      DECIMAL(4,2) NOT NULL CHECK (Nota BETWEEN 1 AND 10),
    Fecha     DATE         NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Notas_Alumnos  FOREIGN KEY (Legajo)    REFERENCES Alumnos(Legajo),
    CONSTRAINT FK_Notas_Materias FOREIGN KEY (MateriaId) REFERENCES Materias(Id)
);
GO

-- =====================================================================
-- DATOS
-- Reemplazá 'HASH_DE_EJEMPLO' por hashes reales generados en la API
-- =====================================================================

-- USUARIOS (1 encargado + 10 alumnos)
INSERT INTO Usuarios (Usuario, PasswordHash, Rol) VALUES
('encargado',  'HASH_DE_EJEMPLO', 'ENCARGADO'),
('jperez',     'HASH_DE_EJEMPLO', 'ALUMNO'),
('mgomez',     'HASH_DE_EJEMPLO', 'ALUMNO'),
('lrodriguez', 'HASH_DE_EJEMPLO', 'ALUMNO'),
('sfernandez', 'HASH_DE_EJEMPLO', 'ALUMNO'),
('mlopez',     'HASH_DE_EJEMPLO', 'ALUMNO'),
('agarcia',    'HASH_DE_EJEMPLO', 'ALUMNO'),
('csanchez',   'HASH_DE_EJEMPLO', 'ALUMNO'),
('vromero',    'HASH_DE_EJEMPLO', 'ALUMNO'),
('ndiaz',      'HASH_DE_EJEMPLO', 'ALUMNO'),
('ftorres',    'HASH_DE_EJEMPLO', 'ALUMNO');
GO

-- ALUMNOS (Legajo como clave)
INSERT INTO Alumnos (Legajo, UsuarioId, Dni, Nombre, Apellido, Email) VALUES
('L-0001', (SELECT Id FROM Usuarios WHERE Usuario = 'jperez'),     '40111222', N'Juan',      N'Pérez',     'jperez@mail.com'),
('L-0002', (SELECT Id FROM Usuarios WHERE Usuario = 'mgomez'),     '41222333', N'María',     N'Gómez',     'mgomez@mail.com'),
('L-0003', (SELECT Id FROM Usuarios WHERE Usuario = 'lrodriguez'), '42333444', N'Lucas',     N'Rodríguez', 'lrodriguez@mail.com'),
('L-0004', (SELECT Id FROM Usuarios WHERE Usuario = 'sfernandez'), '42555666', N'Sofía',     N'Fernández', 'sfernandez@mail.com'),
('L-0005', (SELECT Id FROM Usuarios WHERE Usuario = 'mlopez'),     '43111222', N'Martín',    N'López',     'mlopez@mail.com'),
('L-0006', (SELECT Id FROM Usuarios WHERE Usuario = 'agarcia'),    '43444555', N'Agustina',  N'García',    'agarcia@mail.com'),
('L-0007', (SELECT Id FROM Usuarios WHERE Usuario = 'csanchez'),   '44222333', N'Camila',    N'Sánchez',   'csanchez@mail.com'),
('L-0008', (SELECT Id FROM Usuarios WHERE Usuario = 'vromero'),    '44666777', N'Valentín',  N'Romero',    'vromero@mail.com'),
('L-0009', (SELECT Id FROM Usuarios WHERE Usuario = 'ndiaz'),      '45111888', N'Nicolás',   N'Díaz',      'ndiaz@mail.com'),
('L-0010', (SELECT Id FROM Usuarios WHERE Usuario = 'ftorres'),    '45333999', N'Florencia', N'Torres',    'ftorres@mail.com');
GO

-- PROFESORES
INSERT INTO Profesores (Dni, Nombre, Apellido, Email) VALUES
('25111222', N'Carlos',  N'Fernández', 'cfernandez@instituto.edu'),
('27333444', N'Laura',   N'Martínez',  'lmartinez@instituto.edu'),
('23444555', N'Roberto', N'Acosta',    'racosta@instituto.edu'),
('26777888', N'Silvina', N'Herrera',   'sherrera@instituto.edu'),
('28999111', N'Gustavo', N'Medina',    'gmedina@instituto.edu'),
('30222333', N'Paula',   N'Castro',    'pcastro@instituto.edu'),
('31555666', N'Diego',   N'Molina',    'dmolina@instituto.edu');
GO

-- CARRERAS (Id 1 y 2)
INSERT INTO Carreras (Nombre, DuracionAnios) VALUES
(N'Tecnicatura en Programación', 2),
(N'Licenciatura en Sistemas',    5);
GO

-- MATERIAS (profesor buscado por DNI)
INSERT INTO Materias (CarreraId, ProfesorId, Nombre, Anio)
SELECT v.CarreraId, p.Id, v.Materia, v.Anio
FROM (VALUES
    -- 1 = Tecnicatura en Programación
    (1, N'Programación I',                    1, '23444555'),  -- Acosta
    (1, N'Matemática',                        1, '26777888'),  -- Herrera
    (1, N'Bases de Datos',                    1, '28999111'),  -- Medina
    (1, N'Inglés Técnico',                    1, '30222333'),  -- Castro
    (1, N'Programación II',                   2, '25111222'),  -- Fernández
    (1, N'Sistemas Operativos',               2, '31555666'),  -- Molina
    (1, N'Metodología de Sistemas',           2, '27333444'),  -- Martínez
    -- 2 = Licenciatura en Sistemas
    (2, N'Algoritmos y Estructuras de Datos', 1, '28999111'),  -- Medina
    (2, N'Análisis Matemático I',             1, '26777888'),  -- Herrera
    (2, N'Arquitectura de Computadoras',      1, '31555666'),  -- Molina
    (2, N'Sistemas y Organizaciones',         1, '30222333'),  -- Castro
    (2, N'Paradigmas de Programación',        2, '23444555')   -- Acosta
) AS v(CarreraId, Materia, Anio, DniProfesor)
LEFT JOIN Profesores p ON p.Dni = v.DniProfesor;
GO

-- INSCRIPCIONES (1 = Tecnicatura, 2 = Licenciatura)
INSERT INTO Inscripciones (Legajo, CarreraId, Fecha) VALUES
('L-0001', 1, '2026-03-02'),
('L-0002', 1, '2026-03-02'),
('L-0003', 1, '2026-03-02'),
('L-0004', 1, '2026-03-03'),
('L-0005', 1, '2026-03-03'),
('L-0006', 2, '2026-03-02'),
('L-0007', 2, '2026-03-04'),
('L-0008', 1, '2026-03-04'),
('L-0009', 2, '2026-03-05'),
('L-0010', 1, '2026-03-05');
GO

-- NOTAS (materia buscada por nombre dentro de la carrera del alumno)
INSERT INTO Notas (Legajo, MateriaId, Tipo, Nota, Fecha)
SELECT v.Legajo, m.Id, v.Tipo, v.Nota, v.Fecha
FROM (VALUES
    -- ===== TECNICATURA EN PROGRAMACIÓN =====
    ('L-0001', N'Programación I', 'PARCIAL 1', 7.50, '2026-05-10'),
    ('L-0001', N'Programación I', 'PARCIAL 2', 8.00, '2026-06-20'),
    ('L-0001', N'Programación I', 'FINAL',     8.00, '2026-07-15'),
    ('L-0001', N'Matemática',     'PARCIAL 1', 6.00, '2026-05-12'),
    ('L-0001', N'Matemática',     'PARCIAL 2', 7.00, '2026-06-22'),

    ('L-0002', N'Programación I', 'PARCIAL 1', 9.00, '2026-05-10'),
    ('L-0002', N'Programación I', 'PARCIAL 2', 9.50, '2026-06-20'),
    ('L-0002', N'Programación I', 'FINAL',     9.00, '2026-07-15'),
    ('L-0002', N'Bases de Datos', 'PARCIAL 1', 8.00, '2026-05-14'),
    ('L-0002', N'Bases de Datos', 'PARCIAL 2', 8.50, '2026-06-24'),

    ('L-0003', N'Programación I', 'PARCIAL 1',     3.00, '2026-05-10'),
    ('L-0003', N'Programación I', 'RECUPERATORIO', 6.00, '2026-06-05'),
    ('L-0003', N'Programación I', 'PARCIAL 2',     7.00, '2026-06-20'),
    ('L-0003', N'Matemática',     'PARCIAL 1',     5.00, '2026-05-12'),
    ('L-0003', N'Inglés Técnico', 'PARCIAL 1',     8.00, '2026-05-16'),

    ('L-0004', N'Programación I', 'PARCIAL 1', 10.00, '2026-05-10'),
    ('L-0004', N'Programación I', 'PARCIAL 2',  9.00, '2026-06-20'),
    ('L-0004', N'Programación I', 'FINAL',      9.50, '2026-07-15'),
    ('L-0004', N'Bases de Datos', 'PARCIAL 1',  9.00, '2026-05-14'),
    ('L-0004', N'Bases de Datos', 'PARCIAL 2', 10.00, '2026-06-24'),
    ('L-0004', N'Bases de Datos', 'FINAL',      9.00, '2026-07-17'),

    ('L-0005', N'Matemática',     'PARCIAL 1',     4.00, '2026-05-12'),
    ('L-0005', N'Matemática',     'RECUPERATORIO', 4.00, '2026-06-08'),
    ('L-0005', N'Bases de Datos', 'PARCIAL 1',     6.50, '2026-05-14'),
    ('L-0005', N'Bases de Datos', 'PARCIAL 2',     7.00, '2026-06-24'),

    ('L-0008', N'Programación I', 'PARCIAL 1', 6.00, '2026-05-10'),
    ('L-0008', N'Programación I', 'PARCIAL 2', 6.50, '2026-06-20'),
    ('L-0008', N'Inglés Técnico', 'PARCIAL 1', 7.00, '2026-05-16'),
    ('L-0008', N'Inglés Técnico', 'PARCIAL 2', 8.00, '2026-06-26'),
    ('L-0008', N'Inglés Técnico', 'FINAL',     7.50, '2026-07-20'),

    ('L-0010', N'Programación I', 'PARCIAL 1', 8.00, '2026-05-10'),
    ('L-0010', N'Programación I', 'PARCIAL 2', 7.50, '2026-06-20'),
    ('L-0010', N'Matemática',     'PARCIAL 1', 9.00, '2026-05-12'),
    ('L-0010', N'Matemática',     'PARCIAL 2', 8.50, '2026-06-22'),
    ('L-0010', N'Matemática',     'FINAL',     9.00, '2026-07-16'),

    -- ===== LICENCIATURA EN SISTEMAS =====
    ('L-0006', N'Algoritmos y Estructuras de Datos', 'PARCIAL 1', 8.00, '2026-05-11'),
    ('L-0006', N'Algoritmos y Estructuras de Datos', 'PARCIAL 2', 7.50, '2026-06-21'),
    ('L-0006', N'Análisis Matemático I',             'PARCIAL 1', 6.00, '2026-05-13'),
    ('L-0006', N'Sistemas y Organizaciones',         'PARCIAL 1', 9.00, '2026-05-15'),
    ('L-0006', N'Sistemas y Organizaciones',         'FINAL',     9.00, '2026-07-18'),

    ('L-0007', N'Algoritmos y Estructuras de Datos', 'PARCIAL 1',     2.00, '2026-05-11'),
    ('L-0007', N'Algoritmos y Estructuras de Datos', 'RECUPERATORIO', 6.50, '2026-06-06'),
    ('L-0007', N'Arquitectura de Computadoras',      'PARCIAL 1',     7.00, '2026-05-17'),
    ('L-0007', N'Arquitectura de Computadoras',      'PARCIAL 2',     7.50, '2026-06-27'),

    ('L-0009', N'Análisis Matemático I',        'PARCIAL 1',  9.50, '2026-05-13'),
    ('L-0009', N'Análisis Matemático I',        'PARCIAL 2', 10.00, '2026-06-23'),
    ('L-0009', N'Análisis Matemático I',        'FINAL',     10.00, '2026-07-16'),
    ('L-0009', N'Arquitectura de Computadoras', 'PARCIAL 1',  5.00, '2026-05-17'),
    ('L-0009', N'Sistemas y Organizaciones',    'PARCIAL 1',  8.00, '2026-05-15')
) AS v(Legajo, Materia, Tipo, Nota, Fecha)
JOIN Inscripciones i ON i.Legajo = v.Legajo
JOIN Materias      m ON m.Nombre = v.Materia AND m.CarreraId = i.CarreraId;
GO

-- =====================================================================
-- VERIFICACIÓN
-- =====================================================================
SELECT 'Usuarios' AS Tabla, COUNT(*) AS Registros FROM Usuarios
UNION ALL SELECT 'Alumnos',       COUNT(*) FROM Alumnos
UNION ALL SELECT 'Profesores',    COUNT(*) FROM Profesores
UNION ALL SELECT 'Carreras',      COUNT(*) FROM Carreras
UNION ALL SELECT 'Materias',      COUNT(*) FROM Materias
UNION ALL SELECT 'Inscripciones', COUNT(*) FROM Inscripciones
UNION ALL SELECT 'Notas',         COUNT(*) FROM Notas;

-- Notas de cada alumno
SELECT a.Legajo, a.Apellido + ', ' + a.Nombre AS Alumno,
       m.Nombre AS Materia, n.Tipo, n.Nota, n.Fecha
FROM Notas n
JOIN Alumnos  a ON a.Legajo = n.Legajo
JOIN Materias m ON m.Id     = n.MateriaId
ORDER BY a.Legajo, m.Nombre, n.Fecha;
GO

-- =====================================================================
-- CONSULTAS ÚTILES PARA EL BACKEND
-- =====================================================================
-- Notas del alumno logueado:
-- SELECT m.Nombre AS Materia, n.Tipo, n.Nota, n.Fecha
-- FROM Notas n
-- JOIN Materias m ON m.Id = n.MateriaId
-- JOIN Alumnos  a ON a.Legajo = n.Legajo
-- WHERE a.UsuarioId = @usuarioId;

-- Baja / alta de un alumno (por legajo):
-- UPDATE Usuarios SET Activo = 0 WHERE Id = (SELECT UsuarioId FROM Alumnos WHERE Legajo = @legajo);
-- UPDATE Usuarios SET Activo = 1 WHERE Id = (SELECT UsuarioId FROM Alumnos WHERE Legajo = @legajo);