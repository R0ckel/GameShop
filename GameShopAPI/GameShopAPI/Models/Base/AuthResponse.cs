namespace GameShopAPI.Models.Base;

public class AuthResponse
{
    public string Message { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
