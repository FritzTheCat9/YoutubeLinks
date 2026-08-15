namespace YoutubeLinks.Api.Data.ValueObjects;

public sealed class Title : ValueObject
{
    public string Value { get; private set; }

    private Title() { }

    public Title(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Title is required.");

        if (value.Length > 300)
            throw new ArgumentException("Title too long.");

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Title t) => t?.Value;

    public static implicit operator Title(string value) => value is null ? null : new Title(value);
}
