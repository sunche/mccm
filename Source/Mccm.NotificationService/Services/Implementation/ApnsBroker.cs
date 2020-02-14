using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Jose;
using Mccm.NotificationService.Common;
using Mccm.NotificationService.Contracts;
using Newtonsoft.Json;

namespace Mccm.NotificationService.Services.Implementation
{
    /// <summary>
    /// Apns broker(Apple service).
    /// https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CommunicatingwithAPNs.html#//apple_ref/doc/uid/TP40008194-CH11-SW1
    /// </summary>
    public class ApnsBroker : IApnsBroker
    {
        private const string Algorithm = "ES256";
        private const string ApnsServerTemplate = "https://api{0}.push.apple.com:{1}/3/device/";
        private const string ExpiredProviderTokenResean = "ExpiredProviderToken";

        // APNs provider authentication token has validity interval of one hour.
        // So takes 59 minutes.
        private const int TokenLifeTimeInSeconds = 3540;

        private readonly string apnsUri;

        private readonly string bundleId;

        private readonly HttpClient client;

        private readonly Http2CustomHandler handler;

        private readonly string keyId;

        private readonly object locker = new object();

        private readonly CngKey privateKey;

        private readonly string teamId;

        private volatile string token;

        private DateTimeOffset tokenCreationTime;

        private ApnsBroker(string keyId, string privateKey, string teamId, string bundleId, bool isTesting, int apnsPort)
        {
            this.keyId = keyId;
            this.teamId = teamId;
            this.bundleId = bundleId;
            handler = new Http2CustomHandler();
            client = new HttpClient(handler);
            apnsUri = string.Format(ApnsServerTemplate, isTesting ? ".development" : string.Empty, apnsPort);

            byte[] secretKeyBlob = Convert.FromBase64String(privateKey);
            this.privateKey = CngKey.Import(secretKeyBlob, CngKeyBlobFormat.Pkcs8PrivateBlob);
        }

        /// <summary>
        /// Create apns broker.
        /// </summary>
        /// <param name="keyId">Apple key Id.</param>
        /// <param name="privateKey">Private key</param>
        /// <param name="teamId">Team id</param>
        /// <param name="bundleId">Bundle Id</param>
        /// <param name="isTesting">Is it testing notification</param>
        /// <param name="apnsPort">Apns port.</param>
        /// <returns><see cref="IApnsBroker"/>.</returns>
        public static IApnsBroker Create(string keyId, string privateKey, string teamId, string bundleId, bool isTesting = false, int apnsPort = 443)
        {
            return new ApnsBroker(keyId, privateKey, teamId, bundleId, isTesting, apnsPort);
        }

        /// <inheritdoc/>
        public ApnsNotification CreateNotification(string key, string value)
        {
            return new ApnsNotification(key, value);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            handler?.Dispose();
            client?.Dispose();
        }

        /// <inheritdoc/>
        public async Task<NotificationResult> Send(ApnsNotification notification, string deviceToken, bool isVoip)
        {
            var hostUri = $"{apnsUri}{deviceToken}";
            var payload = notification.GetPayload();
            var counter = 0;

            NotificationResult result = NotificationResult.Successful();

            while (counter < 5)
            {
                HttpResponseMessage response = await SendRequest(isVoip, payload, hostUri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return NotificationResult.Successful();
                }

                result = await GetErrorResult(response);
                if (result.Error.Code != NotificationErrorCode.ExpiredAccessToken)
                {
                    return result;
                }

                RefreshToken(DateTimeOffset.Now, true);
                counter++;
            }

            return result;
        }

        private static NotificationErrorCode GetErrorCodeFromResponse(HttpResponseMessage response, ApnsError apnsError)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.RequestEntityTooLarge:
                    return NotificationErrorCode.BadRequest;
                case HttpStatusCode.Forbidden:
                    if (apnsError?.Reason.Equals(ExpiredProviderTokenResean) == true)
                    {
                        return NotificationErrorCode.ExpiredAccessToken;
                    }
                    else
                    {
                        return NotificationErrorCode.AccessDenied;
                    }
                case HttpStatusCode.Gone:
                    return NotificationErrorCode.InvalidDeviceToken;
                default:
                    return NotificationErrorCode.Unspecified;
            }
        }

        private static ApnsError TryGetApnsError(string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<ApnsError>(content);
            }
            catch (JsonSerializationException)
            {
                return null;
            }
        }

        private string GetAccessToken()
        {
            DateTime currentTime = DateTime.Now;

            if (HasTokenExpired(currentTime))
            {
                RefreshToken(currentTime);
            }

            return token;
        }

        private async Task<NotificationResult> GetErrorResult(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            ApnsError apnsError = TryGetApnsError(content);

            NotificationErrorCode errorCode = GetErrorCodeFromResponse(response, apnsError);

            return NotificationResult.WithError(errorCode, $"{response.StatusCode}:{content}");
        }

        private bool HasTokenExpired(DateTimeOffset currentTime)
        {
            return (currentTime - tokenCreationTime).TotalSeconds >= TokenLifeTimeInSeconds;
        }

        private void RefreshToken(DateTimeOffset currentTime, bool forceRefresh = false)
        {
            lock (locker)
            {
                if (!forceRefresh && !HasTokenExpired(currentTime))
                {
                    return;
                }

                var payload = new Dictionary<string, object>()
                {
                    { "iss", teamId },
                    { "iat", currentTime.ToUnixTimeSeconds() }
                };

                var header = new Dictionary<string, object>()
                {
                    { "alg", Algorithm },
                    { "kid", keyId }
                };

                token = JWT.Encode(payload, privateKey, JwsAlgorithm.ES256, header);
                tokenCreationTime = currentTime;
            }
        }

        private async Task<HttpResponseMessage> SendRequest(bool isVoip, string payload, string hostUri)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, hostUri);
            string accessToken = GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            request.Headers.Add("apns-expiration", "0");
            request.Headers.Add("apns-topic", string.Format("{0}{1}", bundleId, isVoip ? ".voip" : string.Empty));
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            return await client.SendAsync(request);
        }
    }
}
