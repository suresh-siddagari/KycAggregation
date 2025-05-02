using System.Text.Json.Serialization;

public class Email
{
    [JsonPropertyName("preferred")]
    public bool Preferred { get; set; }

    [JsonPropertyName("email_address")]
    public string EmailAddress { get; set; }
}