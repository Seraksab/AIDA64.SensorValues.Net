using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Xml;

namespace AIDA64;

/// <summary>
/// Shared memory reader to access the sensor values shared by Aida64
/// </summary>
public class SharedMemoryReader
{
  private const string SharedMemoryFile = "AIDA64_SensorValues";

  /// <summary>
  /// Reads the sensor values from shared memory
  /// </summary>
  /// <returns>The sensor values</returns>
  public IEnumerable<SensorValue> ReadSensorValues()
  {
    var sharedMemoryXml = ReadSharedMemory();

    var xmlDoc = new XmlDocument();
    xmlDoc.LoadXml($"<root>{sharedMemoryXml}</root>");

    if (xmlDoc.FirstChild == null) yield break;

    foreach (XmlNode node in xmlDoc.FirstChild.ChildNodes)
    {
      var id = ReadNodeText(node, "id");
      if (id == null) continue;

      yield return new SensorValue(
        id, 
        ReadNodeText(node, "label") ?? string.Empty, 
        ReadNodeText(node, "value") ?? string.Empty, 
        node.Name.Trim()
        );
    }
  }

  private static string ReadSharedMemory()
  {
    using var mmf = MemoryMappedFile.OpenExisting(SharedMemoryFile, MemoryMappedFileRights.Read);
    using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
    
    Span<byte> bytes = stackalloc byte[(int)accessor.Capacity];
    accessor.SafeMemoryMappedViewHandle.ReadSpan(0, bytes);
    var endIdx = bytes.IndexOf(Convert.ToByte('\x00'));
    return Encoding.ASCII.GetString(bytes[..endIdx]);
  }

  private static string? ReadNodeText(XmlNode node, string xPath) => node.SelectSingleNode(xPath)?.InnerText.Trim();
}