using Authentication_Authorization_Platform___IAM.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorization_Platform___IAM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = Policies.DocumentsRead)]
        public IActionResult List() => Ok(new[] { "doc-1", "doc-2" });

        [HttpPost("upload")]
        [Authorize(Policy = Policies.DocumentsUpload)]
        public IActionResult Upload() => Ok(new { message = "uploaded" });

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.DocumentsDelete)]
        public IActionResult Delete(string id) => Ok(new { message = $"deleted {id}" });
    }
}
