namespace UserActivity.Application.DTOs;
public class UserLoginCountDto
{
    public string UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public long LoginCount { get; set; }
}

