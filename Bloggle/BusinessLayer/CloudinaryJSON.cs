using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloggle.BusinessLayer
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class CloudinaryJSON
    {
        [JsonProperty("asset_id")]
        public string AssetId { get; set; }

        [JsonProperty("public_id")]
        public string PublicId { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("version_id")]
        public string VersionId { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("resource_type")]
        public string ResourceType { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("placeholder")]
        public bool Placeholder { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("secure_url")]
        public string SecureUrl { get; set; }

        [JsonProperty("asset_folder")]
        public string AssetFolder { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
    }


}
