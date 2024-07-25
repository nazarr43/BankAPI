using Newtonsoft.Json;

namespace Contracts;
public record UserInfoDto
{
    [JsonProperty("username")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}

