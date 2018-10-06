using System.Collections.Generic;

namespace ConversionExtensionsGenerator
{
    public class ClassMappingResult : BaseResult
    {
        public List<ClassMappingInfo> Mappings { get; set; } = new List<ClassMappingInfo>();
    }
}