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
        (SemanticVersionObject)SemanticVersionObject.FromString("1.0.0"),
        (SemanticVersionObject)SemanticVersionObject.FromString("1.1.1"),
        (SemanticVersionObject)SemanticVersionObject.FromString("1.1.0"),
        (SemanticVersionObject)SemanticVersionObject.FromString("0.9.0"),
      };

    list.Sort();

    Assert.Equal(
      new List<SemanticVersionObject>
      {
          (SemanticVersionObject)SemanticVersionObject.FromString("1.1.1"),
          (SemanticVersionObject)SemanticVersionObject.FromString("1.1.0"),
          (SemanticVersionObject)SemanticVersionObject.FromString("1.0.0"),
          (SemanticVersionObject)SemanticVersionObject.FromString("0.9.0"),
      },
      list);
  }

  [Fact]
  public void SortVersionDescending_ShouldSortDescending()
  {
    var list = new SemanticVersionObject[]
    {
        (SemanticVersionObject)SemanticVersionObject.FromString("1.0.0"),
        (SemanticVersionObject)SemanticVersionObject.FromString("1.1.1"),
        (SemanticVersionObject)SemanticVersionObject.FromString("1.1.0"),
        (SemanticVersionObject)SemanticVersionObject.FromString("0.9.0"),
    };

    Array.Sort(list, SemanticVersionObject.SortVersionDescending());

    Assert.Equal(
      new List<SemanticVersionObject>
      {
          (SemanticVersionObject)SemanticVersionObject.FromString("1.1.1"),
          (SemanticVersionObject)SemanticVersionObject.FromString("1.1.0"),
          (SemanticVersionObject)SemanticVersionObject.FromString("1.0.0"),
          (SemanticVersionObject)SemanticVersionObject.FromString("0.9.0"),
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
  public void CompareTo_ShouldReturn1IfGivenNull()
  {
    Assert.Equal(1, new SemanticVersionObject().CompareTo(null));
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
}
