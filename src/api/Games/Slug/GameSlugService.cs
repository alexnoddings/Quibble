using Quibble.Data.Entites.Games;

namespace Quibble.Games.Slug;

public class GameSlugService : IGameSlugService
{
    // a-zA-Z0-9 (62 chars)
    //      + 916,132,832 permutations for 5 chars (sufficient)
    //      - some characters look very similar (eg i/l)
    //      - potential confusion around capitals
    // Length = 5;
    // Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    // a-zA-Z0-9 (52 chars) (similar character sets (ijl, IJL, 1) and (o, O, 0) completely removed)
    //      + 380,204,032 permutations for 5 chars (sufficient)
    //      + no confusion about similar characters
    //      - potential confusion around capitals
    // Length = 5;
    // Alphabet = "abcdefghkmnpqrstuvwxyzABCDEFGHKMNPQRSTUVWXYZ23456789";

    // A-Z0-9 (36 chars) (not case-sensitive, a-z automatically capitalised)
    //      - 60,466,176 permutations for 5 chars (too low)
    //      + 2,176,782,336 permutations for 6 chars (sufficient)
    //      - requires 6 chars rather than 5
    //      - some characters look very similar (eg i/l)
    //      + no confusion around capitalisation
    // Length = 6;
    // Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";

    // A-Z0-9 (30 chars) (not case-sensitive, a-z automatically capitalised, similar character sets (IJL, 1) and (o, O, 0) completely removed)
    //      - 24,300,000 permutations for 5 chars (too low)
    //      + 729,000,000 permutations for 6 chars (sufficient)
    //      - requires 6 chars rather than 5
    //      + no confusion about similar characters
    //      + no confusion around capitalisation
    private const int Length = GameConstraints.Slug.Length;
    private const string Alphabet = "ABCDEFGHKMNPQRSTUVWZYZ23456789";

    public string CreateSlug()
    {
        Span<char> slug = stackalloc char[Length];
        Random.Shared.GetItems(Alphabet, slug);
        return slug.ToString();
    }
}
