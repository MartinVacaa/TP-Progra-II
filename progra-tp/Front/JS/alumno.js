const contenedor= document.getElementById("lista-alumnos")
const URL= "https://localhost:7024/api/Alumno"
async function CargarAlumno(){
const token =localStorage.getItem("authToken")

if(!token){
    console.warn("No hay token autenticado redirigiendo al login...")
    window.location.href='login.html'
    return;
}
console.log("Token recuperado exitosamente",token)


const response = await fetch(URL,{
    method:'GET',
    headers:{"Content-Type": "application/json",
    "Authorization":`Bearer ${token}` 
}
})
if(response.status===401||response.status===403){
    throw new Error("No tienes autorización o tu sesión ha expirado")
}

if(!response.ok){
    throw new Error("No se ha podido cargar la lista de alumnos"); 
}
const alumnos = await response.json();
console.log(alumnos)
 ListadoAlumno(alumnos)

}

function ListadoAlumno(alumnos){
    contenedor.innerHTML= ``
    alumnos.forEach(element => {
        contenedor.innerHTML +=`
        <tr>
            <td>${element.legajo}</td>
            <td>${element.nombre}</td>
            <td>${element.apellido}</td>
            <td>${element.dni}</td>
            <td>${element.email}</td>
             
        </tr>
        `

    });
}

CargarAlumno();