using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto
{
    /// <summary>
    /// All returning data to be passed around the cluster must inherit from PrestoResult
    /// </summary>
    [Serializable()]
    public abstract class PrestoResult
    {
    }
}
