namespace Example.Application.Dto
{
    public class ExceptionDto
    {
        public string? Title { get; set; }
        public int Status { get; set; }
        public string? TraceId { get; set; }
        public dynamic? Errors { get; set; }
    }
}
