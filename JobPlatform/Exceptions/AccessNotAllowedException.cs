namespace JobPlatform.Exceptions
{
    public class AccessNotAllowedException: Exception
    {
        public AccessNotAllowedException(): base("You are not authorized to access the requested resource")
        { }
    }
}
