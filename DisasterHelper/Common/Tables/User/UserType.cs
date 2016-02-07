using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Tables.User
{
    public enum UserType
    {
        Volunteer,
        Medic,
        Manager
    }

    public static class UserTypeExtensions
    {
        public static List<NameType> GetUserTypes()
        {
            return new List<NameType>
            {
                new NameType { Type = UserType.Volunteer, Value = "Volunteer" },
                new NameType { Type = UserType.Medic, Value = "Medic" },
                new NameType { Type = UserType.Manager, Value = "Hospital Manager" }
            };
        }
    }

    public class NameType
    {
        public UserType Type { get; set; }
        public string Value { get; set; }
    }
}
