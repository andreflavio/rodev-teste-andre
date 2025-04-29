using System;
using System.ComponentModel;
using System.Reflection;

namespace RO.DevTest.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        FieldInfo field = value.GetType().GetField(value.ToString());
        if (field == null)
            return value.ToString();

        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute == null ? value.ToString() : attribute.Description;
    }
}