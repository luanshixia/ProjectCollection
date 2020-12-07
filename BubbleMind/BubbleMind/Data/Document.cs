using System;
using System.Collections.Generic;
using System.Text;

namespace BubbleMind.Data
{
    public class Document
    {
        public string Filename { get; set; }

        public MindMap MindMap { get; set; }

        public NodeStyle DefaultNodeStyle { get; set; }

        public Dictionary<string, object> Metadata { get; set; }
    }
}
