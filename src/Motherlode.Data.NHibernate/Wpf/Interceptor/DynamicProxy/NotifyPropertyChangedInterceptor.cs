using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory;
using NHibernate.Proxy.DynamicProxy;

namespace Motherlode.Data.NHibernate.Wpf.Interceptor.DynamicProxy
{
    internal class NotifyPropertyChangedInterceptor : IInterceptor
    {
        #region Constants and Fields

        private readonly Dictionary<string, string[]> _advancedDependencies;
        private readonly object _instance;
        private readonly string _typeName;

        private PropertyChangedEventHandler _subscribers = delegate { };

        #endregion

        // key = propertyName, value = names of the properties depending on the key property

        #region Constructors and Destructors

        public NotifyPropertyChangedInterceptor(Type type)
            : this(type, Activator.CreateInstance(type))
        {
        }

        public NotifyPropertyChangedInterceptor(object instance) : this(instance.GetType(), instance)
        {
        }

        private NotifyPropertyChangedInterceptor(Type type, object instance)
        {
            var advancedDependencies = new Dictionary<string, List<string>>();
            foreach (var p in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public))
            {
                foreach (var attr in p.GetCustomAttributes(true).OfType<DependsOnAttribute>())
                {
                    List<string> deps;
                    if (type.GetProperty(
                        attr.PropertyName,
                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public) == null)
                    {
                        throw new FormatException(
                            string.Format(
                                "The property {1} of type {0} has invalid DependsOn attribute pointing " +
                                "to the property {2} that does not exist.",
                                type.FullName,
                                p.Name,
                                attr.PropertyName));
                    }

                    if (advancedDependencies.TryGetValue(attr.PropertyName, out deps))
                    {
                        deps.Add(p.Name);
                    }
                    else
                    {
                        deps = new List<string>
                            {
                                p.Name
                            };
                        advancedDependencies[attr.PropertyName] = deps;
                    }
                }
            }

            this._advancedDependencies = advancedDependencies.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());

            this._typeName = type.FullName;
            this._instance = instance;
        }

        #endregion

        #region Public Methods and Operators

        public object Intercept(InvocationInfo info)
        {
            if (info.TargetMethod.DeclaringType == typeof(ObjectFactoryBase.ITypeInfo))
            {
                return this._typeName;
            }

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

            object result;

            if (info.TargetMethod.IsSpecialName && info.TargetMethod.Name.StartsWith("set_"))
            {
                string propertyName = info.TargetMethod.Name.Substring(4);

                object oldValue = info.TargetMethod.DeclaringType.InvokeMember(
                    propertyName,
                    BindingFlags.GetProperty,
                    null,
                    this._instance,
                    new object[0]);
                object newValue = info.Arguments[0];

                if (Equals(oldValue, newValue))
                {
                    return oldValue;
                }

                result = info.TargetMethod.Invoke(this._instance, info.Arguments);

                this._subscribers(info.Target, new PropertyChangedEventArgs(propertyName));

                string[] dependencies;
                if (this._advancedDependencies.TryGetValue(propertyName, out dependencies))
                {
                    foreach (string dependency in dependencies)
                    {
                        this._subscribers(info.Target, new PropertyChangedEventArgs(dependency));
                    }
                }

                return result;
            }

            result = info.TargetMethod.Invoke(this._instance, info.Arguments);
            return result;
        }

        #endregion
    }
}
