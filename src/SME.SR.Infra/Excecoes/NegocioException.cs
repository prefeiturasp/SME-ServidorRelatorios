using System;
using System.Runtime.Serialization;

namespace SME.SR.Infra
{
    [Serializable]
    public class NegocioException : Exception
    {
        public NegocioException(string message) : base(message)
        {
        }

        protected NegocioException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
