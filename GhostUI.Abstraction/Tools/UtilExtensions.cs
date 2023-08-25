using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostUI.Abstraction.Tools
{
    public static class UtilExtensions
    {
        public static bool HasError(this string error)
        {
            return !string.IsNullOrWhiteSpace(error);
        }
    }
}
