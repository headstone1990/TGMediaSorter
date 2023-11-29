namespace TGMediaSorter;

public class AuthenticatorData(string phoneNumber, string apiHash, int apiId, string password)
{
    public string PhoneNumber { get; set; } = phoneNumber;
    public string ApiHash { get; set; } = apiHash;
    public int ApiId { get; set; } = apiId;
    public string Password { get; set; } = password;
}