namespace Domain.Shared
{
    public interface IDocumentWrapper
    {
        IDatabaseWrapper CurDbWrapper { get; }
    }
}