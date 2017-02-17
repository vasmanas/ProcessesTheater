namespace ProcessesTheater.Core
{
    /// <summary>
    /// Enum for decision how to execute of sertain conditions.
    /// </summary>
    public enum ExecutionDecision
    {
        /// <summary>
        /// Continue execution without interuption.
        /// </summary>
        Continue = 1,

        /// <summary>
        /// Throw exception if condition is met.
        /// </summary>
        ThrowException = 2,

        /// <summary>
        /// Quit execution if condition is met.
        /// </summary>
        QuitExecution = 3,
    }
}