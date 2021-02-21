using System;
using System.Linq;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Atheer.Utilities.Markdown
{
    /// <summary>
    /// Relying on the Markdig library for Markdown parsing.
    /// Here I am adding a class of bootstrap to differentiate background of <pre><code></code></pre> blocks from text
    /// </summary>
    public class CodeStyleExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.DocumentProcessed += AddStyle;
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        private void AddStyle(MarkdownDocument document)
        {
            foreach (var node in document.Descendants())
            {
                if (node is FencedCodeBlock)
                {
                    node.GetAttributes().AddClass("bg-light");
                }
            }
        }
    }
}