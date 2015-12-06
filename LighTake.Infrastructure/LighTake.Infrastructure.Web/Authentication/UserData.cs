using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LighTake.Infrastructure.Web
{
    [DataContract]
    public class UserData
    {
        [DataMember]
        public string UserName { get; set; }
    }
}
