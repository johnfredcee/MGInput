﻿namespace MGInput
{
    /// <summary>
    /// Represents a pair of lowercase and uppercase symbols.
    /// </summary>
    public class InputSymbol
    {
        /// <summary>
        /// Gets the lowercase symbol.
        /// </summary>
        public string Lowercase { get; private set; }
        /// <summary>
        /// Gets the uppercase symbol.
        /// </summary>
        public string Uppercase { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="InputSymbol"/>.
        /// </summary>
        /// <param name="lowercase">Lowercase symbol of the pair.</param>
        /// <param name="uppercase">Uppercase symbol of the pair.</param>
        public InputSymbol(string lowercase, string uppercase = null)
        {
            Lowercase = lowercase;
            Uppercase = uppercase;
        }
    }
}
