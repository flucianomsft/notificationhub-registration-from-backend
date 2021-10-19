using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NotificationHubWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private NotificationHubClient hub;

        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        public RegisterController()
        {
            hub = Notifications.Instance.Hub;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, DeviceRegistration deviceUpdate)
        {
            Installation installation = new Installation();
            installation.InstallationId = id;
            installation.PushChannel = deviceUpdate.Handle;
            installation.Tags = deviceUpdate.Tags;
            // ESEMPI DI TAG: ->
            //  InstallationID: InstallationId
            //  User: giliguor@microsoft.com 
            //  IsMainDevice: true
            //  EmployeeType: PostOffice
            //  Location:Italy
            //  IsMobile: true


            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    installation.Platform = NotificationPlatform.Mpns;
                    break;
                case "wns":
                    installation.Platform = NotificationPlatform.Wns;
                    break;
                case "apns":
                    installation.Platform = NotificationPlatform.Apns;
                    break;
                case "fcm":
                    installation.Platform = NotificationPlatform.Fcm;
                    // ESEMPIO PER IMPOSTARE I NOTIFICATION TEMPLATE:
                    //installation.Templates.Add("defaultTemplate", new InstallationTemplate { Body = "{ \"data\" : {\"message\":\"$(message)\"}}" });
                    break;
                default:
                    return BadRequest();
            }


            // In the backend we can control if a user is allowed to add tags
            //installation.Tags = new List<string>(deviceUpdate.Tags);
            //installation.Tags.Add("username:" + username);

            await hub.CreateOrUpdateInstallationAsync(installation);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await hub.DeleteInstallationAsync(id);
            return Ok();
        }


    }
}
