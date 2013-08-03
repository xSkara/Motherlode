using System;
using System.ComponentModel;
using Motherlode.Data.NHibernate.Wpf.Interceptor.DynamicProxy;
using NHibernate.Proxy.DynamicProxy;

namespace Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory
{
    public class EditableObjectsFactory : ObjectFactoryBase
    {
        #region Constructors and Destructors

        public EditableObjectsFactory() : base(new[] { typeof(IEditableObject) })
        {
        }

        #endregion

        #region Methods

        protected override IInterceptor createInterceptor(Type type)
        {
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    "The objects being created with notifiable factory should not implement INPC interface.");
            }

            return new EditableObjectInterceptor(type);
        }

        #endregion
    }
}
