using System;
using System.Collections.Generic;

namespace Domain.Shared
{
    public interface IDatabaseWrapper
    {
        IntPtr GetSymbolTableId(string symbolTableName);
        Dictionary<string, IntPtr> GetSymbolTableRecordNames(IntPtr symbolTableId);
    }
}