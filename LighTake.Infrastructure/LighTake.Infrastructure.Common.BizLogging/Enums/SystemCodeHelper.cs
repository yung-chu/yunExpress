using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common.BizLogging.Enums
{
    public class SystemCodeHelper
    {
        /// <summary>
        /// 请大写Key
        /// </summary>
        static Dictionary<string, string> _dict = new Dictionary<string, string>();

        //数据来自 , 权限系统
        static SystemCodeHelper()
        {
            _dict.Add("LMS", "S012");
            _dict.Add("LIS", "S010");
            _dict.Add("LIS3", "S013");
            _dict.Add("PCM", "S008");
        }

        /// <summary>
        /// 获取系统的系统码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetSystemCode(SystemType code)
        {
            string key = Enum.GetName(typeof(SystemType), code);

            if (_dict.ContainsKey(key))
            {
                return _dict[key.Trim().ToUpper()];//大写
            }
            else
            {
                throw new BusinessLogicException(string.Format("请先配置[{0}]的系统码.", key));
            }
        }



    }
}
