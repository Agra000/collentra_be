namespace collentra_be.DTO.Response
{
    public class ResultMessageResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string? url { get; set; }
    }
}
