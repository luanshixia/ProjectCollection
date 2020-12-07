using BubbleMind.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BubbleMind.Core
{
    public static class DataManager
    {
        public const double NodeSize = 100;

        public static string CurrentFileName { get; private set; }

        public static MindMap CurrentDocument { get; private set; }

        public static void New()
        {
            DataManager.CurrentFileName = null;
            DataManager.CurrentDocument = new MindMap
            {
            };
        }

        public static void Open(string fileName)
        {
            DataManager.CurrentFileName = fileName;
            DataManager.CurrentDocument = Workflow.FromJson(File.ReadAllText(fileName));
        }

        public static void SaveAs(string fileName)
        {
            DataManager.CurrentFileName = fileName;
            File.WriteAllText(fileName, DataManager.CurrentDocument.ToJson());
        }
    }
}
