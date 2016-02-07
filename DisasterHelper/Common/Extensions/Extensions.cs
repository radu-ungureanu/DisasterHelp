using Common.Tables.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class Extensions
    {
        public static int AsInt(this UserType type)
        {
            return (int)type;
        }
    }
}
