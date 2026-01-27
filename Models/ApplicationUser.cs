using Microsoft.AspNetCore.Identity;

namespace Authentication_Authorization_Platform___IAM.Models
{
    public class ApplicationUser: IdentityUser
    {

        // IdentityUser table has been extended with the property name of FullName
        public string FullName { get; set; }
    }
}
