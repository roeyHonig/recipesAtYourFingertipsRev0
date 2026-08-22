namespace recipesAtYourFingertipsRev0.Models;

public class ExternalLogin
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Provider { get; set; } = string.Empty;

    public string ProviderUserId { get; set; } = string.Empty;
}