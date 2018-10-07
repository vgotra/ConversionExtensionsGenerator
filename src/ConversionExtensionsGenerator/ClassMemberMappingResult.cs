using System.Collections.Generic;

namespace ConversionExtensionsGenerator
{

    public class ClassMemberMappingResult : BaseResult
    {
        public List<ClassMemberMappingInfo> Mappings { get; set; } = new List<ClassMemberMappingInfo>();
    }
}