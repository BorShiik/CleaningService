using CleanDeal.DTO.DTOs;
using CleanDeal.Model.Models;

namespace CleanDeal.DTO.ViewModel
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ServiceType> Services { get; set; } = new List<ServiceType>();
        public IEnumerable<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}