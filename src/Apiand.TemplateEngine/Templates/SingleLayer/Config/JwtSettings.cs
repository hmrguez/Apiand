namespace XXXnameXXX.Config;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public int ExpirationDays { get; set; } = 7;
}
