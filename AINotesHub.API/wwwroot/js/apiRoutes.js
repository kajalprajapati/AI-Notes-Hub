const ApiRoutes = {
    version: "v2",

    get base() {
        return `/api/${this.version}`;
    },

    get auth() {
        return `${this.base}/auth`;
    },

    get login() {
        return `${this.auth}/login`;
    },

    get register() {
        return `${this.auth}/register`;
    },

    get notes() {
        return `${this.base}/notes`;
    },

    get notesPaged() {
        return `${this.notes}/paged`;
    },

    get attachments() {
        return `${this.base}/attachments`;
    }
};

//const ApiRoutes = {
//    version: "v2",
//    base: "/api/v2",

//    auth: "/api/v2/auth",
//    login: "/api/v2/auth/login",
//    register: "/api/v2/auth/register",

//    notes: "/api/v2/notes",
//    notesPaged: "/api/v2/notes/paged",

//    attachments: "/api/v2/attachments"
//};