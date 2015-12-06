using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LMS.Core;

namespace LMS.WebAPI.Client.Controllers
{
    public class ClientUpdateController : ApiController
    {
        [HttpGet]
        public UpdateResponse LatestVersion(string appName)
        {
            UpdateResponse updateResponse = new UpdateResponse();

            switch (appName)
            {
                case "LMS.WinForm.Client":
                    updateResponse.Success = true;
                    updateResponse.Version = sysConfig.LmsClientLatestVersion;
                    updateResponse.Url = sysConfig.ClientUpdatePath + sysConfig.LmsClientLatestName;

                    break;
            }

            return updateResponse;
        }
    }

    public class UpdateResponse
    {
        public bool Success { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
    }
}
