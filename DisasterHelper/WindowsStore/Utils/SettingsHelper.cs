using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
namespace WindowsStore.Utils
{
    public class SettingsHelper : SettingsHelperCommon
    {
        #region Set
        public override void SetUsernameToSettings(string value)
        {
            SetValueToSettings(SettingsKeys.UsernameKey, value);
        }
        public override void SetPasswordToSettings(string value)
        {
            SetValueToSettings(SettingsKeys.PasswordKey, value);
        }
        public override void SetRememberMeToSettings(bool value)
        {
            if (value)
                SetValueToSettings(SettingsKeys.RememberMeKey, "true");
            else
                SetValueToSettings(SettingsKeys.RememberMeKey, "false");

        }
        #endregion

        #region Get
        public override string GetUsernameFromSettings()
        {
            return GetValueFromSettings(SettingsKeys.UsernameKey);
        }

        public override string GetPasswordFromSettings()
        {
            return GetValueFromSettings(SettingsKeys.PasswordKey);
        }

        public override bool GetRememberMeFromSettings()
        {
            var remember = GetValueFromSettings(SettingsKeys.RememberMeKey);
            if (remember != null && remember == "true")
                return true;
            return false;
        }
        #endregion

        #region Delete
        public override void DeleteUsernameFromSettings()
        {
            DeleteValueFromSettings(SettingsKeys.UsernameKey);
        }

        public override void DeletePasswordFromSettings()
        {
            DeleteValueFromSettings(SettingsKeys.PasswordKey);
        }

        public override void DeleteRememberMeFromSettings()
        {
            DeleteValueFromSettings(SettingsKeys.RememberMeKey);
        }
        #endregion

        #region Helpers
        private string GetValueFromSettings(string key)
        {
            var settings = ApplicationData.Current.LocalSettings.Values;
            if (settings.ContainsKey(key))
                return (string)settings[key];

            return null;
        }

        private void SetValueToSettings(string key, string value)
        {
            var settings = ApplicationData.Current.LocalSettings.Values;
            if (settings.ContainsKey(key))
                settings.Remove(key);

            settings.Add(new KeyValuePair<string, object>(key, value));
        }

        private void DeleteValueFromSettings(string key)
        {
            var settings = ApplicationData.Current.LocalSettings.Values;
            if (settings.ContainsKey(key))
                settings.Remove(key);
        }
        #endregion
    }
}
