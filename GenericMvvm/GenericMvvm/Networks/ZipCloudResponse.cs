using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    [DataContract]
    public class ZipCloudResponse
    {
        [DataMember]
        public string status { get; set; }

        [DataMember]
        public string message { get; set; }

        [DataMember]
        public result[] results { get; set; }

        [DataContract]
        public class result
        {
            [DataMember]
            public string zipcode { get; set; }

            [DataMember]
            public string prefcode { get; set; }

            [DataMember]
            public string address1 { get; set; }

            [DataMember]
            public string address2 { get; set; }

            [DataMember]
            public string address3 { get; set; }

            [DataMember]
            public string kana1 { get; set; }

            [DataMember]
            public string kana2 { get; set; }

            [DataMember]
            public string kana3 { get; set; }
        }
    }
}
