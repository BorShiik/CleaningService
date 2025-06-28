namespace CleanDeal.Models
{
    public class ServicePackageItem
    {
        public int Id { get; set; }
        public int ServicePackageId { get; set; }
        public ServicePackage ServicePackage { get; set; } = null!;
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; } = null!;
    }
}