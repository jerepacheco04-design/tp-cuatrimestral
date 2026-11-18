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
