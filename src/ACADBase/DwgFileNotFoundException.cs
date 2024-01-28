using System.IO;

namespace ACADBase
{
    public class DwgFileNotFoundException:FileNotFoundException
    {
        public DwgFileNotFoundException(string message) : base(message)
        {
        }

        public static DwgFileNotFoundException _(string dwgPath)
        {
            return new DwgFileNotFoundException($"Drawing file [{dwgPath}] not found.");
        }
        
    }
}