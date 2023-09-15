using AIDA64.Reader;

namespace AIDA64.Tests;

public class UnitTests
{
  [Fact]
  public void SharedMemoryReader_ShouldReturnSensorValues()
  {
    var reader = new SharedMemoryReader();
    var sensorValues = reader.Read().ToList();

    Assert.NotNull(sensorValues);
    Assert.True(sensorValues.Count > 0);
  }

  [Fact]
  public void RegistryReader_ShouldReturnSensorValues()
  {
    var reader = new RegistryReader();
    var sensorValues = reader.Read().ToList();

    Assert.NotNull(sensorValues);
    Assert.True(sensorValues.Count > 0);
  }

  [Fact]
  public void Readers_ShouldReturnSameResult()
  {
    var registry = new RegistryReader().Read().ToList();
    var sharedMemory = new SharedMemoryReader().Read().ToList();

    Assert.Equal(registry.Count, sharedMemory.Count);
    Assert.Equal(registry.ToHashSet(), sharedMemory.ToHashSet());
  }
}