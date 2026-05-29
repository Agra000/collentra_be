namespace collentra_be.DTO.Response
{
    public class GetRatingResponse
    {
        public Guid? RaterId { get; set; }

        public double Rating { get; set; }
        public int? RateCount { get; set; }
        public string? Comment { get; set; }
        public DateTime? TimeRated { get; set; }
    }
}
