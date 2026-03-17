# Circuit Engine (C#)

A polymorphic circuit simulation engine in C#. Circuits are composable units — chain them, wire their outputs together, serialize their state, or deep-clone the entire graph.

## Features

- **MathCircuit** — arithmetic (`+`, `-`, `*`, `/`, `%`, `^`)
- **ComparisonCircuit** — logical comparisons (`=`, `!`, `<`, `>`) returning 0 or 1
- **MinMaxCircuit** — returns min or max of two inputs
- **LinearCircuit** — chains circuits in sequence; each output feeds into the next as input
- **CircuitBlock** — composite of math + comparison circuits with explicit output-to-input wiring
- **CircuitIterator** — cursor over LinearCircuit with bounds-safe arithmetic operators
- **Freeze** — lock a circuit's operator to make it immutable
- **Serialization** — save and restore full circuit state to/from text streams
- **Clone** — deep copy any circuit, preserving all internal state

## Design

```
ICircuit (interface)
  └── BaseCircuit (operator + freeze logic, serialization hooks)
        ├── MathCircuit
        ├── ComparisonCircuit
        ├── MinMaxCircuit
        └── CircuitBlock (wraps MathCircuit[] + ComparisonCircuit[] + wire map)
```

The Template Method pattern governs serialization: `BaseCircuit.SerializeCommon` / `DeserializeCommon` handle shared state; subclasses extend with type-specific fields.

## Build & Run

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run
```

## Example

```csharp
// Chain: 7+19=26, 26-9=17, 17*2=34
var lc = new LinearCircuit();
lc.Insert(lc.End(), new MathCircuit(7, 19, '+'));
lc.Insert(lc.End(), new MathCircuit(0,  9, '-'));
lc.Insert(lc.End(), new MathCircuit(0,  2, '*'));
Console.WriteLine(lc.LastOutput()); // 34

// Wire math outputs into a comparison circuit
var cb = new CircuitBlock(2, 1, new[]{ '+', '-' }, new[]{ '>' });
cb.WireCircuit(0, 0, true);   // math[0] -> comp[0].num1
cb.WireCircuit(1, 0, false);  // math[1] -> comp[0].num2
Console.WriteLine(cb.GetResult()); // 1 if math[0] > math[1]
```

## See Also

[circuit-engine-cpp](../circuit-engine-cpp) — the original C++ implementation of this engine, featuring manual memory management with smart pointers and a custom STL-style iterator.
