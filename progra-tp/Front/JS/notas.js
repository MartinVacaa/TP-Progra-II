// ==========================================
// 1. VARIABLES GENERALES
// ==========================================
const API = "https://localhost:44387/api";

const tablaNotas = document.getElementById("tablaNotas");
const buscador = document.getElementById("buscador");

const modalNota = document.getElementById("modalNota");
const tituloModal = document.getElementById("tituloModal");
const selAlumno = document.getElementById("selAlumno");
const selMateria = document.getElementById("selMateria");
const selTipo = document.getElementById("selTipo");
const inputValor = document.getElementById("inputValor");
const inputFecha = document.getElementById("inputFecha");

const modalEliminar = document.getElementById("modalEliminar");
const textoEliminar = document.getElementById("textoEliminar");

let notas = [];
let alumnos = [];
let materias = [];
let idEditando = 0;    // 0 = estoy creando una nota nueva, otro número = estoy editando esa nota
let idAEliminar = 0;


// ==========================================
// 2. FUNCIONES AUXILIARES
// ==========================================

// Datos que se mandan en cada llamada a la API (incluye el token)
function cabeceras() {
    const cab = { "Content-Type": "application/json" };
    const token = localStorage.getItem("authToken");
    if (token) {
        cab["Authorization"] = "Bearer " + token;
    }
    return cab;
}

function traer(ruta) {
    return fetch(API + ruta, { headers: cabeceras() });
}

// Si la respuesta no es correcta, corta el proceso; si es correcta, devuelve la lista
function leerLista(respuesta) {
    if (!respuesta.ok) {
        throw new Error("Error al traer los datos");
    }
    return respuesta.json();
}

// Cartelito de aviso abajo a la derecha
function mostrarMensaje(texto, esError) {
    const cuadro = document.getElementById("mensaje");
    cuadro.textContent = texto;
    if (esError) {
        cuadro.className = "mensaje error";
    } else {
        cuadro.className = "mensaje";
    }
    cuadro.style.display = "block";
    setTimeout(function () {
        cuadro.style.display = "none";
    }, 3000);
}

// Si el servidor devuelve un texto largo de validación, mostramos algo simple
function textoDeError(texto) {
    if (texto === "" || texto.indexOf("{") === 0) {
        return "Revisá los datos ingresados.";
    }
    return texto;
}

function nombreAlumno(legajo) {
    for (let i = 0; i < alumnos.length; i++) {
        if (alumnos[i].legajo === legajo) {
            return alumnos[i].nombre + " " + alumnos[i].apellido;
        }
    }
    return legajo;
}

function nombreMateria(id) {
    for (let i = 0; i < materias.length; i++) {
        if (materias[i].id === id) {
            return materias[i].nombre;
        }
    }
    return "Materia " + id;
}

function buscarNota(id) {
    for (let i = 0; i < notas.length; i++) {
        if (notas[i].id === id) {
            return notas[i];
        }
    }
    return null;
}

// Pasa "2026-05-10T00:00:00" a "10/05/2026"
function formatearFecha(fecha) {
    const partes = fecha.substring(0, 10).split("-");
    return partes[2] + "/" + partes[1] + "/" + partes[0];
}

// Devuelve la fecha de hoy con formato "2026-11-19" (el que usa el campo de fecha)
function fechaDeHoy() {
    const hoy = new Date();
    let mes = hoy.getMonth() + 1;
    let dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes;
    }
    if (dia < 10) {
        dia = "0" + dia;
    }
    return hoy.getFullYear() + "-" + mes + "-" + dia;
}


// ==========================================
// 3. CARGAR DATOS DE LA API
// ==========================================
function cargarDatos() {
    traer("/Alumno")
        .then(leerLista)
        .then(function (datos) {
            alumnos = datos;
            return traer("/Materia");
        })
        .then(leerLista)
        .then(function (datos) {
            materias = datos;
            return traer("/Nota");
        })
        .then(leerLista)
        .then(function (datos) {
            notas = datos;
            llenarSelects();
            mostrarNotas();
        })
        .catch(function () {
            mostrarMensaje("No se pudieron cargar los datos. Revisá que hayas iniciado sesión como encargado.", true);
        });
}

// Llena los desplegables de alumno y materia del formulario
function llenarSelects() {
    selAlumno.innerHTML = "";
    for (let i = 0; i < alumnos.length; i++) {
        const a = alumnos[i];
        selAlumno.innerHTML += '<option value="' + a.legajo + '">' + a.legajo + " - " + a.nombre + " " + a.apellido + "</option>";
    }

    selMateria.innerHTML = "";
    for (let j = 0; j < materias.length; j++) {
        const m = materias[j];
        selMateria.innerHTML += '<option value="' + m.id + '">' + m.nombre + "</option>";
    }
}


// ==========================================
// 4. MOSTRAR LA TABLA
// ==========================================
function mostrarNotas() {
    const filtro = buscador.value.toLowerCase();
    tablaNotas.innerHTML = "";
    let hayFilas = false;

    for (let i = 0; i < notas.length; i++) {
        const nota = notas[i];
        const alumno = nombreAlumno(nota.legajo);
        const materia = nombreMateria(nota.materiaId);
        const texto = (nota.legajo + " " + alumno + " " + materia + " " + nota.tipo).toLowerCase();

        if (texto.indexOf(filtro) !== -1) {
            hayFilas = true;

            // Se considera aprobada desde 6 (cambialo si tu consigna dice otra cosa)
            let clase = "nota-desaprobada";
            if (nota.valor >= 6) {
                clase = "nota-aprobada";
            }

            tablaNotas.innerHTML += `
                <tr>
                    <td>${nota.legajo}</td>
                    <td>${alumno}</td>
                    <td>${materia}</td>
                    <td>${nota.tipo}</td>
                    <td><span class="${clase}">${nota.valor}</span></td>
                    <td>${formatearFecha(nota.fecha)}</td>
                    <td>
                        <button class="btn-notas editar" onclick="abrirEditar(${nota.id})">Editar</button>
                        <button class="btn-notas eliminar chico" onclick="pedirEliminar(${nota.id})">Eliminar</button>
                    </td>
                </tr>
            `;
        }
    }

    if (hayFilas === false) {
        tablaNotas.innerHTML = '<tr><td colspan="7" style="text-align:center;">No hay notas para mostrar.</td></tr>';
    }
}

buscador.addEventListener("input", mostrarNotas);


// ==========================================
// 5. CREAR Y EDITAR
// ==========================================
function abrirNueva() {
    idEditando = 0;
    tituloModal.textContent = "Nueva nota";
    selAlumno.disabled = false;
    selMateria.disabled = false;
    selTipo.value = "PARCIAL 1";
    inputValor.value = "";
    inputFecha.value = fechaDeHoy();
    modalNota.classList.add("abierto");
}

function abrirNuevaCon(legajo, materiaId, tipo) {
    idEditando = 0;
    tituloModal.textContent = "Nueva nota";
    selAlumno.value = legajo;
    selMateria.value = String(materiaId);
    selAlumno.disabled = true;      // el alumno y la materia vienen de la fila
    selMateria.disabled = true;
    selTipo.value = tipo;
    inputValor.value = "";
    inputFecha.value = fechaDeHoy();
    modalNota.classList.add("abierto");
}



function abrirEditar(id) {
    const nota = buscarNota(id);
    if (nota === null) {
        return;
    }
    idEditando = id;
    tituloModal.textContent = "Editar nota";
    selAlumno.value = nota.legajo;
    selMateria.value = String(nota.materiaId);
    selAlumno.disabled = true;      // el alumno y la materia no se cambian al editar
    selMateria.disabled = true;
    selTipo.value = nota.tipo;
    inputValor.value = nota.valor;
    inputFecha.value = nota.fecha.substring(0, 10);
    modalNota.classList.add("abierto");
}

function cerrarModalNota() {
    modalNota.classList.remove("abierto");
}

function guardarNota() {
    const valor = parseFloat(inputValor.value);
    const fecha = inputFecha.value;

    if (selAlumno.value === "" || selMateria.value === "" || isNaN(valor) || fecha === "") {
        mostrarMensaje("Completá todos los campos", true);
        return;
    }
    if (valor < 1 || valor > 10) {
        mostrarMensaje("La nota debe estar entre 1 y 10", true);
        return;
    }

    const nota = {
        legajo: selAlumno.value,
        materiaId: parseInt(selMateria.value),
        tipo: selTipo.value,
        valor: valor,
        fecha: fecha
    };

    // Evitamos repetir la misma instancia para el mismo alumno y materia
    for (let i = 0; i < notas.length; i++) {
        const otra = notas[i];
        if (otra.id !== idEditando && otra.legajo === nota.legajo && otra.materiaId === nota.materiaId && otra.tipo === nota.tipo) {
            mostrarMensaje("Ese alumno ya tiene una nota de " + textoTipo(nota.tipo) + " en esa materia. Editala en lugar de crear otra.", true);
            return;
        }
    }

    let ruta = "/Nota";
    let metodo = "POST";
    if (idEditando !== 0) {
        ruta = "/Nota/" + idEditando;
        metodo = "PUT";
    }

    fetch(API + ruta, {
        method: metodo,
        headers: cabeceras(),
        body: JSON.stringify(nota)
    })
        .then(function (respuesta) {
            if (respuesta.ok) {
                cerrarModalNota();
                mostrarMensaje("Nota guardada", false);
                cargarDatos();
            } else {
                respuesta.text().then(function (texto) {
                    mostrarMensaje(textoDeError(texto), true);
                });
            }
        })
        .catch(function () {
            mostrarMensaje("No se pudo conectar con el servidor", true);
        });
}


// ==========================================
// 6. ELIMINAR
// ==========================================
function pedirEliminar(id) {
    const nota = buscarNota(id);
    if (nota === null) {
        return;
    }
    idAEliminar = id;
    textoEliminar.innerHTML = "¿Seguro que querés eliminar la nota <strong>" + nota.tipo + "</strong> de <strong>" + nombreAlumno(nota.legajo) + "</strong> en <strong>" + nombreMateria(nota.materiaId) + "</strong>? No se puede deshacer.";
    modalEliminar.classList.add("abierto");
}

function cerrarModalEliminar() {
    modalEliminar.classList.remove("abierto");
}

function eliminarNota() {
    fetch(API + "/Nota/" + idAEliminar, {
        method: "DELETE",
        headers: cabeceras()
    })
        .then(function (respuesta) {
            cerrarModalEliminar();
            if (respuesta.ok) {
                mostrarMensaje("Nota eliminada", false);
                cargarDatos();
            } else {
                respuesta.text().then(function (texto) {
                    mostrarMensaje(textoDeError(texto), true);
                });
            }
        })
        .catch(function () {
            mostrarMensaje("No se pudo conectar con el servidor", true);
        });
}


// ==========================================
// 7. BOTONES Y ARRANQUE
// ==========================================
document.getElementById("btnNuevaNota").addEventListener("click", abrirNueva);
document.getElementById("btnCancelar").addEventListener("click", cerrarModalNota);
document.getElementById("btnGuardar").addEventListener("click", guardarNota);
document.getElementById("btnCancelarEliminar").addEventListener("click", cerrarModalEliminar);
document.getElementById("btnConfirmarEliminar").addEventListener("click", eliminarNota);

cargarDatos();








const TIPOS = ["PARCIAL 1", "PARCIAL 2", "RECUPERATORIO", "FINAL"];
let abiertos = [];   // legajos de los alumnos que están desplegados

function estaAbierto(legajo) {
    for (let i = 0; i < abiertos.length; i++) {
        if (abiertos[i] === legajo) {
            return true;
        }
    }
    return false;
}

// Abre o cierra el desplegable de un alumno
function alternarAlumno(legajo) {
    if (estaAbierto(legajo)) {
        const nuevos = [];
        for (let i = 0; i < abiertos.length; i++) {
            if (abiertos[i] !== legajo) {
                nuevos.push(abiertos[i]);
            }
        }
        abiertos = nuevos;
    } else {
        abiertos.push(legajo);
    }
    mostrarNotas();
}

// Busca la nota de un alumno en una materia y un tipo (si no existe, devuelve null)
function buscarNotaDe(legajo, materiaId, tipo) {
    for (let i = 0; i < notas.length; i++) {
        if (notas[i].legajo === legajo && notas[i].materiaId === materiaId && notas[i].tipo === tipo) {
            return notas[i];
        }
    }
    return null;
}



// Arma la tabla que se ve al desplegar un alumno
function armarDetalle(legajo, idsMaterias) {
    const tiposAMostrar = ["PARCIAL 1", "PARCIAL 2", "RECUPERATORIO", "FINAL"];
    let filas = "";

    for (let m = 0; m < idsMaterias.length; m++) {
        const materiaId = idsMaterias[m];
        let primera = true;

        for (let t = 0; t < tiposAMostrar.length; t++) {
            const tipo = tiposAMostrar[t];
            const nota = buscarNotaDe(legajo, materiaId, tipo);

            let claseFila = "";
            let textoMateria = "";
            if (primera === true) {
                claseFila = "inicio-materia";
                textoMateria = "<strong>" + nombreMateria(materiaId) + "</strong>";
            }

            if (nota === null) {
                // No tiene nota en esta instancia: guion y botón verde para cargarla
                filas += `
                    <tr class="${claseFila}">
                        <td>${textoMateria}</td>
                        <td>${textoTipo(tipo)}</td>
                        <td>-</td>
                        <td>-</td>
                        <td class="celda-acciones">
                            <button class="btn-notas cargar" onclick="abrirNuevaCon('${legajo}', ${materiaId}, '${tipo}')">Cargar</button>
                        </td>
                    </tr>
                `;
            } else {
                filas += `
                    <tr class="${claseFila}">
                        <td>${textoMateria}</td>
                        <td>${textoTipo(tipo)}</td>
                        <td>${nota.valor}</td>
                        <td>${formatearFecha(nota.fecha)}</td>
                        <td class="celda-acciones">
                            <button class="btn-notas editar" onclick="abrirEditar(${nota.id})">Editar</button>
                            <button class="btn-notas eliminar chico" onclick="pedirEliminar(${nota.id})">Eliminar</button>
                        </td>
                    </tr>
                `;
            }
            primera = false;
        }
    }

    return `
        <table class="tabla-detalle">
            <thead>
                <tr>
                    <th>Materia</th>
                    <th>Tipo</th>
                    <th>Nota</th>
                    <th>Fecha</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>${filas}</tbody>
        </table>
    `;
}

function mostrarNotas() {
    const filtro = buscador.value.toLowerCase();
    tablaNotas.innerHTML = "";
    let hayFilas = false;

    for (let a = 0; a < alumnos.length; a++) {
        const alumno = alumnos[a];
        const nombre = alumno.nombre + " " + alumno.apellido;

        // Se juntan las materias del alumno y se cuentan sus notas
        const idsMaterias = [];
        let cantidad = 0;
        let textoMaterias = "";

        for (let i = 0; i < notas.length; i++) {
            if (notas[i].legajo === alumno.legajo) {
                cantidad++;

                let yaEsta = false;
                for (let k = 0; k < idsMaterias.length; k++) {
                    if (idsMaterias[k] === notas[i].materiaId) {
                        yaEsta = true;
                    }
                }
                if (yaEsta === false) {
                    idsMaterias.push(notas[i].materiaId);
                    textoMaterias += " " + nombreMateria(notas[i].materiaId);
                }
            }
        }

        const texto = (alumno.legajo + " " + nombre + textoMaterias).toLowerCase();

        // Solo se muestran los alumnos que tienen notas y coinciden con la búsqueda
        if (cantidad > 0 && texto.indexOf(filtro) !== -1) {
            hayFilas = true;

            let claseFila = "fila-alumno";
            if (estaAbierto(alumno.legajo)) {
                claseFila += " abierta";
            }

            tablaNotas.innerHTML += `
                <tr class="${claseFila}" onclick="alternarAlumno('${alumno.legajo}')">
                    <td><strong>${alumno.legajo}</strong></td>
                    <td>${nombre}</td>
                    <td>${cantidad}</td>
                    <td class="flecha"><span>▸</span></td>
                </tr>
            `;

            if (estaAbierto(alumno.legajo)) {
                tablaNotas.innerHTML += `
                    <tr class="fila-detalle">
                        <td colspan="4">${armarDetalle(alumno.legajo, idsMaterias)}</td>
                    </tr>
                `;
            }
        }
    }

    if (hayFilas === false) {
        tablaNotas.innerHTML = '<tr><td colspan="4" style="text-align:center;">No hay notas para mostrar.</td></tr>';
    }
}

buscador.addEventListener("input", mostrarNotas);



function textoTipo(tipo) {
    if (tipo === "PARCIAL 1") {
        return "Parcial 1";
    }
    if (tipo === "PARCIAL 2") {
        return "Parcial 2";
    }
    if (tipo === "RECUPERATORIO") {
        return "Recuperatorio";
    }
    if (tipo === "FINAL") {
        return "Final";
    }
    return tipo;
}


