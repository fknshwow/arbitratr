namespace ArbitratR.Results
{
    /// <summary>
    /// Provides global configuration options for error code formatting.
    /// </summary>
    public static class ErrorOptions
    {
        private static char _separator = '-';

        /// <summary>
        /// Gets or sets the separator character used in error codes.
        /// </summary>
        /// <remarks>
        /// The default separator is <c>'-'</c>. This can be configured during ArbitratR registration.
        /// </remarks>
        public static char Separator
        {
            get => _separator;
            set
            {
                if (char.IsWhiteSpace(value) || value == '\0')
                {
                    throw new ArgumentException("Separator cannot be null or empty.", nameof(value));
                }

                _separator = value;
            }
        }
    }
}
