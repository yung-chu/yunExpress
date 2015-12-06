using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    public abstract class MessageManager
    {
        public abstract string GetMessage(int category, int messageId, params object[] args);
    }

    /// <summary>
    /// category为0的代表公共类别
    /// messageId从0到1000为系统自带的公共类类别下的messageId
    /// </summary>
    public class DefaultMessageManager : MessageManager
    {
        protected static Dictionary<int, Dictionary<int, string>> dicCategories = new Dictionary<int, Dictionary<int, string>>();
        protected static Dictionary<int, string> dicMessages = new Dictionary<int, string>();

        static DefaultMessageManager()
        {
            Dictionary<int, string> dicCommomCategory = new Dictionary<int, string>();
            dicCommomCategory.Add(CommonMessageIds.RequiredField, "The {0} can not be empty.");
            dicCommomCategory.Add(CommonMessageIds.RangeField, "The {0} must be between {1} and {2}.");
            dicCommomCategory.Add(CommonMessageIds.GreaterThan, "The {0} must be greater than {1}.");
            dicCommomCategory.Add(CommonMessageIds.LessThan, "The {0} must be less than {1}.");
            dicCommomCategory.Add(CommonMessageIds.Email, "Email is in an incorrect format.");
            dicCommomCategory.Add(CommonMessageIds.ValidationCode, "Validation code you entered is incorrect.");
            dicCommomCategory.Add(CommonMessageIds.SignInError, "Email or password you entered is incorrect.");

            dicCategories.Add(0, dicCommomCategory);
        }

        protected Dictionary<int, string> GetCommonMessage()
        {
            return dicCategories[0];
        }

        public override string GetMessage(int category, int messageId, params object[] args)
        {
            return string.Format(dicCategories[category][messageId], args);
        }
    }

    public static class CommonMessageIds
    {
        public static readonly int RequiredField = 0;
        public static readonly int RangeField = 1;
        public static readonly int GreaterThan = 2;
        public static readonly int LessThan = 3;
        public static readonly int Email = 4;
        public static readonly int ValidationCode = 5;
        public static readonly int SignInError = 6;
    }
}
