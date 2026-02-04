using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Xml;
using AIDA64.Model;

namespace AIDA64.Readers;

/// <summary>
/// Reads the sensor values shared by Aida64 from shared memory
/// </summary>
public class SharedMemoryReader : ISensorValueReader
{
  private const string SharedMemoryFile = "AIDA64_SensorValues";

  private MemoryMappedFile? _mmf;
  private MemoryMappedViewAccessor? _accessor;

  /// <inheritdoc />
  public IEnumerable<SensorValue> Read()
  {
    if (!TryGetAccessor(out var accessor)) yield break;

    var capacity = (int)accessor.Capacity;
    int length;
    unsafe
    {
      byte* ptr = null;
      accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
      try
      {
        ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(ptr, capacity);
        int nullIdx = span.IndexOf((byte)0);
        length = nullIdx == -1 ? capacity : nullIdx;
      }
      finally
      {
        accessor.SafeMemoryMappedViewHandle.ReleasePointer();
      }
    }

    if (length == 0) yield break;

    using var stream = _mmf!.CreateViewStream(0, length, MemoryMappedFileAccess.Read);
    using var reader = XmlReader.Create(stream, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });

    while (reader.Read())
    {
      if (reader.NodeType != XmlNodeType.Element) continue;

      var typeName = reader.Name;
      var sensorType = SensorTypeFromString(typeName);

      string? id = null;
      string? label = null;
      string? value = null;

      using var subReader = reader.ReadSubtree();
      while (subReader.Read())
      {
        if (subReader.NodeType != XmlNodeType.Element) continue;
        var elementName = subReader.Name;
        switch (elementName)
        {
          case "id":
            subReader.Read();
            id = subReader.Value;
            break;
          case "label":
            subReader.Read();
            label = subReader.Value;
            break;
          case "value":
            subReader.Read();
            value = subReader.Value;
            break;
        }
      }

      if (id != null)
      {
        yield return new SensorValue(
          Id: id,
          Label: label ?? string.Empty,
          Value: value ?? string.Empty,
          Type: sensorType
        );
      }
    }
  }

  /// <inheritdoc />
  public void Dispose()
  {
    _accessor?.Dispose();
    _mmf?.Dispose();
    GC.SuppressFinalize(this);
  }

  private bool TryGetAccessor(out MemoryMappedViewAccessor accessor)
  {
    if (_accessor != null)
    {
      accessor = _accessor;
      return true;
    }

    try
    {
      _mmf = MemoryMappedFile.OpenExisting(SharedMemoryFile, MemoryMappedFileRights.Read);
      _accessor = _mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
      accessor = _accessor;
      return true;
    }
    catch (FileNotFoundException)
    {
      accessor = null!;
      return false;
    }
  }

  private static SensorType SensorTypeFromString(string type)
  {
    if (type.Length == 0) return SensorType.Unknown;

    return type switch
    {
      "sys" => SensorType.System,
      "fan" => SensorType.CoolingFan,
      "duty" => SensorType.FanSpeed,
      "temp" => SensorType.Temperature,
      "volt" => SensorType.Voltage,
      "curr" => SensorType.Current,
      "pwr" => SensorType.Power,
      "flow" => SensorType.WaterFlow,
      _ => SensorType.Unknown
    };
  }
}