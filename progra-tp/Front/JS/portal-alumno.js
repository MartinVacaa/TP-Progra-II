// Portal del alumno: todas las consultas usan los endpoints protegidos de MiAlumno.
const API = "http://localhost:5074/api/MiAlumno"

// ---------------- 0. PROTECCIÓN DE LA PÁGINA ----------------
const token = localStorage.getItem("authToken")
const rol = localStorage.getItem("rol")

if (!token) {
    window.location.href = "login.html"
} else if (rol !== "ALUMNO") {
    window.location.href = "index.html"
}

function headers() {
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    }
}

function $(id) { return document.getElementById(id) }

function escaparHTML(valor) {
    return String(valor ?? "").replace(/[&<>"']/g, caracter => ({
        "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
    })[caracter])
}

function mostrarToast(mensaje, tipo = "ok") {
    const toast = $("toast")
    toast.textContent = mensaje
    toast.className = `toast visible ${tipo}`
    clearTimeout(toast.timer)
    toast.timer = setTimeout(() => toast.className = "toast", 3500)
}

function formatearFecha(fechaIso) {
    if (!fechaIso) return "-"
    const [anio, mes, dia] = fechaIso.substring(0, 10).split("-")
    return `${dia}/${mes}/${anio}`
}

async function obtenerDatos(ruta) {
    const response = await fetch(`${API}/${ruta}`, { headers: headers() })

    if (response.status === 401) {
        localStorage.removeItem("authToken")
        localStorage.removeItem("rol")
        localStorage.removeItem("usuario")
        window.location.href = "login.html"
        throw new Error("Tu sesión venció. Iniciá sesión nuevamente.")
    }

    if (response.status === 403) {
        throw new Error(`No tenés permiso para consultar ${ruta} (403).`)
    }

    if (!response.ok) {
        throw new Error(`No se pudo cargar ${ruta} (error ${response.status}).`)
    }

    return response.json()
}

// ---------------- 1. NAVEGACIÓN ENTRE SECCIONES ----------------
const botones = document.querySelectorAll(".nav-btn")
const secciones = document.querySelectorAll(".seccion-vista")

botones.forEach(btn => {
    btn.addEventListener("click", () => {
        botones.forEach(b => b.classList.remove("activo"))
        secciones.forEach(s => s.classList.remove("activa"))

        btn.classList.add("activo")
        $(`sec-${btn.dataset.seccion}`).classList.add("activa")
        $("titulo-seccion").textContent = btn.textContent.replace(/^[^\wÁÉÍÓÚáéíóúÑñ]+/, "").trim()
    })
})

// ---------------- 2. DATOS DESDE LA API ----------------
// Estos tres endpoints ya filtran por el alumno autenticado.
async function cargarPerfil() {
    const perfil = await obtenerDatos("perfil")
    $("nombre-alumno").textContent = `${perfil.nombre} ${perfil.apellido}`
    $("legajo-alumno").textContent = perfil.legajo
    return perfil
}

async function cargarMaterias() {
    // Ejemplo: [{ id: 1, nombre: "Programación I", anio: 1, carrera: "..." }]
    return obtenerDatos("materias")
}

async function cargarNotas() {
    // Ejemplo: [{ materia: "Programación I", tipo: "PARCIAL 1", nota: 8, fecha: "..." }]
    return obtenerDatos("notas")
}

function actualizarResumen(materias, notas) {
    $("stat-materias").textContent = materias.length

    // El endpoint /perfil no incluye carrera; la obtenemos de /materias.
    const carreras = [...new Set(materias.map(m => m.carrera).filter(Boolean))]
    $("stat-carrera").textContent = carreras.length
        ? carreras.join(" / ")
        : "Carrera no disponible"

    // Promedio simple de todas las calificaciones cargadas (no por materia).
    const valores = notas.map(n => Number(n.nota)).filter(Number.isFinite)
    const promedio = valores.length
        ? valores.reduce((suma, valor) => suma + valor, 0) / valores.length
        : null

    $("stat-promedio").textContent = promedio === null ? "-" : promedio.toFixed(2)
}

// ---------------- 3. MIS MATERIAS ----------------
const ORDEN_TIPOS = ["PARCIAL 1", "PARCIAL 2", "RECUPERATORIO", "FINAL"]

function promedioDe(notas) {
    if (notas.length === 0) return null
    return notas.reduce((suma, n) => suma + Number(n.nota), 0) / notas.length
}

function estadoMateria(notas) {
    if (notas.length === 0) {
        return { texto: "Sin calificar", clase: "cursando" }
    }

    const final = notas.find(n => n.tipo === "FINAL")
    if (final) {
        return Number(final.nota) >= 4
            ? { texto: "Aprobada", clase: "promocionada" }
            : { texto: "Final desaprobado", clase: "desaprobada" }
    }

    const tieneAmbosParciales = notas.some(n => n.tipo === "PARCIAL 1") &&
        notas.some(n => n.tipo === "PARCIAL 2")

    if (tieneAmbosParciales && promedioDe(notas) >= 6) {
        return { texto: "Regular", clase: "regular" }
    }

    return { texto: "Cursando", clase: "cursando" }
}

function agruparNotas(notas) {
    const porMateria = {}
    notas.forEach(n => {
        if (!porMateria[n.materia]) porMateria[n.materia] = []
        porMateria[n.materia].push(n)
    })
    return porMateria
}

function renderMaterias(materias, porMateria) {
    const tbody = $("tabla-materias")

    if (materias.length === 0) {
        tbody.innerHTML = `<tr><td colspan="5" class="texto-vacio">No se encontraron materias para tu inscripción.</td></tr>`
        return
    }

    tbody.innerHTML = materias.map(materia => {
        const notas = porMateria[materia.nombre] || []
        const estado = estadoMateria(notas)
        const promedio = promedioDe(notas)

        return `
            <tr>
                <td><strong>${escaparHTML(materia.nombre)}</strong></td>
                <td>${escaparHTML(materia.carrera || "-")}</td>
                <td>${notas.length}</td>
                <td>${promedio === null ? "-" : promedio.toFixed(2)}</td>
                <td><span class="estado ${estado.clase}">${estado.texto}</span></td>
            </tr>`
    }).join("")
}

// ---------------- 4. MIS NOTAS ----------------
function condicionNota(n) {
    const minimo = n.tipo === "FINAL" ? 4 : 6
    return Number(n.nota) >= minimo ? "Aprobado" : "Desaprobado"
}

function renderSelectorNotas(materias, porMateria) {
    const selector = $("selector-materias-notas")

    // Se muestran también materias que todavía no tienen calificaciones.
    const nombres = [...new Set([
        ...materias.map(m => m.nombre),
        ...Object.keys(porMateria)
    ])]

    if (nombres.length === 0) {
        selector.innerHTML = ""
        $("titulo-materia-nota").textContent = ""
        $("tabla-detalle-notas").innerHTML = `<tr><td colspan="4" class="texto-vacio">No hay materias para mostrar.</td></tr>`
        return
    }

    selector.innerHTML = nombres
        .map((nombre, i) => `<button class="materia-pill ${i === 0 ? "activa" : ""}" data-materia="${escaparHTML(nombre)}">${escaparHTML(nombre)}</button>`)
        .join("")

    selector.querySelectorAll(".materia-pill").forEach(pill => {
        pill.addEventListener("click", () => {
            selector.querySelectorAll(".materia-pill").forEach(p => p.classList.remove("activa"))
            pill.classList.add("activa")
            const nombre = pill.dataset.materia
            renderDetalleNotas(nombre, porMateria[nombre] || [])
        })
    })

    renderDetalleNotas(nombres[0], porMateria[nombres[0]] || [])
}

function renderDetalleNotas(materia, notas) {
    $("titulo-materia-nota").textContent = materia

    if (notas.length === 0) {
        $("tabla-detalle-notas").innerHTML = `<tr><td colspan="4" class="texto-vacio">Esta materia todavía no tiene notas cargadas.</td></tr>`
        return
    }

    const ordenadas = [...notas].sort((a, b) =>
        ORDEN_TIPOS.indexOf(a.tipo) - ORDEN_TIPOS.indexOf(b.tipo))

    $("tabla-detalle-notas").innerHTML = ordenadas.map(n => {
        const condicion = condicionNota(n)
        const clase = condicion === "Aprobado" ? "nota-aprobada" : "nota-desaprobada"
        return `
            <tr>
                <td><strong>${escaparHTML(n.tipo)}</strong></td>
                <td>${formatearFecha(n.fecha)}</td>
                <td><strong class="${clase}">${escaparHTML(n.nota)}</strong></td>
                <td>${condicion}</td>
            </tr>`
    }).join("")
}

// ---------------- 5. FINALES (solo visual: no se guardan en la base de datos) ----------------
function toggleInscripcion(idFinal, boton) {
    const badgeEstado = $(`estado-${idFinal}`)
    const estaInscripto = boton.classList.contains("anular")

    if (!estaInscripto) {
        badgeEstado.textContent = "Inscripto"
        badgeEstado.className = "estado promocionada"
        boton.textContent = "Anular inscripción"
        boton.classList.replace("inscribir", "anular")
        mostrarToast("Inscripción simulada: no se guardó en la base de datos")
    } else {
        badgeEstado.textContent = "Disponible"
        badgeEstado.className = "estado disponible"
        boton.textContent = "Inscribirse"
        boton.classList.replace("anular", "inscribir")
        mostrarToast("Anulación simulada: no se guardó en la base de datos")
    }
    actualizarProximoFinal()
}

function actualizarProximoFinal() {
    const meses = ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"]
    const inscriptas = [...document.querySelectorAll("#tabla-finales tr")]
        .filter(tr => tr.querySelector(".btn-accion.anular"))
        .map(tr => {
            const [dia, mes, anio] = tr.cells[1].textContent.trim().substring(0, 10).split("/")
            return { materia: tr.cells[0].textContent.trim(), fecha: new Date(anio, mes - 1, dia) }
        })
        .sort((a, b) => a.fecha - b.fecha)

    if (inscriptas.length === 0) {
        $("stat-proximo-final").textContent = "-"
        $("stat-proximo-final-materia").textContent = "No estás inscripto a ninguna mesa"
        return
    }

    const proximo = inscriptas[0]
    $("stat-proximo-final").textContent = `${proximo.fecha.getDate()} ${meses[proximo.fecha.getMonth()]}`
    $("stat-proximo-final-materia").textContent = proximo.materia
}

// ---------------- 6. TRÁMITES (solo visual: no envían correos) ----------------
function solicitarTramite(boton, nombreTramite) {
    boton.textContent = `✓ ${nombreTramite} solicitado (demo)`
    boton.classList.replace("inscribir", "solicitado")
    boton.disabled = true
    mostrarToast("Solicitud simulada: no se envió ningún correo")
}

// ---------------- 7. CIERRE DE SESIÓN ----------------
$("btn-abrir-logout").addEventListener("click", e => {
    e.preventDefault()
    $("modal-logout").classList.add("activo")
})
$("btn-cancelar-logout").addEventListener("click", () => $("modal-logout").classList.remove("activo"))
$("btn-confirmar-logout").addEventListener("click", () => {
    localStorage.removeItem("authToken")
    localStorage.removeItem("rol")
    localStorage.removeItem("usuario")
    window.location.href = "login.html"
})

// ---------------- INICIO ----------------
async function iniciarPortal() {
    actualizarProximoFinal()

    try {
        await cargarPerfil()
        const [materias, notas] = await Promise.all([cargarMaterias(), cargarNotas()])
        const porMateria = agruparNotas(notas)

        actualizarResumen(materias, notas)
        renderMaterias(materias, porMateria)
        renderSelectorNotas(materias, porMateria)

        // Antes decía "Materias con calificaciones", pero ahora incluye todas.
        const tituloMaterias = document.querySelector("#sec-materias .card-tabla h3")
        if (tituloMaterias) tituloMaterias.textContent = "Materias de mi carrera"
    } catch (error) {
        console.error(error)
        mostrarToast(error.message, "error")
    }
}

if (token && rol === "ALUMNO") iniciarPortal()
