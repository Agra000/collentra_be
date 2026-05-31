namespace collentra_be.DTO.Response
{
    public class GetRatingResponse
    {
        public Guid? ratingId { get; set; }
        public string? raterName { get; set; }
        public string? raterEmail { get; set; }
        public double Rating { get; set; }
        public int? RateCount { get; set; }
        public string? Comment { get; set; }
        public DateTime? TimeRated { get; set; }
    }
}
