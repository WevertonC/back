using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.Validations.DomainValidations
{
    public class DomainValidations
    {
        private readonly List<string> _errors;

        public DomainValidations()
        {
            _errors = new List<string>();
        }

        public void AddError(string error)
        {
            _errors.Add(error);
        }

        public ReadOnlyCollection<string> GetErrors()
        {
            return _errors.AsReadOnly();
        }

        public string GetErrorsAsString()
        {
            var builder = new StringBuilder();

            foreach (var error in _errors)
            {
                builder.Append($"{error}\n");
            }

            return builder.ToString();
        }

        public bool IsValid()
        {
            return _errors.Count == 0;
        }
    }
}
