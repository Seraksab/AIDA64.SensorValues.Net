using System;
using System.Collections.Generic;
using AIDA64.Model;
using Microsoft.Win32;

namespace AIDA64.Readers;

/// <summary>
/// Reads the sensor values shared by Aida64 from the registry 
/// </summary>
public class RegistryReader : ISensorValueReader
{
  private const string RegistryPath = @"Software\FinalWire\AIDA64\SensorValues";

  private RegistryKey? _mainKey;

  /// <inheritdoc />
  public IEnumerable<SensorValue> Read()
  {
    _mainKey ??= Registry.CurrentUser.OpenSubKey(RegistryPath);
    if (_mainKey == null) yield break;

    var valueNames = _mainKey.GetValueNames();
    var labels = new Dictionary<string, string>(StringComparer.Ordinal);
    var values = new Dictionary<string, string>(StringComparer.Ordinal);

    foreach (var name in valueNames)
    {
      if (name.StartsWith("Label."))
      {
        var id = name[6..];
        labels[id] = _mainKey.GetValue(name) as string ?? string.Empty;
      }
      else if (name.StartsWith("Value."))
      {
        var id = name[6..];
        values[id] = _mainKey.GetValue(name) as string ?? string.Empty;
      }
    }

    foreach (var (id, value) in labels)
    {
      yield return new SensorValue(
        Id: id,
        Label: value,
        Value: values.GetValueOrDefault(id, string.Empty),
        Type: SensorTypeFromTypeChar(id[0])
      );
    }
  }

  /// <inheritdoc />
  public void Dispose()
  {
    _mainKey?.Dispose();
    GC.SuppressFinalize(this);
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