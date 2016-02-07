using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tables.Disaster
{
    public enum PacientStatusType
    {
        Stable,
        Critical,
        Dead
    }

    public static class PacientStatusTypeExtensions
    {
        public static List<PacientStatusType> GetPacientStatusTypes()
        {
            return new List<PacientStatusType>
            {
                PacientStatusType.Stable,
                PacientStatusType.Critical,
                PacientStatusType.Dead
            };
        }
    }
}
