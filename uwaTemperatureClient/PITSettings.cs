using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uwaTemperatureClient
{
    public class PITSettings
    {
        public string Server { get; set; }

        public PITSettings()
        {
            Server = "";
        }
        public void Save()
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["serverURL"] = Server;
        }

        public void Load()
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("serverURL"))
                Server = roamingSettings.Values["serverURL"].ToString();
        }

        public Boolean CheckOk()
        {
            if (!CheckServerProperty())
                return false;
            return true;
        }

        private Boolean CheckServerProperty()
        {
            if (string.IsNullOrWhiteSpace(Server))
                return false;
            if (!Uri.IsWellFormedUriString(Server, UriKind.Absolute))
                return false;
            Uri tmp;
            if (!Uri.TryCreate(Server, UriKind.Absolute, out tmp))
                return false;
            return tmp.Scheme == "http" || tmp.Scheme == "https";
        }

    }
}
