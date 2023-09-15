using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Xml;
using AIDA64.Model;

namespace AIDA64.Reader;

/// <summary>
/// Reads the sensor values shared by Aida64 from shared memory
/// </summary>
public class SharedMemoryReader : ISensorValueReader
{
  private const string SharedMemoryFile = "AIDA64_SensorValues";

  /// <inheritdoc />
  public IEnumerable<SensorValue> Read()
  {
    var content = ReadSharedMemory();
    if (string.IsNullOrEmpty(content)) yield break;

    var xmlDoc = new XmlDocument();
    xmlDoc.LoadXml($"<root>{content}</root>");

    if (xmlDoc.FirstChild == null) yield break;

    foreach (XmlNode node in xmlDoc.FirstChild.ChildNodes)
    {
      var id = ReadNodeText(node, "id");
      if (id == null) continue;

      yield return new SensorValue(
        Id: id,
        Label: ReadNodeText(node, "label") ?? string.Empty,
        Value: ReadNodeText(node, "value") ?? string.Empty,
        Type: SensorTypeFromString(node.Name)
      );
    }
  }

  private static string ReadSharedMemory()
  {
    try
    {
      using var mmf = MemoryMappedFile.OpenExisting(SharedMemoryFile, MemoryMappedFileRights.Read);
      using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
      Span<byte> bytes = stackalloc byte[(int)accessor.Capacity];
      accessor.SafeMemoryMappedViewHandle.ReadSpan(0, bytes);
      var endIdx = bytes.IndexOf(Convert.ToByte('\x00'));
      return Encoding.ASCII.GetString(bytes[..endIdx]);
    }
    catch (FileNotFoundException)
    {
      return string.Empty;
    }
  }

  private static string? ReadNodeText(XmlNode node, string xPath) => node.SelectSingleNode(xPath)?.InnerText.Trim();

  private static SensorType SensorTypeFromString(string type) => type.Trim().ToLowerInvariant() switch
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