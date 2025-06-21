using CleanDeal.DTOs;
using CleanDeal.Models;
using System.Collections.Generic;

namespace CleanDeal.ViewModel
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ServiceType> Services { get; set; } = new List<ServiceType>();
        public IEnumerable<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}