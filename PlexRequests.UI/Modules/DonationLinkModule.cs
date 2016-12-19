﻿using System;
using System.Threading.Tasks;
using Nancy;
using NLog;
using Ombi.Core;
using Ombi.Core.SettingModels;
using Ombi.Helpers;
using ISecurityExtensions = Ombi.Core.ISecurityExtensions;

namespace Ombi.UI.Modules
{
    public class DonationLinkModule : BaseAuthModule
    {
        public DonationLinkModule(ICacheProvider provider, ISettingsService<PlexRequestSettings> pr, ISecurityExtensions security) : base("customDonation", pr, security)
        {
            Cache = provider;
            Get["/", true] = async (x, ct) => await GetCustomDonationUrl(pr);
        }

        private ICacheProvider Cache { get; }

        private static Logger Log = LogManager.GetCurrentClassLogger();

        private async Task<Response> GetCustomDonationUrl(ISettingsService<PlexRequestSettings> pr)
        {
            PlexRequestSettings settings = await pr.GetSettingsAsync();
            try
            {
                if (settings.EnableCustomDonationUrl && Security.IsLoggedIn(Context))
                {
                    return Response.AsJson(new { url = settings.CustomDonationUrl, message = settings.CustomDonationMessage, enabled = true });
                }

                return Response.AsJson(new { enabled = false });
            }
            catch (Exception e)
            {
                Log.Warn("Exception Thrown when attempting to check the custom donation url");
                Log.Warn(e);
                return Response.AsJson(new { url = settings.CustomDonationUrl, message = settings.CustomDonationMessage });
            }
        }
    }
    
}
