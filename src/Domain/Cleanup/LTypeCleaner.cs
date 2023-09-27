using Domain.Shared;

namespace Domain.Cleanup
{
    public class LTypeCleaner
    {
        public IDocumentWrapper DwgDocumentWrapper { get; }
        public IDatabaseWrapper CurDbWrapper { get; }

        public LTypeCleaner(IDocumentWrapper dwgDocumentWrapper)
        {
            DwgDocumentWrapper = dwgDocumentWrapper;
            CurDbWrapper=dwgDocumentWrapper.CurDbWrapper;
        }

        public bool Cleanup()
        {
            
            return true;
        }

    }
}