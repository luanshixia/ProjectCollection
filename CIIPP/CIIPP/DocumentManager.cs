using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;

namespace CIIPP
{
    /// <summary>
    /// 可保存和恢复的文档，每个文档包括一个城市和若干项目
    /// </summary>
    [Serializable]
    public class Document
    {
        /// <summary>
        /// 城市
        /// </summary>
        public CityStatistics City { get; private set; }

        /// <summary>
        /// 项目集合
        /// </summary>
        public ProjectCollection Projects { get; private set; }

        /// <summary>
        /// 文档文件名
        /// </summary>
        public string FileName { get; private set; }

        private Document()
        {
            City = new CityStatistics();
            Projects = new ProjectCollection();
            FileName = string.Empty;
        }

        /// <summary>
        /// 保存到指定文件名。请使用"*.ciipp"。
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void SaveAs(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate,
                System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                bf.Serialize(fs, this);
            }
            FileName = fileName;
        }

        /// <summary>
        /// 保存到当前文件名。
        /// </summary>
        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.FileStream fs = new System.IO.FileStream(this.FileName, System.IO.FileMode.Create,
                System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                bf.Serialize(fs, this);
            }
        }

        /// <summary>
        /// 打开指定文件。
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>打开的文档</returns>
        public static Document Open(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.Stream fs = System.IO.File.OpenRead(fileName))
            {
                Document doc = bf.Deserialize(fs) as Document;
                doc.FileName = fileName;
                CityStatistics.Current = doc.City; // newly 20120206
                doc.City.SetExpressions();
                doc.Projects.ForEach(x => x.SetExpressions()); // newly 20120206
                return doc;
            }            
        }

        /// <summary>
        /// 新建文档。
        /// </summary>
        /// <returns>新建的文档</returns>
        public static Document New()
        {
            return new Document();
        }
    }

    /// <summary>
    /// 文档管理器
    /// </summary>
    public static class DocumentManager
    {
        /// <summary>
        /// 当前文档
        /// </summary>
        public static Document CurrentDocument { get; private set; }

        /// <summary>
        /// 打开文档
        /// </summary>
        /// <param name="fileName">文件名</param>
        public static void Open(string fileName)
        {
            CurrentDocument = Document.Open(fileName);
        }

        /// <summary>
        /// 新建文档
        /// </summary>
        public static void New()
        {
            CurrentDocument = Document.New();
        }
    }
}
