namespace Authentication_Authorization_Platform___IAM.Models
{
    public class AuditLog
    {
        public string Id { get; set; }
        public string ActorUserId { get; set; } = default!;
        public string ActorEmail { get; set; } = default!;
        public string TargetUserId { get; set; } = default!;
        public string TargetEmail { get; set; } = default!;

        // Functions: RoleAssigned, RoleRemoved, PermissionsAdded
        public string ActionType { get; set; } = default!;
        public string Detail { get; set; } = default!;

        // log timestamp
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
