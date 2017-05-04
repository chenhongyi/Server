using System.Net.Http.Formatting;
using Newtonsoft.Json;

namespace PublicGate.App_Start
{
    public static class FormatterConfig
    {
        public static void ConfigureFormatters(MediaTypeFormatterCollection formatters)
        {
            formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.None;
        }
    }
}
