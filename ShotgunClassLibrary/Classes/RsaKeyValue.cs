using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ShotgunClassLibrary.Classes
{
    [XmlRoot("RSAKeyValue")]
    public class RsaKeyValue
    {
        [XmlElement("Modulus")]
        public string Modulus { get; set; }

        [XmlElement("Exponent")]
        public string Exponent { get; set; }
    }
}
