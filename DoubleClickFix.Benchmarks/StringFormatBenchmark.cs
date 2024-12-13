using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
| Method                 | Mean      | Error     | StdDev    | Rank | Gen0   | Allocated |
|----------------------- |----------:|----------:|----------:|-----:|-------:|----------:|
| WithInterpolatedString | 66.503 ns | 0.6574 ns | 0.5489 ns |    7 | 0.0130 |     109 B |
| WithConcatenatedString | 57.648 ns | 0.7970 ns | 0.6656 ns |    6 | 0.0250 |     210 B |
| WithStringBuilder      | 51.742 ns | 0.6867 ns | 0.5734 ns |    5 | 0.0401 |     336 B |
| WithSpan               | 43.884 ns | 0.5474 ns | 0.4274 ns |    4 | 0.0343 |     287 B |
| WithLazyString         | 11.135 ns | 0.1023 ns | 0.0854 ns |    3 | 0.0163 |     136 B |
| WithStruct             |  7.262 ns | 0.0332 ns | 0.0310 ns |    2 |      - |         - |
| WithFunc               |  3.602 ns | 0.0412 ns | 0.0344 ns |    1 | 0.0077 |      64 B |
 */
namespace DoubleClickFix.Benchmarks
{
    struct LogEntry
    {
        private string? log;
        private Lazy<string>? lazyLog;
        public LogEntry(string a)
        {
            log=a;
        }
        public LogEntry(Lazy<string> a)
        {
            lazyLog = a;
        }
        public override string ToString()
        {
            if (log != null)
            {
                return log;
            }
            else if (lazyLog != null)
            {
                return lazyLog.Value;
            }
            return "";
        }
    }

    struct LogEntry2
    {
        private static readonly string ignoredDoubleClickText = Resources.DoubleClickIgnored;
        private readonly string? log;
        private readonly string? button;
        private readonly int TimeDifference = -1;
        private readonly int IgnoredClicks = -1;
        public LogEntry2(string log)
        {
            this.log = log;
        }
        public LogEntry2(string button, int timeDifference)
        {
            this.button = button;
            TimeDifference = timeDifference;
        }
        public LogEntry2(string button, int timeDifference, int ignoredClicks)
        {
            this.button = button;
            TimeDifference = timeDifference;
            IgnoredClicks = ignoredClicks;
        }

        public override string ToString()
        {
            return $"{ignoredDoubleClickText} ({button}): {TimeDifference} ms (#{IgnoredClicks})";
        }
        // Implicit conversion from string to LogMessageBuilder
        public static implicit operator LogEntry2(string message)
        {
            return new LogEntry2(message);
        }
    }


    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.SlowestToFastest)]
    public class StringFormatBenchmark
    {
        private string ignoredDoubleClickText = "Double click ignored";
        private string button = "left";
        private int TimeDifference
        {
            get { return Random.Shared.Next(500); }
        }

        private int IgnoredClicks
        {
            get { return Random.Shared.Next(500); }
        }

        //string text = $"{ignoredDoubleClickText} ({buttonTextLookup[button]}): {timeDifference} ms (#{ignoredClicks})";
        // handle format or action or add parameters
        [Benchmark]
        public void WithInterpolatedString() {
            string text = $"{ignoredDoubleClickText} ({button}): {TimeDifference} ms (#{IgnoredClicks})";
            Log(text);
        }

        [Benchmark]
        public void WithConcatenatedString()
        {
            string text = "Doppelklick ignoriert " + button + ": " + TimeDifference + " ms (#" + IgnoredClicks + ")";
            Log(text);
        }
        [Benchmark]
        public void WithStringBuilder()
        {
            var sb = new StringBuilder(80);  // Pre-allocate size to reduce resizing overhead
            sb.Append("Doppelklick ignoriert ")
              .Append(button)
              .Append(": ")
              .Append(TimeDifference)
              .Append(" ms (#")
              .Append(IgnoredClicks)
              .Append(")");
            Log(sb.ToString());
        }

        [Benchmark]
        public void WithStruct()
        {
            var entry = new LogEntry2(button, TimeDifference, IgnoredClicks);
            LazyLog(entry);
        }
        [Benchmark]
        public void WithFunc()
        {
            Func<string> entry = () => $"Doppelklick ignoriert {button}: {TimeDifference} ms (#{IgnoredClicks})";
            LazyLog(entry);
        }
        [Benchmark]
        public void WithLazyString()
        {
            Lazy<string> text = new(() =>
                $"Doppelklick ignoriert {button}: {TimeDifference} ms (#{IgnoredClicks})"
            );
            LazyLog(new LogEntry(text));
        }
        [Benchmark]
        public void WithSpan()
        {
            Span<char> buffer = new Span<char>(new char[80]);  // Pre-allocate a buffer

            int pos = 0;
            // Copy static parts
            ignoredDoubleClickText.AsSpan().CopyTo(buffer.Slice(pos));
            pos += ignoredDoubleClickText.Length;

            // Copy dynamic parts
            button.AsSpan().CopyTo(buffer.Slice(pos));
            pos += button.Length;

            ": ".AsSpan().CopyTo(buffer.Slice(pos));
            pos += ": ".Length;

            int n = TimeDifference;
            n.TryFormat(buffer.Slice(pos), out int charsWritten);
            pos += charsWritten;

            " ms (#".AsSpan().CopyTo(buffer.Slice(pos));
            pos += " ms (#".Length;

            n = IgnoredClicks;
            n.TryFormat(buffer.Slice(pos), out charsWritten);
            pos += charsWritten;

            ")".AsSpan().CopyTo(buffer.Slice(pos));
            pos += ")".Length;

            // Create the final string from the populated buffer
            Log(new string(buffer.Slice(0, pos)));
        }
        private void Log(string message) {
        }
        private void LazyLog(Func<string> message)
        {
        }
        private void LazyLog(LogEntry message)
        {
        }
        private void LazyLog(LogEntry2 message)
        {
        }
    }
}
