using Contentful.CodeFirst;
using Contentful.Core.Models;
using Contentful.Essential.Models;
using System.Collections.Generic;

namespace Contentful.Essential.Tests
{
    public class TestDlvyEntry1 : IContentType
    {
        [IgnoreContentField]
        public SystemProperties Sys { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public bool IsTrue { get; set; }
    }

    public class TestMgmtEntry1 : IManagementContentType
    {
        public Dictionary<string, string> Title { get; set; }
        public Dictionary<string, int> Number { get; set; }
        public Dictionary<string, bool> IsTrue { get; set; }
    }
}
