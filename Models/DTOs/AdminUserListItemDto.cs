namespace Authentication_Authorization_Platform___IAM.Models.DTOs
{
    public class AdminUserListItemDto
    {
        public string  Id { get; set; } = default!;
        public string  FullName { get; set; } = default!;
        public string Email { get; set; } = default!;

        public List<string> Roles { get; set; } = new();

        public List<string> Permissions { get; set; } = new();
    }
}
