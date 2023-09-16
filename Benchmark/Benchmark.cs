using AIDA64.Model;
using AIDA64.Readers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace AIDA64.Benchmark;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.ColdStart, warmupCount: 0)]
public class Benchmark
{
  private readonly RegistryReader _registryReader = new();
  private readonly SharedMemoryReader _sharedMemoryReader = new();

  [Benchmark]
  public IList<SensorValue> Registry() => _registryReader.Read().ToList();

  [Benchmark]
  public IList<SensorValue> SharedMemory() => _sharedMemoryReader.Read().ToList();
}