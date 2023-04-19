using BenchmarkDotNet.Running;
using FirstBenchmark;

#if DEBUG

var test = new JsonDeserializationTest();
CheckJokes(test.Newtonsoft);
CheckJokes(test.SystemText);

static void CheckJokes(Func<Joke[]?> func)
{
    try
    {
        var jokes = func();
        if (jokes == null || !jokes.Any())
        {
            throw new Exception();
        }

        Console.WriteLine("Succeeded");
    }
    catch
    {
        Console.WriteLine("Failed");
    }
}

#else

BenchmarkRunner.Run<JsonDeserializationTest>();

#endif
