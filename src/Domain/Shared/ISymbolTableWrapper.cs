using System;
using System.Collections.Generic;

namespace Domain.Shared
{
    public interface ISymbolTableWrapper
    {
        Dictionary<string, IntPtr> SymbolTableRecordNames { get; set; }
        void ReadSymbolTableRecordNames();
    }
}