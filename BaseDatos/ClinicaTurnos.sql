IF DB_ID(N'ClinicaTurnos') IS NULL
    CREATE DATABASE ClinicaTurnos;
GO
USE ClinicaTurnos;
GO
IF OBJECT_ID(N'dbo.Pacientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pacientes
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Dni nvarchar(20) NOT NULL,
        Nombre nvarchar(80) NOT NULL,
        Apellido nvarchar(80) NOT NULL,
        FechaNacimiento date NOT NULL,
        Telefono nvarchar(30) NOT NULL,
        Email nvarchar(254) NOT NULL,
        Direccion nvarchar(250) NOT NULL
    );
END
GO
IF OBJECT_ID(N'dbo.Especialidades', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Especialidades
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Nombre nvarchar(80) NOT NULL
    );
END
GO
IF OBJECT_ID(N'dbo.TurnosTrabajo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TurnosTrabajo
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Nombre nvarchar(80) NOT NULL,
        CONSTRAINT UQ_TurnosTrabajo_Nombre UNIQUE (Nombre)
    );
END
GO
IF OBJECT_ID(N'dbo.FranjasHorarias', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FranjasHorarias
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        TurnoTrabajoId int NOT NULL,
        Dia tinyint NOT NULL,
        HoraEntrada time(0) NOT NULL,
        HoraSalida time(0) NOT NULL,
        CONSTRAINT FK_FranjasHorarias_TurnosTrabajo FOREIGN KEY (TurnoTrabajoId) REFERENCES dbo.TurnosTrabajo(Id) ON DELETE CASCADE,
        CONSTRAINT CK_FranjasHorarias_Dia CHECK (Dia BETWEEN 0 AND 6),
        CONSTRAINT CK_FranjasHorarias_Horario CHECK (HoraEntrada < HoraSalida),
        CONSTRAINT UQ_FranjasHorarias_Turno_Dia UNIQUE (TurnoTrabajoId, Dia)
    );
END
GO
IF OBJECT_ID(N'dbo.Medicos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Medicos
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Nombre nvarchar(80) NOT NULL,
        Apellido nvarchar(80) NOT NULL,
        Email nvarchar(254) NOT NULL,
        Matricula nvarchar(30) NOT NULL,
        TurnoTrabajoId int NOT NULL,
        CONSTRAINT UQ_Medicos_Matricula UNIQUE (Matricula),
        CONSTRAINT FK_Medicos_TurnosTrabajo FOREIGN KEY (TurnoTrabajoId) REFERENCES dbo.TurnosTrabajo(Id)
    );
END
GO
IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        NombreUsuario nvarchar(50) NOT NULL,
        ClaveHash varbinary(32) NOT NULL,
        ClaveSalt varbinary(16) NOT NULL,
        Nombre nvarchar(80) NOT NULL,
        Apellido nvarchar(80) NOT NULL,
        Email nvarchar(254) NOT NULL,
        Perfil tinyint NOT NULL,
        Activo bit NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
        CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE (NombreUsuario),
        CONSTRAINT CK_Usuarios_Perfil CHECK (Perfil BETWEEN 1 AND 3)
    );
END
GO
IF COL_LENGTH(N'dbo.Usuarios', N'ClaveIteraciones') IS NULL
    ALTER TABLE dbo.Usuarios ADD ClaveIteraciones int NULL;
GO
IF COL_LENGTH(N'dbo.Medicos', N'UsuarioId') IS NULL
    ALTER TABLE dbo.Medicos ADD UsuarioId int NULL;
GO
IF OBJECT_ID(N'dbo.FK_Medicos_Usuarios', N'F') IS NULL
    ALTER TABLE dbo.Medicos ADD CONSTRAINT FK_Medicos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id);
GO
SET QUOTED_IDENTIFIER ON;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Medicos') AND name = N'UX_Medicos_Usuario')
    CREATE UNIQUE INDEX UX_Medicos_Usuario ON dbo.Medicos(UsuarioId) WHERE UsuarioId IS NOT NULL;
GO
DECLARE @SalUsuario varbinary(16) = 0x4E6F727465436C696E69636131323334;
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario = N'recepcionista')
BEGIN
    INSERT INTO dbo.Usuarios (NombreUsuario, ClaveHash, ClaveSalt, Nombre, Apellido, Email, Perfil, Activo)
    VALUES (N'recepcionista', HASHBYTES('SHA2_256', @SalUsuario + CONVERT(varbinary(max), N'recepcion123')),
            @SalUsuario, N'Recepción', N'Clínica', N'recepcion@clinica.local', 2, 1);
END
GO
IF OBJECT_ID(N'dbo.MedicosEspecialidades', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MedicosEspecialidades
    (
        MedicoId int NOT NULL,
        EspecialidadId int NOT NULL,
        CONSTRAINT PK_MedicosEspecialidades PRIMARY KEY (MedicoId, EspecialidadId),
        CONSTRAINT FK_MedicosEspecialidades_Medicos FOREIGN KEY (MedicoId) REFERENCES dbo.Medicos(Id) ON DELETE CASCADE,
        CONSTRAINT FK_MedicosEspecialidades_Especialidades FOREIGN KEY (EspecialidadId) REFERENCES dbo.Especialidades(Id)
    );
END
GO
IF OBJECT_ID(N'dbo.Turnos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Turnos
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        PacienteId int NOT NULL,
        MedicoId int NOT NULL,
        EspecialidadId int NOT NULL,
        FechaHora datetime2(0) NOT NULL,
        Estado tinyint NOT NULL CONSTRAINT DF_Turnos_Estado DEFAULT (1),
        MotivoConsulta nvarchar(500) NOT NULL,
        ObservacionesDiagnostico nvarchar(1000) NULL,
        CONSTRAINT FK_Turnos_Pacientes FOREIGN KEY (PacienteId) REFERENCES dbo.Pacientes(Id),
        CONSTRAINT FK_Turnos_MedicosEspecialidades FOREIGN KEY (MedicoId, EspecialidadId) REFERENCES dbo.MedicosEspecialidades(MedicoId, EspecialidadId),
        CONSTRAINT CK_Turnos_Estado CHECK (Estado BETWEEN 1 AND 5)
    );
END
GO
IF COL_LENGTH(N'dbo.Turnos', N'RecepcionistaId') IS NULL
    ALTER TABLE dbo.Turnos ADD RecepcionistaId int NULL;
GO
IF OBJECT_ID(N'dbo.FK_Turnos_Usuarios_Recepcionista', N'F') IS NULL
    ALTER TABLE dbo.Turnos ADD CONSTRAINT FK_Turnos_Usuarios_Recepcionista FOREIGN KEY (RecepcionistaId) REFERENCES dbo.Usuarios(Id);
GO
