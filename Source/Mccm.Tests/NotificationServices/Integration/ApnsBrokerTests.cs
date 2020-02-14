using System;
using System.IO;
using System.Threading.Tasks;
using Mccm.NotificationService.Services;
using Mccm.NotificationService.Services.Implementation;
using Xunit;

namespace Mccm.Tests.NotificationServices.Integration
{
    public class ApnsBrokerTests
    {
        // Enter your values here.
        private const string BundleID = "";
        private const string DeviceToken = "";
        private const string KeyId = "";
        private const string TeamId = "";
        private const string ValidPrivateKeyPath = "";

        [Fact]
        public async Task SendNotificationOk()
        {
            var privateKey = File.ReadAllText(ValidPrivateKeyPath);
            using IApnsBroker apnsBroker = ApnsBroker.Create(KeyId, privateKey, TeamId, BundleID, true);

            var notification = apnsBroker.CreateNotification("push", "hello!");

            var result = await apnsBroker.Send(notification, DeviceToken, true);
            Assert.True(result.Success);
        }
    }
}
