using System;
using System.Collections.Generic;

namespace BIG
{
    public interface ISystem : IDisposable
    {
        Tuple<ulong, ulong> Query { get; set; }
        void Update(in int frame, in float time, in HashSet<int> entities);
    }
}