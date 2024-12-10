namespace JobPlatform.Exceptions
{
    public class AllowanceExceededException : Exception
    {
        public AllowanceExceededException() :base("The subscriber has exceeded the allowance of his current subscription")
        { }
    }
}
