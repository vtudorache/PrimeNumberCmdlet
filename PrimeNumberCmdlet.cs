using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Garnet.PowerShell.Series
{

    internal class PrimeHelper
    {
        private static List<UInt64> _list = new List<UInt64>();

        private static bool HasDivisorFor(UInt64 n)
        {
            foreach (var p in _list)
            {
                if (n % p == 0) return true;
                if (p * p > n) break;
            }
            return false;
        }

        private static void CalcToNextAfter(UInt64 n)
        {
            UInt64 last = 2;
            if (_list.Count < 1) 
            {
                _list.Add(last);
            } 
            else 
            {
                last = _list[_list.Count - 1];
            }
            if (last <= n) 
            {
                last += 1;
                while (true) 
                {
                    if (!HasDivisorFor(last)) 
                    {
                        _list.Add(last);
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
            while ((i < _list.Count) && (_list[i] <= n)) i += 1;
            return _list[i];
        }

        public static UInt64 GetLastBefore(UInt64 n)
        {
            if (n < 3) throw new ArgumentOutOfRangeException("n", "There is no prime number before 2.");
            CalcToNextAfter(n);
            var i = _list.Count - 2;
            while ((i > 0) && (_list[i] >= n)) i -= 1;
            return _list[i];
        }

        public static UInt64[] GetRange(UInt64 lower, UInt64 upper)
        {
            if (upper < lower) return null;
            List<UInt64> result = new List<UInt64>();
            CalcToNextAfter(upper);
            foreach (var p in _list)
            {
                if ((p >= lower) && (p < upper)) result.Add(p);
            }
            return result.ToArray();
        }
    }

    [Cmdlet(VerbsCommon.Get,"PrimeNumberArray")]
    [OutputType(typeof(UInt64[]))]
    [CmdletBinding()]
    public class PrimeNumberArrayCommand : Cmdlet
    {
        private UInt64 _lowerLimit = 1;
        private UInt64 _upperLimit = 3;

        protected override void ProcessRecord()
        {
            WriteObject(PrimeHelper.GetRange(_lowerLimit, _upperLimit), true);
        }

        [Parameter(Position = 1, ValueFromPipeline = true)]
        public UInt64 From
        {
            get { return _lowerLimit; }
            set { _lowerLimit = value; }
        }

        [Parameter(Mandatory = true, Position = 2, ValueFromPipeline = true)]
        public UInt64 To
        {
            get { return _upperLimit; }
            set { _upperLimit = value; }
        }
    }

    [Cmdlet(VerbsCommon.Get, "NextPrimeNumber")]
    [OutputType(typeof(UInt64))]
    [CmdletBinding()]
    public class NextPrimeNumberCommand : Cmdlet
    {
        private UInt64 _lowerLimit = 1;

        protected override void ProcessRecord()
        {
            WriteObject(PrimeHelper.GetNextAfter(_lowerLimit));
        }

        [Parameter(Position = 1, ValueFromPipeline = true)]
        public UInt64 After
        {
            get { return _lowerLimit; }
            set { _lowerLimit = value; }
        }
    }

    [Cmdlet(VerbsCommon.Get, "LastPrimeNumber")]
    [OutputType(typeof(UInt64))]
    [CmdletBinding()]
    public class LastPrimeNumberCommand : Cmdlet
    {
        private UInt64 _upperLimit = 1;

        protected override void ProcessRecord()
        {
            WriteObject(PrimeHelper.GetLastBefore(_upperLimit));
        }

        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public UInt64 Before
        {
            get { return _upperLimit; }
            set { _upperLimit = value; }
        }
    }
}
