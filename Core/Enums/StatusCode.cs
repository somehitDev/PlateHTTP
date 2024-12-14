using System.ComponentModel;


namespace PlateHTTP.Enums {
    public enum StatusCode {
        OK = 200,
        Redirect = 302,
        RedirectMethod = 303,
        TemporaryRedirect = 307,
        PermanentRedirect = 308,

        [Description("Bad Request")]
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,

        [Description("Not Found")]
        NotFound = 404,

        [Description("Method Not Allowed")]
        MethodNotAllowed = 405,

        [Description("Unsupported Media Type")]
        UnsupportedMediaType = 415,

        [Description("Internal Server Error")]
        InternalServerError = 500,
        
        [Description("Not Implemented")]
        NotImplemented = 501,

        [Description("Bad Gateway")]
        BadGateway = 502
    }
}
