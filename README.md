# godot-resources-cs

a modern c# implementation of a godot-inspired text resource system.

this project provides a "complete" lexer, parser, runtime model, serializer, and resource io pipeline for `.tres`-style resources using clean engine-grade architecture.

i want to add support for binary resources, scenes, and typed resource conversion in the future. idk

the system is designed to resemble real game engine tooling while remaining lightweight, extensible, and easy to maintain.

<div align="center">

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![License](https://img.shields.io/github/license/gabriel-aplok/godot-resources-cs?style=for-the-badge)
![Build](https://img.shields.io/github/actions/workflow/status/gabriel-aplok/godot-resources-cs/build.yml?style=for-the-badge)
![NuGet](https://img.shields.io/nuget/v/ImGui.NET?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/ImGui.NET?style=for-the-badge)

</div>

---

# features

- godot-inspired `.tres` resource syntax
- handwritten lexer/tokenizer
- recursive descent parser
- strongly-typed runtime resource model
- deterministic serializer/writer
- support for:
  - strings
  - integers
  - floats
  - booleans
  - null
  - arrays
  - dictionaries
  - external resources
  - subresources
  - comments
  - nested sections
- line/column parser diagnostics
- modern c# architecture
- extensible engine-style design
- zero parser generators
- zero third-party parsing libraries

---

# example resource

```gdscript
[gd_resource type="resource" script_class="itemdata" format=3 uid="uid://item123"]

[ext_resource type="texture2d" uid="uid://texture123" path="res://icon.svg" id="1_t"]

[resource]
name = "sword"
damage = 15
weight = 1.25
enabled = true
icon = extresource("1_t")
metadata/custom = "hello"
```

---

# architecture overview

the project is split into several independent systems.

## lexer

converts raw text into tokens.

supports:

- identifiers
- strings
- comments
- numbers
- punctuation
- brackets
- braces
- parentheses

example:

```text
name = "sword"
```

becomes:

```text
identifier(name)
equals
string("sword")
```

---

## parser

fully handwritten recursive descent parser.

it:

- consumes tokens
- validates syntax
- produces runtime objects
- reports detailed parse errors

i wanted to use it cuz:

- gives full syntax control
- is easier to extend
- resembles real engine tooling
- avoids parser generator complexity

---

## runtime model

runtime objects represent parsed resource data.

main types:

| type                   | description              |
| ---------------------- | ------------------------ |
| `resourcefile`         | root file model          |
| `resourcesection`      | section container        |
| `variant`              | generic value wrapper    |
| `externalresource`     | external asset reference |
| `resourcereference`    | resource links           |
| `extresourcereference` | external reference       |
| `subresourcereference` | subresource reference    |

---

## serializer

converts runtime objects back into text resources.

features:

- deterministic output
- stable formatting
- proper escaping
- multiline formatting
- indentation support
- clean readable output

---

# supported value types

## primitive values

```gdscript
health = 100
speed = 2.5
enabled = true
description = "hello"
empty = null
```

---

## arrays

```gdscript
tags = ["weapon", "melee", "rare"]
```

---

## dictionaries

```gdscript
stats = {
    "damage": 25,
    "crit": 0.15
}
```

---

## resource references

```gdscript
icon = extresource("1_tex")
mesh = subresource("1_mesh")
```

---

# getting started

## requirements

- .net 8 sdk or newer
- c# 12

---

## clone repository

```bash
git clone https://github.com/gabriel-aplok/godot-resources-cs.git
cd godot-resources-cs
```

---

## build

```bash
dotnet build
```

---

## run example

```bash
dotnet run
```

---

# loading resources

```csharp
using GodotResources.Core.IO;

var file = ResourceLoader.Load("item.tres");
```

---

# reading values

```csharp
string name = file.Resource.Get<string>("name");

int damage = file.Resource.Get<int>("damage");
```

---

# editing values

```csharp
file.Resource["damage"] = 25;

file.Resource["weight"] = 2.0f;
```

---

# saving resources

```csharp
ResourceSaver.Save(file, "item_modified.tres");
```

---

# example full usage

```csharp
using GodotResources.Core.IO;
using GodotResources.Core.Runtime;

ResourceFile file = ResourceLoader.Load("item.tres");

string name = file.Resource.Get<string>("name");

int damage =
    file.Resource.Get<int>("damage");

Console.WriteLine($"loaded item: {name}");

file.Resource["damage"] =
    new Variant(25);

ResourceSaver.Save(
    file,
    "item_modified.tres");
```

---

# parser diagnostics

parser exceptions include:

- token information
- line numbers
- column numbers
- expected token types

example:

```text
expected equals but got rightbracket at 12:5
```

---

# design goals

the project prioritizes:

- correctness
- maintainability
- extensibility
- readability
- deterministic serialization
- engine-grade architecture

not:

- minimalism
- reflection-heavy systems
- hidden magic
- parser generators

---

# future plans

planned future features:

- binary `.res` support
- embedded resources
- typed object conversion
- scene support
- inheritance
- custom serializers
- editor integration
- hot reload support
- incremental parsing
- source generators
- async io pipeline

---

# inspiration

this project is heavily inspired by:

- godot engine
- unreal asset pipelines
- custom engine tooling
- modern serialization systems

---

# license

mit license

---

# notes

this project is not affiliated with or endorsed by the godot engine project.

godot is a trademark of the godot foundation.
