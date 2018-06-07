using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Authorization
{
    public class IdentityResultException : Exception
    {
        public IdentityResultException(string message, IEnumerable<string> errors) : base(message)
        {
            var errorsArray = errors.ToArray();
            for (var i = 0; i < errorsArray.Length; i++)
            {
                Data.Add($"Error {i + 1}", errorsArray[i]);
            }
        }
    }
}