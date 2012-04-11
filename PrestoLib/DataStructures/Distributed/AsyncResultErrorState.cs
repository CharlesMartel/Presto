using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed
{
    /// <summary>
    /// Describes the error of the get opertion on the distributed data structure.
    /// </summary>
    public enum AsyncResultErrorState
    {
        /// <summary>
        /// The get operation experienced no errors.
        /// </summary>
        NONE,
        /// <summary>
        /// The passed in Key was not found.
        /// </summary>
        KEY_NOT_FOUND
    }
}
