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
        listanot.innerHTML+=`
        <tr>
        <td>${alumno.legajo}</td>
        <td>${alumno.nombre}</td>
        <td>${alumno.apellido}</td>
        <td>${alumno.carrera}</td>
        <td>${alumno.materia}</td>
        <td>${alumno.nota}</td>
        <td>${alumno.tipo}</td>
        </tr>
        
        `// Como 'inscripciones', 'materias' y 'notas' son listas (arrays), 
        // recorremos el JSON anidado de forma segura:

   

    })
}




CargarNotas();