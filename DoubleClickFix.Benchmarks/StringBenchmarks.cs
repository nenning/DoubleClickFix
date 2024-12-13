using BenchmarkDotNet.Attributes;

namespace DoubleClickFix.Benchmarks
{
    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.SlowestToFastest)]
    public class StringBenchmarks
    {
      //  string test = $"{ignoredDoubleClickText} ({buttonTextLookup[button]}): {timeDifference} ms (#{ignoredClicks})";

    }
}
