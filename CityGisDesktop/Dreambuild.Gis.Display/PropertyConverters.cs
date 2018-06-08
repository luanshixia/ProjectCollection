using Dreambuild.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;

namespace Dreambuild.Properties
{
    /// <summary>
    /// 枚举转换器。
    /// 用此类之前，必须保证在枚举项中定义了Description
    /// </summary>
    public class EnumValueDescriptionConverter : ExpandableObjectConverter
    {
        private Dictionary<object, string> _dict;
        private string _cultureInfo = null;

        public EnumValueDescriptionConverter()
        {
            _dict = new Dictionary<object, string>();
        }

        private void LoadDict(ITypeDescriptorContext context)
        {
            _cultureInfo = LocalizationHelper.CurrentLocale;
            _dict = GetEnumValueDisplayNameDict(context.PropertyDescriptor.PropertyType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        private bool IsExpired()
        {
            if ((_dict == null) || (_dict.Count <= 0) || (_cultureInfo != LocalizationHelper.CurrentLocale))
            {
                return true;
            }
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                if (context.PropertyDescriptor.PropertyType.IsEnum)
                {
                    if (IsExpired())
                        LoadDict(context);
                    if (_dict.Values.Contains(value.ToString()))
                    {
                        foreach (object obj in _dict.Keys)
                        {
                            if (_dict[obj] == value.ToString())
                            {
                                return obj;
                            }
                        }
                    }
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (IsExpired())
                LoadDict(context);

            StandardValuesCollection vals = new TypeConverter.StandardValuesCollection(_dict.Keys);

            return vals;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return string.Empty;

            if (IsExpired())
                LoadDict(context);

            foreach (object key in _dict.Keys)
            {
                if (key.ToString() == value.ToString() || _dict[key] == value.ToString())
                {
                    return _dict[key].ToString();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public Dictionary<object, string> GetEnumValueDisplayNameDict(Type enumType)
        {
            Dictionary<object, string> dict = new Dictionary<object, string>();
            FieldInfo[] fieldinfos = enumType.GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (objs.Length > 0)
                    {
                        string description = ((DescriptionAttribute)objs[0]).Description;

                        dict.Add(Enum.Parse(enumType, field.Name), LocalizationHelper.GetString(description));
                    }
                }

            }
            return dict;
        }
    }

    public class ListConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                return FromString(value as string);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is List<T>)
            {
                var list = value as List<T>;
                if (destinationType == typeof(string))
                {
                    return Stringify(list);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    return new InstanceDescriptor(this.GetType().GetMethod("FromString"),
                        new object[] { Stringify(list) }
                    );
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static string Stringify(List<T> list)
        {
            return string.Join(",", list.Select(i => i.ToString()).ToArray());
        }

        public static List<T> FromString(string value)
        {
            return value
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => (T)Convert.ChangeType(s.Trim(), typeof(T)))
                .ToList();
        }
    }
}
