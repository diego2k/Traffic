using Newtonsoft.Json;

public class Envelope
{
    [JsonProperty("content")]
    public string content { get; set; }

    [JsonProperty("type")]
    public string type { get; set; }

}