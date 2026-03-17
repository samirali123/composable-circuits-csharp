using System.IO;

namespace CircuitEngine
{
    public interface ICircuit
    {
        void SetNum1(int value);
        void SetNum2(int value);
        void SetOperator(char op);
        void Freeze();
        bool IsFrozen { get; }
        int GetResult();
        void Serialize(TextWriter writer);
        void Deserialize(TextReader reader);
        ICircuit Clone();
    }
}
