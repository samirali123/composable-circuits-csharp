using System;
using System.IO;

namespace CircuitEngine
{
    // Returns the minimum ('<') or maximum ('>') of two integer inputs.
    // '=' returns the shared value when both inputs are equal, otherwise 0.
    public class MinMaxCircuit : BaseCircuit
    {
        private int _num1;
        private int _num2;

        private static readonly char[] ValidOperators = { '<', '>', '=' };

        public MinMaxCircuit(int num1, int num2, char op)
        {
            if (!IsValidOperator(op))
                throw new ArgumentException($"Invalid operator '{op}'");
            SetNum1(num1);
            SetNum2(num2);
            SetOperator(op);
        }

        public override void SetNum1(int value) => _num1 = value;
        public override void SetNum2(int value) => _num2 = value;

        protected override void ValidateOperator(char op)
        {
            if (!IsValidOperator(op))
                throw new ArgumentException($"Invalid operator '{op}'");
        }

        protected override int ComputeResult()
        {
            switch (Operator)
            {
                case '<': return Math.Min(_num1, _num2);
                case '>': return Math.Max(_num1, _num2);
                case '=': return (_num1 == _num2) ? _num1 : 0;
                default:  throw new InvalidOperationException("Invalid operator");
            }
        }

        private bool IsValidOperator(char op)
        {
            foreach (char v in ValidOperators)
                if (op == v) return true;
            return false;
        }

        public override ICircuit Clone()
        {
            var clone = new MinMaxCircuit(_num1, _num2, Operator);
            if (IsFrozen) clone.Freeze();
            return clone;
        }

        public override void Serialize(TextWriter writer)
        {
            SerializeCommon(writer);
            writer.WriteLine(_num1);
            writer.WriteLine(_num2);
        }

        public override void Deserialize(TextReader reader)
        {
            DeserializeCommon(reader);
            _num1 = int.Parse(reader.ReadLine());
            _num2 = int.Parse(reader.ReadLine());
        }
    }
}
