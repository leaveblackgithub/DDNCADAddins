using System;
using System.Collections.Generic;

namespace Domain.Shared
{
    public interface IDatabaseWrapper
    {
        IntPtr GetSymbolTableIdIntPtr(string symbolTableName);
        Dictionary<string, IntPtr> GetSymbolTableRecordNames(IntPtr symbolTableId);
    }
}