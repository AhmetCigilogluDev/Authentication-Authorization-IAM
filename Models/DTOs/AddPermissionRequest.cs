namespace Authentication_Authorization_Platform___IAM.Models.DTOs
{
    public class AddPermissionRequest
    {
        public string UserId { get; set; } = default!;
        public string Permission { get; set; } = default!;
    }
}
