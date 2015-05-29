using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using AutoCADCommands;

using System.Xml.Serialization;

namespace TongJi.Drawing
{
    public class TemplateDrawing
    {
        public List<ComponentDefinition> Components { get; set; }
        private static Type[] _types = new Type[] { typeof(TextTemplateParameter), typeof(BlockTemplateParameter) };

        public TemplateDrawing()
        {
            Components = new List<ComponentDefinition>();
        }

        public void Save(string fileName)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TemplateDrawing), _types);
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                serializer.Serialize(fs, this);
            }
        }

        public static TemplateDrawing Load(string fileName)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TemplateDrawing), _types);
            using (System.IO.Stream fs = System.IO.File.OpenRead(fileName))
            {
                TemplateDrawing result = serializer.Deserialize(fs) as TemplateDrawing;
                return result;
            }
        }
    }

    public class TemplateDrawingInstance
    {
        public TemplateDrawing Template { get; private set; }
        public Dictionary<string, ComponentSubstitution> Substitutions { get; private set; }

        public TemplateDrawingInstance(TemplateDrawing template)
        {
            Template = template;
            Substitutions = new Dictionary<string, ComponentSubstitution>();
        }

        public ObjectId DrawToLayout(string layoutName)
        {
            var id = DbHelper.GetLayoutId(layoutName);
            var layout = id.QOpenForRead<Layout>();
            LayoutManager.Current.CurrentLayout = layoutName;
            var vps = layout.GetViewports();
            if (vps.Count > 1)
            {
                var vpId = vps[1];
                if (Template.Components.Any(x => x.ID == "vp1"))
                {
                    var vp1 = Template.Components.First(x => x.ID == "vp1");
                    var lookAt = Point3d.Origin;
                    double viewHeight = 1000;
                    if (Substitutions.ContainsKey("vp1"))
                    {
                        var vp1InstParams = Substitutions["vp1"];
                        lookAt = new Point3d((double)vp1InstParams["TargetX"], (double)vp1InstParams["TargetY"], 0);
                        viewHeight = (double)vp1InstParams["ViewHeight"];
                    }
                    Layouts.SetViewport(vpId, vp1.Width, vp1.Height, vp1.Position, lookAt, viewHeight);
                    Template.Components.Remove(vp1);
                }
            }

            foreach (var component in Template.Components)
            {
                if (Substitutions.ContainsKey(component.ID))
                {
                    GenerateComponent(component, Substitutions[component.ID]);
                }
                else
                {
                    GenerateComponent(component, null);
                }
            }

            return id;
        }

        private static ObjectId GenerateComponent(ComponentDefinition def, ComponentSubstitution sub)
        {
            ObjectId result = ObjectId.Null;
            if (def.Type == TemplateComponent.Text)
            {
                var param = def.Parameter as TextTemplateParameter;
                string text = sub == null ? param.Text.Replace("{", "{{").Replace("}", "}}") : string.Format(param.Text, sub.Substitutions);
                result = Draw.MText(text, param.TextHeight, def.Position, 0, def.Align == AlignType.Center, textStyle: param.TextStyleName);
            }
            else if (def.Type == TemplateComponent.Block)
            {
                var param = def.Parameter as BlockTemplateParameter;
                result = Draw.Insert(param.BlockName, def.Position);
            }
            return result;
        }
    }

    public enum TemplateComponent
    {
        Text,
        Block,
        Viewport,
        Custom
    }

    public enum AlignType
    {
        TopLeft,
        Center
    }

    public class ComponentDefinition
    {
        public string ID { get; set; }
        public Point3d Position { get { return new Point3d(PosX, PosY, 0); } }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public AlignType Align { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public TemplateComponent Type { get; set; }
        public object Parameter { get; set; }

        public ComponentDefinition()
        {
            ID = string.Empty;
            PosX = 0;
            PosY = 0;
            Align = AlignType.TopLeft;
            Width = 0;
            Height = 0;
            Type = TemplateComponent.Block;
            Parameter = new BlockTemplateParameter();
        }
    }

    public class TextTemplateParameter
    {
        public string Text { get; set; }
        public double TextHeight { get; set; }
        public string TextStyleName { get; set; }

        public TextTemplateParameter()
        {
            Text = string.Empty;
            TextHeight = 10;
            TextStyleName = "Standard";
        }
    }

    public class BlockTemplateParameter
    {
        public string BlockName { get; set; }

        public BlockTemplateParameter()
        {
            BlockName = string.Empty;
        }
    }

    public class ComponentSubstitution
    {
        public Dictionary<string, object> InstanceParameters { get; private set; }
        public object[] Substitutions { get; private set; }

        public ComponentSubstitution(params object[] substitutions)
        {
            InstanceParameters = new Dictionary<string, object>();
            Substitutions = substitutions;
        }

        public object this[string key]
        {
            get
            {
                return InstanceParameters[key];
            }
            set
            {
                InstanceParameters[key] = value;
            }
        }
    }
}
