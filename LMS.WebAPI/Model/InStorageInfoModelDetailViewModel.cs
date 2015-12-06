using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Model
{
    public class InStorageInfoModelDetailViewModel
    {
        public InStorageInfoModelDetailViewModel()
        {
            InStorageInfoModel = new InStorageModel();
            Customer = new CustomerInStorageModel();
        }
        public InStorageModel InStorageInfoModel { get; set; }
        public CustomerInStorageModel Customer { get; set; }
    }
}