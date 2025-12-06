namespace HwoodiwissReverseProxy;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
public sealed class ContentTypeMappingAttribute : Attribute
{
    public ContentTypeMappingAttribute(string contentType, string pattern)
    {
        ContentType = contentType;
        Pattern = pattern;
    }

    public string ContentType { get; }
    public string Pattern { get; }
}
