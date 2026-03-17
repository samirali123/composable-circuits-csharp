using System;

namespace CircuitEngine
{
    // Dynamic array of ICircuit instances. LastOutput() chains them: each circuit's
    // result feeds as SetNum1 into the next, returning the final output.
    public class LinearCircuit
    {
        private ICircuit[] _circuits;
        private int _size;

        public LinearCircuit()
        {
            _circuits = new ICircuit[4];
            _size = 0;
        }

        public int Size => _size;

        public ICircuit GetAt(int index)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _circuits[index];
        }

        public int LastOutput()
        {
            if (_size == 0) throw new InvalidOperationException("LinearCircuit is empty");
            int result = _circuits[0].GetResult();
            for (int i = 1; i < _size; i++)
            {
                _circuits[i].SetNum1(result);
                result = _circuits[i].GetResult();
            }
            return result;
        }

        public CircuitIterator Begin() => new CircuitIterator(this, 0);
        public CircuitIterator End() => new CircuitIterator(this, _size);

        public CircuitIterator Insert(CircuitIterator pos, ICircuit circuit)
        {
            if (pos.Owner != this)
                throw new ArgumentException("Iterator belongs to a different LinearCircuit");
            if (pos.Position < 0 || pos.Position > _size)
                throw new ArgumentOutOfRangeException(nameof(pos));
            EnsureCapacity(_size + 1);
            int index = pos.Position;
            for (int i = _size; i > index; i--)
                _circuits[i] = _circuits[i - 1];
            _circuits[index] = circuit;
            _size++;
            return new CircuitIterator(this, index);
        }

        public CircuitIterator Erase(CircuitIterator pos)
        {
            if (pos.Owner != this)
                throw new ArgumentException("Iterator belongs to a different LinearCircuit");
            int index = pos.Position;
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException(nameof(pos));
            for (int i = index; i + 1 < _size; i++)
                _circuits[i] = _circuits[i + 1];
            _circuits[_size - 1] = null;
            _size--;
            return new CircuitIterator(this, index);
        }

        private void EnsureCapacity(int min)
        {
            if (_circuits.Length >= min) return;
            int newCap = Math.Max(_circuits.Length * 2, min);
            var newArr = new ICircuit[newCap];
            Array.Copy(_circuits, newArr, _size);
            _circuits = newArr;
        }
    }
}
