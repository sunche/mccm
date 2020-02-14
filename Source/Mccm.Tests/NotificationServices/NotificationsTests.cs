using System;
using System.Collections.Generic;
using System.Text;
using Mccm.NotificationService.Common;
using Mccm.NotificationService.Services.Implementation;
using Xunit;

namespace Mccm.Tests.NotificationServices
{
    public class NotificationsTests
    {
        /// <summary>
        /// Check fcm payload for device notification.
        /// </summary>
        [Fact]
        public void GetGetFcmPayloadConentToDeviceOk()
        {
            var notification = new FcmNotification("testBodyKey", "testBodyValue");
            var fcmPayload = notification.GetPayload("testTarget", false, true);

            var expectedPayload = "{" +
                "\"message\":{" +
                "\"notification\":{" +
                "\"body\":\"{\\\"testBodyKey\\\":\\\"testBodyValue\\\"}\"" +
                "}," +
                "\"token\":\"testTarget\"" +
                "}," +
                "\"validate_only\":true" +
                "}";

            Assert.Equal(expectedPayload, fcmPayload);
        }

        /// <summary>
        /// Check fcm payload fot topic notification.
        /// </summary>
        [Fact]
        public void GetGetFcmPayloadConentToTopicOk()
        {
            var notification = new FcmNotification("testBodyKey", "testBodyValue");
            var fcmPayload = notification.GetPayload("testTarget", true, true);

            var expectedPayload = "{" +
                "\"message\":{" +
                "\"notification\":{" +
                "\"body\":\"{\\\"testBodyKey\\\":\\\"testBodyValue\\\"}\"" +
                "}," +
                "\"topic\":\"testTarget\"" +
                "}," +
                "\"validate_only\":true" +
                "}";

            Assert.Equal(expectedPayload, fcmPayload);
        }
    }
}
