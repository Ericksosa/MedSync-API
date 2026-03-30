# 🩺 MedSync – Plataforma de Telemedicina para Gestión de Citas Médicas

---

## 📚 UNIVERSODAD APEC

**Asignatura:**  
Desarrollo de Software con Tecnología Open Source  

**Código / Sección:**  
202610 – ISO815-3173  

**Facilitador:**  
Omar Reyes  

---

## 👨‍💻 Sustentado por

| Nombre | Matrícula |
|------|------|
| Jerlyn Rodriguez | A00113235 |
| Erick Sosa | A00115078 |
| Eorys Pina | A00115249 |

---

# 📌 Descripción del Proyecto

**MedSync** es una aplicación web orientada a la gestión digital de citas médicas mediante una arquitectura basada en **RESTful APIs**.  

El sistema permite a los pacientes reservar, modificar o cancelar citas médicas de forma rápida y eficiente, mientras que los especialistas pueden gestionar su agenda y disponibilidad en tiempo real.

Esta solución busca modernizar el proceso tradicional de asignación de citas médicas, eliminando procesos manuales y mejorando la experiencia de los pacientes.

---

# ⚠️ Problemática

En muchos centros médicos de la República Dominicana, la gestión de citas se realiza mediante procesos manuales o llamadas telefónicas. Esto genera diversos problemas:

- ❌ Errores en la asignación de horarios  
- ❌ Largas esperas para los pacientes  
- ❌ Falta de control en la agenda médica  
- ❌ Baja satisfacción del usuario  
- ❌ Dificultad para gestionar disponibilidad de especialistas  

**MedSync** propone una solución tecnológica que automatiza estos procesos y centraliza la información médica en una plataforma digital.

---

# 🎯 Objetivos del Sistema

El proyecto tiene como objetivo principal desarrollar una plataforma que permita:

- 📅 Reservar citas médicas en línea  
- 🔄 Reprogramar citas existentes  
- ❌ Cancelar citas médicas  
- 👨‍⚕️ Gestionar disponibilidad de doctores  
- 📊 Mantener control organizado de agendas médicas  

---

# 🏗 Arquitectura del Sistema

El sistema está basado en una **arquitectura RESTful**, donde el backend expone endpoints que permiten a aplicaciones web o móviles interactuar con el sistema.

La arquitectura permite:

- Separación entre frontend y backend  
- Escalabilidad del sistema  
- Integración con otras aplicaciones  
- Manejo eficiente de datos mediante APIs  

---

# ⚙️ Tecnologías Utilizadas

| Tecnología | Uso |
|------|------|
| ASP.NET Core | Desarrollo del API |
| .NET | Framework de desarrollo |
| Entity Framework | Acceso y manejo de datos |
| Swagger | Documentación y pruebas del API |
| GitHub | Control de versiones |
| SQL Database | Almacenamiento de datos |

---

# 🔗 Ejemplo de Endpoints REST

| Método | Endpoint | Descripción |
|------|------|------|
| GET | `/api/appointments` | Obtener todas las citas |
| GET | `/api/appointments/{id}` | Obtener una cita específica |
| POST | `/api/appointments` | Crear una nueva cita |
| PUT | `/api/appointments/{id}` | Actualizar una cita |
| DELETE | `/api/appointments/{id}` | Eliminar una cita |


