using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PrimeNumber
{

    internal class PrimeNumberHelper
    {
        private static List<UInt64> p_list = new List<UInt64>();

        private static bool HasDivisorFor(UInt64 n)
        {
            foreach (var p in p_list)
            {
                if (n % p == 0) return true;
                if (p * p > n) break;
            }
            return false;
        }

        private static void CalcToNextAfter(UInt64 n)
        {
            UInt64 last = 2;
            if (p_list.Count < 1) {
                p_list.Add(last);
            } else {
                last = p_list[p_list.Count - 1];
            }
            if (last <= n) {
                last += 1;
                while (true) {
                    if (!HasDivisorFor(last)) {
                        p_list.Add(last);
                    #if DEBUG
                        Console.WriteLine($"Added {last} to the list of known prime numbers.");
                    #endif
                        if (last > n) break;
                    }
                    last += 1;
                }
            }
        }

        public static UInt64 GetNextAfter(UInt64 n)
        {
            CalcToNextAfter(n);
            var i = 0;
            while ((i < p_list.Count) && (p_list[i] <= n)) i += 1;
            return p_list[i];
        }

        public static UInt64 GetLastBefore(UInt64 n)
        {
            if (n < 3) throw new ArgumentOutOfRangeException("n", "There is no prime number before 2.");
            CalcToNextAfter(n);
            var i = p_list.Count - 2;
            while ((i > 0) && (p_list[i] >= n)) i -= 1;
            return p_list[i];
        }

        public static UInt64[] GetRange(UInt64 lower, UInt64 upper)
        {
            if (upper < lower) return null;
            List<UInt64> r_list = new List<UInt64>();
            CalcToNextAfter(upper);
            foreach (var p in p_list)
            {
                if ((p >= lower) && (p < upper)) r_list.Add(p);
            }
            return r_list.ToArray();
        }
    }

    [Cmdlet(VerbsCommon.Get,"PrimeNumberArray")]
    [OutputType(typeof(UInt64[]))]
    [CmdletBinding()]
    public class PrimeNumberArrayCmdlet : PSCmdlet
    {
        private UInt64 p_lowerLimit = 1;
        private UInt64 p_upperLimit = 3;

        protected override void ProcessRecord()
        {
            WriteObject(PrimeNumberHelper.GetRange(p_lowerLimit, p_upperLimit));
        }

        [Parameter(Position = 1, ValueFromPipeline = true)]
        public UInt64 From
        {
            get { return p_lowerLimit; }
            set { p_lowerLimit = value; }
        }

        [Parameter(Mandatory = true, Position = 2, ValueFromPipeline = true)]
        public UInt64 To
        {
            get { return p_upperLimit; }
            set { p_upperLimit = value; }
        }
    }

    [Cmdlet(VerbsCommon.Get, "NextPrimeNumber")]
    [OutputType(typeof(UInt64))]
    [CmdletBinding()]
    public class NextPrimeNumberCmdlet : PSCmdlet
    {
        private UInt64 p_lowerLimit = 1;

        protected override void ProcessRecord()
        {
            WriteObject(PrimeNumberHelper.GetNextAfter(p_lowerLimit));
        }

        [Parameter(Position = 1, ValueFromPipeline = true)]
        public UInt64 After
        {
            get { return p_lowerLimit; }
            set { p_lowerLimit = value; }
        }
    }

    [Cmdlet(VerbsCommon.Get, "LastPrimeNumber")]
    [OutputType(typeof(UInt64))]
    [CmdletBinding()]
    public class LastPrimeNumberCmdlet : PSCmdlet
    {
        private UInt64 p_upperLimit = 1;

        protected override void ProcessRecord()
        {
            WriteObject(PrimeNumberHelper.GetLastBefore(p_upperLimit));
        }

        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public UInt64 Before
        {
            get { return p_upperLimit; }
            set { p_upperLimit = value; }
        }
    }
}
