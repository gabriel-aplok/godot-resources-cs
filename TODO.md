godot-inspired c# resource system with a handwritten lexer, parser, serializer, and runtime model for .tres-style files.

# future extensions

this architecture cleanly supports future features:

## binary resource support

```csharp
IBinaryResourceSerializer
```

---

## scene support

```text
[node]
[connection]
```

sections.

---

## [✅] typed resource conversion

```csharp
IResourceConverter<T>
```

---

## [✅] nested resources

add recursive parsing:

```gdscript
resource = {
    "nested": SubResource("1")
}
```

---

## deterministic serialization

current writer already:

- preserves order
- deterministic formatting
- stable output

---

# important design notes

## why recursive descent

manual recursive descent:

- provides engine-level control
- better diagnostics
- easier custom syntax support
- avoids parser generator complexity

---

## why variant

Godot-like resources require:

- dynamic runtime typing
- serialization abstraction
- flexible editor tooling

variant acts as a universal container.

---

## engine style architecture

the implementation resembles:

- godot internals
- unreal asset systems
- custom engine tooling

because:

- lexer/parser separation
- runtime model abstraction
- deterministic serialization
- explicit references
- future-proof extensibility
