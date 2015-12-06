using System;
using System.Collections.Generic;
using System.Linq;
using LMS.Data.Entity;
using LMS.Data.Repository;

namespace LMS.Services.CommonServices
{
    public class InsuredCalculationService : IInsuredCalculationService
    {
        private readonly IInsuredCalculationRepository _insuredCalculationRepository;

        public InsuredCalculationService(IInsuredCalculationRepository insuredCalculationRepository)
        {
            _insuredCalculationRepository = insuredCalculationRepository;
        }

        public List<InsuredCalculation> GetList()
        {
            return _insuredCalculationRepository.GetAll().ToList();
        }

        public InsuredCalculation GetbyInsuredID(int insuredID)
        {
            return _insuredCalculationRepository.Get(insuredID);
        }
    }
}