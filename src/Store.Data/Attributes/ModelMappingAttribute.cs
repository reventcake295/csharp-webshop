using Store.Data.Enums;

namespace Store.Data.Attributes;

[AttributeUsage(AttributeTargets.Property)]
[Serializable]
public class ModelMappingAttribute : Attribute
{
    public required string Name;
    
    public string? Accessor;
    
    public PropertyType PropertyType = PropertyType.Simple;
}