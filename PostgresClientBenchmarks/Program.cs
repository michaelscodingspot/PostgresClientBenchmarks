using BenchmarkDotNet.Running;

Console.WriteLine("Starting");

BenchmarkRunner.Run<SingleInsert>();
