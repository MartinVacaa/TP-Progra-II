const listanot= document.getElementById("tablaNotas")
const URL= "http://localhost:5074/api/Alumno/notas"

async function CargarNotas(){
const response = await fetch(URL)
if(!response.ok){
    throw Error("Ha ocurrido un error al traer las notas ")
    return    
}
    const notas = await response.json()
    console.log(notas)
    mostrarNotas(notas)

}

function mostrarNotas(alumnos){
     listanot.innerHTML=''
alumnos.forEach(alumno => {
        // Como 'inscripciones', 'materias' y 'notas' son listas (arrays), 
        // recorremos el JSON anidado de forma segura:
        alumno.inscripciones.forEach(inscripcion => {
            if (inscripcion.carrera && inscripcion.carrera.materias) {
                inscripcion.carrera.materias.forEach(materia => {
                    if (materia.notas) {
                        materia.notas.forEach(nota => {
                            // Acumulamos las filas con +=
                            listanot.innerHTML += `
                                <tr>
                                    <td>${alumno.nombre} ${alumno.apellido}</td>
                                    <td>${inscripcion.carrera.nombre}</td>
                                    <td>${materia.nombre}</td>
                                    <td>${nota.valor}</td>
                                    <td>${nota.tipo}</td>
                                </tr>
                            `;
                        });
                    }
                });
            }
        });
    });
}




CargarNotas();