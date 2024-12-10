namespace JobPlatform.Exceptions
{
    public class UnableToDeleteException : Exception
    {
        public UnableToDeleteException(): base("The requested cannoct be deleted as there are related records associated with it")
        {
        }
    }
}
