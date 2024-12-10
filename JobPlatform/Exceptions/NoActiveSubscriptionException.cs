namespace JobPlatform.Exceptions
{
    public class NoActiveSubscriptionException : Exception
    {
        public NoActiveSubscriptionException()
            :base("The customer has no active subscription")
        { }
    }
}
