using DroidSolutions.Oss.SemanticVersion;

using Xunit;

namespace DroidSolutions.Oss.SemanticVersionTest;

public class SemanticVersionPrereleaseComparerTest
{
  [Theory]
  [InlineData("", "", 0)]
  [InlineData("", "alpha.1", -1)]
  [InlineData("beta.2", "", 1)]
  [InlineData("beta.1", "beta.2", 1)]
  [InlineData("beta.3", "beta.20", 1)]
  [InlineData("alpha.30", "beta.20", -1)]
  [InlineData("alpha", "20", 1)]
  [InlineData("20", "develop.12", -1)]
  public void Compare_ReturnsCorrectResult(string x, string y, int expected)
  {
    var comparer = new SemanticVersionPrereleaseComparer();
    Assert.Equal(expected, comparer.Compare(x, y));
  }
}
