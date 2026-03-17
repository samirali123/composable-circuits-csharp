using System;
using System.IO;

namespace CircuitEngine
{
    // Returns 1 (true) or 0 (false) based on a comparison of two integers.
    // Operators: '=' (equal), '!' (not equal), '<' (less than), '>' (greater than)
    public class ComparisonCircuit : BaseCircuit
    {
        private int _num1;
        private int _num2;

        private static readonly char[] ValidOperators = { '=', '!', '<', '>' };

        public ComparisonCircuit(int num1, int num2, char op)
        {
            if (!IsValidOperator(op))
                throw new ArgumentException($"Invalid operator '{op}'");
            SetNum1(num1);
            SetNum2(num2);
            SetOperator(op);
        }

        public override void SetNum1(int value) => _num1 = value;
        public override void SetNum2(int value) => _num2 = value;

        public int Num1 => _num1;
        public int Num2 => _num2;
        public new char CurrOperator => Operator;

        protected override void ValidateOperator(char op)
        {
            if (!IsValidOperator(op))
                throw new ArgumentException($"Invalid operator '{op}'");
        }

        protected override int ComputeResult()
        {
            switch (Operator)
            {
                case '>': return _num1 > _num2 ? 1 : 0;
                case '<': return _num1 < _num2 ? 1 : 0;
                case '=': return _num1 == _num2 ? 1 : 0;
                case '!': return _num1 != _num2 ? 1 : 0;
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
            var clone = new ComparisonCircuit(_num1, _num2, CurrOperator);
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
