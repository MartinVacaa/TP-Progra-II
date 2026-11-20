const btningresar= document.getElementById("btn-ingresar")

btningresar.addEventListener("click", async (e) => {
    e.preventDefault();

    const usuario = document.getElementById("usuario").value;
    const clave = document.getElementById("contrasena").value;

    console.log("Intentando conectar con:", "https://localhost:44387/api/Jwt");
    console.log("Enviando datos:", { UserName: usuario, Password: clave });

    try {
        const response = await fetch("https://localhost:44387/api/Jwt", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ 
                UserName: usuario, 
                Password: clave 
            })
        });

        console.log("Status de la respuesta:", response.status);

        if (!response.ok) {
            const textoError = await response.text();
            throw new Error(`Error del servidor (${response.status}): ${textoError}`);
        }

        const datos = await response.json();
        console.log("¡Login exitoso! Token recibido:", datos);

        localStorage.setItem("authToken", datos.token);
        window.location.href = "index.html";

    } catch (error) {
        console.error("Fallo completo en el fetch:", error.message);
        alert("Error: " + error.message);
    }
});