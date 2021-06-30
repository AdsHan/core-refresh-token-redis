using System.Collections.Generic;
using System.Linq;

namespace RTO.Auth.API.Services
{
    public class ValidateResult
    {

        protected List<string> Errors = new List<string>();

        public string GetErrorsMessages()
        {
            return string.Concat(Errors.ToArray());
        }

        public bool ValidateOperation()
        {
            return !Errors.Any();
        }

        public void AddProcessingError(string error)
        {
            Errors.Add(error);
        }
    }
}
