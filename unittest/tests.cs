using NUnit.Framework;
using RaptorDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class tests
{
    [TestFixtureSetUp]
    public static void Setup()
    {

    }

    private static Dictionary<long, bool> CreateList(int count, int size)
    {
        var testList = new Dictionary<long, bool>();
        var r = new Random();
        for (int i = 0; i < count; i++)
        {
            var cc = r.Next(size);
            if (testList.ContainsKey(cc) == false)
                testList.Add(cc, true);
        }
        return testList;
    }

    [Test]
    public static void OptimizeTest()
    {
        // optimize
        var op1 = new MGRB();

        op1.Set(100, true);
        op1.Set(1000, true);
        op1.Set(2000, true);

        op1.Set(100000, true);

        op1.Set(50000, true);
        op1.Set(55000, true);
        op1.Set(60000, true);
        op1.Set(3000, true);
        op1.Set(51000, true);

        op1.Set(100000, false);

        op1.Optimize();
    }

    [Test]
    public static void And()
    {
        var list = CreateList(10000, 1000 * 1000);
        Console.WriteLine("count = " + list.Count);
        Stopwatch sw = new Stopwatch();
        sw.Start();
        MGRB a = new MGRB();
        foreach (var l in list)
        {
            a.Set(l.Key, true);
        }
        sw.Stop();
        Console.WriteLine("mgrb set time : " + sw.ElapsedMilliseconds);

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

        Assert.True(ok);
        Assert.AreEqual(list.Count, c);
        Assert.AreEqual(list.Keys.Max(), a.Length);
    }

    [Test]
    public static void Or()
    {
        var list = CreateList(10000, 1000 * 1000);
        var y = new MGRB();
        var z = new MGRB();
        foreach (var l in list)
        {
            y.Set(l.Key, true);
            z.Set(l.Key + 1000000, true);
        }
        var zz = y.Or(z);
        var c = zz.CountOnes();

        Assert.AreEqual(list.Count * 2, c);
    }

    [Test]
    public static void SerializeDeserialize()
    {
        var list = CreateList(10000, 1000 * 1000);
        var y = new MGRB();
        foreach (var l in list)
        {
            y.Set(l.Key, true);
        }

        var o = y.Serialize();
        var z = new MGRB();
        z.Deserialize(o);

        var a = y.AndNot(z);
        a.Optimize();
        var c = a.CountOnes();

        Assert.AreEqual(0, c);
    }


    [Test]
    public static void OR1()
    {
        var a = new MGRB();
        var b = new MGRB();
        for (int i = 0; i < 1000 * 1000; i += 10)
        {
            a.Set(1 + i, true);
            a.Set(3 + i, true);
            a.Set(5 + i, true);
            a.Set(7 + i, true);
            a.Set(9 + i, true);

            b.Set(0 + i, true);
            b.Set(2 + i, true);
            b.Set(4 + i, true);
            b.Set(6 + i, true);
            b.Set(8 + i, true);
        }

        var o = a.Or(b).Optimize();
        var c = o.CountOnes();

        Console.WriteLine("a count = " + a.CountOnes());
        Console.WriteLine("b count = " + b.CountOnes());
        Console.WriteLine("count = " + c);
    }

    [Test]
    public static void SmallValuesLength()
    {
        var a = new MGRB();
        var r = new Random();
        for (int i =0;i<100; i++)
        {
            a.Set(r.Next(10000), true);
        }

        var b = MGRB.Fill(20000);

        var p = a.Or(b);

        var c = p.CountOnes();

        Assert.AreEqual(b.Length, p.Length);

        var o = a.And(b);

        Assert.AreEqual(a.Length, o.Length);
    }
}
