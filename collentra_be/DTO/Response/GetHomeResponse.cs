namespace collentra_be.DTO.Response
{
    public class GetHomeResponse
    {
        public int groupCount { get; set; }
        public int taskRemaining { get; set; }
        public int taskCompleted { get; set; }
        public double teamPerformance { get; set; }
        public DateTime memberSince { get; set; }
        public DateTime dob { get; set; }
        public char gender { get; set; }
        public bool status { get; set; }
        public string? message { get; set; }
    }
}
