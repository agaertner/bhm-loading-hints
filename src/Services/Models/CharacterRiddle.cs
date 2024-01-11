using Blish_HUD.Content;
using Newtonsoft.Json;

namespace Nekres.Loading_Screen_Hints.Services.Models {
    public class CharacterRiddle {
        [JsonProperty("image")]
        public string ImageUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public AsyncTexture2D Image { get; set; }
    }
}
