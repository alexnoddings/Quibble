namespace Quibble.Answers.Similarity;

// Hamming is a really rubbish algo here, but it's incredibly easy and good enough for now
internal sealed class HammingSimilarityCalculator : SimilarityCalculator
{
    public HammingSimilarityCalculator(double threshold) : base(threshold)
    {
    }

    protected override double CalculateSimilarity(NormalisedAnswer a, NormalisedAnswer b)
    {
        var totalChars = Math.Max(a.Text.Length, b.Text.Length);
        if (totalChars == 0)
            return 1;

        var similarity =
            a.Text.Length >= b.Text.Length
                ? CalculateSimilarity(a.Text, b.Text)
                : CalculateSimilarity(b.Text, a.Text);

        return (double)similarity / totalChars;
    }

    private static int CalculateSimilarity(string longer, string shorter)
    {
        var similarity = 0;
        for (var i = 0; i < shorter.Length; i++)
        {
            if (longer[i] == shorter[i])
                similarity++;
        }
        return similarity;
    }
}
