namespace Authentication_Authorization_Platform___IAM.Models.DTOs
{
    public class AdminActionResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public bool ReloginRequired { get; set; }
    }
}
