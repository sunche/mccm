using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Mccm.NotificationService.Common;
using Mccm.NotificationService.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mccm.NotificationService.Services.Implementation
{
    /// <summary>
    /// FirBase broker implementation.
    /// </summary>
    public class FcmBroker : IFcmBroker
    {
        private const string FcmUriTemplate = "https://fcm.googleapis.com/v1/projects/{0}/messages:send";
        private const string ProjectIdFireBaseKeyPropertyName = "project_id";
        private const string RegisterTopicUriTemplate = "https://iid.googleapis.com/iid/v1:batchAdd";
        private static readonly List<string> scopes = new List<string>(new string[] { "https://www.googleapis.com/auth/firebase.messaging" });

        private readonly HttpClient client;
        private readonly GoogleCredential googleCredential;
        private readonly bool isTestingMode;
        private readonly string notificationUri;

        private FcmBroker(string fireBaseKey, bool isTestingMode)
        {
            client = new HttpClient();
            notificationUri = string.Format(FcmUriTemplate, GetProjectId(fireBaseKey));
            googleCredential = GoogleCredential.FromJson(fireBaseKey).CreateScoped(scopes);

            this.isTestingMode = isTestingMode;
        }

        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="fireBaseKey">FireBase key</param>
        /// <param name="isTestingMode">Is testing mode for broker.</param>
        /// <returns></returns>
        public static IFcmBroker Create(string fireBaseKey, bool isTestingMode = false)
        {
            return new FcmBroker(fireBaseKey, isTestingMode);
        }

        /// <inheritdoc/>
        public FcmNotification CreateNotification(string key, object value)
        {
            return new FcmNotification(key, value);
        }

        /// <inheritdoc/>
        public FcmNotification CreateNotification(string title, string key, object value)
        {
            return new FcmNotification(title, key, value);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            client?.Dispose();
        }

        /// <inheritdoc/>
        public async Task<NotificationResult> RegisterToTopic(string deviceToken, string topic, string fcmServerKey)
        {
            string dryRunProperty = isTestingMode ? ",\"dry_run\":true" : string.Empty;
            var requestBody = $"{{\"to\": \"/topics/{topic}\",\"registration_tokens\":[\"{deviceToken}\"]" +
                $"{dryRunProperty}}}";
            return await SendRequest(requestBody, fcmServerKey, RegisterTopicUriTemplate, false);
        }

        /// <inheritdoc/>
        public async Task<NotificationResult> Send(FcmNotification notification, string deviceToken)
        {
            string requestBody = notification.GetPayload(deviceToken, false, isTestingMode);
            return await SendViaHttpV1(requestBody);
        }

        /// <inheritdoc/>
        public async Task<NotificationResult> SendToTopic(FcmNotification notification, string topic)
        {
            string requestBody = notification.GetPayload(topic, true, isTestingMode);
            return await SendViaHttpV1(requestBody);
        }

        private static NotificationErrorCode GetErrorCodeFromResponse(FcmResponse fcmResponse)
        {
            switch (fcmResponse?.Error.ErrorStatus)
            {
                case FcmErrorStatus.InvalidArgument:
                    return NotificationErrorCode.BadRequest;
                case FcmErrorStatus.Unauthenticated:
                    return NotificationErrorCode.AccessDenied;
                case FcmErrorStatus.NotFound:
                    return NotificationErrorCode.InvalidDeviceToken;
                default:
                    return NotificationErrorCode.Unspecified;
            }
        }

        private static async Task<NotificationResult> GetErrorResult(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            FcmResponse fcmResponse = TryGetFcmResponse(content);
            NotificationErrorCode errorCode = GetErrorCodeFromResponse(fcmResponse);

            return NotificationResult.WithError(errorCode, $"{response.StatusCode}:{fcmResponse?.Error.Message}; Details: {content}");
        }

        private static FcmResponse TryGetFcmResponse(string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<FcmResponse>(content);
            }
            catch (JsonSerializationException)
            {
                return null;
            }
        }

        private async Task<string> GetAccessToken()
        {
            var test = await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync(notificationUri);
            return test;
        }

        private string GetProjectId(string fireBaseKey)
        {
            return JObject.Parse(fireBaseKey).GetValue(ProjectIdFireBaseKeyPropertyName).ToString();
        }

        private async Task<NotificationResult> SendRequest(string requestBody, string authenticationKey, string uri, bool useHttpV1)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var headerAuthenticationKey = useHttpV1 ? "Bearer" : "key";
            var headerAuthenticationValue = useHttpV1 ? authenticationKey : $"={authenticationKey}";
            request.Headers.Authorization = new AuthenticationHeaderValue(headerAuthenticationKey, headerAuthenticationValue);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await GetErrorResult(response);
            }

            return NotificationResult.Successful();
        }

        private async Task<NotificationResult> SendViaHttpV1(string requestBody)
        {
            string token;
            try
            {
                token = await GetAccessToken();
            }
            catch (TokenResponseException e)
            {
                return NotificationResult.WithError(NotificationErrorCode.AccessDenied, e.Message);
            }

            return await SendRequest(requestBody, token, notificationUri, true);
        }
    }
}
