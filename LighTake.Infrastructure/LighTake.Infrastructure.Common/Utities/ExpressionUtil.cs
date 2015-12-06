using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LighTake.Infrastructure.Common.Utities
{
    /// <summary>
    /// 表达式处理工具类
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年04月21日
    /// 修改历史 : 无
    /// </remarks>
    public class ExpressionUtil
    {
        #region ASCII排序数组
        static int[] ASCII_Sort_Array = new int[] { 
            32,
            129,
            130,
            131,
            132,
            133,
            134,
            135,
            136,
            137,
            138,
            139,
            140,
            141,
            142,
            143,
            144,
            145,
            146,
            147,
            148,
            149,
            150,
            151,
            152,
            153,
            154,
            155,
            156,
            157,
            158,
            159,
            160,
            161,
            162,
            163,
            164,
            165,
            166,
            167,
            168,
            169,
            170,
            171,
            172,
            173,
            174,
            175,
            176,
            177,
            178,
            179,
            180,
            181,
            182,
            183,
            184,
            185,
            186,
            187,
            188,
            189,
            190,
            191,
            192,
            193,
            194,
            195,
            196,
            197,
            198,
            199,
            200,
            201,
            202,
            203,
            204,
            205,
            206,
            207,
            208,
            209,
            210,
            211,
            212,
            213,
            214,
            215,
            216,
            217,
            218,
            219,
            220,
            221,
            222,
            223,
            224,
            225,
            226,
            227,
            228,
            229,
            230,
            231,
            232,
            233,
            234,
            235,
            236,
            237,
            238,
            239,
            240,
            241,
            242,
            243,
            244,
            245,
            246,
            247,
            248,
            249,
            250,
            251,
            252,
            253,
            254,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            14,
            15,
            16,
            17,
            18,
            19,
            20,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            30,
            31,
            127,
            39,
            45,
            9,
            10,
            11,
            12,
            13,
            33,
            34,
            35,
            36,
            37,
            38,
            40,
            41,
            42,
            44,
            46,
            47,
            58,
            59,
            63,
            64,
            91,
            92,
            93,
            94,
            95,
            96,
            123,
            124,
            125,
            126,
            43,
            60,
            61,
            62,
            128,
            48,
            49,
            50,
            51,
            52,
            53,
            54,
            55,
            56,
            57,
            65,
            97,
            98,
            66,
            67,
            99,
            100,
            68,
            69,
            101,
            102,
            70,
            71,
            103,
            104,
            72,
            73,
            105,
            106,
            74,
            75,
            107,
            108,
            76,
            77,
            109,
            110,
            78,
            79,
            111,
            112,
            80,
            81,
            113,
            114,
            82,
            83,
            115,
            116,
            84,
            85,
            117,
            118,
            86,
            87,
            119,
            120,
            88,
            89,
            121,
            122,
            90,
            255,
        };

        #endregion

        /// <summary>
        /// 分解表达式。
        /// </summary>
        /// <param name="exp">表达式。</param>
        /// <returns>分解的二维数组，[0]为比较符、[1]为值。</returns>
        public static string[] ExtractExpression(string exp)
        {
            string[] sarray = new string[2];

            if (exp == null || exp == "")
                exp = "Null";

            if (exp == "Min" || exp == "Max" || exp == "Null")
            {
                sarray[0] = "";
                sarray[1] = exp;

                return sarray;
            }

            Regex re = new Regex("([><]{1}={0,1})([\\s\\S]*)");

            if (re.IsMatch(exp))
            {
                Match m = re.Match(exp);
                sarray[0] = m.Result("$1");
                sarray[1] = m.Result("$2");
            }
            else
            {
                throw new Exception("表达式解析错误！");
            }

            return sarray;
        }

        /// <summary>
        /// 将字符串转换为Char()函数连接串。
        /// </summary>
        /// <param name="s">要转换的字符串。</param>
        /// <returns>Char()函数连接串。</returns>
        public static string String2CharStr(string s)
        {
            string rStr = "";

            for (int i = 0; i < s.Length; i++)
            {

                rStr += "char(" + (Char.ConvertToUtf32(s, i)).ToString() + ")";
                if (i != s.Length - 1)
                    rStr += "+";
            }

            return rStr;
        }


        /// <summary>
        /// 将字符串转换为ASCII码数组。
        /// </summary>
        /// <param name="s">字符串。</param>
        /// <returns>ASCII码数组</returns>
        public static int[] String2ASCIICodes(string s)
        {
            int[] codes = new int[s.Length];

            for (int i = 0; i < s.Length; i++)
            {
                codes[i] = Char.ConvertToUtf32(s, i);
            }

            return codes;
        }

        public static int CompareBySqlAsciiSort(string s1, string s2)
        {
            return CompareBySqlAsciiSort(s1, s2, true);
        }

        /// <summary>
        /// 根据SQL的ASCII规则比较字符串。
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns>比较结果，1：s1>s2、0：s1=s2、1：s1<s2。</returns>
        public static int CompareBySqlAsciiSort(string s1, string s2, bool ignoreCase)
        {
            int ri = 0;

            if (ignoreCase)
            {
                s1 = s1.ToLower();
                s2 = s2.ToLower();
            }

            int len = (s1.Length < s2.Length ? s1.Length : s2.Length);
            for (int i = 0; i < len; i++)
            {
                if (IndexOfAsciiCodes(s1[i]) > IndexOfAsciiCodes(s2[i]))
                {
                    return 1;
                }
                else if (IndexOfAsciiCodes(s1[i]) < IndexOfAsciiCodes(s2[i]))
                {
                    return -1;
                }
            }

            ri = (s1.Length < s2.Length ? -1 : 1);

            if (s1.Length == s2.Length)
                ri = 0;

            return ri;
        }

        public static int IndexOfAsciiCodes(char c)
        {
            int idx = (int)c;

            if (idx > 255)
                return idx;

            for (int i = 0; i < ASCII_Sort_Array.Length; i++)
            {
                if (ASCII_Sort_Array[i] == idx)
                    return i;
            }

            return idx;
        }
    }
}
