using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class SettingsKeys
    {
        public const string UsernameKey = "username";
        public const string PasswordKey = "password";
        public const string RememberMeKey = "rememberme";
    }

    public abstract class SettingsHelperCommon
    {
        public abstract string GetUsernameFromSettings();
        public abstract void SetUsernameToSettings(string value);
        public abstract void DeleteUsernameFromSettings();
        public abstract string GetPasswordFromSettings();
        public abstract void SetPasswordToSettings(string value);
        public abstract void DeletePasswordFromSettings();
        public abstract void SetRememberMeToSettings(bool value);
        public abstract bool GetRememberMeFromSettings();
        public abstract void DeleteRememberMeFromSettings();

        public void DeleteAll()
        {
            DeletePasswordFromSettings();
            DeleteUsernameFromSettings();
            DeleteRememberMeFromSettings();
        }
    }
}
