namespace Quibble.Answers.Similarity;

internal abstract class SimilarityCalculator
{
    private readonly double _threshold;

    protected SimilarityCalculator(double threshold)
    {
        _threshold = threshold;
    }

    protected abstract double CalculateSimilarity(NormalisedAnswer a, NormalisedAnswer b);

    public bool AreSimilar(NormalisedAnswer a, NormalisedAnswer b)
    {
        var similarity = CalculateSimilarity(a, b);
        return similarity > _threshold;
    }
}
