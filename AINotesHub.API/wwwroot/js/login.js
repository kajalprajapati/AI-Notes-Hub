// Check if user is already logged in
//debugger;
const token = localStorage.getItem("token");

if (token) {
    window.location.href = "notes.html";
}

document.getElementById("loginBtn").addEventListener("click", login)

async function login() {

    try {
        //debugger;
        const usernameOrEmail =
            document.getElementById("usernameOrEmail").value;

        const password =
            document.getElementById("password").value;

        const response =
            await fetch("/api/auth/login", {

                method: "POST",

                headers: {
                    "Content-Type": "application/json"
                },

                body: JSON.stringify({
                    usernameOrEmail,
                    password
                })
            });

        if (!response.ok) {

            throw new Error("Login Failed");
        }

        const data = await response.json();

        console.log("Login Response:", data);

        localStorage.setItem("token", data.token);

        window.location.href = "index.html";

    }
    catch (error) {

        console.error(error);

        document.getElementById("message").textContent =
            error.message;
    }
}
