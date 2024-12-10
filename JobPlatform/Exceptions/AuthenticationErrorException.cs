namespace JobPlatform.Exceptions
{
    public class AuthenticationErrorException : Exception
    {
        public AuthenticationErrorException(string? parameter) : base($"Authentication error! You must submit a valid {parameter}")
        {
        }
    }
}
