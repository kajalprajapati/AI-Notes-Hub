const token = localStorage.getItem("token");


//if (!token) {
//    window.location.href = "login.html";
//}
console.log("AI Notes Hub Loaded");
console.log("AINotesHub.js loaded");

//Feature 4: Connect to API with Fetch
async function loadNotes() {
    const response = await fetch("/api/Notes");
    const notes = await response.json();
    console.log("notes");
}


async function loadNotes(page) {

    //debugger;

    try {
        console.log("loadNotes called");
        console.log("Page:", page);
        console.log(localStorage);
        const token = localStorage.getItem("token");
        console.log("Token:", token);

        //const container = document.getElementById("notesContainer");

        //`/api/notes/paged?page=${page}&pageSize=${pageSize}`,

        const response =
            await fetch(
                `/api/v2/notes/paged?page=${page}&pageSize=${pageSize}`,
                {
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                });

        console.log("Status:", response.status);

        const data = await response.json();

        console.log(data);

        console.table(data);

        // Render latest notes

        renderNotes(data.notes);

        document.getElementById("pageNumber")
            .textContent = page;

        // Calculate display count
        const start = ((page - 1) * pageSize) + 1;

        const end = Math.min(
            page * pageSize,
            data.totalCount
        );

        document.getElementById("notesInfo").textContent =
            `Showing ${start}-${end} of ${data.totalCount} notes`;

    }
    catch (error) {

        console.error("Load Notes Error:", error);
    }

}

function renderNotes(notes) {
    try {

        const container = document.getElementById("notesContainer");

        container.innerHTML = "";

        if (!Array.isArray(notes)) {
            throw new Error("Notes data is not an array.");
        }


        notes.forEach(note => {

            const card = document.createElement("div");

            card.className = "note-card";

            card.innerHTML = `
            <h3>${note.title}</h3>
            <p>${note.content}</p>
            <small>Created: ${note.createdDate}</small>
        `;

            container.appendChild(card);
        });
    }

    catch (error) {

        console.error("Render Notes Error:", error);

        const container = document.getElementById("notesContainer");

        container.innerHTML =
            "<p>Unable to load notes.</p>";
    }

}
let currentPage = 1;
const pageSize = 10

document.getElementById("nextBtn")
    .addEventListener("click", () => {

        currentPage++;

        loadNotes(currentPage);
    });

document.getElementById("prevBtn")
    .addEventListener("click", () => {

        if (currentPage > 1) {

            currentPage--;

            loadNotes(currentPage);
        }
    });

;

//const startRecord = (currentPage - 1) * pageSize + 1;

//const endRecord = Math.min(
//    currentPage * pageSize,
//    notes.totalCount
//);


document.addEventListener("DOMContentLoaded", () => {
    loadNotes(currentPage);
}

)

//loadNotes(currentPage);
