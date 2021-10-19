using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationHubWebAPI
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString(
                "Endpoint=sb://demofelcu.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=Nhuv0l7nn0uIUwBwjKK3i4iq+LY9D+AlOk4bxJNNe9U=",
                                                                            "demofelcu1");
        }
    }
}
