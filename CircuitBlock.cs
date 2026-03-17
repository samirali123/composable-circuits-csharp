using System;
using System.IO;

namespace CircuitEngine
{
    // Composite circuit: a set of MathCircuits whose outputs can be wired
    // into the inputs of ComparisonCircuits. Wiring is explicit — use WireCircuit()
    // to connect a math output to a comparison input slot.
    public class CircuitBlock : BaseCircuit
    {
        private MathCircuit[] _mCircuit;
        private ComparisonCircuit[] _cCircuit;
        private Wire[][] _wires;

        private struct Wire
        {
            public bool Used;
            public int MathIndex;
        }

        public CircuitBlock(int m, int n, char[] mathOps, char[] compOps)
        {
            if (m < 0) throw new ArgumentOutOfRangeException(nameof(m));
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
            if (mathOps == null || mathOps.Length < m)
                throw new ArgumentException("mathOps must have at least m elements");
            if (compOps == null || compOps.Length < n)
                throw new ArgumentException("compOps must have at least n elements");

            _mCircuit = new MathCircuit[m];
            for (int i = 0; i < m; i++)
                _mCircuit[i] = new MathCircuit(0, 0, mathOps[i]);

            _cCircuit = new ComparisonCircuit[n];
            for (int i = 0; i < n; i++)
                _cCircuit[i] = new ComparisonCircuit(0, 0, compOps[i]);

            _wires = new Wire[n][];
            for (int i = 0; i < n; i++)
            {
                _wires[i] = new Wire[2];
                _wires[i][0] = new Wire { Used = false, MathIndex = -1 };
                _wires[i][1] = new Wire { Used = false, MathIndex = -1 };
            }
        }

        public override void SetNum1(int value)
        {
            foreach (var mc in _mCircuit) mc.SetNum1(value);
        }

        public override void SetNum2(int value)
        {
            foreach (var mc in _mCircuit) mc.SetNum2(value);
        }

        protected override void ValidateOperator(char op)
        {
            throw new NotSupportedException("CircuitBlock does not support SetOperator");
        }

        protected override int ComputeResult()
        {
            for (int i = 0; i < _wires.Length; i++)
            {
                if (_wires[i][0].Used)
                    _cCircuit[i].SetNum1(_mCircuit[_wires[i][0].MathIndex].GetResult());
                if (_wires[i][1].Used)
                    _cCircuit[i].SetNum2(_mCircuit[_wires[i][1].MathIndex].GetResult());
            }
            return _cCircuit[0].GetResult();
        }

        public void MathSet(int index, bool isNum1, int value)
        {
            if (index < 0 || index >= _mCircuit.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (isNum1) _mCircuit[index].SetNum1(value);
            else        _mCircuit[index].SetNum2(value);
        }

        public void OperUpdate(bool isMath, int index, char op)
        {
            if (isMath)
            {
                if (index < 0 || index >= _mCircuit.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _mCircuit[index].SetOperator(op);
            }
            else
            {
                if (index < 0 || index >= _cCircuit.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _cCircuit[index].SetOperator(op);
                for (int input = 0; input < 2; input++)
                {
                    if (_wires[index][input].Used)
                    {
                        int result = _mCircuit[_wires[index][input].MathIndex].GetResult();
                        if (input == 0) _cCircuit[index].SetNum1(result);
                        else            _cCircuit[index].SetNum2(result);
                    }
                }
            }
        }

        public void OperFreeze(bool isMath, int index)
        {
            if (isMath)
            {
                if (index < 0 || index >= _mCircuit.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _mCircuit[index].Freeze();
            }
            else
            {
                if (index < 0 || index >= _cCircuit.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _cCircuit[index].Freeze();
            }
        }

        public int CCircuitResult(int index)
        {
            if (index < 0 || index >= _cCircuit.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _cCircuit[index].GetResult();
        }

        public void WireCircuit(int mIndex, int cIndex, bool toNum1)
        {
            if (mIndex < 0 || mIndex >= _mCircuit.Length)
                throw new ArgumentOutOfRangeException(nameof(mIndex));
            if (cIndex < 0 || cIndex >= _cCircuit.Length)
                throw new ArgumentOutOfRangeException(nameof(cIndex));

            int result = _mCircuit[mIndex].GetResult();
            if (toNum1)
            {
                _cCircuit[cIndex].SetNum1(result);
                _wires[cIndex][0].Used = true;
                _wires[cIndex][0].MathIndex = mIndex;
            }
            else
            {
                _cCircuit[cIndex].SetNum2(result);
                _wires[cIndex][1].Used = true;
                _wires[cIndex][1].MathIndex = mIndex;
            }
        }

        public void UnuseWire(int cIndex, bool fromNum1)
        {
            if (cIndex < 0 || cIndex >= _cCircuit.Length)
                throw new ArgumentOutOfRangeException(nameof(cIndex));
            int wireIndex = fromNum1 ? 0 : 1;
            _wires[cIndex][wireIndex].Used = false;
            _wires[cIndex][wireIndex].MathIndex = -1;
        }

        public override ICircuit Clone() => CloneBlock();

        private CircuitBlock CloneBlock()
        {
            int m = _mCircuit.Length;
            int n = _cCircuit.Length;

            var mathOps = new char[m];
            for (int i = 0; i < m; i++) mathOps[i] = _mCircuit[i].CurrOperator;

            var compOps = new char[n];
            for (int i = 0; i < n; i++) compOps[i] = _cCircuit[i].CurrOperator;

            var copy = new CircuitBlock(m, n, mathOps, compOps);

            for (int i = 0; i < m; i++)
                copy._mCircuit[i] = (MathCircuit)_mCircuit[i].Clone();

            for (int i = 0; i < n; i++)
                copy._cCircuit[i] = (ComparisonCircuit)_cCircuit[i].Clone();

            for (int i = 0; i < _wires.Length; i++)
                for (int j = 0; j < _wires[i].Length; j++)
                    copy._wires[i][j] = new Wire { Used = _wires[i][j].Used, MathIndex = _wires[i][j].MathIndex };

            return copy;
        }

        public override void Serialize(TextWriter writer)
        {
            writer.WriteLine(_mCircuit.Length);
            writer.WriteLine(_cCircuit.Length);
            foreach (var mc in _mCircuit) mc.Serialize(writer);
            foreach (var cc in _cCircuit) cc.Serialize(writer);
            for (int i = 0; i < _wires.Length; i++)
                for (int j = 0; j < _wires[i].Length; j++)
                {
                    writer.WriteLine(_wires[i][j].Used ? "1" : "0");
                    writer.WriteLine(_wires[i][j].MathIndex);
                }
        }

        public override void Deserialize(TextReader reader)
        {
            int m = int.Parse(reader.ReadLine());
            int n = int.Parse(reader.ReadLine());

            _mCircuit = new MathCircuit[m];
            for (int i = 0; i < m; i++)
            {
                _mCircuit[i] = new MathCircuit(0, 0, '+');
                _mCircuit[i].Deserialize(reader);
            }

            _cCircuit = new ComparisonCircuit[n];
            for (int i = 0; i < n; i++)
            {
                _cCircuit[i] = new ComparisonCircuit(0, 0, '=');
                _cCircuit[i].Deserialize(reader);
            }

            _wires = new Wire[n][];
            for (int i = 0; i < n; i++)
            {
                _wires[i] = new Wire[2];
                for (int j = 0; j < 2; j++)
                {
                    bool used = reader.ReadLine() == "1";
                    int mathIdx = int.Parse(reader.ReadLine());
                    _wires[i][j] = new Wire { Used = used, MathIndex = mathIdx };
                }
            }
        }
    }
}
