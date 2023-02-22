using System;
using System.Collections.Generic;

using DroidSolutions.Oss.SemanticVersion;

using Xunit;

namespace DroidSolutions.Oss.SemanticVersionTest;

public class SemanticVersionObjectTest
{
  public static IEnumerable<object[]> FromStringTestData() => new[]
  {
    new object[] { "1.0.0", new SemanticVersionObject(1, 0, 0) },
    new object[] { "v1.0.0", new SemanticVersionObject(1, 0, 0) },
    new object[] { "1.1.1", new SemanticVersionObject(1, 1, 1) },
    new object[] { "11.111.1111", new SemanticVersionObject(11, 111, 1111) },
    new object[] { "1.0.0-alpha.1", new SemanticVersionObject(1, 0, 0, "alpha.1") },
    new object[] { "1.0.0-beta.12", new SemanticVersionObject(1, 0, 0, "beta.12") },
  };

  [Theory]
  [MemberData(nameof(FromStringTestData))]
  public void FromString_ShouldParseVersionString(string version, SemanticVersionObject expected)
  {
    ISemanticVersion actual = SemanticVersionObject.FromString(version);
    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Constructor_ShouldInitializeCorrectly()
  {
    Assert.Equal(new SemanticVersionObject(), new SemanticVersionObject { Major = 1 });
    Assert.Equal(new SemanticVersionObject(1), new SemanticVersionObject { Major = 1 });
    Assert.Equal(new SemanticVersionObject(1, 1), new SemanticVersionObject { Major = 1, Minor = 1 });
    Assert.Equal(new SemanticVersionObject(1, 1, 1), new SemanticVersionObject { Major = 1, Minor = 1, Patch = 1 });
    Assert.Equal(
      new SemanticVersionObject(1, 1, 1, "alpha.1"),
      new SemanticVersionObject { Major = 1, Minor = 1, Patch = 1, PreRelease = "alpha.1" });
  }

  [Theory]
  [InlineData("v1")]
  [InlineData("v1.2")]
  [InlineData("v1.2.a")]
  public void FromString_ThrowsArgumentException_IfInvalidVersionGiven(string version)
  {
    Assert.Throws<ArgumentException>(() => SemanticVersionObject.FromString(version));
  }

  [Fact]
  public void ListOf_SemanticVersionObject_ShouldBeSortedDescending()
  {
    var list = new List<SemanticVersionObject>
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
  }

  [Fact]
  public void SortVersionDescending_ShouldSortDescending()
  {
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
  }

  [Theory]
  [InlineData("1.1.0", 1)]
  [InlineData("0.9.0", -1)]
  [InlineData("1.0.0", 0)]
  public void CompareTo_ShouldCompareCorrectly(string compare, int expected)
  {
    var x = new SemanticVersionObject
    {
      Major = 1,
      Minor = 0,
      Patch = 0,
    };
    ISemanticVersion y = SemanticVersionObject.FromString(compare);

    Assert.Equal(expected, x.CompareTo(y));
  }

  [Fact]
  public void CompareTo_ShouldReturnMinus1IfGivenNull()
  {
    Assert.Equal(-1, new SemanticVersionObject().CompareTo(null));
  }

  [Fact]
  public void CompareTo_ShouldThrowIfGivenNoSemanticVersionObject()
  {
    Assert.Throws<ArgumentException>(() => new SemanticVersionObject().CompareTo(new object()));
  }

  [Theory]
  [InlineData("v1.0.0", true, "v1.0.0")]
  [InlineData("v22.222.2222", false, "22.222.2222")]
  [InlineData("v3333.33.333-develop.16", true, "v3333.33.333-develop.16")]
  public void ToVersionString_ShouldWorkCorrectly(string input, bool withV, string expected)
  {
    var version = SemanticVersionObject.FromString(input) as SemanticVersionObject;
    Assert.Equal(expected, version.ToVersionString(withV));
  }

  [Fact]
  public void ToString_ReturnsVersionWithLeadingV()
  {
    var version = new SemanticVersionObject { Major = 12, Minor = 13, Patch = 14, PreRelease = "alpha.3" };
    Assert.Equal("v12.13.14-alpha.3", version.ToString());
  }

  [Theory]
  [InlineData("v1.0.0", "v1.1.0", false)]
  [InlineData("v1.0.0", "v1.0.0", false)]
  [InlineData("v1.1.0", "v1.0.0", true)]
  public void IsNewer_ShouldWork(string ownVersion, string otherVersion, bool expected)
  {
    var version = SemanticVersionObject.FromString(ownVersion);
    var actual = version.IsNewerThan(SemanticVersionObject.FromString(otherVersion));
    Assert.Equal(expected, actual);
  }

  [Theory]
  [InlineData("v1.1.0", "v1.0.0", false)]
  [InlineData("v1.0.0", "v1.0.0", false)]
  [InlineData("v1.0.0", "v1.1.0", true)]
  public void IsOlder_ShouldWork(string ownVersion, string otherVersion, bool expected)
  {
    var version = SemanticVersionObject.FromString(ownVersion);
    var actual = version.IsOlderThan(SemanticVersionObject.FromString(otherVersion));
    Assert.Equal(expected, actual);
  }

  [Theory]
  [InlineData("v1.0.0", false)]
  [InlineData("2.3.4", false)]
  [InlineData("3.2.1-alpha.1", true)]
  [InlineData("4.2.1-beta.15-special", true)]
  [InlineData("5.0.5-develop.69-build_420", true)]
  public void IsPreRelease_ShouldWork(string version, bool expected)
  {
    SemanticVersionObject semanticVersion = SemanticVersionObject.FromString(version);
    Assert.Equal(expected, semanticVersion.IsPreRelease());
  }

  [Theory]
  [InlineData("v1.0.0", "v1.0.0", true)]
  [InlineData("v1.0.0", "1.0.0", true)]
  [InlineData("v1.0.0", "v1.0.1", false)]
  [InlineData("v1.0.1", "v1.0.0", false)]
  public void IEquatable_Equals_ShouldWork(string version1, string version2, bool expected)
  {
    SemanticVersionObject semanticVersion1 = SemanticVersionObject.FromString(version1);
    SemanticVersionObject semanticVersion2 = SemanticVersionObject.FromString(version2);
    Assert.Equal(expected, semanticVersion1.Equals(semanticVersion2));
  }

  [Theory]
  [InlineData("v1.0.0", "v1.0.0", true)]
  [InlineData("v1.0.0", "1.0.0", true)]
  [InlineData("v1.0.0", "v1.0.1", false)]
  [InlineData("v1.0.1", "v1.0.0", false)]
  public void Object_Equals_ShouldWork(string version1, string version2, bool expected)
  {
    SemanticVersionObject semanticVersion1 = SemanticVersionObject.FromString(version1);
    SemanticVersionObject semanticVersion2 = SemanticVersionObject.FromString(version2);
    Assert.Equal(expected, semanticVersion1.Equals((object)semanticVersion2));
  }

  [Theory]
  [InlineData("v1.0.0", "v1.0.0")]
  [InlineData("v1.0.0", "1.0.0")]
  [InlineData("v1.0.0-beta.1", "1.0.0-beta.1")]
  public void Object_GetHashCode_ShouldWork(string version1, string version2)
  {
    SemanticVersionObject semanticVersion1 = SemanticVersionObject.FromString(version1);
    SemanticVersionObject semanticVersion2 = SemanticVersionObject.FromString(version2);

    Assert.Equal(semanticVersion1.GetHashCode(), semanticVersion2.GetHashCode());
  }

  [Theory]
  [InlineData("v1.0.0", "v1.0.0", true)]
  [InlineData("v1.0.0", "1.0.0", true)]
  [InlineData("v1.0.1", "v1.0.0", false)]
  [InlineData("v1.0.0-beta.1", "1.0.0-beta.1", true)]
  [InlineData(null, "v1.0.0", false)]
  [InlineData("v1.0.0", null, false)]
  public void EqualityOperator_ShouldWork(string? version1, string? version2, bool expected)
  {
    SemanticVersionObject? semanticVersion1 = version1 == null ? null : SemanticVersionObject.FromString(version1);
    SemanticVersionObject? semanticVersion2 = version2 == null ? null : SemanticVersionObject.FromString(version2);

    Assert.Equal(expected, semanticVersion1 == semanticVersion2);
  }

  [Theory]
  [InlineData("v1.0.0", "v1.0.0", false)]
  [InlineData("v1.0.0", "1.0.0", false)]
  [InlineData("v1.0.1", "v1.0.0", true)]
  [InlineData("v1.0.0-beta.1", "1.0.0-beta.1", false)]
  [InlineData(null, "v1.0.0", true)]
  [InlineData("v1.0.0", null, true)]
  public void UnequalityOperator_ShouldWork(string? version1, string? version2, bool expected)
  {
    SemanticVersionObject? semanticVersion1 = version1 == null ? null : SemanticVersionObject.FromString(version1);
    SemanticVersionObject? semanticVersion2 = version2 == null ? null : SemanticVersionObject.FromString(version2);

    Assert.Equal(expected, semanticVersion1 != semanticVersion2);
  }

  [Theory]
  [InlineData("v1.0.0", "v1.0.0", false)]
  [InlineData("v1.0.0", "1.0.1", false)]
  [InlineData("v2.0.0", "v2.0.0-beta.1", true)]
  [InlineData("v3.0.0", "3.1.0", false)]
  [InlineData(null, "v1.0.0", false)]
  [InlineData("v1.0.0", null, true)]
  public void GreaterThanOperator_ShouldWork(string? greater, string? lower, bool expected)
  {
    SemanticVersionObject? greaterSemanticVersion = greater == null ? null : SemanticVersionObject.FromString(greater);
    SemanticVersionObject? lowerSemanticVersion = lower == null ? null : SemanticVersionObject.FromString(lower);

    Assert.Equal(expected, greaterSemanticVersion > lowerSemanticVersion);
  }

  [Theory]
  [InlineData("v1.0.0", "v1.0.0", false)]
  [InlineData("v1.0.0", "1.0.1", true)]
  [InlineData("v2.0.0", "v2.0.0-beta.1", false)]
  [InlineData("v3.0.0", "3.1.0", true)]
  [InlineData(null, "v1.0.0", false)]
  [InlineData("v1.0.0", null, false)]
  public void LowerThanOperator_ShouldWork(string? lower, string? greater, bool expected)
  {
    SemanticVersionObject? lowerSemanticVersion = lower == null ? null : SemanticVersionObject.FromString(lower);
    SemanticVersionObject? greaterSemanticVersion = greater == null ? null : SemanticVersionObject.FromString(greater);

    bool actual = lowerSemanticVersion < greaterSemanticVersion;
    Assert.Equal(expected, actual);
  }

  [Theory]
  [InlineData("v1.0.0", "v1.0.0", true)]
  [InlineData("v1.0.0", "1.0.1", false)]
  [InlineData("v2.0.0", "v2.0.0-beta.1", true)]
  [InlineData("v3.0.0", "3.1.0", false)]
  [InlineData(null, "v1.0.0", false)]
  [InlineData("v1.0.0", null, true)]
  public void GreaterThanOrEqualOperator_ShouldWork(string? greater, string? lower, bool expected)
  {
    SemanticVersionObject? greaterSemanticVersion = greater == null ? null : SemanticVersionObject.FromString(greater);
    SemanticVersionObject? lowerSemanticVersion = lower == null ? null : SemanticVersionObject.FromString(lower);

    Assert.Equal(expected, greaterSemanticVersion >= lowerSemanticVersion);
  }

  [Theory]
  [InlineData("v1.0.0", "v1.0.0", true)]
  [InlineData("v1.0.0", "1.0.1", true)]
  [InlineData("v2.0.0", "v2.0.0-beta.1", false)]
  [InlineData("v3.0.0", "3.1.0", true)]
  [InlineData(null, "v1.0.0", false)]
  [InlineData("v1.0.0", null, false)]
  public void LowerThanOrEqualOperator_ShouldWork(string? lower, string? greater, bool expected)
  {
    SemanticVersionObject? lowerSemanticVersion = lower == null ? null : SemanticVersionObject.FromString(lower);
    SemanticVersionObject? greaterSemanticVersion = greater == null ? null : SemanticVersionObject.FromString(greater);

    bool actual = lowerSemanticVersion <= greaterSemanticVersion;
    Assert.Equal(expected, actual);
  }
}
