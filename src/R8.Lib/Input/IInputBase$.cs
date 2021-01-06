using System;
using System.Collections.Generic;
using System.Text;

namespace R8.Lib.Input
{
    public interface IInputBase<TId> : IInputBase
    {
        TId Id { get; set; }
    }
}