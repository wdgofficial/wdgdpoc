using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.DTO
{
    public class EstimateRawTransactionOM
    {
        /// <summary>
        /// 总数据量
        /// </summary>
        public long totalSize { get; set; }

        /// <summary>
        /// 总手续费
        /// </summary>
        public long totalFee { get; set; }

        /// <summary>
        /// 找零
        /// </summary>
        public long Change { get; set; }
    }
}
