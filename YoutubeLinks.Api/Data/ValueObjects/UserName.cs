namespace YoutubeLinks.Api.Data.ValueObjects;

public sealed class Username : ValueObject
{
    public string Value { get; private set; }

    private Username() { }

    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be empty.");

        if (value.Length < 3 || value.Length > 20)
            throw new ArgumentException("Username must be between 3 and 20 characters.");

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Username u) => u?.Value;

    public static implicit operator Username(string value) => value is null ? null : new Username(value);
}
