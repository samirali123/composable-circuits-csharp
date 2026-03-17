using System;

namespace CircuitEngine
{
    // Traversal cursor for LinearCircuit. pos is always in [0, owner.Size].
    // Operators +, -, ++, -- respect bounds. Subtracting two iterators
    // from different owners throws ArgumentException.
    public class CircuitIterator
    {
        private readonly LinearCircuit _owner;
        private int _pos;

        public CircuitIterator(LinearCircuit lc, int pos)
        {
            _owner = lc ?? throw new ArgumentNullException(nameof(lc));
            _pos = pos;
        }

        public LinearCircuit Owner => _owner;
        public int Position => _pos;

        public static CircuitIterator operator ++(CircuitIterator it)
        {
            if (it._pos < it._owner.Size) ++it._pos;
            return it;
        }

        public static CircuitIterator operator --(CircuitIterator it)
        {
            if (it._pos > 0) --it._pos;
            return it;
        }

        public static bool operator ==(CircuitIterator a, CircuitIterator b)
            => a._owner == b._owner && a._pos == b._pos;

        public static bool operator !=(CircuitIterator a, CircuitIterator b)
            => !(a == b);

        public static CircuitIterator operator +(CircuitIterator it, int n)
            => new CircuitIterator(it._owner, Math.Min(it._pos + n, it._owner.Size));

        public static CircuitIterator operator -(CircuitIterator it, int n)
            => new CircuitIterator(it._owner, Math.Max(it._pos - n, 0));

        public static int operator -(CircuitIterator a, CircuitIterator b)
        {
            if (a._owner != b._owner)
                throw new ArgumentException("Iterators from different LinearCircuits");
            return a._pos - b._pos;
        }

        public ICircuit Deref() => _owner.GetAt(_pos);

        public override bool Equals(object obj) => obj is CircuitIterator other && this == other;
        public override int GetHashCode() => HashCode.Combine(_owner, _pos);
    }
}
