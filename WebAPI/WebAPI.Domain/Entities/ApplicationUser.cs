using Microsoft.AspNetCore.Identity;
using System.Xml.Serialization;

namespace WebAPI.Domain.Entities;
public class ApplicationUser : IdentityUser
{
    public string? ExternalUserId { get; set; }
    [XmlIgnore]
    public ICollection<IdentityUserRole<string>> UserRoles { get; set; }
    public List<Account> Accounts { get; set; }
}

