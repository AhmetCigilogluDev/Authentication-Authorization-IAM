namespace Authentication_Authorization_Platform___IAM.Models.DTOs
{
    public class RemovePermissionRequest
    {
        public string UserId { get; set; } = default!;
        public string Permission { get; set; } = default!;
    }
}
