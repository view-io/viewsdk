﻿namespace View.Sdk.Vector
{
    using System;
    using System.Collections.Generic;
    using Timestamps;

    /// <summary>
    /// Vector query result.
    /// </summary>
    public class VectorQueryResult
    {
        #region Public-Members

        /// <summary>
        /// Boolean indicating whether or not the operation was successful.
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// Timestamps.
        /// </summary>
        public Timestamp Timestamp { get; set; } = new Timestamp();

        /// <summary>
        /// HTTP status code.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return _StatusCode;
            }
            set
            {
                if (value < 0 || value > 599) throw new ArgumentOutOfRangeException(nameof(StatusCode));
                _StatusCode = value;
            }
        }

        /// <summary>
        /// Query.
        /// </summary>
        public string Query { get; set; } = null;

        /// <summary>
        /// Result.
        /// </summary>
        public object Result { get; set; } = null;

        #endregion

        #region Private-Members

        private int _StatusCode = 200;
        private List<decimal> _Embeddings = new List<decimal>();

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public VectorQueryResult()
        {
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
