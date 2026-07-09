// Redirect to login if user is not authenticated
debugger;

const token = localStorage.getItem("token");

if (!token) {
    window.location.href = "login.html";
}

// Home page loadeds
console.log("Home Page Loaded");

// Optional: Welcome message
const username = localStorage.getItem("username");

if (username) {
    const welcome = document.getElementById("welcomeUser");

    if (welcome) {
        welcome.innerText = `Welcome, ${username}!`;
    }
}

// Go to Notes button
const notesBtn = document.getElementById("goToNotesBtn");

if (notesBtn) {
    notesBtn.addEventListener("click", () => {
        window.location.href = "notes.html";
    });
}