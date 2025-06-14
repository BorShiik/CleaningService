namespace CleanDeal.DTOs
{
    public class CleanerRatingDTO
    {
        public string CleanerId { get; set; } = string.Empty;
        public string CleanerName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
    }
}
