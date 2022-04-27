namespace a;

using BenchmarkDotNet.Attributes;
using Blob;

[MemoryDiagnoser]
[ShortRunJob]
public class CloneBenchmark
{
    private Graphic<double> Graphic;
    private const int Count = 10_000_000;

    [GlobalSetup]
    public void Setup()
    {
        Graphic = CreateGraphic(Count);
    }

    [Benchmark(Baseline = true)]
    public Graphic<double> Clone()
    {
        return (Graphic<double>)Graphic.Clone();
    }
    
    [Benchmark]
    public Graphic<double> ProtoClone()
    {
        return ProtoBufSerializerWithCompress.DeepCopy(Graphic);
    }
    
    Graphic<double> CreateGraphic(int pointCount)
    {
        var points = new SortedSet<Point<double>>(GetPoints(pointCount));
        var graphic = new Graphic<double>
        {
            Name = "foo",
            Points = points
        };

        return graphic;
    }

    private static IEnumerable<Point<double>> GetPoints(int count)
    {
        return Enumerable
            .Range(0, count)
            .Select(i => new Point<double>
            {
                X = i,
                Y = Random.Shared.NextDouble()
            });
    }
}