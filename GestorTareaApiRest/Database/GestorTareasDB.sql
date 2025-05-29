CREATE DATABASE GestorTareasDB;
GO

USE GestorTareasDB;
GO

CREATE TABLE Usuario (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Nombres NVARCHAR(100) NOT NULL,
	ApellidoPaterno NVARCHAR(100) NOT NULL,
	ApellidoMaterno NVARCHAR(100) NOT NULL,
	Usuario NVARCHAR(100) NOT NULL UNIQUE,
	ClaveSecreta NVARCHAR(255) NOT NULL
);
GO

CREATE TABLE Tarea (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Titulo NVARCHAR(100) NOT NULL,
	Descripcion NVARCHAR(MAX),
	Completado BIT NOT NULL DEFAULT 0,
	UsuarioId INT NOT NULL,
	FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
);
GO

CREATE PROCEDURE sp_RegistrarUsuario
	@Nombres NVARCHAR(100),
	@ApellidoPaterno NVARCHAR(100),
	@ApellidoMaterno NVARCHAR(100),
	@Usuario NVARCHAR(100),
	@ClaveSecreta NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Usuario (Nombres, ApellidoPaterno, ApellidoMaterno, Usuario, ClaveSecreta)
	VALUES (@Nombres, @ApellidoPaterno, @ApellidoMaterno, @Usuario, @ClaveSecreta);
END;
GO

CREATE PROCEDURE sp_Login
	@Usuario NVARCHAR(100)
AS
BEGIN
	SELECT Id, Usuario, ClaveSecreta FROM Usuario
	WHERE Usuario = @Usuario;
END;
GO

CREATE PROCEDURE sp_RegistrarTarea
	@Titulo NVARCHAR(100),
	@Descripcion NVARCHAR(MAX),
	@Completado BIT,
	@UsuarioId INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Tarea (Titulo, Descripcion, Completado, UsuarioId)
	VALUES (@Titulo, @Descripcion, @Completado, @UsuarioId);
END;
GO

CREATE PROCEDURE sp_ListarTareas
    @Completado BIT = NULL,
    @Pagina INT = 1,
    @Limite INT = 10
AS
BEGIN
    SELECT * FROM Tarea
    WHERE @Completado IS NULL OR Completado = @Completado
    ORDER BY Id
    OFFSET (@Pagina - 1) * @Limite ROWS
    FETCH NEXT @Limite ROWS ONLY;
END;
GO

CREATE PROCEDURE sp_ListarTareasUsuario
    @UsuarioId INT,
    @Completado BIT = NULL,
    @Pagina INT = 1,
    @Limite INT = 10
AS
BEGIN
    SELECT * FROM Tarea
    WHERE UsuarioId = @UsuarioId AND (@Completado IS NULL OR Completado = @Completado)
    ORDER BY Id
    OFFSET (@Pagina - 1) * @Limite ROWS
    FETCH NEXT @Limite ROWS ONLY;
END;
GO

CREATE PROCEDURE sp_ObtenerTarea
	@Id INT,
	@UsuarioID INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM Tarea
	WHERE Id = @Id AND UsuarioId = @UsuarioID;
END;
GO

CREATE PROCEDURE sp_ActualizarTarea
	@Id INT,
	@Titulo NVARCHAR(100),
	@Descripcion NVARCHAR(MAX),
	@Completado BIT,
	@UsuarioID INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE Tarea
	SET Titulo = @Titulo,
		Descripcion = @Descripcion,
		Completado = @Completado
	WHERE Id = @Id AND UsuarioId = @UsuarioID;

	SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO

CREATE PROCEDURE sp_EliminarTarea
	@Id INT,
	@UsuarioID INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM Tarea
	WHERE Id = @Id AND UsuarioId = @UsuarioID;

	SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO