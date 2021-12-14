namespace CloudCMS
{
    public interface IPlatformDocument : IDocument, IReferenceable, ITypedID
    {
        IPlatform Platform { get; }
    }
}