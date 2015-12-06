using System;
using System.Collections.Generic;
using System.Text;

namespace SsoFramework
{
    [Serializable]
    public class SsoTicket
    {
        private string _userId;
        private DateTime _timeStamp;
        private int _expireDuration;

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public int ExpireDuration
        {
            get { return _expireDuration; }
            set { _expireDuration = value; }
        }

        public string Encode()
        {
            return SsoCipher.EncryptVector(
                _userId, 
                _timeStamp == default(DateTime)?null:_timeStamp.ToString(),
                _expireDuration.ToString());
        }

        public static bool TryParse(string text, out SsoTicket ticket)
        {
            ticket = null;
            string[] vector;
            if (!SsoCipher.TryParseVector(text, out vector))
                return false;
            if (vector.Length != 3)
                return false;
            ticket = new SsoTicket();
            ticket._userId = vector[0];
            ticket._timeStamp = Convert.ToDateTime(vector[1]);
            ticket._expireDuration = Convert.ToInt32(vector[2]);
            return true;
        }
    }
}
