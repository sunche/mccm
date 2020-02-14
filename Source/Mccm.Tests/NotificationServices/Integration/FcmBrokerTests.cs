using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mccm.NotificationService.Common;
using Mccm.NotificationService.Services.Implementation;
using Xunit;

namespace Mccm.Tests.NotificationServices.Integration
{
    public class FcmBrokerTests
    {
        private const string ExpiredDeviceToken = "";
        private const string FcmServerKey = "";
        private const string InvalidFireBaseKeyPath = "";
        private const string ValidDeviceToken = "";
        private const string ValidFireBaseKeyPath = "";

        [Fact]
        public async Task RegisterOnTopicAndSendToTopicOk()
        {
            var fireBaseKey = File.ReadAllText(ValidFireBaseKeyPath);
            var broker = FcmBroker.Create(fireBaseKey, true);
            var topicName = "statistic";
            var result = await broker.RegisterToTopic(ValidDeviceToken, topicName, FcmServerKey);

            Assert.True(result.Success);

            var notification = broker.CreateNotification("test notification", "type", "statistic");
            result = await broker.SendToTopic(notification, topicName);

            Assert.True(result.Success);
        }

        /// <summary>
        /// Check sending to device.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SendNotificationToDeviceOk()
        {
            var fireBaseKey = File.ReadAllText(ValidFireBaseKeyPath);
            using var broker = FcmBroker.Create(fireBaseKey, true);
            var message = broker.CreateNotification("test notification", "type", "statistic");
            var result = await broker.Send(message, ValidDeviceToken);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task SendNotificationWithAuthTokenReturnAccessDenied()
        {
            var fireBaseKey = File.ReadAllText(InvalidFireBaseKeyPath);
            using var broker = FcmBroker.Create(fireBaseKey, true);
            var message = broker.CreateNotification("test notification", "type", "statistic");
            var result = await broker.Send(message, ValidDeviceToken);
            Assert.Equal(NotificationErrorCode.AccessDenied, result.Error.Code);
        }

        [Fact]
        public async Task SendNotificationWithInvalidDeviceTokenReturnInvalidDeviceTokenError()
        {
            var fireBaseKey = File.ReadAllText(ValidFireBaseKeyPath);
            using var broker = FcmBroker.Create(fireBaseKey, true);
            var message = broker.CreateNotification("test notification", "type", "statistic");
            var result = await broker.Send(message, ExpiredDeviceToken);
            Assert.Equal(NotificationErrorCode.InvalidDeviceToken, result.Error.Code);
        }
    }
}
