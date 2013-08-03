using System.ComponentModel;
using System.Reflection;
using Castle.DynamicProxy;

namespace NHibernateConcepts.Wpf.Interceptor
{
    internal class NotifyPropertyChangedInterceptor : IInterceptor
    {
        private readonly string _typeName;

        private PropertyChangedEventHandler _subscribers = delegate { };

        public NotifyPropertyChangedInterceptor(string typeName)
        {
            _typeName = typeName;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.DeclaringType == typeof(ObjectFactoryBase.ITypeInfo))
            {
                invocation.ReturnValue = _typeName;
                return;
            }

            if (invocation.Method.DeclaringType == typeof(INotifyPropertyChanged))
            {
                var propertyChangedEventHandler = (PropertyChangedEventHandler)invocation.Arguments[0];
                if (invocation.Method.Name.StartsWith("add_"))
                {
                    _subscribers += propertyChangedEventHandler;
                }
                else
                {
                    _subscribers -= propertyChangedEventHandler;
                }

                return;
            }

            if (invocation.Method.IsSpecialName && invocation.Method.Name.StartsWith("set_"))
            {
                var propertyName = invocation.Method.Name.Substring(4);

                var oldValue = invocation.Method.DeclaringType.InvokeMember(
                    propertyName,
                    BindingFlags.GetProperty,
                    null,
                    invocation.Proxy,
                    new object[0]);
                var newValue = invocation.Arguments[0];

                if (Equals(oldValue, newValue))
                {
                    return;
                }

                invocation.Proceed();

                _subscribers(invocation.InvocationTarget, new PropertyChangedEventArgs(propertyName));
                return;
            }

            invocation.Proceed();
        }
    }
}
