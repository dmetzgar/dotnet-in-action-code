using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Newtonsoft.Json;
using STJ = System.Text.Json;

namespace FirstBenchmark;

[ShortRunJob]
[EventPipeProfiler(EventPipeProfile.CpuSampling)]
public class JsonDeserializationTest
{
    private readonly string _jsonString;

    public JsonDeserializationTest() 
    {
        var reader = new StreamReader("jokes.json");
        _jsonString = reader.ReadToEnd();
    }

    [Benchmark]
    public Joke[]? Newtonsoft() => 
        JsonConvert.DeserializeObject<Joke[]>(_jsonString);

    [Benchmark]
    public Joke[]? SystemText() =>
        STJ.JsonSerializer.Deserialize<Joke[]>(_jsonString);
}
