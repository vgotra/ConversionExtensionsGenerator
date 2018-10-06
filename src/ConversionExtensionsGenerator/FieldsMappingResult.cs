using System.Collections.Generic;

namespace ConversionExtensionsGenerator
{

    public class FieldsMappingResult : BaseResult
    {
        public List<ClassMemberMappingInfo> Mappings { get; set; } = new List<ClassMemberMappingInfo>();
    }
}