# AIDA64.SensorValues.Net

[![Nuget](https://img.shields.io/nuget/v/AIDA64.SensorValues.Net?style=flat-square)](https://www.nuget.org/packages/AIDA64.SensorValues.Net)
![GitHub](https://img.shields.io/github/license/Seraksab/AIDA64.SensorValues.Net)

A small and simple library to read sensor values shared by [AIDA64](https://www.aida64.com/) via

- Shared Memory
- Registry

**Note**: This requires shared memory or registry sharing to be enabled in the AIDA64 settings under 'External
Applications'

## Usage

```csharp
var reader = new SharedMemoryReader();
foreach (var sensorValue in reader.Read())
{
  Console.Out.WriteLine(sensorValue);
}
```

## Benchmark

| Method                    |     Mean |     Error |    StdDev | Allocated |
|---------------------------|---------:|----------:|----------:|----------:|
| RegistryReader.Read()     | 6.922 ms | 0.2855 ms | 0.8418 ms | 262.16 KB |
| SharedMemoryReader.Read() | 1.078 ms | 0.4771 ms | 1.4067 ms |    455 KB |
