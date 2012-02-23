using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrestoLib
{
    /// <summary>
    /// All returning data to be passed around the cluster must implement IPrestoResult
    /// </summary>
    [Serializable]
    interface IPrestoResult
    {
    }
}
