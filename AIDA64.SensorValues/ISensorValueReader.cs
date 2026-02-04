using System;
using System.Collections.Generic;
using AIDA64.Model;

namespace AIDA64;

/// <summary>
/// Reader to access the sensor values shared by Aida64
/// </summary>
public interface ISensorValueReader : IDisposable
{
  /// <summary>
  /// Reads the sensor values
  /// </summary>
  /// <returns>The sensor values</returns>
  public IEnumerable<SensorValue> Read();
}