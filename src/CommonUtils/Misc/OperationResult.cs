namespace CommonUtils.Misc
{
    public struct OperationResult<T>
    {
        public T ReturnValue { get; private set; }
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }

        private OperationResult(bool isSuccess = true, string errorMessage = null, T returnValue = default)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ReturnValue = returnValue;
        }

        public static OperationResult<VoidValue> Success()
        {
            return OperationResult<VoidValue>.Success(VoidValue._);
        }

        public static OperationResult<T> Success(T returnValue)
        {
            return new OperationResult<T>(true, null, returnValue);
        }

        public static OperationResult<T> Failure(string errorMessage)
        {
            return new OperationResult<T>(false, errorMessage);
        }
    }
}