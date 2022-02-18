using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

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
        /*var result = */BlazorApp1.Helpers.RegexMatches(".", _str, 0);
        //return result; this is commented because the real method doesnt return, it runs on a task, so when running benchmark I change the code to no use the Task and return directly
        return "";
    }
}