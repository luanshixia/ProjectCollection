using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TongJi.Web.DAL;

namespace TongJi.Web.Workflow
{
    public class FormManager
    {
        public int FormId { get; private set; }
        public FormExpression FormDef { get; private set; }

        public FormManager(int formId)
        {
            FormId = formId;
            FormDef = new FormExpression(GetDbRecord().def);
        }

        public static int NewForm(string name)
        {
            entity_form form = new entity_form { name = name, def = new FormExpression().GetXml() };
            LinqHelper.Workflow.entity_form.InsertOnSubmit(form);
            LinqHelper.Workflow.SubmitChanges();
            return LinqHelper.Workflow.entity_form.Max(x => x.id);
        }

        public entity_form GetDbRecord()
        {
            return LinqHelper.Workflow.entity_form.Single(x => x.id == FormId);
        }

        public static IEnumerable<entity_form> GetDbTable()
        {
            return LinqHelper.Workflow.entity_form;
        }

        public void SaveChanges()
        {
            GetDbRecord().def = FormDef.GetXml();
            LinqHelper.Workflow.SubmitChanges();
        }
    }

    public class FormExpression
    {
        private XElement _root;
        private XElement _data;
        private XElement _dicts;
        private XElement _layout;

        public FormExpression()
        {
            _root = new XElement("FormDef");
            _data = new XElement("Data");
            _dicts = new XElement("Dictionaries");
            _layout = new XElement("Layout");
            _root.Add(_data, _dicts, _layout);
        }

        public FormExpression(XElement xe)
        {
            _root = xe;
            _data = xe.Elements("Data").First();
            _dicts = xe.Elements("Dictionaries").First();
            _layout = xe.Elements("Layout").First();
        }

        public XElement GetXml()
        {
            return XElement.Parse(_root.ToString());
        }

        public void AddField(string name, FormDataType dataType)
        {
            _data.Add(new XElement("Field", new XAttribute("Name", name), new XAttribute("DataType", dataType.ToString())));
        }

        public void AddEnumField(string name, string dictName, List<string> values)
        {
            _data.Add(new XElement("Field", new XAttribute("Name", name), new XAttribute("DataType", "Enum"), new XAttribute("Dictionary", dictName)));
            XElement dict = new XElement("Dictionary", new XAttribute("Name", dictName));
            _dicts.Add(dict);
            foreach (string value in values)
            {
                dict.Add(new XElement("Add", new XAttribute("Key", values.IndexOf(value)), new XAttribute("Value", value)));
            }
        }

        public void RemoveField(string name)
        {
            if (_data.Elements().Any(x => x.AttValue("Name") == name))
            {
                _data.Elements().First(x => x.AttValue("Name") == name).Remove();
            }
        }

        public IEnumerable<XElement> GetFields()
        {
            foreach (var field in _data.Elements())
            {
                yield return field;
            }
        }

        public IEnumerable<string> GetDictValues(string dictName)
        {
            var dict = _dicts.Elements().First(x => x.AttValue("Name") == dictName);
            foreach (var value in dict.Elements())
            {
                yield return value.AttValue("Value");
            }
        }
    }

    public enum FormDataType
    {
        String = 0,
        Int32,
        Double,
        DateTime,
        Enum
    }
}
