const registrados= document.getElementById("totalAlumnos");
const porcentaje= document.getElementById("porcentajeDesaprobados");
const contenedor= document.getElementById("tablaPromedios");
const URL="https://localhost:7024/api/Encargado"   

function traertoken()
{
    const token= localStorage.getItem("authToken")
if(!token){
    console.warn("No hay token autenticado redirigiendo al login...")
    window.location.href='login.html'
    return;
}
console.log("Token recuperado exitosamente",token)
return token
}



async function traerestadisticas(){
const token= traertoken();

const response = await fetch(URL,{
    method:'GET',
    headers:{"Content-Type": "application/json",
    "Authorization":`Bearer ${token}`} 
})
if(response.status===401||response.status===403){
    throw new Error("No tienes autorización o tu sesión ha expirado")
}

if(!response.ok){
    throw new Error("No se ha podido cargar la lista de alumnos"); 
}
    
const promedios= await response.json();
traerpromedios(promedios)

}



function traerpromedios(promedios){
    contenedor.innerHTML=''
    promedios.forEach(element => {
        contenedor.innerHTML+=`
        <tr>
            <td>${element.legajo}</td>
            <td>${element.nombre}</td>
            <td>${element.apellido}</td>
            <td>${element.promedio}</td>  
        </tr>
        
        
        `
    });
}

traerestadisticas();

//////////porcentaje//////
const URLPORCENTAJE= "https://localhost:7024/api/Encargado/Desaprobados"

async function desaporbadosporc(){
    const token=traertoken()
    const response= await fetch(URLPORCENTAJE,{
        method:'GET',
        headers:{
            "Content-Type": "application/json",
            "Authorization":`Bearer ${token}`
        }

    })
    if(response.status===401||response.status===403){
    throw new Error("No tienes autorización o tu sesión ha expirado")
    }

if(!response.ok){
    throw new Error("No se ha podido cargar la lista de alumnos"); 
    }
    const alumnosdesaprobados= await response.json()
    porcentaje.innerHTML=`%${alumnosdesaprobados.porcentaje}`
}

desaporbadosporc();
////cantidad alumnos/////
const URLCANTIDAD= "https://localhost:7024/api/Encargado/Registrados"

async function traeralumnosregitrados() {
    const token= traertoken();
    const response= await fetch(URLCANTIDAD,{
        method:'GET',
        headers:{
            "Content-Type": "application/json",
            "Authorization":`Bearer ${token}`
        }
    })
    if(response.status==401||response.status===403){
        throw new Error("No tienes autorización o tu sesión ha expirado")
    }
    if(!response.ok){
    throw new Error("No se ha podido cargar la lista de alumnos"); 
    }
    const cant=await response.json();
    registrados.innerHTML=`${cant}`
}

traeralumnosregitrados();




    
  