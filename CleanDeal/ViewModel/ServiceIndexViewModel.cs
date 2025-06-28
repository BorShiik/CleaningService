using CleanDeal.Models;
using System.Collections.Generic;

namespace CleanDeal.ViewModel
{
    public class ServiceIndexViewModel
    {
        public IEnumerable<ServiceType> ServiceTypes { get; set; } = new List<ServiceType>();
        public IEnumerable<ServicePackage> Packages { get; set; } = new List<ServicePackage>();
        public int LoyaltyPoints { get; set; }
    }
}