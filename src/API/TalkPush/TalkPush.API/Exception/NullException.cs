namespace TalkPush.API.Exception
{
    public class NullException : BadRequestException
    {
        public NullException(string message, string details) : base(message, details)
        {
        }
    }
}
