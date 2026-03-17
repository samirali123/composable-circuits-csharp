using System;
using System.IO;

namespace CircuitEngine
{
    // Shared base for all circuit types. Manages the operator and freeze state.
    // Subclasses implement ValidateOperator() and ComputeResult() for type-specific logic.
    public abstract class BaseCircuit : ICircuit
    {
        private bool _isFrozen;
        private char _operator;

        public char CurrOperator => _operator;

        public abstract void SetNum1(int value);
        public abstract void SetNum2(int value);

        public void Freeze() => _isFrozen = true;
        public bool IsFrozen => _isFrozen;

        public void SetOperator(char op)
        {
            if (_isFrozen)
                throw new InvalidOperationException("Operator is frozen");
            ValidateOperator(op);
            _operator = op;
        }

        public int GetResult() => ComputeResult();

        public virtual void Serialize(TextWriter writer) { }
        public virtual void Deserialize(TextReader reader) { }

        protected abstract void ValidateOperator(char op);
        protected abstract int ComputeResult();

        protected char Operator => _operator;

        protected void SerializeCommon(TextWriter writer)
        {
            writer.WriteLine(Operator);
            writer.WriteLine(IsFrozen ? "1" : "0");
        }

        protected void DeserializeCommon(TextReader reader)
        {
            char op = char.Parse(reader.ReadLine());
            SetOperator(op);
            if (reader.ReadLine() == "1") Freeze();
        }

        public abstract ICircuit Clone();
    }
}
