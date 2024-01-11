using Newtonsoft.Json;

namespace Nekres.Loading_Screen_Hints.Services.Models {
    public class Quote {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
    }
}
