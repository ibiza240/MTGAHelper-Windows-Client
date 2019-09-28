using System;
using System.Collections.Generic;
using System.Text;
using MTGAHelper.Lib.Config;

namespace MTGAHelper.Web.Models.Response.User
{
    public class UserPreferencesResponse
    {
        public Dictionary<UserPreferenceEnum, dynamic> UserPreferences { get; set; } = new Dictionary<UserPreferenceEnum, dynamic>
        {
            { UserPreferenceEnum.ThemeIsDark, true },
            { UserPreferenceEnum.CollectionSetsOrder, "NewestFirst" },
            { UserPreferenceEnum.LandsPickAll, false },
        };

        public UserPreferencesResponse(Dictionary<UserPreferenceEnum, dynamic> userPreferences)
        {
            UserPreferences = userPreferences;
        }
    }
}
