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
    using View.Sdk;

    /// <summary>
    /// View Processing Pipeline SDK.
    /// </summary>
    public class ViewProcessorSdk : ViewSdkBase
    {
        #region Public-Members

        /// <summary>
        /// Enable or disable logging of request bodies.
        /// </summary>
        public bool LogRequests { get; set; } = false;

        /// <summary>
        /// Enable or disable logging of response bodies.
        /// </summary>
        public bool LogResponses { get; set; } = false;

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        /// <param name="endpoint">Endpoint URL.</param>
        public ViewProcessorSdk(string endpoint = "http://localhost:8501/processor") : base(endpoint)
        {
            if (string.IsNullOrEmpty(endpoint)) throw new ArgumentNullException(nameof(endpoint));

            Header = "[ViewProcessorSdk] ";
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Process a document.
        /// </summary>
        /// <param name="obj">Object metadata.</param>
        /// <param name="mdRule">Metadata rule.</param>
        /// <param name="embedRule">Embeddings rule.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task<ProcessorResponse> Process(
            ObjectMetadata obj, 
            MetadataRule mdRule, 
            EmbeddingsRule embedRule, 
            CancellationToken token = default)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (mdRule == null) throw new ArgumentNullException(nameof(mdRule));

            string url = Endpoint;

            try
            {
                using (RestRequest req = new RestRequest(url, HttpMethod.Post))
                {
                    req.ContentType = Constants.JsonContentType;

                    ProcessorRequest procReq = new ProcessorRequest
                    {
                        Object = obj,
                        MetadataRule = mdRule,
                        EmbeddingsRule = embedRule
                    };

                    string json = Serializer.SerializeJson(procReq, true);

                    if (LogRequests) Log(Severity.Debug, "request body: " + Environment.NewLine + json);

                    using (RestResponse resp = await req.SendAsync(json, token).ConfigureAwait(false))
                    {
                        if (resp != null)
                        {
                            if (resp.StatusCode >= 200 && resp.StatusCode <= 299)
                            {
                                Log(Severity.Debug, "success reported from " + url + ": " + resp.StatusCode + ", " + resp.ContentLength + " bytes");

                                if (!String.IsNullOrEmpty(resp.DataAsString))
                                {
                                    if (LogResponses) Log(Severity.Debug, "response body: " + Environment.NewLine + resp.DataAsString);

                                    ProcessorResponse procResp = Serializer.DeserializeJson<ProcessorResponse>(resp.DataAsString);
                                    return procResp;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                Log(Severity.Warn, "non-success reported from " + url + ": " + resp.StatusCode + ", " + resp.ContentLength + " bytes");

                                if (!String.IsNullOrEmpty(resp.DataAsString))
                                {
                                    if (LogResponses) Log(Severity.Debug, "response body: " + Environment.NewLine + resp.DataAsString);

                                    ProcessorResponse procResp = Serializer.DeserializeJson<ProcessorResponse>(resp.DataAsString);
                                    return procResp;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            Log(Severity.Warn, "no response from " + url);
                            return null;
                        }
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                Log(Severity.Warn, "exception while interacting with " + url + ": " + hre.Message);
                return new ProcessorResponse
                {
                    Success = false,
                    Error = new ApiErrorResponse(ApiErrorEnum.InternalError, null, null, hre)
                };
            }
        }

        #endregion

        #region Private-Methods
         
        #endregion
    }
}
