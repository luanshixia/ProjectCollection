using System;
using System.Collections.Generic;
using System.Text;

namespace BookKeeper
{
    public class CommandEngine
    {
    }

    public sealed class CommandModuleAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public sealed class CommandActionAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public class DemoCommandModule
    {

    }
}
