using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Motherlode.Data.DataAnnotation
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class FileExistsAttribute : ValidationAttribute
    {
        #region Public Methods and Operators

        public override bool IsValid(object value)
        {
            return File.Exists(value as string);
        }

        #endregion
    }
}
