using System.Text.RegularExpressions;

namespace YoutubeLinks.Api.Data.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; private set; }

    private Email() { } // EF Core

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");

        var regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(value, regex))
            throw new ArgumentException("Invalid email format.");

        Value = value.Trim().ToLower();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email e) => e?.Value;

    public static implicit operator Email(string value) => value is null ? null : new Email(value);
}