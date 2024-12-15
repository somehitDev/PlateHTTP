using System;
using System.Threading;
using System.Threading.Tasks;
using PlateHTTP.Core;
using PlateHTTP.Extensions.Templating;


var server = new WebApplication();
server.EnableLogging("debug");
server.UseTemplateLoader(new ResourceTemplateLoader("wwwroot.templates"));

server.Get("/", async ( request, response ) => {
    await response.RenderTemplate("index.html");
});
server.Post("/api", async ( request, response ) => {
    await response.SendJSON(request.GetJSON());
});

var helloApp = new WebApplication();
helloApp.Get("/", async ( request, response ) => {
    await response.SendHTML($"Hello, {request.QueryParams["name"]}!");
});
server.Mount("/hello", helloApp);

server.Start();
