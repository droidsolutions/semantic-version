using DroidSolutions.Oss.SemanticVersion;

using Xunit;

namespace DroidSolutions.Oss.SemanticVersionTest;

public class SemanticVersionComparerTest
{
  [Theory]
  [InlineData("1.0.0", "2.0.0", 1)]
  [InlineData("1.0.0", "1.1.0", 1)]
  [InlineData("1.0.0", "1.0.1", 1)]
  [InlineData("1.0.0-alpha.3", "1.0.0", 1)]
  [InlineData("1.0.0", "1.0.0", 0)]
  [InlineData("1.0.0-beta.4", "1.0.0-beta.4", 0)]
  [InlineData("1.0.1", "1.0.0", -1)]
  [InlineData("1.1.0", "1.0.0", -1)]
  [InlineData("2.0.0", "1.0.0", -1)]
  [InlineData("1.0.0", "1.0.0-develop.12", -1)]
  [InlineData("1.0.0", "1.0.0+abcdefg", 0)]
  [InlineData("1.0.0+111", "1.0.0+112", 0)]
  public void Compare_Returns_Expected(string x, string y, int expected)
  {
    ISemanticVersion versionX = SemanticVersionObject.FromString(x);
    ISemanticVersion versionY = SemanticVersionObject.FromString(y);

    SemanticVersionComparer comparer = new();
    Assert.Equal(expected, comparer.Compare(versionX, versionY));
  }

  [Theory]
  [InlineData(null, "1.0.0", -1)]
  [InlineData("1.0.0", null, 1)]
  [InlineData(null, null, 0)]
  public void Compare_HandlesNullValues(string? x, string? y, int expected)
  {
    ISemanticVersion? versionX = string.IsNullOrEmpty(x) ? null : SemanticVersionObject.FromString(x);
    ISemanticVersion? versionY = string.IsNullOrEmpty(y) ? null : SemanticVersionObject.FromString(y);

    SemanticVersionComparer comparer = new();
    Assert.Equal(expected, comparer.Compare(versionX, versionY));
  }

  [Theory]
  [InlineData("1.0.0+100", "1.0.0+200")]
  [InlineData("1.0.0+abc", "1.0.0+def")]
  public void Compare_IgnoresBuild(string x, string y)
  {
    ISemanticVersion versionX = SemanticVersionObject.FromString(x);
    ISemanticVersion versionY = SemanticVersionObject.FromString(y);

    SemanticVersionComparer comparer = new();
    Assert.Equal(0, comparer.Compare(versionX, versionY));
  }
}
