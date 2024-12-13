using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DoubleClickFix.Benchmarks
{

    [Flags]
    public enum MouseButtons
    {
        /// <summary>
        ///  The left mouse button was pressed.
        /// </summary>
        Left = 0x00100000,

        /// <summary>
        ///  No mouse button was pressed.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        ///  The right mouse button was pressed.
        /// </summary>
        Right = 0x00200000,

        /// <summary>
        ///  The middle mouse button was pressed.
        /// </summary>
        Middle = 0x00400000,

        XButton1 = 0x00800000,

        XButton2 = 0x01000000,
    }

/* Results:     
| Method           | Mean      | Error     | StdDev    | Ratio | Rank | Allocated | Alloc Ratio |
|----------------- |----------:|----------:|----------:|------:|-----:|----------:|------------:|
| WithResourceFile | 32.984 ns | 0.2402 ns | 0.2129 ns |  1.00 |    3 |         - |          NA |
| WithLookupFrozen |  4.106 ns | 0.0174 ns | 0.0163 ns |  0.12 |    2 |         - |          NA |
| WithLookup       |  3.792 ns | 0.0231 ns | 0.0205 ns |  0.11 |    1 |         - |          NA |
 */

    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.SlowestToFastest)]
    public class LocalizationBenchmarks
    {

        [Benchmark(Baseline = true)]
        public void WithResourceFile() => TranslateButtonWithResources(MouseButtons.Left);

        [Benchmark]
        public void WithLookup() => TranslateButtonWithLookup(MouseButtons.Left);

        [Benchmark]
        public void WithLookupFrozen() => TranslateButtonWithLookup(MouseButtons.Left);

        private static string TranslateButtonWithResources(MouseButtons button)
        {
            return button switch
            {
                MouseButtons.Left => Resources.Left,
                MouseButtons.Right => Resources.Right,
                MouseButtons.Middle => Resources.Middle,
                MouseButtons.XButton1 => Resources.X1,
                MouseButtons.XButton2 => Resources.X2,
                _ => "?",
            };
        }

        private static Dictionary<MouseButtons, string> lookup = new Dictionary<MouseButtons, string> {
            { MouseButtons.Left, Resources.Left },
            { MouseButtons.Right, Resources.Right },
            { MouseButtons.Middle, Resources.Middle },
            { MouseButtons.XButton1, Resources.X1 },
            { MouseButtons.XButton2, Resources.X2 },
        };

        private FrozenDictionary<MouseButtons, string> lookupFrozen = lookup.ToFrozenDictionary();

        private string TranslateButtonWithLookup(MouseButtons button)
        {
            return lookup[button];
        }
    }
}
