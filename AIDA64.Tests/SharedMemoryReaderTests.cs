namespace AIDA64.Tests;

public class SharedMemoryReaderTests
{
  [Fact]
  public void ReadSensorValues_ShouldReturnSensorValues_WhenXmlIsValid()
  {
    var sharedMemoryReader = new SharedMemoryReader();
    var sensorValues = sharedMemoryReader.ReadSensorValues().ToList();

    Assert.NotNull(sensorValues);
    Assert.True(sensorValues.Count > 0);
  }
}