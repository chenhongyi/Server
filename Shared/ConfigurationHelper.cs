using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class ConfigurationHelper
    {
        public static string ReadValue(string sectionName, string parameterName)
        {
            CodePackageActivationContext context = FabricRuntime.GetActivationContext();
            ConfigurationSettings configSettings = context.GetConfigurationPackageObject("Config").Settings;
            ConfigurationSection configSection = configSettings.Sections.FirstOrDefault(s => (s.Name == sectionName));

            ConfigurationProperty configurationProperty = configSection?.Parameters.FirstOrDefault(p => (p.Name == parameterName));
            return configurationProperty?.Value;
        }
    }
}
