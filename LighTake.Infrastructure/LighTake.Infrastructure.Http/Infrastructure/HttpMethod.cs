using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Http.Infrastructure
{
    public enum EnumHttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public enum EnumContentType
    {
        Json,
        Xml,
        String
    }
}
