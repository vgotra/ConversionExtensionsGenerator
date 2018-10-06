using System.Collections.Generic;

namespace ConversionExtensionsGenerator
{
    public class GenerationResult : BaseResult
    {
        public List<ExtensionFile> ExtensionFiles { get; set; } = new List<ExtensionFile>();
    }
}