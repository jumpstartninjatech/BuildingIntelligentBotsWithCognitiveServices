using System;
using System.Collections.Generic;
using System.Text;

namespace samples
{
    public class APIClass
    {
        public string query { get; set; }

        public string intent { get; set; }

        public entities[] entities { get; set; }

    }

    public class entities
    {
        public string entity { get; set; }

        public string type { get; set; }

        public string startIndex { get; set; }

        public string endIndex { get; set; }
    }
      

}
