using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Entity;

namespace LMS.Services.CustomerServices
{
    public interface ICustomerService
    {

        List<Customer> GetCustomerList(CustomerParam param);
    }

}
