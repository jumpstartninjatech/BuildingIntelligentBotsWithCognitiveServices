using System;
using System.Collections.Generic;
using System.Text;

namespace samples
{
    public class GetStatusClass
    {
        public string ResponseCode { get; set; }

        public string Message { get; set; }

        public data[] data { get; set; }

    }

    public class data
    {
        public string MemberID { get; set; }

        public string MemberName { get; set; }
    }
}
