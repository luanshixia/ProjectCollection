using System;
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
        private static readonly string[] SupportedLocales =
        {
            Locales.ZH_CN,
            Locales.EN_US
        };

        private static readonly Dictionary<string, ResourceDictionary> LocaleResources = LocalizationHelper.SupportedLocales.ToDictionary(
            locale => locale,
            locale => new ResourceDictionary
            {
                Source = new Uri(string.Format(@"Resources\{0}.xaml", locale), UriKind.RelativeOrAbsolute)
            });

        public static string CurrentLocale { get; private set; } = Locales.ZH_CN;

        public static void SetLocale(string locale)
        {
            LocalizationHelper.CurrentLocale = locale;
            Application.Current.Resources.MergedDictionaries[0] = LocalizationHelper.LocaleResources[locale];
        }

        public static string GetString(string key)
        {
            return LocalizationHelper.LocaleResources[CurrentLocale][key] as string;
        }

        public static PropertyDescriptorCollection GetLocalizedPropertyDescriptors(PropertyDescriptorCollection descriptors)
        {
            return new PropertyDescriptorCollection(
                properties: descriptors
                    .Cast<PropertyDescriptor>()
                    .Select(descriptor => new LocalizedPropertyDescriptor(descriptor))
                    .ToArray());
        }
    }

    public class LocalizedPropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor Source;

        public override Type ComponentType => this.Source.ComponentType;

        public override bool IsReadOnly => this.Source.IsReadOnly;

        public override Type PropertyType => this.Source.PropertyType;

        public override string Description => GetLocalizationString(this.Source.Description);

        public override string Category => GetLocalizationString(this.Source.Category);

        public override string DisplayName => GetLocalizationString(this.Source.DisplayName);

        public LocalizedPropertyDescriptor(PropertyDescriptor descriptor)
            : base(descriptor)
        {
            this.Source = descriptor;
        }

        public override bool CanResetValue(object component) => this.Source.CanResetValue(component);

        public override object GetValue(object component) => this.Source.GetValue(component);

        public override void ResetValue(object component) => this.Source.ResetValue(component);

        public override void SetValue(object component, object value) => this.Source.SetValue(component, value);

        public override bool ShouldSerializeValue(object component) => this.Source.ShouldSerializeValue(component);

        private static string GetLocalizationString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            return LocalizationHelper.GetString(key) ?? string.Empty;
        }
    }

    public sealed class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string Key;

        public LocalizedDisplayNameAttribute(string key)
            : base() => this.Key = key;

        public override string DisplayName => LocalizationHelper.GetString(this.Key);
    }

    public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string Key;

        public LocalizedDescriptionAttribute(string key)
            : base() => this.Key = key;

        public override string Description => LocalizationHelper.GetString(this.Key);
    }

    public class LocalizedExpandableObjectConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(
            ITypeDescriptorContext context,
            object value,
            Attribute[] attributes)
        {
            var originalProperties = base.GetProperties(context, value, attributes);
            return LocalizationHelper.GetLocalizedPropertyDescriptors(originalProperties);
        }
    }
}
