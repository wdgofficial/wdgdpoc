using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Models
{
    public class Message : BaseModel
    {
        private string signature;
        public string Signature
        {
            get { return signature; }
            set
            {
                signature = value;
                OnChanged("Signature");
            }
        }

        private string publicKey;
        public string PublicKey
        {
            get { return publicKey; }
            set
            {
                publicKey = value;
                OnChanged("PublicKey");
            }
        }
    }
}
