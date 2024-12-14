using System;
using System.Threading.Tasks;
using Scriban;
using Scriban.Runtime;
using Scriban.Parsing;


namespace PlateHTTP.Extensions.Templating {
    public abstract class AbstractTemplateLoader: ITemplateLoader {
        // non-implemented methods
        string ITemplateLoader.Load(TemplateContext context, SourceSpan callerSpan, string templatePath) {
            throw new NotImplementedException();
        }
        ValueTask<string> ITemplateLoader.LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath) {
            throw new NotImplementedException();
        }

        public abstract string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName);
        public abstract string GetPath(string templateName);

        public abstract Task<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath);
        public abstract Task<string> LoadAsync(string templatePath);
    }
}
