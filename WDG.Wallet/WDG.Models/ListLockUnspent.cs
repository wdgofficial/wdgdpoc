using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Models
{
    public class ListLockUnspent : BaseModel
    {
        private string txid { get; set; }
        public string TxId
        {
            get { return txid; }
            set
            {
                txid = value;
                OnChanged("TxId");
            }
        }

        private int vout { get; set; }
        public int Vout
        {
            get { return vout; }
            set
            {
                vout = value;
                OnChanged("Vout");
            }
        }
    }
}
