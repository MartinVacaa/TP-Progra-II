const API = "https://localhost:7024/api/Alumno"

// ---------------- 0. PROTECCIÓN DE LA PÁGINA ----------------
// Solo entra un usuario logueado con rol ALUMNO
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

// GET /api/Alumno/perfil -> { legajo, nombre, apellido, email, carrera, materiasCursando }
async function cargarPerfil() {
    const response = await fetch(`${API}/perfil`, { headers: headers() })

    if (response.status === 401 || response.status === 403) {
        localStorage.clear()
        window.location.href = "login.html"
        return null
    }
    if (!response.ok) throw new Error(await response.text() || "No se pudo cargar tu perfil")

    const perfil = await response.json()
    $("nombre-alumno").textContent = `${perfil.nombre} ${perfil.apellido}`
    $("legajo-alumno").textContent = perfil.legajo
    $("stat-materias").textContent = perfil.materiasCursando
    $("stat-carrera").textContent = perfil.carrera ?? "Sin carrera inscripta"
    return perfil
}

// GET /api/Alumno/{legajo} -> promedio (decimal). Si no tiene notas, la API responde error
async function cargarPromedio(legajo) {
    const response = await fetch(`${API}/${encodeURIComponent(legajo)}`, { headers: headers() })
    if (!response.ok) {
        $("stat-promedio").textContent = "-"
        return
    }
    const promedio = await response.json()
    $("stat-promedio").textContent = Number(promedio).toFixed(2)
}

// GET /api/Alumno/notas -> todas las notas; nos quedamos con las del legajo
// Cada nota: { legajo, nombre, apellido, carrera, materia, nota, tipo, fecha }
async function cargarNotas(legajo) {
    const response = await fetch(`${API}/notas`, { headers: headers() })
    if (!response.ok) throw new Error("No se pudieron cargar tus notas")

    const todas = await response.json()
    const misNotas = todas.filter(n => n.legajo === legajo)

    // Agrupamos por materia: { "Programación I": [notas...], ... }
    const porMateria = {}
    misNotas.forEach(n => {
        if (!porMateria[n.materia]) porMateria[n.materia] = []
        porMateria[n.materia].push(n)
    })

    renderMaterias(porMateria)
    renderSelectorNotas(porMateria)
}

// ---------------- 3. MIS MATERIAS ----------------
const ORDEN_TIPOS = ["PARCIAL 1", "PARCIAL 2", "RECUPERATORIO", "FINAL"]

function estadoMateria(notas) {
    const final = notas.find(n => n.tipo === "FINAL")
    if (final) {
        return final.nota >= 4
            ? { texto: "Aprobada", clase: "promocionada" }
            : { texto: "Final desaprobado", clase: "desaprobada" }
    }
    const tieneAmbosParciales = notas.some(n => n.tipo === "PARCIAL 1") && notas.some(n => n.tipo === "PARCIAL 2")
    if (tieneAmbosParciales && promedioDe(notas) >= 6) {
        return { texto: "Regular", clase: "regular" }
    }
    return { texto: "Cursando", clase: "cursando" }
}

function promedioDe(notas) {
    return notas.reduce((suma, n) => suma + Number(n.nota), 0) / notas.length
}

function renderMaterias(porMateria) {
    const tbody = $("tabla-materias")
    const materias = Object.keys(porMateria)

    if (materias.length === 0) {
        tbody.innerHTML = `<tr><td colspan="5" class="texto-vacio">Todavía no tenés calificaciones cargadas.</td></tr>`
        return
    }

    tbody.innerHTML = ""
    materias.forEach(materia => {
        const notas = porMateria[materia]
        const estado = estadoMateria(notas)
        tbody.innerHTML += `
            <tr>
                <td><strong>${materia}</strong></td>
                <td>${notas[0].carrera}</td>
                <td>${notas.length}</td>
                <td>${promedioDe(notas).toFixed(2)}</td>
                <td><span class="estado ${estado.clase}">${estado.texto}</span></td>
            </tr>`
    })
}

// ---------------- 4. MIS NOTAS ----------------
function condicionNota(n) {
    const minimo = n.tipo === "FINAL" ? 4 : 6
    return n.nota >= minimo ? "Aprobado" : "Desaprobado"
}

function renderSelectorNotas(porMateria) {
    const selector = $("selector-materias-notas")
    const materias = Object.keys(porMateria)

    if (materias.length === 0) {
        selector.innerHTML = ""
        $("titulo-materia-nota").textContent = ""
        $("tabla-detalle-notas").innerHTML = `<tr><td colspan="4" class="texto-vacio">Todavía no tenés calificaciones cargadas.</td></tr>`
        return
    }

    selector.innerHTML = materias
        .map((m, i) => `<button class="materia-pill ${i === 0 ? "activa" : ""}" data-materia="${m}">${m}</button>`)
        .join("")

    selector.querySelectorAll(".materia-pill").forEach(pill => {
        pill.addEventListener("click", () => {
            selector.querySelectorAll(".materia-pill").forEach(p => p.classList.remove("activa"))
            pill.classList.add("activa")
            renderDetalleNotas(pill.dataset.materia, porMateria[pill.dataset.materia])
        })
    })

    renderDetalleNotas(materias[0], porMateria[materias[0]])
}

function renderDetalleNotas(materia, notas) {
    $("titulo-materia-nota").textContent = materia
    const ordenadas = [...notas].sort((a, b) => ORDEN_TIPOS.indexOf(a.tipo) - ORDEN_TIPOS.indexOf(b.tipo))

    $("tabla-detalle-notas").innerHTML = ordenadas.map(n => {
        const condicion = condicionNota(n)
        const clase = condicion === "Aprobado" ? "nota-aprobada" : "nota-desaprobada"
        return `
            <tr>
                <td><strong>${n.tipo}</strong></td>
                <td>${formatearFecha(n.fecha)}</td>
                <td><strong class="${clase}">${Number(n.nota)}</strong></td>
                <td>${condicion}</td>
            </tr>`
    }).join("")
}

// ---------------- 5. FINALES (solo visual) ----------------
function toggleInscripcion(idFinal, boton) {
    const badgeEstado = $(`estado-${idFinal}`)
    const estaInscripto = boton.classList.contains("anular")

    if (!estaInscripto) {
        badgeEstado.textContent = "Inscripto"
        badgeEstado.className = "estado promocionada"
        boton.textContent = "Anular inscripción"
        boton.classList.replace("inscribir", "anular")
        mostrarToast("Te inscribiste a la mesa de final")
    } else {
        badgeEstado.textContent = "Disponible"
        badgeEstado.className = "estado disponible"
        boton.textContent = "Inscribirse"
        boton.classList.replace("anular", "inscribir")
        mostrarToast("Anulaste la inscripción a la mesa")
    }
    actualizarProximoFinal()
}

// "Próximo Final" = la mesa más cercana en la que el alumno está inscripto
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

// ---------------- 6. TRÁMITES (solo visual) ----------------
function solicitarTramite(boton, nombreTramite) {
    boton.textContent = `✓ ${nombreTramite} solicitado`
    boton.classList.replace("inscribir", "solicitado")
    boton.disabled = true
    mostrarToast(`${nombreTramite} solicitado. Se enviará a tu correo.`)
}

// ---------------- 7. CIERRE DE SESIÓN ----------------
$("btn-abrir-logout").addEventListener("click", (e) => {
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
        const perfil = await cargarPerfil()
        if (!perfil) return
        await Promise.all([cargarPromedio(perfil.legajo), cargarNotas(perfil.legajo)])
    } catch (error) {
        console.error(error)
        mostrarToast(error.message, "error")
    }
}

if (token && rol === "ALUMNO") iniciarPortal()