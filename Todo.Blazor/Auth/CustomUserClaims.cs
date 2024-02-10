using System.Security.Claims;

namespace BlazorApp2.Models
{
    public record CustomUserClaims(string name=null!, string email=null!, string DisplayName=null!, string id = null!,string roles=null!);
}
