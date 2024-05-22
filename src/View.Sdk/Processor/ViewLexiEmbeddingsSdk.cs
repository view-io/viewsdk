﻿namespace View.Sdk.Processor
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using RestWrapper;
    using View.Serializer;
    using View.Sdk.Shared.Embeddings;
    using View.Sdk.Shared.Processing;
    using View.Sdk.Shared.Search;
    using View.Sdk.Shared.Udr;

    /// <summary>
    /// View SDK for generating embeddings with Lexi search results.
    /// </summary>
    public class ViewLexiEmbeddingsSdk
    {
        #region Public-Members

        /// <summary>
        /// Method to invoke to send log messages.
        /// </summary>
        public Action<string> Logger { get; set; } = null;
         
        #endregion

        #region Private-Members

        private string _Header = "[ViewLexiEmbeddingsSdk] ";
        private Uri _Uri = null;
        private string _Endpoint = "http://localhost:8501/lexi/embeddings";
        private SerializationHelper _Serializer = new SerializationHelper();
        
        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        /// <param name="endpoint">Endpoint URL.</param>
        public ViewLexiEmbeddingsSdk(string endpoint = "http://localhost:8501/lexi/embeddings")
        {
            if (string.IsNullOrEmpty(endpoint)) throw new ArgumentNullException(nameof(endpoint));

            _Uri = new Uri(endpoint);
            _Endpoint = _Uri.ToString();
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Validate connectivity.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Boolean indicating success.</returns>
        public async Task<bool> ValidateConnectivity(CancellationToken token = default)
        {
            string url = _Endpoint;

            using (RestRequest req = new RestRequest(url))
            {
                using (RestResponse resp = await req.SendAsync(token).ConfigureAwait(false))
                {
                    if (resp != null && resp.StatusCode == 200)
                    {
                        Log("success reported from " + url);
                        return true;
                    }
                    else if (resp != null)
                    {
                        Log("non-success reported from " + url + ": " + resp.StatusCode);
                        return false;
                    }
                    else
                    {
                        Log("no response from " + url);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Process a document.
        /// </summary>
        /// <param name="results">Search results.</param>
        /// <param name="embedRule">Embeddings rule.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task<LexiEmbeddingsResponse> Process(
            View.Sdk.Shared.Search.SearchResult results, 
            EmbeddingsRule embedRule, 
            CancellationToken token = default)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            if (embedRule == null) throw new ArgumentNullException(nameof(embedRule));

            string url = _Endpoint;

            using (RestRequest req = new RestRequest(url, HttpMethod.Post))
            {
                req.ContentType = Constants.JsonContentType;

                LexiEmbeddingsRequest procReq = new LexiEmbeddingsRequest
                {
                    Results = results,
                    EmbeddingsRule = embedRule
                };

                using (RestResponse resp = await req.SendAsync(_Serializer.SerializeJson(procReq, true), token).ConfigureAwait(false))
                {
                    if (resp != null && resp.StatusCode >= 200 && resp.StatusCode <= 299)
                    {
                        Log("success status from " + url + ": " + resp.StatusCode);
                        LexiEmbeddingsResponse procResp = _Serializer.DeserializeJson<LexiEmbeddingsResponse>(resp.DataAsString);
                        return procResp;
                    }
                    else if (resp != null)
                    {
                        Log("non-success status from " + url + ": " + resp.StatusCode + Environment.NewLine + resp.DataAsString);
                        LexiEmbeddingsResponse procResp = _Serializer.DeserializeJson<LexiEmbeddingsResponse>(resp.DataAsString);
                        return procResp;
                    }
                    else
                    {
                        Log("no response from " + url);
                        return null;
                    }
                }
            }
        }

        #endregion

        #region Private-Methods

        private void Log(string msg)
        {
            if (String.IsNullOrEmpty(msg)) return;
            Logger?.Invoke(_Header + msg);
        }

        #endregion
    }
}