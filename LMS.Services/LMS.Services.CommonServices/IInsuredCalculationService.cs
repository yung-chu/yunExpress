using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.Services.CommonServices
{
    public interface IInsuredCalculationService
    {
        List<InsuredCalculation> GetList();
        InsuredCalculation GetbyInsuredID(int insuredID);
    }
}
