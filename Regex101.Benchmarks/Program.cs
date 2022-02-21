using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Regex101;

BenchmarkRunner.Run<RegexExecutor>();

[MemoryDiagnoser]
public class RegexExecutor
{
    private string _str;

    [GlobalSetup]
    public void Setup() => _str = "".PadLeft(20_000, 'a');

    [Benchmark]
    public string Option()
    {
        return Helpers.RegexMatches(".", _str, 0);
    }
}