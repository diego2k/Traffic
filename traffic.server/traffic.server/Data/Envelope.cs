using Newtonsoft.Json;

public class Envelope
{
    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

}