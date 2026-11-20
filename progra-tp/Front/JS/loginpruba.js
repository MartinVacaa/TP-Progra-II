const API_LOGIN = "http://localhost:5074/api/Jwt"
const CLAIM_ROL = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"

// Lee el payload del JWT (la parte del medio) sin librerías
function leerToken(token) {
    const base64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")
    const json = decodeURIComponent(atob(base64).split("")
        .map(c => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2)).join(""))
    return JSON.parse(json)
}

document.getElementById("form-login").addEventListener("submit", async (e) => {
    e.preventDefault()

    const usuario = document.getElementById("usuario").value.trim()
    const clave = document.getElementById("contrasena").value

    try {
        const response = await fetch(API_LOGIN, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ UserName: usuario, Password: clave })
        })

        if (!response.ok) {
            const textoError = await response.text()
            throw new Error(textoError || `Error del servidor (${response.status})`)
        }

        const datos = await response.json()

    if (datos.requiereCambioPassword === true) {
        sessionStorage.setItem("usuarioCambioPassword", usuario)
        window.location.href = "cambiar-password.html"
        return
    }

    const payload = leerToken(datos.token)

        const rol = payload[CLAIM_ROL] || payload.role

        localStorage.setItem("authToken", datos.token)
        localStorage.setItem("rol", rol)
        localStorage.setItem("usuario", payload.sub)

        // Redirección según el rol que viene en el token
        if (rol === "ALUMNO") {
            window.location.href = "portal-alumno.html"
        } else {
            window.location.href = "index.html"
        }

    } catch (error) {
        console.error("Fallo en el login:", error.message)
        alert("Error: " + error.message)
    }
})