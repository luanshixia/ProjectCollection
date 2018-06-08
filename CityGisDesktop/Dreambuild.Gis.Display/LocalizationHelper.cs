using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Dreambuild.Utils
{
    public interface ILanguageSwitcher
    {
        void RefreshLanguage();
    }

    public static class Locales
    {
        public const string ZH_CN = "zh-CN";
        public const string EN_US = "en-US";
    }

    public static class LocalizationHelper
    {
        private static string[] _locales = { Locales.ZH_CN, Locales.EN_US };
        private static Dictionary<string, ResourceDictionary> _localResources;
        public static string CurrentLocale { get; private set; }

        static LocalizationHelper()
        {
            CurrentLocale = Locales.ZH_CN;
            _localResources = _locales.ToDictionary(x => x, x => new ResourceDictionary
            {
                Source = new Uri(string.Format(@"Resources\{0}.xaml", x), UriKind.RelativeOrAbsolute)
            });
        }

        public static void SetLocale(string localeIdentifier)
        {
            CurrentLocale = localeIdentifier;
            Application.Current.Resources.MergedDictionaries[0] = _localResources[CurrentLocale];
        }

        public static string GetString(string key)
        {
            return _localResources[CurrentLocale][key] as string;
        }

        public static PropertyDescriptorCollection GetLocalizedPropertyDescriptor(PropertyDescriptorCollection descriptors)
        {
            List<LocalizedPropertyDescriptor> tmpPDCLst = new List<LocalizedPropertyDescriptor>();
            PropertyDescriptorCollection tmpPDC = descriptors;
            IEnumerator tmpIe = tmpPDC.GetEnumerator();
            LocalizedPropertyDescriptor tmpCPD;
            PropertyDescriptor tmpPD;
            while (tmpIe.MoveNext())
            {
                tmpPD = tmpIe.Current as PropertyDescriptor;
                tmpCPD = new LocalizedPropertyDescriptor(tmpPD);
                tmpPDCLst.Add(tmpCPD);
            }
            return new PropertyDescriptorCollection(tmpPDCLst.ToArray());
        }
    }

    public class LocalizedPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor mDescriptor;

        public LocalizedPropertyDescriptor(PropertyDescriptor descriptor)
            : base(descriptor)
        {
            mDescriptor = descriptor;
        }

        #region PropertyDescriptor Specific

        public override bool CanResetValue(object component)
        {
            return mDescriptor.CanResetValue(component);
        }

        public override Type ComponentType
        {
            get { return mDescriptor.ComponentType; }
        }

        public override object GetValue(object component)
        {
            return mDescriptor.GetValue(component);
        }

        public override bool IsReadOnly
        {
            get { return mDescriptor.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return mDescriptor.PropertyType; }
        }

        public override void ResetValue(object component)
        {
            mDescriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            mDescriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return mDescriptor.ShouldSerializeValue(component);
        }


        #region PropertyDescriptor Overrides

        public override string Description
        {
            get
            {
                return GetLocalizationString(mDescriptor.Description);
            }
        }

        public override string Category
        {
            get
            {
                return GetLocalizationString(mDescriptor.Category);
            }
        }

        public override string DisplayName
        {
            get
            {
                return GetLocalizationString(mDescriptor.DisplayName);
            }
        }

        private string GetLocalizationString(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return key;
            }

            string tmp = LocalizationHelper.GetString(key);
            return String.IsNullOrEmpty(tmp) ? "" : tmp;
        }

        #endregion PropertyDescriptor Overrides

        #endregion PropertyDescriptor Specific
    }

    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string _key;

        public LocalizedDisplayNameAttribute(string key)
            : base()
        {
            this._key = key;
        }

        public override string DisplayName
        {
            get
            {
                return LocalizationHelper.GetString(_key);
            }
        }
    }

    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _key;

        public LocalizedDescriptionAttribute(string key)
            : base()
        {
            this._key = key;
        }

        public override string Description
        {
            get
            {
                return LocalizationHelper.GetString(_key);
            }
        }
    }

    public class LocalizedExpandableObjectConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
            System.Attribute[] attributes)
        {
            PropertyDescriptorCollection originalCollection = base.GetProperties(context, value, attributes);
            PropertyDescriptorCollection newCollection = LocalizationHelper.GetLocalizedPropertyDescriptor(originalCollection);
            return newCollection;
        }
    }
}
