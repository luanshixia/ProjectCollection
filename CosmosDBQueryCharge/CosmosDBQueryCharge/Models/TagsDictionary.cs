using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CosmosDBQueryCharge
{
    /// <summary>
    /// Represents a resource tag dictionary.
    /// </summary>
    public class TagsDictionary : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagsDictionary"/> class.
        /// </summary>
        public TagsDictionary()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagsDictionary"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public TagsDictionary(IDictionary<string, string> dictionary)
            : base(dictionary, StringComparer.InvariantCultureIgnoreCase)
        {
        }
    }
}
