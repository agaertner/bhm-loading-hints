using Newtonsoft.Json;

namespace Nekres.Loading_Screen_Hints.Services.Models {
    public class ModuleKnowledge {
        [JsonProperty("namespace")]
        public string ManifestNamespace { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
