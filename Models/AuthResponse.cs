namespace Authentication_Authorization_Platform___IAM.Models
{
    public class AuthResponse
    {

        // Contrsct : API -> JSON ; Api is giving the response back to the ui.

        public bool Success { get; set; }

        public string Message { get; set; } = "";

        public string? Token { get; set; }  // to return JWT Token 
    }
}
