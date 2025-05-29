# 📁 Gestor de Tareas API REST (.NET 8)

Este es un proyecto de una API REST que te permite **registrar usuarios, iniciar sesión con autenticación JWT** y gestionar tareas personales (crear, editar, eliminar y listar).
Fue desarrollado en el lenguaje C# usando **.NET 8**, con SQL Server como base de datos.

---

## ✅ Características principales

- Registro de usuarios con contraseñas encriptadas usando BCrypt
- Inicio de sesión con token JWT
- CRUD de tareas por usuario
- Filtros por estado de la tarea (completada o no)
- Paginación (ej: 10 tareas por página)
- Documentación con Swagger (interfaz gráfica para probar la API)
- Prueba unitaria para el servicio de JWT

---

## 🚀 Tecnologías utilizadas

- C# y .NET 8 (Web API)
- SQL Server (con stored procedures)
- JWT (para autenticación segura)
- Swagger / OpenAPI
- BCrypt.Net (para encriptar contraseñas)
- MSTest (para pruebas unitarias)

---

## ⚙️ Requisitos previos para correrlo

- Tener instalado [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- Tener instalado **SQL Server** (puede ser SQL Server Express)
- Tener instalado Visual Studio 2022 (recomendado)

---

## 📂 Estructura del proyecto

```
GestorTareaApiRest/
├── Controllers/          # Controladores de la API
├── DTOs/                # Objetos para entrada y salida de datos
├── Models/              # Modelos de datos
├── Services/            # Lógica de JWT
├── Program.cs           # Configuración principal de la app
├── appsettings.json     # Cadena de conexión y datos JWT
└── README.md            # Este archivo explicativo
```

---

## 📥 Instalación y configuración

### 1. Clona el proyecto desde GitHub:
```bash
git clone https://github.com/tu-usuario/GestorTareaApiRest.git
```

### 2. Restaura la base de datos

Ve a la carpeta `Database/` y ejecuta el archivo `GestorTareasDB.sql` en tu SQL Server. Este script creará:

- La base de datos `GestorTareasDB`
- Las tablas `Usuario` y `Tarea`
- Todos los procedimientos almacenados necesarios

### 3. Configura la cadena de conexión

Abre `appsettings.json` y coloca tu conexión local:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GestorTareasDB;User Id=usuario;Password=password;TrustServerCertificate=True;"
},
```

### 4. Ejecuta el proyecto

Abre Visual Studio y presiona `Ctrl + F5` para correrlo. Luego ve a:
```
https://localhost:{puerto}/swagger
```
Desde ahí puedes probar todos los endpoints sin necesidad de usar Postman.

---

## 🌐 Endpoints disponibles

| Método | Ruta              | Qué hace                          |
|--------|-------------------|----------------------------------|
| POST   | /login            | Iniciar sesión y recibir token   |
| POST   | /usuarios         | Registrar nuevo usuario          |
| POST   | /usuarios/{id}/tareas | Listar tareas del usuario logueado, con opción de filtrar por estado y paginación. |
| GET    | /Tarea            | Listar todas las tareas, con opción de filtrar por estado y paginación. |
| POST   | /Tarea            | Crear nueva tarea asociada al usuario autenticado. |
| GET    | /Tarea/{id}       | Obtener tarea por ID asociada al usuario autenticado. |
| PUT    | /Tarea/{id}       | Editar tarea existente asociada al usuario autenticado. |
| DELETE | /Tarea/{id}       | Eliminar tarea asociada al usuario autenticado. |

> Para acceder a las rutas de tareas necesitas un token JWT en el encabezado `Authorization` con formato `Bearer TU_TOKEN`.

---

## 🎯 Pruebas unitarias

En el proyecto `GestorTareaApiRest.Tests` se incluyó una prueba unitaria para el servicio `JwtService`, que verifica que el token se genere correctamente.

```csharp
[TestMethod]
public void GenerarToken_DeberiaRetornarTokenValido()
{
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "Jwt:Key", "clave_super_segura_1234567890" },
            { "Jwt:Issuer", "GestorTareasAPI" },
            { "Jwt:Audience", "GestorTareasUsuario" }
        }).Build();

    var jwtService = new JwtService(config);
    var token = jwtService.GenerarToken(1, "usuario");

    Assert.IsNotNull(token);
    Assert.IsTrue(token.Length > 0);
}
```

---

## 📄 Autor

Este proyecto fue desarrollado por **Percy Abad** para aplicar conocimientos en **.NET y desarrollo de APIs**.

> Si tienes dudas, sugerencias o deseas colaborar, no dudes en escribirme. 🚀
