using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Models
{
    public class ListPageUnspent : BaseModel
    {
        private List<PageUnspent> unspentList;
        private long count;

        public List<PageUnspent> UnspentList
        {
            get
            {
                if (unspentList == null)
                    unspentList = new List<PageUnspent>();
                return unspentList;
            }
            set
            {
                value = unspentList;
                OnChanged("UnspentList");
            }
        }

        public long Count
        {
            get { return count; }
            set
            {
                count = value;
                OnChanged("Count");
            }
        }
    }

    public class PageUnspent : BaseModel
    {
        private string _txid { get; set; }
        private long _vout { get; set; }
        private string _address { get; set; }
        private string _account { get; set; }
        private string _scriptPubKey { get; set; }
        private string _redeemScript { get; set; }
        private long _amount { get; set; }
        private long _confirmations { get; set; }
        private bool _spendable { get; set; }
        private bool _solvable { get; set; }


        public string Txid { get { return _txid; } set { _txid = value;OnChanged("Txid"); } }
        public long Vout { get { return _vout; } set { _vout = value; OnChanged("Vout"); } }
        public string Address { get { return _address; } set { _address = value; OnChanged("Address"); } }
        public string Account { get { return _account; } set { _account = value; OnChanged("Account"); } }
        public string ScriptPubKey { get { return _scriptPubKey; } set { _scriptPubKey = value; OnChanged("ScriptPubKey"); } }
        public string RedeemScript { get { return _redeemScript; } set { _redeemScript = value; OnChanged("RedeemScript"); } }
        public long Amount { get { return _amount; } set { _amount = value; OnChanged("Amount"); } }
        public long Confirmations { get { return _confirmations; } set { _confirmations = value; OnChanged("Confirmations"); } }
        public bool Spendable { get { return _spendable; } set { _spendable = value; OnChanged("Spendable"); } }
        public bool Solvable { get { return _solvable; } set { _solvable = value; OnChanged("Solvable"); } }
    }
}
