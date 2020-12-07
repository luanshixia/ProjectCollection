using System;
using System.Collections.Generic;
using System.Text;

namespace BubbleMind.Data
{
    public class MindMap
    {
        public string Content { get; set; }

        public MindMap[] Children { get; set; }
    }
}
