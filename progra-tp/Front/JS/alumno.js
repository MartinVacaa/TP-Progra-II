const URL = "https://localhost:7024/api/Alumno"
const contenedor = document.getElementById("lista-alumnos")
const formAlumno = document.getElementById("formAlumno")
const modalAlumno = document.getElementById("modalAlumno")
const modalEliminar = document.getElementById("modalEliminar")

let alumnos = []                 // lista que devuelve GET /api/Alumno
let alumnoSeleccionado = null    // alumno marcado en la tabla
let modoEdicion = false

// ---------------- PETICIONES A LA API ----------------
function headers() {
    const token = localStorage.getItem("authToken")
    if (!token) window.location.href = "login.html"
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    }
}

// Lee el error que devuelve la API (texto plano o ProblemDetails de validación)
async function leerError(response) {
    if (response.status === 401 || response.status === 403) {
        return "No tienes autorización o tu sesión ha expirado"
    }
    const texto = await response.text()
    try {
        const json = JSON.parse(texto)
        if (json.errors) return Object.values(json.errors).flat().join(" | ")
        return json.title || texto
    } catch {
        return texto || `Error ${response.status}`
    }
}

// GET /api/Alumno  -> devuelve Alumno: { legajo, usuarioId, dni, nombre, apellido, email }
async function CargarAlumno() {
    try {
        const response = await fetch(URL, { headers: headers() })
        if (!response.ok) throw new Error(await leerError(response))
        alumnos = await response.json()
        ListadoAlumno()
    } catch (error) {
        mostrarToast(error.message, "error")
    }
}

// POST /api/Alumno  -> body AlumnoCreateDto
async function crearAlumno(dto) {
    const response = await fetch(URL, {
        method: "POST",
        headers: headers(),
        body: JSON.stringify(dto)
    })
    if (!response.ok) throw new Error(await leerError(response))
}

// PUT /api/Alumno/{legajo}  -> body AlumnoUpdateDto
async function editarAlumno(legajo, dto) {
    const response = await fetch(`${URL}/${encodeURIComponent(legajo)}`, {
        method: "PUT",
        headers: headers(),
        body: JSON.stringify(dto)
    })
    if (!response.ok) throw new Error(await leerError(response))
}

// DELETE /api/Alumno/{legajo}
async function eliminarAlumno(legajo) {
    const response = await fetch(`${URL}/${encodeURIComponent(legajo)}`, {
        method: "DELETE",
        headers: headers()
    })
    if (!response.ok) throw new Error(await leerError(response))
}

// ---------------- TABLA Y SELECCIÓN ----------------
function ListadoAlumno() {
    contenedor.innerHTML = ""
    alumnos.forEach(a => {
        contenedor.innerHTML += `
        <tr data-legajo="${a.legajo}">
            <td>${a.legajo}</td>
            <td>${a.nombre}</td>
            <td>${a.apellido}</td>
            <td>${a.dni}</td>
            <td>${a.email ?? ""}</td>
        </tr>`
    })
    seleccionar(null)
}

// Un solo listener para toda la tabla (el legajo es string, por eso se lee del data-legajo)
contenedor.addEventListener("click", (e) => {
    const fila = e.target.closest("tr")
    if (fila) seleccionar(fila.dataset.legajo)
})

function seleccionar(legajo) {
    alumnoSeleccionado = alumnos.find(a => a.legajo === legajo) || null

    contenedor.querySelectorAll("tr").forEach(tr =>
        tr.classList.toggle("fila-seleccionada", tr.dataset.legajo === legajo))

    document.getElementById("btnEditar").disabled = !alumnoSeleccionado
    document.getElementById("btnEliminar").disabled = !alumnoSeleccionado
    document.getElementById("seleccionInfo").textContent = alumnoSeleccionado
        ? `Seleccionado: ${alumnoSeleccionado.nombre} ${alumnoSeleccionado.apellido} (Legajo ${alumnoSeleccionado.legajo})`
        : "Seleccioná un alumno de la tabla para editar o eliminar"
}

// ---------------- MODAL REGISTRAR / EDITAR ----------------
function campo(id) { return document.getElementById(id) }

function prepararFormulario(editar) {
    modoEdicion = editar
    formAlumno.reset()
    formAlumno.classList.toggle("modo-edicion", editar)

    // En edición: legajo no se cambia y usuario/contraseña no se envían
    campo("legajo").readOnly = editar
    campo("nombreUsuario").required = !editar
    campo("password").required = !editar
}

function abrirModalRegistrar() {
    prepararFormulario(false)
    campo("tituloModalAlumno").textContent = "Registrar Alumno"
    campo("btnGuardar").textContent = "Registrar"
    modalAlumno.classList.add("activo")
    campo("legajo").focus()
}

function abrirModalEditar() {
    if (!alumnoSeleccionado) return
    prepararFormulario(true)

    const a = alumnoSeleccionado
    campo("legajo").value = a.legajo
    campo("nombre").value = a.nombre
    campo("apellido").value = a.apellido
    campo("dni").value = a.dni
    campo("email").value = a.email ?? ""

    campo("tituloModalAlumno").textContent = "Editar Alumno"
    campo("btnGuardar").textContent = "Guardar cambios"
    modalAlumno.classList.add("activo")
    campo("nombre").focus()
}

function cerrarModalAlumno() {
    modalAlumno.classList.remove("activo")
}

formAlumno.addEventListener("submit", async (e) => {
    e.preventDefault()

    // Campos comunes (AlumnoUpdateDto)
    const dto = {
        nombre: campo("nombre").value.trim(),
        apellido: campo("apellido").value.trim(),
        dni: campo("dni").value.trim(),
        email: campo("email").value.trim()
    }

    campo("btnGuardar").disabled = true
    try {
        if (modoEdicion) {
            await editarAlumno(alumnoSeleccionado.legajo, dto)
            mostrarToast("Alumno actualizado correctamente")
        } else {
            // AlumnoCreateDto = comunes + legajo, nombreUsuario y password
            dto.legajo = campo("legajo").value.trim()
            dto.nombreUsuario = campo("nombreUsuario").value.trim()
            dto.password = campo("password").value
            await crearAlumno(dto)
            mostrarToast("Alumno registrado correctamente")
        }
        cerrarModalAlumno()
        await CargarAlumno()
    } catch (error) {
        mostrarToast(error.message, "error")
    } finally {
        campo("btnGuardar").disabled = false
    }
})

// ---------------- MODAL ELIMINAR ----------------
function abrirModalEliminar() {
    if (!alumnoSeleccionado) return
    const a = alumnoSeleccionado
    campo("textoEliminar").innerHTML =
        `Vas a eliminar a <strong>${a.nombre} ${a.apellido}</strong> (Legajo ${a.legajo}).<br>
         También se borran sus notas, inscripciones y su usuario. Esta acción no se puede deshacer.`
    modalEliminar.classList.add("activo")
}

function cerrarModalEliminar() {
    modalEliminar.classList.remove("activo")
}

async function confirmarEliminar() {
    if (!alumnoSeleccionado) return
    campo("btnConfirmarEliminar").disabled = true
    try {
        await eliminarAlumno(alumnoSeleccionado.legajo)
        cerrarModalEliminar()
        mostrarToast("Alumno eliminado correctamente")
        await CargarAlumno()
    } catch (error) {
        mostrarToast(error.message, "error")
    } finally {
        campo("btnConfirmarEliminar").disabled = false
    }
}

// ---------------- AVISOS Y CIERRE DE MODALES ----------------
function mostrarToast(mensaje, tipo = "ok") {
    const toast = campo("toast")
    toast.textContent = mensaje
    toast.className = `toast visible ${tipo}`
    clearTimeout(toast.timer)
    toast.timer = setTimeout(() => toast.className = "toast", 3500)
}

document.addEventListener("keydown", (e) => {
    if (e.key === "Escape") { cerrarModalAlumno(); cerrarModalEliminar() }
})
document.querySelectorAll(".modal-overlay").forEach(overlay =>
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.classList.remove("activo")
    }))

function cerrarSesion() {
    localStorage.removeItem("authToken")
    window.location.href = "login.html"
}

function navegarA() {
    window.location.href = "index.html"
}

CargarAlumno()