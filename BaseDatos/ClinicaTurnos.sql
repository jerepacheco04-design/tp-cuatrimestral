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
IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Cardiología')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Cardiología');
IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Dermatología')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Dermatología');
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
DECLARE @HorarioMedicos int;
SELECT @HorarioMedicos = Id FROM dbo.TurnosTrabajo WHERE Nombre = N'Horario de médicos de prueba';
IF @HorarioMedicos IS NULL
BEGIN
    INSERT INTO dbo.TurnosTrabajo (Nombre) VALUES (N'Horario de médicos de prueba');
    SET @HorarioMedicos = SCOPE_IDENTITY();
END
IF NOT EXISTS (SELECT 1 FROM dbo.FranjasHorarias WHERE TurnoTrabajoId = @HorarioMedicos AND Dia = 1)
    INSERT INTO dbo.FranjasHorarias (TurnoTrabajoId, Dia, HoraEntrada, HoraSalida) VALUES (@HorarioMedicos, 1, '09:00', '17:00');
IF NOT EXISTS (SELECT 1 FROM dbo.FranjasHorarias WHERE TurnoTrabajoId = @HorarioMedicos AND Dia = 2)
    INSERT INTO dbo.FranjasHorarias (TurnoTrabajoId, Dia, HoraEntrada, HoraSalida) VALUES (@HorarioMedicos, 2, '09:00', '17:00');
IF NOT EXISTS (SELECT 1 FROM dbo.FranjasHorarias WHERE TurnoTrabajoId = @HorarioMedicos AND Dia = 3)
    INSERT INTO dbo.FranjasHorarias (TurnoTrabajoId, Dia, HoraEntrada, HoraSalida) VALUES (@HorarioMedicos, 3, '09:00', '17:00');
IF NOT EXISTS (SELECT 1 FROM dbo.FranjasHorarias WHERE TurnoTrabajoId = @HorarioMedicos AND Dia = 4)
    INSERT INTO dbo.FranjasHorarias (TurnoTrabajoId, Dia, HoraEntrada, HoraSalida) VALUES (@HorarioMedicos, 4, '09:00', '17:00');
IF NOT EXISTS (SELECT 1 FROM dbo.FranjasHorarias WHERE TurnoTrabajoId = @HorarioMedicos AND Dia = 5)
    INSERT INTO dbo.FranjasHorarias (TurnoTrabajoId, Dia, HoraEntrada, HoraSalida) VALUES (@HorarioMedicos, 5, '09:00', '17:00');
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
IF COL_LENGTH(N'dbo.Medicos', N'UsuarioId') IS NULL
    ALTER TABLE dbo.Medicos ADD UsuarioId int NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Medicos_Usuarios' AND parent_object_id = OBJECT_ID(N'dbo.Medicos'))
    ALTER TABLE dbo.Medicos ADD CONSTRAINT FK_Medicos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Medicos_UsuarioId' AND object_id = OBJECT_ID(N'dbo.Medicos'))
    CREATE UNIQUE INDEX UX_Medicos_UsuarioId ON dbo.Medicos(UsuarioId) WHERE UsuarioId IS NOT NULL;
GO
DECLARE @SalUsuario varbinary(16) = 0x4E6F727465436C696E69636131323334;
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario = N'recepcionista')
BEGIN
    INSERT INTO dbo.Usuarios (NombreUsuario, ClaveHash, ClaveSalt, Nombre, Apellido, Email, Perfil, Activo)
    VALUES (N'recepcionista', HASHBYTES('SHA2_256', @SalUsuario + CONVERT(varbinary(max), N'recepcion123')),
            @SalUsuario, N'Recepción', N'Clínica', N'recepcion@clinica.local', 2, 1);
END
DECLARE @SalAdministrador varbinary(16) = 0x436C696E69636141646D696E32303236;
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario = N'administrador')
BEGIN
    INSERT INTO dbo.Usuarios (NombreUsuario, ClaveHash, ClaveSalt, Nombre, Apellido, Email, Perfil, Activo)
    VALUES (N'administrador', HASHBYTES('SHA2_256', @SalAdministrador + CONVERT(varbinary(max), N'admin123')),
            @SalAdministrador, N'Administrador', N'Clínica', N'admin@clinica.local', 1, 1);
END
DECLARE @SalMedicoUno varbinary(16) = 0x4D656469636F556E6F3230323653616C;
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario = N'medico1')
BEGIN
    INSERT INTO dbo.Usuarios (NombreUsuario, ClaveHash, ClaveSalt, Nombre, Apellido, Email, Perfil, Activo)
    VALUES (N'medico1', HASHBYTES('SHA2_256', @SalMedicoUno + CONVERT(varbinary(max), N'medico123')),
            @SalMedicoUno, N'Médico', N'Uno', N'medico1@clinica.local', 3, 1);
END
DECLARE @SalMedicoDos varbinary(16) = 0x4D656469636F446F733230323653616C;
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario = N'medico2')
BEGIN
    INSERT INTO dbo.Usuarios (NombreUsuario, ClaveHash, ClaveSalt, Nombre, Apellido, Email, Perfil, Activo)
    VALUES (N'medico2', HASHBYTES('SHA2_256', @SalMedicoDos + CONVERT(varbinary(max), N'medico123')),
            @SalMedicoDos, N'Médico', N'Dos', N'medico2@clinica.local', 3, 1);
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
DECLARE @HorarioMedicosId int;
DECLARE @EspecialidadUnoId int;
DECLARE @EspecialidadDosId int;
DECLARE @UsuarioUnoId int;
DECLARE @UsuarioDosId int;
DECLARE @MedicoUnoId int;
DECLARE @MedicoDosId int;
SELECT @HorarioMedicosId = Id FROM dbo.TurnosTrabajo WHERE Nombre = N'Horario de médicos de prueba';
SELECT @EspecialidadUnoId = Id FROM dbo.Especialidades WHERE Nombre = N'Cardiología';
SELECT @EspecialidadDosId = Id FROM dbo.Especialidades WHERE Nombre = N'Dermatología';
SELECT @UsuarioUnoId = Id FROM dbo.Usuarios WHERE NombreUsuario = N'medico1' AND Perfil = 3;
SELECT @UsuarioDosId = Id FROM dbo.Usuarios WHERE NombreUsuario = N'medico2' AND Perfil = 3;

IF @UsuarioUnoId IS NOT NULL AND @HorarioMedicosId IS NOT NULL
BEGIN
    SELECT @MedicoUnoId = Id FROM dbo.Medicos WHERE UsuarioId = @UsuarioUnoId;
    IF @MedicoUnoId IS NULL AND NOT EXISTS (SELECT 1 FROM dbo.Medicos WHERE Matricula = N'DEMO001')
    BEGIN
        INSERT INTO dbo.Medicos (Nombre, Apellido, Email, Matricula, TurnoTrabajoId, UsuarioId)
        VALUES (N'Médico', N'Uno', N'medico1@clinica.local', N'DEMO001', @HorarioMedicosId, @UsuarioUnoId);
        SET @MedicoUnoId = SCOPE_IDENTITY();
    END
    IF @MedicoUnoId IS NOT NULL AND @EspecialidadUnoId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.MedicosEspecialidades WHERE MedicoId = @MedicoUnoId)
        INSERT INTO dbo.MedicosEspecialidades (MedicoId, EspecialidadId) VALUES (@MedicoUnoId, @EspecialidadUnoId);
END

IF @UsuarioDosId IS NOT NULL AND @HorarioMedicosId IS NOT NULL
BEGIN
    SELECT @MedicoDosId = Id FROM dbo.Medicos WHERE UsuarioId = @UsuarioDosId;
    IF @MedicoDosId IS NULL AND NOT EXISTS (SELECT 1 FROM dbo.Medicos WHERE Matricula = N'DEMO002')
    BEGIN
        INSERT INTO dbo.Medicos (Nombre, Apellido, Email, Matricula, TurnoTrabajoId, UsuarioId)
        VALUES (N'Médico', N'Dos', N'medico2@clinica.local', N'DEMO002', @HorarioMedicosId, @UsuarioDosId);
        SET @MedicoDosId = SCOPE_IDENTITY();
    END
    IF @MedicoDosId IS NOT NULL AND @EspecialidadDosId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.MedicosEspecialidades WHERE MedicoId = @MedicoDosId)
        INSERT INTO dbo.MedicosEspecialidades (MedicoId, EspecialidadId) VALUES (@MedicoDosId, @EspecialidadDosId);
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
