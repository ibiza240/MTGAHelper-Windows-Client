using System;
using System.Runtime.Serialization;

namespace MTGAHelper.Entity
{
    [Serializable]
    internal class CardsLoaderException : Exception
    {
        public CardsLoaderException()
        {
        }

        public CardsLoaderException(string message) : base(message)
        {
        }

        public CardsLoaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CardsLoaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}