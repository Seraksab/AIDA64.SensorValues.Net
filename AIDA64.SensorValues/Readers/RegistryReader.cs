using System.Collections.Generic;
using System.Linq;
using AIDA64.Model;
using Microsoft.Win32;

namespace AIDA64.Readers;

/// <summary>
/// Reads the sensor values shared by Aida64 from the registry 
/// </summary>
public class RegistryReader : ISensorValueReader
{
  private const string RegistryPath = @"Software\FinalWire\AIDA64\SensorValues";

  /// <inheritdoc />
  public IEnumerable<SensorValue> Read()
  {
    using var mainKey = Registry.CurrentUser.OpenSubKey(RegistryPath);
    if (mainKey == null) yield break;

    foreach (var valueName in mainKey.GetValueNames().Where(e => e.StartsWith("Label")))
    {
      var splitName = valueName.Split('.');
      if (splitName.Length != 2 || string.IsNullOrWhiteSpace(splitName[1])) continue;

      var id = splitName[1];
      yield return new SensorValue(
        Id: id,
        Label: Registry.GetValue(mainKey.Name, $"Label.{id}", string.Empty) as string ?? string.Empty,
        Value: Registry.GetValue(mainKey.Name, $"Value.{id}", string.Empty) as string ?? string.Empty,
        Type: SensorTypeFromTypeChar(id[0])
      );
    }
  }

  private static SensorType SensorTypeFromTypeChar(char type) => type switch
  {
    'S' => SensorType.System,
    'F' => SensorType.CoolingFan,
    'D' => SensorType.FanSpeed,
    'T' => SensorType.Temperature,
    'V' => SensorType.Voltage,
    'C' => SensorType.Current,
    'P' => SensorType.Power,
    'W' => SensorType.WaterFlow,
    _ => SensorType.Unknown
  };
}