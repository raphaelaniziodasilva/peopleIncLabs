namespace PeopleIncApi.Exceptions
{
    public class HeaderException : Exception
    {
        public HeaderException()
        {
        }

        public HeaderException(string message)
            : base(message)
        {
        }

        public HeaderException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}