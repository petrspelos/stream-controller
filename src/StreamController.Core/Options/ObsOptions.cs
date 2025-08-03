namespace StreamController.Core.Options;

public class ObsOptions
{
    public string Url { get; init; } = "ws://localhost:4455";

    public string Password { get; init; } = string.Empty;
}
