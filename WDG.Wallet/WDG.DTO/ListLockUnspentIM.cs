using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.DTO
{
    public class ListLockUnspentIM
    {
        public string txid { get; set; }
        public int vout { get; set; }
    }
}
