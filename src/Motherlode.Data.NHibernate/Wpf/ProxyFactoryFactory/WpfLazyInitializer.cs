using System;
using System.ComponentModel;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Proxy.Poco;
using NHibernate.Type;

namespace Motherlode.Data.NHibernate.Wpf.ProxyFactoryFactory
{
    [Serializable]
    public class WpfLazyInitializer : BasicLazyInitializer, IInterceptor
    {
        #region Constants and Fields

        [NonSerialized]
        private static readonly MethodInfo _exceptionInternalPreserveStackTrace =
            typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

        [NonSerialized]
        private bool _initialized;

        [NonSerialized]
        private PropertyChangedEventHandler _subscribers = delegate { };

        #endregion

        #region Constructors and Destructors

        public WpfLazyInitializer(
            string entityName,
            Type persistentClass,
            object id,
            MethodInfo getIdentifierMethod,
            MethodInfo setIdentifierMethod,
            IAbstractComponentType componentIdType,
            ISessionImplementor session)
            : base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session)
        {
        }

        #endregion

        #region Public Properties

        public object ProxyInstance { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Initialize()
        {
            base.Initialize();

            var notifyPropertyChanged = this.Target as INotifyPropertyChanged;

            if (notifyPropertyChanged != null && !this._initialized)
            {
                // We subscribe to our Target's property changed event so we can pass it on
                // to any objects that subscribe to the proxies event.
                notifyPropertyChanged.PropertyChanged += this.targetPropertyChanged;
                this._initialized = true;
            }
        }

        public object Intercept(InvocationInfo info)
        {
            if (info.TargetMethod.DeclaringType == typeof(INotifyPropertyChanged))
            {
                var propertyChangedEventHandler = (PropertyChangedEventHandler)info.Arguments[0];
                if (info.TargetMethod.Name.StartsWith("add_"))
                {
                    this._subscribers += propertyChangedEventHandler;
                }
                else
                {
                    this._subscribers -= propertyChangedEventHandler;
                }

                return null;
            }

            // DefaultLazyInitializer implementation
            object returnValue;
            try
            {
                returnValue = base.Invoke(info.TargetMethod, info.Arguments, info.Target);

                // Avoid invoking the actual implementation, if possible
                if (returnValue != InvokeImplementation)
                {
                    return returnValue;
                }

                returnValue = info.TargetMethod.Invoke(this.GetImplementation(), info.Arguments);
            }
            catch (TargetInvocationException ex)
            {
                _exceptionInternalPreserveStackTrace.Invoke(ex.InnerException, new Object[] { });
                throw ex.InnerException;
            }

            return returnValue;
        }

        #endregion

        #region Methods

        private void targetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this._subscribers(this.ProxyInstance, e);
        }

        #endregion
    }
}
