// =====================================================
// auth.js - Sesión del ENCARGADO (index, notas, estadísticas)
// =====================================================

// Protección: sin token vuelve al login; si es ALUMNO va a su portal
if (!localStorage.getItem("authToken")) {
    window.location.href = "login.html"
} else if (localStorage.getItem("rol") === "ALUMNO") {
    window.location.href = "portal-alumno.html"
}

// Botón "Cerrar Sesión" del menú lateral
function cerrarSesion() {
    localStorage.removeItem("authToken")
    localStorage.removeItem("rol")
    localStorage.removeItem("usuario")
    window.location.href = "login.html"
}

// Botón "Alumnos" del menú lateral
function navegarA() {
    window.location.href = "index.html"
}