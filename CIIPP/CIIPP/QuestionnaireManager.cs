using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIIPP
{
    /// <summary>
    /// 问卷选项管理器
    /// </summary>
    public static class QuestionnaireManager
    {
        public const string ConfigFile = "Questionnaire.csv";
        private static QuestionCollection _questions = new QuestionCollection();
        public static QuestionCollection Questions { get { return _questions; } }

        static QuestionnaireManager()
        {
            var lines = System.IO.File.ReadAllLines(ConfigFile, Encoding.Default).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Split(',')).ToList();
            Question curQ = new Question();
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line[0]))
                {
                    curQ = new Question { Name = line[0], DisplayName = line[1], GroupName = line[2], Chinese = line[3], English = line[4] };
                    _questions.Add(curQ);
                }
                else
                {
                    curQ.Options.Add(new QuestionOption { Point = Convert.ToInt32(line[1]), Chinese = line[3], English = line[4] });
                }
            }
        }
    }

    /// <summary>
    /// 问题
    /// </summary>
    public class Question
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string GroupName { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }
        public List<QuestionOption> Options { get; set; }

        public Question()
        {
            Options = new List<QuestionOption>();
        }
    }

    /// <summary>
    /// 选项
    /// </summary>
    public class QuestionOption
    {
        public int Point { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }
    }

    public class QuestionCollection : List<Question>
    {
        public Question this[string name]
        {
            get
            {
                return this.First(x => x.Name == name);
            }
        }

        public double WholePoint
        {
            get
            {
                return this.Sum(x => x.Options.Max(y => y.Point));
            }
        }
    }

    public static class VarDescriptionManager
    {
        public const string ConfigFile = "VarDescription.csv";
        public static Dictionary<string, string> Descriptions { get; private set; }

        static VarDescriptionManager()
        {
            Descriptions = new Dictionary<string, string>();
            var lines = System.IO.File.ReadAllLines(ConfigFile, Encoding.Default).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Split(',')).ToList();
            foreach (var line in lines)
            {
                Descriptions.Add(line[0], line[1]);
            }
        }
    }
}
