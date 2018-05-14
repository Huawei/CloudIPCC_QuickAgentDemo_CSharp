using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using QuickAgent.Common;
using System.Xml;

namespace QuickAgent
{
    class ConfigHelper
    {
        public static string ConfigFileName
        {
            get { return Path.Combine(Path.GetDirectoryName(typeof(ConfigHelper).Assembly.Location), "Config.xml"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public AgentInfo AgentInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        public void Save()
        {
            var agentInfo = this.AgentInfo;
            var settings = this.Settings;

            XmlDocument xmlDoc = new XmlDocument();

            var rootNode = xmlDoc.CreateElement("configuration");
            //var agentInfoNode = xmlDoc.CreateElement("agentInfo");
            var settingsNode = xmlDoc.CreateElement("settings");

            //if (agentInfoNode != null)
            //{
            //    agentInfoNode.SetAttribute("agentId", agentInfo.AgentId);
            //    agentInfoNode.SetAttribute("phoneNumber", agentInfo.PhoneNumber);
            //}

            if (settingsNode != null)
            {
                foreach (var item in settings)
                {
                    var node = xmlDoc.CreateElement("setting");

                    node.SetAttribute("key", item.Key);
                    node.SetAttribute("value", item.Value);

                    settingsNode.AppendChild(node);
                }
            }

            //rootNode.AppendChild(agentInfoNode);
            rootNode.AppendChild(settingsNode);

            xmlDoc.AppendChild(rootNode);

            using (var stream = new FileStream(ConfigFileName, FileMode.Create, FileAccess.Write))
            {
                xmlDoc.Save(stream);
            }
        }

        public static ConfigHelper Load()
        {
            try
            {
                string fileName = ConfigFileName;

                if (!File.Exists(fileName))
                {
                    return null;
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);

                ConfigHelper config = new ConfigHelper();
                config.AgentInfo = new AgentInfo();
                config.Settings = new Dictionary<string, string>();

                var agentInfoNode = xmlDoc.SelectSingleNode("/configuration/agentInfo") as XmlElement;

                if (agentInfoNode != null)
                {
                    config.AgentInfo.AgentId = agentInfoNode.GetAttribute("agentId");
                    config.AgentInfo.PhoneNumber = agentInfoNode.GetAttribute("phoneNumber");
                }

                var settingNode = xmlDoc.SelectNodes("/configuration/settings/setting");

                if (settingNode != null && settingNode.Count > 0)
                {
                    foreach (XmlNode node in settingNode)
                    {
                        var node2 = node as XmlElement;

                        if (node2 != null)
                        {
                            var key = node2.GetAttribute("key");
                            var value = node2.GetAttribute("value");

                            if (key != null)
                            {
                                config.Settings[key] = value;
                            }
                        }
                    }
                }

                return config;
            }
            catch
            {
                throw new Exception("Load config failed. ");
            }
        }
    }
}
