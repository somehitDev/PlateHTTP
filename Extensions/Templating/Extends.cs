using System.Threading.Tasks;
using PlateHTTP.Interfaces.Core;
using PlateHTTP.Enums;



namespace PlateHTTP.Extensions.Templating {
    public static class TemplatingExtends {
        public static void UseTemplateLoader(this IWebApplication webApplication, AbstractTemplateLoader templateLoader) {
            webApplication.Extensions["Templating"] = templateLoader;
        }

        public static async Task RenderTemplate(this IResponse response, string templateName, object? context = null, StatusCode statusCode = StatusCode.OK) {
            if (!response.WebApplication.Extensions.ContainsKey("Templating")) {
                response.WebApplication.Logger?.Error("`RenderTemplate` only use after map template by `UseTemplateLoader`!");
                await response.SendText("404 Not Found!", StatusCode.NotFound);
            }
            else {
                var loader = (AbstractTemplateLoader)response.WebApplication.Extensions["Templating"];

                var template = Scriban.Template.Parse(
                    await loader.LoadAsync(loader.GetPath(templateName))
                );

                var ctx = new Scriban.TemplateContext();
                ctx.PushGlobal(Scriban.Runtime.ScriptObject.From(context));
                ctx.TemplateLoader = loader;

                await response.SendHTML(await template.RenderAsync(ctx), statusCode);
            }
        }
        public static async Task RenderHTML(this IResponse response, string html, object? context = null, StatusCode statusCode = StatusCode.OK) {
            if (!response.WebApplication.Extensions.ContainsKey("Templating")) {
                response.WebApplication.Logger?.Error("`RenderHTML` only use after map template by `UseTemplateLoader`!");
                await response.SendText("404 Not Found!", StatusCode.NotFound);
            }
            else {
                var loader = (AbstractTemplateLoader)response.WebApplication.Extensions["Templating"];

                var template = Scriban.Template.Parse(html);

                var ctx = new Scriban.TemplateContext();
                ctx.PushGlobal(Scriban.Runtime.ScriptObject.From(context));
                ctx.TemplateLoader = loader;

                await response.SendHTML(await template.RenderAsync(ctx), statusCode);
            }
        }
    }
}
