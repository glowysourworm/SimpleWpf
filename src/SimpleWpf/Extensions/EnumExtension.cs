﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWpf.Extensions
{
    public static class EnumExtension
    {
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

            if (member == null)
                throw new Exception("No Member Defined for Enum Type");

            var attributes = member.GetCustomAttributes(typeof(T), true);

            return attributes.Any() ? (T)attributes.First() : default(T);
        }

        public static bool HasAttribute<T>(this Enum value) where T : Attribute
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

            if (member == null)
                throw new Exception("No Member Defined for Enum Type");

            var attributes = member.GetCustomAttributes(typeof(T), true);

            return attributes.Any();
        }

        public static bool Has<T>(this Enum value, T flag) where T : Enum
        {
            return value.HasFlag(flag);
        }

        public static bool IsFlagEnum<T>(this Enum value) where T : Enum
        {
            return value.HasAttribute<FlagsAttribute>();
        }

        public static IEnumerable<string> GetFlaggedNames<T>(this Enum value) where T : Enum
        {
            var valueInt = Convert.ToUInt32(value);
            var valueNames = new List<string>();

            foreach (var enumValue in Enum.GetValues(typeof(T)).Cast<int>())
            {
                // Has Flag
                if ((valueInt & enumValue) > 0 || 
                    (valueInt == 0 && enumValue == 0))
                    valueNames.Add(Enum.GetName(typeof(T), enumValue));
            }

            return valueNames;
        }

        public static IEnumerable<T> Enumerate<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
