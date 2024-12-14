using System;
using PlateHTTP.Core;
using PlateHTTP.Extensions.Templating;


var server = new WebApplication();
server.EnableLogging("debug");
server.UseTemplateLoader(new ResourceTemplateLoader("wwwroot.templates"));

server.Get("/", async ( request, response ) => {
    await response.RenderTemplate("index.html");
});

var helloApp = new WebApplication();
helloApp.Get("/", async ( request, response ) => {
    await response.SendHTML($"Hello, {request.QueryParams["name"]}!");
});
server.Mount("/hello", helloApp);

server.Start();
