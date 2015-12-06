using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using LMS.Data.Entity;
using LMS.Data.Repository;

namespace LMS.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerService)
        {
            _customerRepository = customerService;
        }

        public List<Customer> GetCustomerList(CustomerParam param)
        {
            Expression<Func<Customer, bool>> filter = p => true;
            filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                  !string.IsNullOrWhiteSpace(param.CustomerCode));


            return _customerRepository.GetList(filter);
        }
    }
}
