using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common.BizLogging.Enums
{
    /// <summary>
    /// 业务系统类型枚举
    /// </summary>
    public enum SystemType
    {
        /// <summary>
        /// 物流
        /// </summary>
        LMS=1,
        /// <summary>
        /// 运费计算系统 
        /// </summary>
        LIS=2

    }

	public class GetSystemType
	{
		public static string GetSystemTypeCode(SystemType type)
		{
			string str = string.Empty;
			switch ((int)type)
			{
				case 1:
					str= "S012";
					break;
				case 2:
					str = "S010";
					break;
			}
			return str;
		}
	}


}
