using System;

namespace Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory
{
    public interface IObjectFactory
    {
        #region Public Methods and Operators

        T Create<T>();

        object Create(Type type);

        #endregion
    }
}
