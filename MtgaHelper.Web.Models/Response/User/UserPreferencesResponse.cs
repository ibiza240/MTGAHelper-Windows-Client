using System;
using System.Collections.Generic;
using System.Text;
using MTGAHelper.Lib.Config;

namespace MTGAHelper.Web.Models.Response.User
{
    public class UserPreferencesResponse
    {
        public Dictionary<string, string> UserPreferences { get; set; } = new Dictionary<string, string>
        {
            { UserPreferenceEnum.ThemeIsDark.ToString(), "True" },
            { UserPreferenceEnum.CollectionSetsOrder.ToString(), "NewestFirst" },
            { UserPreferenceEnum.LandsPickAll.ToString(), "False" },
        };

        public UserPreferencesResponse(Dictionary<string, string> userPreferences)
        {
            UserPreferences = userPreferences;
        }
    }
}
