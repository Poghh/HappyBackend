namespace Happy.Backend.Api.Constants;

public static class CommonMessageConstants
{
    // Authorization Messages
    public const string MissingAuthorizationHeader = "Missing Authorization header";
    public const string InvalidAuthorizationScheme = "Invalid Authorization scheme";
    public const string InvalidOrExpiredToken = "Invalid or expired token";

    // Token Request Messages
    public const string AppSecretRequired = "AppSecret is required";
    public const string PhoneRequired = "Phone is required";
    public const string InvalidAppCredentials = "Invalid app credentials";
}
