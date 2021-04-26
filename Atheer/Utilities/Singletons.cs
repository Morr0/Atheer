using Atheer.Utilities.Markdown;
using Markdig;

namespace Atheer.Utilities
{
    public static class Singletons
    {
        public static MarkdownPipeline MarkdownPipeline { get; } = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions().UseBootstrap().Use<MarkdownExtension>().Build();
    }
}