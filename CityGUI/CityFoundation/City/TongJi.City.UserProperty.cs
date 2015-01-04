// TongJi.City.UserProperty.cs
//
// @author: WANG Yang
//
// This file is designed to take the functionality of the previous Autodesk SDF file exporting machanism, using XML to replace SDF.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TongJi.City
{
    public class UserProperty
    {
        private XDocument xd;
        private XElement xroot;

        public const string eleComponent = "Component";
        public const string eleDictionaries = "Dictionaries";
        public const string eleProperty = "Property";
        public const string attCode = "Number";
        public const string attName = "Name";
        public const string attPropName = "Caption";
        public const string attGeoType = "FDOType";
        public const string attCadType = "DXFType";

        public UserProperty(string fileName)
        {
            xd = XDocument.Load(fileName);
            xroot = xd.Elements().ToList()[0];
        }

        public XElement[] AllComponents
        {
            get
            {
                return xroot.Elements(eleComponent).ToArray();
            }
        }

        public string[] AllCodes
        {
            get
            {
                return xroot.Elements(eleComponent).Select(x => x.AttValue(attCode)).ToArray();
            }
        }

        public string[] AllNames
        {
            get
            {
                return xroot.Elements(eleComponent).Select(x => x.AttValue(attName)).ToArray();
            }
        }

        public string[] AllDictNames // newly 20121224
        {
            get
            {
                return xroot.Elements(eleDictionaries).First().Elements().Select(x => x.AttValue("Name")).ToArray();
            }
        }

        public string[] GetPropNamesByCode(string code) // newly 20130808
        {
            return GetAllPropNames(GetComponentByCode(code));
        }

        public string[] GetPropNamesByName(string name) // newly 20130808
        {
            return GetAllPropNames(GetComponentByName(name));
        }

        public XElement GetComponentByCode(string code)
        {
            return xroot.Elements(eleComponent).Single(x => x.AttValue(attCode) == code);
        }

        public XElement GetComponentByName(string name)
        {
            return xroot.Elements(eleComponent).Single(x => x.AttValue(attName) == name);
        }

        public static CityEntityType GetComponentGeoType(XElement component)
        {
            switch (component.AttValue(attGeoType))
            {
                case "1":
                    return CityEntityType.Spot;
                case "2":
                    return CityEntityType.Linear;
                case "4":
                    return CityEntityType.Region;
                default:
                    return CityEntityType.None;
            }
        }

        public static string GetComponentCadType(XElement component)
        {
            return component.AttValue(attCadType);
        }

        public static XElement[] GetAllProperties(XElement component)
        {
            return component.Descendants(eleProperty).ToArray();
        }

        public static string[] GetAllPropNames(XElement component)
        {
            return component.Descendants(eleProperty).Select(x => x.AttValue(attPropName)).ToArray();
        }

        public static XElement GetProperty(XElement component, string propName)
        {
            return component.Descendants(eleProperty).Single(x => x.AttValue(attPropName) == propName);
        }

        public static XElement GetGroupOfProperty(XElement property)
        {
            return property.Parent;
        }

        public XElement GetDictionary(string dictName)
        {
            return xroot.Element(eleDictionaries).Elements().Single(x => x.AttValue(attName) == dictName);
        }
    }

    public class UserPropertyRecord
    {
        private PropertyDictionary _properties = new PropertyDictionary();
        public PropertyDictionary Properties { get { return _properties; } }

        public string GeoData { get; set; }
        public XElement Component { get; private set; }

        public UserPropertyRecord(XElement component)
        {
            Component = component;
            GeoData = string.Empty;
            UserProperty.GetAllPropNames(component).ToList().ForEach(x => _properties[x] = string.Empty);
        }

        public XElement ToXElement()
        {
            XElement xe = new XElement("Record");
            xe.Add(new XAttribute("Geo", GeoData));
            XElement xeProp = new XElement("Properties");
            foreach (var prop in _properties.AllEntries)
            {
                xeProp.Add(new XAttribute(prop, _properties[prop] ?? string.Empty));
            }
            xe.Add(xeProp);

            return xe;
        }
    }

    public class UserPropertyTable
    {
        private List<UserPropertyRecord> _records = new List<UserPropertyRecord>();
        public List<UserPropertyRecord> Records { get { return _records; } }

        public XElement Component { get; private set; }
        public string Code { get { return Component.AttValue(UserProperty.attCode); } }
        public string CadType { get { return Component.AttValue(UserProperty.attCadType); } }

        public UserPropertyTable(XElement component)
        {
            Component = component;
        }

        public XElement ToXElement()
        {
            XElement xe = new XElement("Table");
            xe.Add(new XAttribute("Code", Component.AttValue(UserProperty.attCode)));
            xe.Add(new XAttribute("Name", Component.AttValue(UserProperty.attName)));
            xe.Add(new XAttribute("GeoType", Component.AttValue(UserProperty.attGeoType)));
            xe.Add(new XAttribute("CadType", Component.AttValue(UserProperty.attCadType)));
            foreach (var record in _records)
            {
                xe.Add(record.ToXElement());
            }

            return xe;
        }
    }

    public class UserPropertyDataHolder
    {
        public UserProperty Schema { get; private set; }

        public List<UserPropertyTable> Tables { get; private set; }

        public UserPropertyDataHolder(UserProperty schema)
        {
            Schema = schema;
            Tables = schema.AllComponents.Select(x => new UserPropertyTable(x)).ToList();
        }

        public void SaveXml(string fileName)
        {
            XDocument xd = new XDocument();
            XElement xroot = new XElement("Database");
            Tables.ForEach(x => xroot.Add(x.ToXElement()));
            xd.Add(xroot);
            xd.Save(fileName);
        }
    }
}
