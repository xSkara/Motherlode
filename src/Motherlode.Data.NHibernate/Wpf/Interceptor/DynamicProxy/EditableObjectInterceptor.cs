using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory;
using NHibernate.Proxy.DynamicProxy;

namespace Motherlode.Data.NHibernate.Wpf.Interceptor.DynamicProxy
{
    public class EditableObjectInterceptor : IInterceptor
    {
        #region Constants and Fields

        private readonly Dictionary<string, object> _editedPropertyValues = new Dictionary<string, object>();
        private readonly object _instance;
        private readonly Type _type;
        private bool _isEditing;

        #endregion

        #region Constructors and Destructors

        public EditableObjectInterceptor(Type type)
        {
            this._type = type;
            this._instance = Activator.CreateInstance(type);
        }

        public EditableObjectInterceptor(object instance)
        {
            this._type = instance.GetType();
            this._instance = instance;
        }

        #endregion

        #region Public Methods and Operators

        public object Intercept(InvocationInfo info)
        {
            if (info.TargetMethod.DeclaringType == typeof(ObjectFactoryBase.ITypeInfo))
            {
                return this._type.FullName;
            }

            if (info.TargetMethod.DeclaringType == typeof(IEditableObject))
            {
                if (info.TargetMethod.Name == "BeginEdit")
                {
                    this._isEditing = true;
                }
                else if (info.TargetMethod.Name == "CancelEdit")
                {
                    this._isEditing = false;
                    this._editedPropertyValues.Clear();
                }
                else if (info.TargetMethod.Name == "EndEdit")
                {
                    this._isEditing = false;
                    this.assignEditedPropertyValues();
                    this._editedPropertyValues.Clear();
                }

                return null;
            }

            object result;

            if (this._isEditing && info.TargetMethod.IsSpecialName)
            {
                if (info.TargetMethod.Name.StartsWith("set_"))
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

                    this._editedPropertyValues[propertyName] = newValue;

                    return null;
                }

                if (info.TargetMethod.Name.StartsWith("get_"))
                {
                    string propertyName = info.TargetMethod.Name.Substring(4);

                    if (this._editedPropertyValues.TryGetValue(propertyName, out result))
                    {
                        return result;
                    }
                }
            }

            result = info.TargetMethod.Invoke(this._instance, info.Arguments);
            return result;
        }

        #endregion

        #region Methods

        private void assignEditedPropertyValues()
        {
            foreach (var property in this._editedPropertyValues)
            {
                this._type.InvokeMember(property.Key, BindingFlags.SetProperty, null, this._instance, new[] { property.Value });
            }
        }

        #endregion
    }
}
