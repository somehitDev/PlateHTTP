using System.Threading.Tasks;
using PlateHTTP.Core;
using PlateHTTP.Enums;



namespace PlateHTTP.Extensions.Templating {
    public static class TemplatingExtends {
        public static void UseTemplateLoader(this WebApplication webApplication, AbstractTemplateLoader templateLoader) {
            webApplication.TemplateLoader = templateLoader;
        }

        public static async Task RenderTemplate(this Response response, string templateName, object? context = null, StatusCode statusCode = StatusCode.OK) {
            if (response.WebApplication.TemplateLoader != null) {
                var loader = (AbstractTemplateLoader)response.WebApplication.TemplateLoader;

                var template = Scriban.Template.Parse(
                    await loader.LoadAsync(loader.GetPath(templateName))
                );

                var ctx = new Scriban.TemplateContext();
                ctx.PushGlobal(Scriban.Runtime.ScriptObject.From(context));
                ctx.TemplateLoader = loader;

                await response.SendHTML(await template.RenderAsync(ctx), statusCode);
            }
            else {
                response.WebApplication.Logger?.Error("`RenderTemplate` only use after map template by `UseTemplateLoader`!");
                await response.SendText("404 Not Found!");
            }
        }
        public static async Task RenderHTML(this Response response, string html, object? context = null, StatusCode statusCode = StatusCode.OK) {
            if (response.WebApplication.TemplateLoader != null) {
                var loader = (AbstractTemplateLoader)response.WebApplication.TemplateLoader;

                var template = Scriban.Template.Parse(html);

                var ctx = new Scriban.TemplateContext();
                ctx.PushGlobal(Scriban.Runtime.ScriptObject.From(context));
                ctx.TemplateLoader = loader;

                await response.SendHTML(await template.RenderAsync(ctx), statusCode);
            }
            else {
                response.WebApplication.Logger?.Error("`RenderHTML` only use after map template by `UseTemplateLoader`!");
                await response.SendText("404 Not Found!");
            }
        }
    }
}
