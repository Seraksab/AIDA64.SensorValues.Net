# AIDA64.SensorValues.Net

[![Nuget](https://img.shields.io/nuget/v/AIDA64.SensorValues.Net?style=flat-square)](https://www.nuget.org/packages/AIDA64.SensorValues.Net)
![GitHub](https://img.shields.io/github/license/Seraksab/AIDA64.SensorValues.Net)

A small and simple library to read sensor values shared by [AIDA64](https://www.aida64.com/) via

- Shared Memory
- Windows Registry

## Prerequisites

In **AIDA64 → Preferences → External Applications**, enable:

- **Shared Memory** and/or
- **Registry**

> If these aren’t enabled, readers will return no values or fail.

## Usage

```csharp
var reader = new SharedMemoryReader();
foreach (var sensorValue in reader.Read())
{
  Console.Out.WriteLine(sensorValue);
}
```

- Shared Memory is typically faster than Registry.
- If you get no results, double-check AIDA64 sharing settings first.

## Benchmark

| Method       |     Mean |    Error |   StdDev |   Median | Allocated |
|--------------|---------:|---------:|---------:|---------:|----------:|
| Registry     | 917.9 us | 105.6 us | 311.3 us | 851.2 us |  74.58 KB |
| SharedMemory | 257.2 us | 181.9 us | 536.4 us | 201.2 us |  110.5 KB |

Run on:

- Windows 11 Pro 25H2
- .NET 10.0.102
- CPU: AMD Ryzen 9 7900X
- RAM: DDR5-6200 CL30

## License

See [LICENSE](LICENSE)
