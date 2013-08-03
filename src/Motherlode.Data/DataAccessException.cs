using System;
using System.Runtime.Serialization;

namespace Motherlode.Data
{
    [Serializable]
    public class DataAccessException : Exception
    {
        #region Constructors and Destructors

        public DataAccessException()
        {
        }

        public DataAccessException(string message) : base(message)
        {
        }

        public DataAccessException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DataAccessException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}
