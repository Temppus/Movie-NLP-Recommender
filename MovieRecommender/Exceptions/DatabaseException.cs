using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Exceptions
{
    public class UserPreferenceException : Exception
    {
        public UserPreferenceException()
        {

        }

        public UserPreferenceException(string message) : base($"User preference exception: {message}")
        {

        }
    }
}