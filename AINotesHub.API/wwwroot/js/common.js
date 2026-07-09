async function loadHeader() {
    const response = await fetch("components/header.html");
    document.getElementById("header").innerHTML = await response.text();
}

async function loadFooter() {
    const response = await fetch("components/footer.html");
    document.getElementById("footer").innerHTML = await response.text();
}

loadHeader();
loadFooter();

function checkAuthentication() {

    const token = localStorage.getItem("token");

    if (!token) {
        window.location.href = "login.html";
    }
}

function logout() {

    localStorage.removeItem("token");
    localStorage.removeItem("username");

    window.location.href = "login.html";
}

document.getElementById("loadNotesBtn").addEventListener("click", loadNotes)


document.getElementById("JsNotestestBtn").addEventListener("click", function () {

    //window.location.href = "notes.html";

    //This creates HTML dynamically from a string.
    const container = document.getElementById("notesContainer");

    container.innerHTML = "<div class='note'>JavaScript Loaded Successfully</div>";

    //Feature 3: Create Elements Dynamically

    const note =
        document.createElement("div");

    note.className = "note";

    note.textContent =
        "Dynamic Note Created";

    container.appendChild(note);


    alert("Button Clicked!")
});

