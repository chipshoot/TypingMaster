using JetBrains.Annotations;

namespace TypingMaster.Core.Utility
{
    public class Guard
    {
        /// <summary>
        /// Ensures that the provided value is not null. Returns the value if not null;
        /// otherwise, throws an ArgumentNullException with the specified parameter name.
        /// <param name="value"> the value to check for null.</param>
        /// <param name="paramName"> the name of the parameter to use in the exception message if the value is null.</param>
        /// </summary>
        [ContractAnnotation("value:null => halt; value:notnull => notnull")]
        [return: System.Diagnostics.CodeAnalysis.NotNull]
        public static T AgainstNull<T>(T? value, string paramName) where T : class
        {
            return value ?? throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Ensures that the provided string is neither null, empty, nor composed only of white-space characters.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="paramName">The name of the parameter to use in the exception message if the string is invalid.</param>
        /// <returns>The original string if it is not null, empty, or white-space.</returns>
        [return: System.Diagnostics.CodeAnalysis.NotNull]
        public static string AgainstNullOrEmpty(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The value cannot be null or empty", paramName);
            }

            return value!;
        }
    }
}