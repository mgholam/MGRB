using RaptorDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 100;
            Global.useSortedList = false;
            Dictionary<long, bool> list = new Dictionary<long, bool>();
            var r = new Random();
            for (int i = 0; i < count; i++)
            {
                var cc = r.Next(1000 * 1000 * 1);// + 1000*1000;
                if (list.ContainsKey(cc) == false)
                    list.Add(cc, true);
            }
            Console.WriteLine("count = " + count);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            MGRB a = new MGRB();
            foreach (var l in list)
            {
                a.Set(l.Key, true);
            }
            sw.Stop();
            Console.WriteLine("mgrb set time : " + sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();
            RaptorDB.WAHBitArray w = new RaptorDB.WAHBitArray();
            foreach (var l in list)
            {
                w.Set((int)l.Key, true);
            }
            sw.Stop();
            Console.WriteLine("wah set time : " + sw.ElapsedMilliseconds);


            var ok = true;
            foreach (var k in a.GetBitIndexes())
                if (a.Get(k) == false)
                    ok = false;

            a.Optimize();
            var c = a.CountOnes();
            Console.WriteLine("max list = " + list.Keys.Max());
            Console.WriteLine("max bm = " + a.Length);
            Console.WriteLine("list count = " + list.Count);
            Console.WriteLine("bm count = " + c);
            Console.WriteLine(" ok = " + ok);

            if (ok == false || list.Count != c)
                throw new Exception();

            if (list.Keys.Max() != a.Length)
                throw new Exception();

            var x = a.AndNot(new MGRB());
            x.Optimize();
            c = x.CountOnes();

            if (list.Count != c)
                throw new Exception();

            var y = new MGRB();
            var z = new MGRB();
            foreach (var l in list)
            {
                y.Set(l.Key, true);
                z.Set(l.Key + 1000000, true);
            }
            var zz = y.Or(z);
            c = zz.CountOnes();
            if (y.CountOnes() * 2 != c)
                throw new Exception();

            // fix : NOT  
            var zn = zz.Not();
            zn.Optimize();
            var iii = zn.CountOnes();

            var o = zz.Serialize();

            var s = fastJSON.JSON.ToNiceJSON(o, new fastJSON.JSONParameters { UseExtensions = false });

            var mmm = new MGRB();
            mmm.Deserialize(o);

            c = zz.AndNot(mmm).CountOnes();
        }
    }
}
