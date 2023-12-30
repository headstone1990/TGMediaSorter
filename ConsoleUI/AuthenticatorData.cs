namespace ConsoleUI;

public class AuthenticatorData
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public AuthenticatorData(string phoneNumber, string apiHash, int apiId, string password)
    {
        PhoneNumber = phoneNumber;
        ApiHash = apiHash;
        ApiId = apiId;
        Password = password;
    }

    public string PhoneNumber { get; set; }
    public string ApiHash { get; set; }
    public int ApiId { get; set; }
    public string Password { get; set; }
}