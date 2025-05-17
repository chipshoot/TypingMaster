using Serilog;
using System.Collections.Immutable;


namespace TypingMaster.Core.Utility
{
    /// <summary>
    /// An immutable implementation of ProcessResult that provides type-safe success results.
    /// </summary>
    /// <typeparam name="T">The type of successful result value.</typeparam>
    public sealed class ProcessResult2<T>
    {
        // Private constructor prevents direct instantiation
        private ProcessResult2(
            T? value,
            ProcessResultStatus status,
            ImmutableList<string> informationList,
            string errorMessage,
            string? callStack)
        {
            Value = value;
            Status = status;
            InformationList = informationList;
            ErrorMessage = errorMessage;
            CallStack = callStack ?? string.Empty;
        }

        // Properties are read-only to ensure immutability
        public T? Value { get; }

        public ProcessResultStatus Status { get; }
        public IEnumerable<string> InformationList { get; }
        public string ErrorMessage { get; }
        public string CallStack { get; }
        public bool HasErrors => Status == ProcessResultStatus.Failure || !string.IsNullOrEmpty(ErrorMessage);
        public bool IsSuccess => Status == ProcessResultStatus.Success && string.IsNullOrEmpty(ErrorMessage);

        #region Factory Methods

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        public static ProcessResult2<T> Success(T value)
        {
            return new ProcessResult2<T>(
                value,
                ProcessResultStatus.Success,
                ImmutableList<string>.Empty,
                string.Empty,
                string.Empty);
        }

        /// <summary>
        /// Creates a successful result with a value and information message.
        /// </summary>
        public static ProcessResult2<T> Success(T value, string information)
        {
            return new ProcessResult2<T>(
                value,
                ProcessResultStatus.Success,
                ImmutableList.Create(information),
                string.Empty,
                string.Empty);
        }

        /// <summary>
        /// Creates a failure result with an error message.
        /// </summary>
        public static ProcessResult2<T> Failure(string errorMessage)
        {
            return new ProcessResult2<T>(
                default,
                ProcessResultStatus.Failure,
                ImmutableList<string>.Empty,
                errorMessage,
                string.Empty);
        }

        /// <summary>
        /// Creates a failure result from an exception.
        /// </summary>
        public static ProcessResult2<T> FromException(Exception exception)
        {
            return new ProcessResult2<T>(
                default,
                ProcessResultStatus.Failure,
                ImmutableList<string>.Empty,
                exception.Message,
                exception.StackTrace ?? string.Empty);
        }

        /// <summary>
        /// Creates an invalid data result.
        /// </summary>
        public static ProcessResult2<T> InvalidData(string errorMessage)
        {
            return new ProcessResult2<T>(
                default,
                ProcessResultStatus.InvalidData,
                ImmutableList<string>.Empty,
                errorMessage,
                string.Empty);
        }

        #endregion Factory Methods

        #region Transformation Methods

        /// <summary>
        /// Creates a new result with the added information message.
        /// </summary>
        public ProcessResult2<T> WithInformation(string information)
        {
            return new ProcessResult2<T>(
                Value,
                Status,
                ((ImmutableList<string>)InformationList).Add(information),
                ErrorMessage,
                CallStack);
        }

        /// <summary>
        /// Creates a new result with multiple information messages.
        /// </summary>
        public ProcessResult2<T> WithInformation(IEnumerable<string> information)
        {
            return new ProcessResult2<T>(
                Value,
                Status,
                ((ImmutableList<string>)InformationList).AddRange(information),
                ErrorMessage,
                CallStack);
        }

        /// <summary>
        /// Creates a new result with the updated error message.
        /// </summary>
        public ProcessResult2<T> WithError(string error)
        {
            return new ProcessResult2<T>(
                default,
                ProcessResultStatus.Failure,
                (ImmutableList<string>)InformationList,
                error,
                CallStack);
        }

        /// <summary>
        /// Combines this result with another result, propagating errors if present.
        /// </summary>
        public ProcessResult2<T> Combine(ProcessResult2<T> other)
        {
            // If either has an error, propagate the error
            if (HasErrors || other.HasErrors)
            {
                var combinedError = string.IsNullOrEmpty(ErrorMessage)
                    ? other.ErrorMessage
                    : string.IsNullOrEmpty(other.ErrorMessage)
                        ? ErrorMessage
                        : $"{ErrorMessage}; {other.ErrorMessage}";

                return new ProcessResult2<T>(
                    default,
                    ProcessResultStatus.Failure,
                    ((ImmutableList<string>)InformationList).AddRange(other.InformationList),
                    combinedError,
                    !string.IsNullOrEmpty(CallStack) ? CallStack : other.CallStack);
            }

            // Otherwise, keep the current value and combine information
            return new ProcessResult2<T>(
                Value,
                Status,
                ((ImmutableList<string>)InformationList).AddRange(other.InformationList),
                string.Empty,
                string.Empty);
        }

        /// <summary>
        /// Transform the result to a different type if successful.
        /// </summary>
        public ProcessResult2<TResult> Map<TResult>(Func<T, TResult> mapper)

        {
            if (HasErrors)
            {
                return new ProcessResult2<TResult>(
                    default,
                    Status,
                    (ImmutableList<string>)InformationList,
                    ErrorMessage,
                    CallStack);
            }

            try
            {
                var mappedValue = Value != null ? mapper(Value) : default;
                return new ProcessResult2<TResult>(
                    mappedValue,
                    Status,
                    (ImmutableList<string>)InformationList,
                    string.Empty,
                    string.Empty);
            }
            catch (Exception ex)
            {
                return ProcessResult2<TResult>.FromException(ex);
            }
        }

        /// <summary>
        /// Bind this result to a function that returns another result.
        /// </summary>
        public ProcessResult2<TResult> Bind<TResult>(Func<T, ProcessResult2<TResult>> binder)
        {
            if (HasErrors)
            {
                return new ProcessResult2<TResult>(
                    default,
                    Status,
                    (ImmutableList<string>)InformationList,
                    ErrorMessage,
                    CallStack);
            }

            try
            {
                return Value != null
                    ? binder(Value)
                    : new ProcessResult2<TResult>(
                        default,
                        Status,
                        (ImmutableList<string>)InformationList,
                        string.Empty,
                        string.Empty);
            }
            catch (Exception ex)
            {
                return ProcessResult2<TResult>.FromException(ex);
            }
        }

        #endregion Transformation Methods

        #region Logging Methods

        /// <summary>
        /// Logs the result to the provided logger.
        /// </summary>
        public ProcessResult2<T> LogResult(ILogger? logger)
        {
            if (logger == null)
                return this;

            // Log information messages
            if (InformationList.Any())
            {
                logger.Information("{@InformationList}", InformationList);
            }

            // Log error message if present
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                logger.Error("{@ErrorMessage}", ErrorMessage);
                if (!string.IsNullOrEmpty(CallStack))
                {
                    logger.Debug("{@CallStack}", CallStack);
                }
            }

            return this;
        }

        #endregion Logging Methods
    }

    /// <summary>
    /// Static factory class to create result objects without specifying type parameter.
    /// </summary>
    public static class ProcessResult2
    {
        /// <summary>
        /// Creates a successful result with no value.
        /// </summary>
        public static ProcessResult2<Nothing> Success()
        {
            return ProcessResult2<Nothing>.Success(new Nothing());
        }

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        public static ProcessResult2<T> Success<T>(T value)
        {
            return ProcessResult2<T>.Success(value);
        }

        /// <summary>
        /// Creates a successful result with a value and information message.
        /// </summary>
        public static ProcessResult2<T> Success<T>(T value, string information)
        {
            return ProcessResult2<T>.Success(value).WithInformation(information);
        }

        /// <summary>
        /// Creates a failure result with an error message.
        /// </summary>
        public static ProcessResult2<T> Failure<T>(string errorMessage)
        {
            return ProcessResult2<T>.Failure(errorMessage);
        }

        /// <summary>
        /// Creates a failure result from an exception.
        /// </summary>
        public static ProcessResult2<T> FromException<T>(Exception exception)
        {
            return ProcessResult2<T>.FromException(exception);
        }

        /// <summary>
        /// Creates an invalid data result.
        /// </summary>
        public static ProcessResult2<T> InvalidData<T>(string errorMessage)
        {
            return ProcessResult2<T>.InvalidData(errorMessage);
        }

        /// <summary>
        /// Marker type for when no result value is needed.
        /// </summary>
        public sealed class Nothing
        {
            internal Nothing()
            { }
        }
    }
}