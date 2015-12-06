using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Models
{
    public class PriceProviderDetail
    {
        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Weight { get; set; }

        public decimal AddWeight { get; set; }

        public decimal SettleWeight { get; set; }

        /// <summary>
        /// 超重/超长费(单件)
        /// </summary>
        private decimal _overWeightOrLengthFee;
        public decimal OverWeightOrLengthFee
        {
            get { return Math.Round(_overWeightOrLengthFee, 2); }
            set { _overWeightOrLengthFee = value; }
        }

        /// <summary>
        /// 超周长费用(单件)
        /// </summary>
        private decimal _overGirthFee;
        public decimal OverGirthFee
        {
            get { return Math.Round(_overGirthFee, 2); }
            set { _overGirthFee = value; }
        }

        /// <summary>
        /// 是否超重
        /// </summary>
        public bool IsOverWeight { get; set; }

        /// <summary>
        /// 是否超最大周围长
        /// </summary>
        public bool IsOverMaxGirth { get; set; }
    }
}
