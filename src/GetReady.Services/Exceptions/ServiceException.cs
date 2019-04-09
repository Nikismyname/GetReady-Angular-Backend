namespace GetReady.Services.Exceptions
{
    using System;

    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message) { }
    }
}
