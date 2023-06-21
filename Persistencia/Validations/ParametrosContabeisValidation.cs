using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Persistencia.Validations
{
    public class ParametrosContabeisValidation
    {
        public sealed class ValidaMascara : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                object instance = validationContext.ObjectInstance;
                Type type = instance.GetType();
                PropertyInfo property = type.GetProperty("UtilizaPlanoAns");
                object propertyValue = property.GetValue(instance);

                var niveis = 1;

                for (var i = 1; i < Convert.ToString(value).Length; i++)
                {
                    if (Convert.ToString(value)[i] == '.')
                    {
                        niveis++;
                    }
                }
                if (niveis == 1 && Convert.ToInt16(propertyValue) == 0)
                {
                    return new ValidationResult("Mascara do plano de contas incorreta.");

                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }
    }
}
