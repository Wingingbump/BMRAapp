using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cert_mailer
{
    public class EnumCertificateType
    {
        public enum CertificateType
        {
            None = 0,
            Default = 1,
            SBA = 2,
            NOAA = 3,
            DOIU = 4
        }
    }
}

