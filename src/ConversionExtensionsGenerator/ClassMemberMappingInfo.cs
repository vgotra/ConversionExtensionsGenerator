using System;

namespace ConversionExtensionsGenerator
{

    public class ClassMemberMappingInfo
    {
        public string SourceClassFullName { get; set; }
        public Type SourceClassMemberType { get; set; }
        public string SourceClassMemberName { get; set; }

        public string TargetClassFullName { get; set; }
        public Type TargetClassMemberType { get; set; }
        public string TargetClassMemberName { get; set; }
    }
}