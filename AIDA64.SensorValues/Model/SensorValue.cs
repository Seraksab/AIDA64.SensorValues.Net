namespace AIDA64.Model;

/// <summary>
/// A single AIDA64 sensor value
/// </summary>
/// <param name="Id">The sensors unique id</param>
/// <param name="Label">The sensors label</param>
/// <param name="Value">The current value</param>
/// <param name="Type">The sensor type</param>
public readonly record struct SensorValue(
  string Id,
  string Label,
  string Value,
  SensorType Type
);