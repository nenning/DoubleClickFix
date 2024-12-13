using BenchmarkDotNet.Running;
using DoubleClickFix.Benchmarks;

// .\DoubleClickFix.Benchmarks> dotnet build -c Release
// dotnet  .\DoubleClickFix.Benchmarks\bin\Release\net8.0\DoubleClickFix.Benchmarks.dll

//var summary = BenchmarkRunner.Run<LocalizationBenchmarks>();
var summary = BenchmarkRunner.Run<StringFormatBenchmark>();