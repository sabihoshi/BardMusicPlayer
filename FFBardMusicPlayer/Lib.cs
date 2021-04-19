using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace FFBardMusicPlayerInit
{
    [DataContract]
    internal class Lib
    {
        [DataContract]
        protected internal struct Dll
        {
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string sha256 { get; set; }
        }
        [DataMember]
        public List<Dll> dlls { get; set; }
        [DataMember]
        public bool expired { get; set; }
        [DataMember]
        public string expiredtext { get; set; }
        [DataMember]
        public int global { get; set; }
        [DataMember]
        public int cn { get; set; }
        [DataMember]
        public int kr { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string message { get; set; }

        internal static Lib Deserialize(string json)
        {
            var deserializer = new DataContractJsonSerializer(typeof(Lib));
            using var stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
            return (Lib)deserializer.ReadObject(stream);
        }
    }
}
