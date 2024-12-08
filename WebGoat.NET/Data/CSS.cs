using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class XSSHandler
{
    public async Task InvokeAsync(HttpContext context)
    {
        string page = context.Request.Query["page"];
        await context.Response.WriteAsync($"The page \"{page}\" was not found.");
    }
}
