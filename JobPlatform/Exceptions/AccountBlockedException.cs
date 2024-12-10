namespace JobPlatform.Exceptions
{
    public class AccountBlockedException : Exception
    {
        public AccountBlockedException(): base("Access not allowed. The account has been blocked due to multiple failed login attempts")
        { }

    }
}
