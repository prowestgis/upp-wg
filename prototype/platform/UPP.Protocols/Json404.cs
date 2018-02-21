using System;

namespace UPP.Protocols
{
    public sealed class Json404
    {
        public static Json404 Default = new Json404("Resource not found");

        public Json404(string message)
        {
            Message = message;
        }

        public Json404()
            : this(String.Empty)
        {
        }

        public string Message { get; private set; }
    }
}
