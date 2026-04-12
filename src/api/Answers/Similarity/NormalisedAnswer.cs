using System.Buffers;

namespace Quibble.Answers.Similarity;

internal sealed record NormalisedAnswer(string Text, string[] Tokens)
{
    private static readonly NormalisedAnswer _empty = new("", []);

    public static NormalisedAnswer Normalise(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return _empty;

        // This is pretty memory inefficient as it allocates a new string per method,
        // but optimising this with spans isn't useful at a small scale
        var sanitised =
            input.Trim()
                .ToLowerInvariant()
                .Normalize()
                .StripPunctuationAndSymbols();

        var tokens = sanitised
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => !_stopWords.Contains(t))
            .ToArray();

        var normalisedTokens = string.Join(' ', tokens);
        return new NormalisedAnswer(normalisedTokens, tokens);
    }

    private static readonly SearchValues<string> _stopWords = SearchValues.Create(
        ["a", "an", "and", "as", "in", "is", "on", "or", "the", "to"],
        StringComparison.OrdinalIgnoreCase
    );
}

file static class StringStripExtensions
{
    public static string StripPunctuationAndSymbols(this string str)
    {
        Span<char> stripped = stackalloc char[str.Length];
        str.CopyTo(stripped);
        for (var i = 0; i < stripped.Length; i++)
        {
            var c = stripped[i];
            if (char.IsPunctuation(c) || char.IsSymbol(c))
                stripped[i] = ' ';
        }
        return new string(stripped);
    }
}
