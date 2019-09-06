using Newtonsoft.Json;

namespace BlackCountryBot.Web.Models
{
    public class DefaultResponse<TValue>
    {
        [JsonProperty("value")]
        public TValue Value { get; set; }
    }
}
