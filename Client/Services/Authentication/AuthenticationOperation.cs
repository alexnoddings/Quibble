using System.Collections.Generic;
using System.Linq;

namespace Quibble.Client.Services.Authentication
{
    public class AuthenticationOperation
    {
        public bool WasSuccessful { get; set; }

        public List<string>? Errors { get; set; }

        protected AuthenticationOperation(bool wasSuccessful, List<string>? errors)
        {
            WasSuccessful = wasSuccessful;
            Errors = errors;
        }

        public static AuthenticationOperation FromSuccess() =>
            new (true, null);

        public static AuthenticationOperation FromError(IEnumerable<string> errors) =>
            new (false, errors.ToList());
    }
}
