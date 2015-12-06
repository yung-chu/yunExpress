
using LMS.WCF.TrackNumber.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LMS.WCF.TrackNumber
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ITrackNumberAssign”。
    [ServiceContract]
    public interface ITrackNumberAssign
    {
        //UriTemplate = "/TrackNumberAssignStandard/{shippingMethodId}/{count}/{countryCode}", 
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        ResutlInfo<string> TrackNumberAssignStandard(int shippingMethodId, int count, string countryCode);
    }
}
