using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Tables.Disaster
{
    public enum BloodType
    {
        O1,
        A2,
        B3,
        AB4
    }

    public static class BloodTypeExtensions
    {
        public static List<BloodType> GetBloodTypes()
        {
            return new List<BloodType>
            {
                BloodType.O1,
                BloodType.A2,
                BloodType.B3,
                BloodType.AB4
            };
        }
    }
}
