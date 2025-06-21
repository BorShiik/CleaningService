using RazorLight;

namespace CleanDeal.Services.Email
{
    public class TemplateRenderer
    {
        private readonly RazorLightEngine _eng;

        public TemplateRenderer(IWebHostEnvironment env)
        {
            var dir = Path.Combine(env.ContentRootPath, "EmailTemplates");
            _eng = new RazorLightEngineBuilder().UseFileSystemProject(dir).UseMemoryCachingProvider().Build();
        }

        public Task<string> RenderAsync<T>(string view, T model) =>
            _eng.CompileRenderAsync($"{view}.cshtml", model);
    }
}
