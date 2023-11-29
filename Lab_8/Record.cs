using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Lab_8
{
    public class RecordComparer : IComparer<Record>
    {
        public int Compare(Record? x, Record? y)
        {
            if (x == null || y == null)
                throw new NullReferenceException();

            if (x.RecordSymbolsPerMinute == y.RecordSymbolsPerMinute)
            {
                if (x.RecordSymbolsPerSecond == y.RecordSymbolsPerSecond)
                {
                    return x.Name.CompareTo(y.Name);
                }
                return x.RecordSymbolsPerSecond.CompareTo(y.RecordSymbolsPerSecond);
            }
            return x.RecordSymbolsPerMinute.CompareTo(y.RecordSymbolsPerMinute);
        }
    }

    public class Record
    {
        public readonly string Name;

        private float recordSymbolsPerMinute = 0;
        public float RecordSymbolsPerMinute 
        {
            get => recordSymbolsPerMinute;
            set
            {
                if (value > recordSymbolsPerMinute)
                {
                    recordSymbolsPerMinute = value;
                }
            }
        }

        private float recordSymbolsPerSecond = 0;

        public float RecordSymbolsPerSecond
        {
            get => recordSymbolsPerSecond;
            set
            {
                if (value > recordSymbolsPerSecond)
                {
                    recordSymbolsPerSecond = value;
                }
            }
        }

        public Record(string name) => Name = name;

        [JsonConstructor]
        public Record(string name, float recordSymbolsPerMinute, float recordSymbolsPerSecond) : this(name)
        {
            RecordSymbolsPerMinute = recordSymbolsPerMinute;
            RecordSymbolsPerSecond = recordSymbolsPerSecond;
        }

        public override string ToString()
        {
            return $"{Name, -25}" +
                $"{string.Format("{0:f1}", RecordSymbolsPerMinute), 5} символов в минуту\t" +
                $"{string.Format("{0:f1}", RecordSymbolsPerSecond), 5} символов в секунду";
        }
    }
}
