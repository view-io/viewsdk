﻿namespace View.Sdk.Shared.Processing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Timestamps;
    using View.Sdk.Shared.Embeddings;
    using View.Sdk.Shared.Search;
    using View.Sdk.Shared.Udr;

    /// <summary>
    /// Lexi embeddings response.
    /// </summary>
    public class LexiEmbeddingsResponse
    {
        #region Public-Members

        /// <summary>
        /// Data flow request GUID.
        /// </summary>
        public string DataFlowRequestGUID { get; set; } = null;

        /// <summary>
        /// Boolean indicating success.
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Timestamps.
        /// </summary>
        public Timestamp Timestamp { get; set; } = new Timestamp();

        /// <summary>
        /// Error response, if any.
        /// </summary>
        public ApiErrorResponse Error { get; set; } = null;

        /// <summary>
        /// Embeddings document.
        /// </summary>
        public EmbeddingsDocument Vector { get; set; } = null;

        #endregion

        #region Private-Members

        private string _RequestGuid = Guid.NewGuid().ToString();

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public LexiEmbeddingsResponse()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
