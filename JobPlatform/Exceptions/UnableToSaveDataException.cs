namespace JobPlatform.Exceptions
{
    public class UnableToSaveDataException: Exception
    {
        public UnableToSaveDataException() : base("Database error: Unable to save data") { }

        public UnableToSaveDataException(string message) : base(message) { }
    }
}
