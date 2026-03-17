using System;
using System.IO;

namespace CircuitEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            LinearCircuitTest();
            CircuitIteratorTest();
            ComparisonCircuitTest();
            MinMaxCircuitTest();
            CircuitBlockTest();
            SerializationTest();
        }

        static void LinearCircuitTest()
        {
            var lcMath = new LinearCircuit();
            lcMath.Insert(lcMath.End(), new MathCircuit(7, 19, '+'));
            lcMath.Insert(lcMath.End(), new MathCircuit(21, 9, '-'));
            lcMath.Insert(lcMath.End(), new MathCircuit(2, 13, '*'));

            Console.WriteLine("lcMath size: " + lcMath.Size);
            Console.WriteLine("lcMath lastOutput: " + lcMath.LastOutput());

            var lcMix = new LinearCircuit();
            lcMix.Insert(lcMix.End(), new MathCircuit(4, 2, '+'));
            lcMix.Insert(lcMix.End(), new ComparisonCircuit(5, 10, '<'));
            lcMix.Insert(lcMix.End(), new MinMaxCircuit(6, 6, '>'));

            Console.WriteLine("lcMix size: " + lcMix.Size);
            Console.WriteLine("lcMix lastOutput: " + lcMix.LastOutput());

            lcMix.Erase(lcMix.Begin() + 1);
            Console.WriteLine("lcMix size after erase: " + lcMix.Size);
        }

        static void CircuitIteratorTest()
        {
            var lc = new LinearCircuit();
            lc.Insert(lc.End(), new MathCircuit(4, 1, '+'));
            lc.Insert(lc.End(), new MathCircuit(7, 13, '+'));
            lc.Insert(lc.End(), new MathCircuit(8, 10, '*'));

            var it1 = lc.Begin();
            var it2 = lc.Begin();

            Console.WriteLine("it1 == it2 initially? " + (it1 == it2));
            ++it2;
            Console.WriteLine("After ++it2, it1 == it2? " + (it1 == it2));
            it1 = it1 + 2;
            Console.WriteLine("After it1 + 2, it1 == it2? " + (it1 == it2));
            Console.WriteLine("Distance it2 - it1: " + (it2 - it1));

            ++it2;
            Console.WriteLine("it2 deref result after ++it2: " + it2.Deref().GetResult());

            var cloned = it2.Deref().Clone();
            cloned.SetNum1(42);
            Console.WriteLine("Clone getResult after SetNum1(42): " + cloned.GetResult());
        }

        static void ComparisonCircuitTest()
        {
            var c = new ComparisonCircuit(5, 7, '>');
            c.Freeze();
            Console.WriteLine("c.IsFrozen? " + c.IsFrozen);

            try { c.SetOperator('='); }
            catch (Exception e) { Console.WriteLine("Exception after freeze: " + e.Message); }

            var c2 = new ComparisonCircuit(10, 5, '=');
            Console.WriteLine("c2 result (==): " + c2.GetResult());
            c2.SetOperator('!');
            Console.WriteLine("c2 result (!=): " + c2.GetResult());
        }

        static void MinMaxCircuitTest()
        {
            var mm = new MinMaxCircuit(2, 3, '<');
            Console.WriteLine("MinMax result (min): " + mm.GetResult());

            mm.SetNum1(5);
            mm.SetNum2(5);
            mm.SetOperator('=');
            Console.WriteLine("MinMax result (equal): " + mm.GetResult());
        }

        static void CircuitBlockTest()
        {
            char[] mathOps = { '-', '/', '+' };
            char[] compOps = { '=', '!' };
            var cb = new CircuitBlock(3, 2, mathOps, compOps);

            cb.SetNum1(4);
            cb.SetNum2(2);
            cb.OperUpdate(true,  0, '/');
            cb.OperUpdate(false, 0, '!');
            cb.OperUpdate(true,  1, '-');
            cb.OperUpdate(false, 1, '=');

            cb.WireCircuit(1, 0, true);
            cb.WireCircuit(0, 1, false);
            cb.WireCircuit(1, 1, true);

            Console.WriteLine("CircuitBlock result: " + cb.GetResult());
            Console.WriteLine("CCircuitResult(0): " + cb.CCircuitResult(0));

            cb.OperFreeze(false, 0);
            try { cb.OperUpdate(false, 0, '!'); }
            catch (Exception e) { Console.WriteLine("Exception after freeze: " + e.Message); }

            Console.WriteLine("CircuitBlock result after modifications: " + cb.GetResult());
        }

        static void SerializationTest()
        {
            char[] mathOps = { '+', '-', '*' };
            char[] compOps = { '!', '=' };
            var cb = new CircuitBlock(3, 2, mathOps, compOps);

            cb.SetNum1(10);
            cb.SetNum2(20);
            cb.OperUpdate(true, 0, '+');
            cb.OperUpdate(false, 0, '!');
            cb.WireCircuit(1, 0, true);

            string filePath = "circuit_serialized.txt";
            using (var writer = new StreamWriter(filePath))
                cb.Serialize(writer);

            var cb2 = new CircuitBlock(3, 2, mathOps, compOps);
            using (var reader = new StreamReader(filePath))
                cb2.Deserialize(reader);

            Console.WriteLine("Deserialized CircuitBlock result: " + cb2.GetResult());
        }
    }
}
