namespace ConversionExtensionsGenerator;

public class ClassMemberMappingInfo
{
    public Type SourceClassType { get; set; }
    public Type SourceClassMemberType { get; set; }
    public string SourceClassMemberName { get; set; }

    public Type TargetClassType { get; set; }
    public Type TargetClassMemberType { get; set; }
    public string TargetClassMemberName { get; set; }
}