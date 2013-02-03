using System;
using System.Reflection;

namespace NQuery.Symbols
{
    public class ReflectionPropertySymbol : PropertySymbol
    {
        public ReflectionPropertySymbol(PropertyInfo propertyInfo)
            : this(propertyInfo, propertyInfo == null ? null : propertyInfo.Name)
        {
        }

        public ReflectionPropertySymbol(PropertyInfo propertyInfo, string name)
            : base(name, propertyInfo == null ? null : propertyInfo.PropertyType)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; private set; }
    }
}