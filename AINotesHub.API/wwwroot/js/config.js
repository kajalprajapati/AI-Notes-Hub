const AppConfig = {

    BaseApiUrl: "/api",

    ApiVersion: "v1",

    DefaultPageSize: 10,

    Controllers: {
        Auth: "auth",
        Notes: "notes"
    },
    Urls: {

        Login() {
            return `${AppConfig.BaseApiUrl}/${AppConfig.ApiVersion}/${AppConfig.Controllers.Auth}/login`;
        },

        GetAllNotes() {
            return `${AppConfig.BaseApiUrl}/${AppConfig.ApiVersion}/${AppConfig.Controllers.Notes}`;
        },

        GetPagedNotes(page, pageSize = AppConfig.DefaultPageSize) {
            return `${AppConfig.BaseApiUrl}/${AppConfig.ApiVersion}/${AppConfig.Controllers.Notes}/paged?page=${page}&pageSize=${pageSize}`;
        }
    }
};