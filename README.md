# DroidSolutions Semantic Version

This is a NuGet package containing a library that provides sort and compare functions for [Semantic Version](https://semver.org/) numbers.

[![Coverage Status](https://coveralls.io/repos/github/droidsolutions/semantic-version/badge.svg?branch=main)](https://coveralls.io/github/droidsolutions/semantic-version?branch=main)
![Nuget](https://img.shields.io/nuget/v/DroidSolutions.Oss.SemanticVersion)
[![semantic-release](https://img.shields.io/badge/%20%20%F0%9F%93%A6%F0%9F%9A%80-semantic--release-e10079.svg)](https://github.com/semantic-release/semantic-release)

# Installation

You can install this package via CLI: `dotnet add package DroidSolutions.Oss.SemanticVersion`.

If you want to install it in a project that is in a subdirectory give the path to the directory containing the `csproj` file or the path to the `csproj` like this: `dotnet add src/MyProject package DroidSolutions.Oss.SemanticVersion`.

# Usage

## ISemanticVersion

There is an interface `ISemanticVersion` for which comparers are implemented. Just create a class and implement the interface.

## SemanticVersionObject

You can also use the `SemanticVersionObject` class that already implements the comparer with some neat and handy additions.

Additionally `SemanticVersionObject` provides a `Build` property containing any build metadata. As per [specification](https://semver.org/#spec-item-10) build metadtaa data may only contain ASCII alphanumerics and hyphens `[0-9A-Za-z-]` and can be appended at the end of the version with a plus sign `+`.

### Constructor

When you create a new `SemanticVersionObject` instance you can give the version number parts straight away.

```csharp
SemanticVersionObject version = new();                   // 1.0.0
SemanticVersionObject version = new(2);                  // 2.0.0
SemanticVersionObject version = new(1, 1);               // 1.1.0
SemanticVersionObject version = new(1, 1, 1);            // 1.1.1
SemanticVersionObject version = new(1, 1, 1, "alpha.1"); // 1.1.1-alpha.1
SemanticVersionObject version = new(1, 1, 1, "alpha.1", "298a915a"); // 1.1.1-alpha.1+298a915a
SemanticVersionObject version = new(1, 1, 1, null, "298a915a"); // 1.1.1+298a915a
```

### FromString

There is also a static `FromString` Method which will return you a `SemanticVersionObject` instance from a version string.

```csharp
var version = SemanticVersionObject.FromString("v1.0.0-beta.1+298a915a985daeb426a0fe7543917874d7fa2995");
```

### FromVersion

There is a static method to take the version from a .NET `Version` instance called `FromVersion`. It takes the `Version` instance as a parameter.

The second parameter controls how build and revision are used. [The Version docs](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-version) state, that revision should be used for assemblies that fix security holes but are otherwise the same. This would match the sense of the `Patch` part of semantic version, so this is the default, setting the `Revision` property to the patch version.

However when using the `Version` build property in `.csproj` the generated version in `AssemblyInfo.cs` ends up in a way where patch version lands in the build property instead of revision. So when using the method with an Assembly version, you can pass `true` to the method and the patch version will be read from the `Build` property and `Revision` is used for build version.

## GetCurrentAppVersion

This retrieves the version from the current running application and returns a `SemanticVersionObject` for it. For this `Assembly.GetEntryAssembly()` is used.

### ToString

You can generate a version string with the `ToString` method:

```csharp
SemanticVersionObject version = new() { Major = 1, Minor = 2, Patch = 3, PreRelease = "beta.1", Build = "298a915a" };
Console.WriteLine(version.ToString()); // v1.2.3-beta.1+298a915a
```

If you do not want the leading `v` you can use the `ToVersionString` method and give false for the `withLeadingV` parameter:

```csharp
var version = new SemanticVersionObject { Major = 1, Minor = 4, Patch = 0 };
Console.WriteLine(version.ToVersionString(false)); // 1.4.0
```

### Sorting

As mentioned a default comparer is included which allows to sort version descending to have the newest version first. You can use the `Sort` LinQ method for this:

```csharp
List<SemanticVersionObject> list = new()
{
  SemanticVersionObject.FromString("1.0.0"),
  SemanticVersionObject.FromString("1.1.1"),
  SemanticVersionObject.FromString("1.1.0"),
  SemanticVersionObject.FromString("0.9.0"),
};

list.Sort();

Assert.Equal(
  new List<SemanticVersionObject>
  {
    SemanticVersionObject.FromString("1.1.1"),
    SemanticVersionObject.FromString("1.1.0"),
    SemanticVersionObject.FromString("1.0.0"),
    SemanticVersionObject.FromString("0.9.0"),
  },
  list);
```

You can also use the Array `Sort` method but then you have to specify the compare method yourself:

```csharp
var list = new SemanticVersionObject[]
{
  SemanticVersionObject.FromString("1.0.0"),
  SemanticVersionObject.FromString("1.1.1"),
  SemanticVersionObject.FromString("1.1.0"),
  SemanticVersionObject.FromString("0.9.0"),
};

Array.Sort(list, SemanticVersionObject.SortVersionDescending());

Assert.Equal(
  new List<SemanticVersionObject>
  {
    SemanticVersionObject.FromString("1.1.1"),
    SemanticVersionObject.FromString("1.1.0"),
    SemanticVersionObject.FromString("1.0.0"),
    SemanticVersionObject.FromString("0.9.0"),
  },
  list);
```

### Compare

The `SemanticVersionObject` class implements the `IComparable` interface so you can use the `CompareTo` on every instance. Just pass another instance to it. The result is an integer that tells you, if the given instance is newer (1), equal (0) or older (-1).

```csharp
SemanticVersionObject oldVersion = new(1, 0, 0);
SemanticVersionObject equalVersion = new(1, 0, 0);
SemanticVersionObject newVersion = new(1, 1, 0);

Console.WriteLine(oldVersion.CompareTo(newVersion)); // 1
Console.WriteLine(newVersion.CompareTo(oldVersion)); // -1
Console.WriteLine(oldVersion.CompareTo(equalVersion)); // 0
```

### General notes on sorting and comparing

Sorting (and comparing) takes into account if there is a prerelease or not. For example consider this:

```csharp
SemanticVersionObject x = new(1, 0, 0);
SemanticVersionObject y = new(1, 0, 0, "alpha.1");

Console.WriteLine(x.CompareTo(y)); // -1
```

Version without a prerelease are always newer if major, minor and patch are equal.

Also consider this:

```csharp
SemanticVersionObject x = new(1, 0, 0, "beta.2");
SemanticVersionObject y = new(1, 0, 0, "beta.10");

Console.WriteLine(x.CompareTo(y)); // 1
```

Prereleases will be split into their parts and then will be compared. Each part must be separated with a dot. So for example beta is higher than alpha but just because b is later in the alphabet than a.

```csharp
SemanticVersionObject x = new(1, 0, 0, "alpha.1");
SemanticVersionObject y = new(1, 0, 0, "beta.1");
SemanticVersionObject z = new(1, 0, 0, "develop.1");

Console.WriteLine(x.CompareTo(y)); // 1
Console.WriteLine(y.CompareTo(z)); // 1
```

As per [spec](https://semver.org/#spec-item-10) build metadata is ignored when comparing.

```csharp
List<SemanticVersionObject> list = new()
{
  SemanticVersionObject.FromString("1.0.0+200"),
  SemanticVersionObject.FromString("1.0.0+100"),
};

list.Sort();

Assert.Equal(
  new List<SemanticVersionObject>
  {
    SemanticVersionObject.FromString("1.0.0+200"),
    SemanticVersionObject.FromString("1.0.0+100"),
  },
  list);
```

### IsNewerThan, IsOlderThan

You can use the `IsNewerThan` or `IsOlderThan` method if you need a boolean value.

```csharp
var x = new SemanticVersionObject(2, 0, 0);
var y = new SemanticVersionObject(1, 0, 0);

var isNewer = x.IsNewerThan(y); // true
var isolder = y.IsOlderThan(x); // true
```

The methods only return `true` if the given instance is newer/older. If they are both the same or the given compare version is null `false` is returned.

### IsPreRelease

You can use the `IsPreRelease` method to determine if the given version is a pre release or a full semantic version.

```csharp
SemanticVersionObject.FromString("v2.0.3").IsPreRelease(); // false
SemanticVersionObject.FromString("3.1.3-beta.12").IsPreRelease(); // true
```

# Development

If you want to add a feature or fix a bug, be sure to read the [contribution guidelines](./CONTRIBUTING.md) first.

You'll need to install the .NET SDK which can be downloaded [here](https://dotnet.microsoft.com/en-us/download).

To build the project, just run `dotnet build` in the repository root. Tests can be executed with `dotnet test` and code coverage is generated by either running `dotnet test --collect:"XPlat Code Coverage"` or `dotnet test /p:CollectCoverage=true`.