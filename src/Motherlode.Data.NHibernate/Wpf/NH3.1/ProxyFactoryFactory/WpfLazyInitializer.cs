using System;
using System.ComponentModel;
using System.Reflection;
using Castle.DynamicProxy;
using NHibernate.ByteCode.Castle;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernateConcepts.Wpf.ProxyFactoryFactory
{
    public class WpfLazyInitializer : LazyInitializer
    {
        private PropertyChangedEventHandler _subscribers = delegate { };

        public WpfLazyInitializer(
            string entityName,
            Type persistentClass,
            object id,
            MethodInfo getIdentifierMethod,
            MethodInfo setIdentifierMethod,
            IAbstractComponentType aType,
            ISessionImplementor session)
            : base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, aType, session)
        {
        }

        public object ProxyInstance { get; set; }

        public override void Intercept(IInvocation invocation)
        {
            // WPF will call a method named add_PropertyChanged to subscribe itself to the property changed events of
            // the given entity. The method to call is stored in invocation.Arguments[0]. We get this and add it to the
            // proxy subscriber list.
            if (invocation.Method.Name.EndsWith("PropertyChanged"))
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
            }
            else
            {
                base.Intercept(invocation);
            }            
        }

        private bool initialized;

        public override void Initialize()
        {
            base.Initialize();

            var notifyPropertyChanged = Target as INotifyPropertyChanged;

            if (notifyPropertyChanged != null && !initialized)
            {
                // We subscribe to our Target's property changed event so we can pass it on
                // to any objects that subscribe to the proxies event.
                notifyPropertyChanged.PropertyChanged += targetPropertyChanged;
                initialized = true;
            }
        }

        private void targetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _subscribers(ProxyInstance, e);
        }
    }

    /*
// We override this class implementation to handle the proxy creation ourselves. We
// need to specify our own Interceptor when a method of our entity is called.
public class DataBindingNotifyPropertyProxyFactory : CastleProxyFactory
{
    public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
    {
        // If it is not a proxy for a class do what you usually did.
        if (!IsClassProxy) return base.GetProxy(id, session);
 
        try
        {               
            CastleLazyInitializer initializer = new DataBindingInterceptor(EntityName,PersistentClass, id,
                       GetIdentifierMethod, SetIdentifierMethod,ComponentIdType, session);
           
            // Add to the list of the interfaces that the proxy class will support the INotifyPropertyChanged interface.
            // This is only needed in the case when we need to cast our proxy object as INotifyPropertyChanged interface.
            ArrayList list = new ArrayList(Interfaces);
            list.Add(typeof(INotifyPropertyChanged));
            System.Type[] interfaces = (System.Type[])list.ToArray(typeof(System.Type));
 
            //We create the proxy (for the class, Interfaces Supported, Interceptor to use)
            object generatedProxy = DefaultProxyGenerator.CreateClassProxy(PersistentClass, interfaces, initializer);
 
            initializer._constructed = true;
            return (INHibernateProxy)generatedProxy;
        }
        catch (Exception e)
        {
            log.Error("Creating a proxy instance failed", e);
            throw new HibernateException("Creating a proxy instance failed", e);
        }
    }
}

public class DataBindingInterceptor : CastleLazyInitializer
 {
     private PropertyChangedEventHandler subscribers = delegate { };
 
     public DataBindingInterceptor(String EntityName,Type persistentClass, object id,MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,IAbstractComponentType aType, ISessionImplementor session)
         : base(EntityName,persistentClass, id, getIdentifierMethod, setIdentifierMethod,aType, session)
     {
     }
 
     public override void Intercept (IInvocation invocation)
     {
         // WPF will call a method named add_PropertyChanged to subscribe itself to the property changed events of
         // the given entity. The method to call is stored in invocation.Arguments[0]. We get this and add it to the
         // proxy subscriber list.
         if (invocation.Method.Name.Contains("PropertyChanged"))
         {
             PropertyChangedEventHandler propertyChangedEventHandler = (PropertyChangedEventHandler)invocation.Arguments[0];
             if (invocation.Method.Name.StartsWith("add_"))
             {
                 subscribers += propertyChangedEventHandler;
             }
             else
             {
                 subscribers -= propertyChangedEventHandler;
             }
         }
        
         // Here we call the actual method of the entity
         base.Intercept(invocation);
 
         // If the method that was called was actually a proeprty setter (set_Line1 for example) we generate the
         // PropertyChanged event for the property but with event generator the proxy. This must do the trick.
         if (invocation.Method.Name.StartsWith("set_"))
         {              
             subscribers(invocation.InvocationTarget, new PropertyChangedEventArgs(invocation.Method.Name.Substring(4)));
         }
     }
 }

    public class ExtendedWithNotifyProxyFactoryFactory : NHibernate.Bytecode.IProxyFactoryFactory
    {
        public IProxyFactory BuildProxyFactory()
        {
            return new DataBindingNotifyPropertyProxyFactory();
        }
    }
    */
    /*public class DataBindingFactory : ObjectFactoryBase
    {
        public DataBindingFactory() : base()
        {
            
        }



        public DataBindingFactory(bool entitiesImplementInpc, bool entitiesImplementIeo)
        {
            if (entitiesImplementInpc && entitiesImplementIeo)
            {
                _createInterceptors = t => new[] { new NotifyPropertyChangedInterceptor(t.FullName), };
                _interfacesToProxy = new[]
                    {
                        typeof(ITypeInfo),
                        typeof(INotifyPropertyChanged),
                        typeof(IEditableObject)
                    };
            }
            else if (entitiesImplementInpc)
            {
                _createInterceptors = t => new IInterceptor[] { new NotifyPropertyChangedInterceptor(t.FullName), };
                _interfacesToProxy = new[]
                    {
                        typeof(ITypeInfo),
                        typeof(INotifyPropertyChanged),
                    };
            }
            else if (entitiesImplementIeo)
            {
                _createInterceptors = t => { throw new NotImplementedException(); };
                _interfacesToProxy = new[]
                    {
                        typeof(ITypeInfo),
                        typeof(IEditableObject)
                    };
            }
            else
            {
                _createInterceptors = t => new IInterceptor[0];
                _interfacesToProxy = new[]
                    {
                        typeof(ITypeInfo)
                    };
            }
        }

        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }

        public object Create(Type type)
        {
            return _proxyGenerator.CreateClassProxy(type, _interfacesToProxy, _createInterceptors(type));
        }
    }*/
}
