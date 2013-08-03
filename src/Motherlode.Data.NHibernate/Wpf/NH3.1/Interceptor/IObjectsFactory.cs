using System;

namespace NHibernateConcepts.Wpf.Interceptor
{
    public interface IObjectsFactory
    {
        T Create<T>();

        object Create(Type type);
    }
}