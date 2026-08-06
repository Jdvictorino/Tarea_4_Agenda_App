# Tarea 4 - Agenda App 📋

Una aplicación de agenda simple desarrollada con **ASP.NET Core 8.0** (MVC) que permite gestionar contactos con autenticación básica, con pruebas automatizadas usando **Selenium + NUnit**.

---

## 🎯 Características

- ✅ Autenticación de usuarios con sesión
- ✅ Gestionar contactos (Crear, Leer, Actualizar, Eliminar)
- ✅ Almacenamiento en memoria (sin base de datos)
- ✅ Interfaz sencilla con validación de datos
- ✅ Protección de rutas (requiere login)
- ✅ Suite de pruebas automatizadas con Selenium WebDriver (15 tests)
- ✅ Reportes HTML automáticos con ExtentReports

---

## 📁 Estructura del Proyecto

```
Tarea_4_Agenda_App/
├── Tarea_4_Agenda_App.slnx   ← Solución Visual Studio
├── APP/                       ← App web ASP.NET Core MVC
│   ├── Tarea_4_Agenda_App.csproj
│   ├── Program.cs
│   ├── Controllers/
│   ├── Models/, Services/, Views/, wwwroot/
│   └── Properties/
└── TEST/                      ← Pruebas Selenium + NUnit
    ├── Tarea_4_Pruebas_Automatizadas.csproj
    ├── Drivers/, Pages/, Tests/, Utils/
    ├── Capturas/
    └── Reportes/
```

---

## 📋 Requisitos

- **.NET 8.0** o superior
- **Google Chrome** (para las pruebas Selenium)
- **Visual Studio 2022+** o **VS Code**
- Acceso a terminal PowerShell o Command Prompt

---

## 🚀 Instalación y Ejecución

### 1. Navegar a la raíz del proyecto

```bash
\Tarea_4_Agenda_App
```

### 2. Compilar la solución completa (app + pruebas)

```bash
dotnet build Tarea_4_Agenda_App.slnx
```

### 3. Ejecutar la aplicación

```bash
dotnet run --project APP\Tarea_4_Agenda_App.csproj
```

La aplicación se iniciará en: **http://localhost:5000**

> Nota: Al acceder a `http://localhost:5000`, te redirigirá automáticamente al login.

---

## 🔐 Credenciales de Acceso

Use las siguientes credenciales para iniciar sesión:

| Campo          | Valor      |
| -------------- | ---------- |
| **Usuario**    | `admin`    |
| **Contraseña** | `admin123` |

---

## 📱 Uso de la Aplicación

### 1. **Pantalla de Login**

- Accede a `http://localhost:5000`
- Ingresa el usuario y contraseña
- Presiona "Iniciar Sesión"

### 2. **Pantalla de Agenda**

Una vez autenticado, podrás:

#### ➕ **Crear un contacto**

1. Completa los campos:
   - **Nombre**: Nombre del contacto
   - **Teléfono**: Número de teléfono
2. Presiona "Guardar"

#### 📖 **Ver contactos**

- La lista se muestra automáticamente en la página principal

#### ✏️ **Actualizar un contacto**

1. Encuentra el contacto en la lista
2. Modifica los datos
3. Presiona "Guardar cambios"

#### ❌ **Eliminar un contacto**

1. Localiza el contacto
2. Presiona "Eliminar"
3. Confirma la acción

### 3. **Cerrar Sesión**

- Presiona el botón "Cerrar Sesión" en la esquina superior

---

## 🧪 Ejecutar Pruebas Automatizadas

> ⚠️ **La aplicación debe estar corriendo** en `http://localhost:5000` antes de ejecutar las pruebas.

### Paso 1: Iniciar la app (en una terminal)

```bash
dotnet run --project APP\Tarea_4_Agenda_App.csproj
```

### Paso 2: Ejecutar las pruebas (en otra terminal)

```bash
dotnet test TEST\Tarea_4_Pruebas_Automatizadas.csproj
dotnet test TEST\Tarea_4_Pruebas_Automatizadas.csproj --verbosity detailed
dotnet test Tarea_4_Agenda_App.slnx --list-tests
```

### Opción alternativa: Visual Studio

1. Abre `Tarea_4_Agenda_App.slnx` en **Visual Studio**
2. Ve a **Test** → **Test Explorer**
3. Presiona **"Ejecutar todos los Tests"**

### Reportes de pruebas

Tras la ejecución, se genera un reporte HTML en:

```
TEST\Reportes\ReporteEjecucion.html
```

Las capturas de pantalla de cada prueba se guardan en:

```
TEST\Capturas\
```

## 👨‍💻 Autor

-Juan Victorino
-Matricula 20220900

---
