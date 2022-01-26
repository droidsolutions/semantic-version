namespace DroidSolutions.Oss.SemanticVersion;

/// <summary>
/// Comparer for prerelease strings.
/// </summary>
public class SemanticVersionPrereleaseComparer : IComparer<string>
{
  /// <summary>
  /// Compares the given prerelease strings and returns an integer representing if y is newer than x.
  /// </summary>
  /// <param name="x">The first prerelease string.</param>
  /// <param name="y">The second prerelease string.</param>
  /// <returns><c>-1</c> if y is older than x, <c>1</c> if y is newer than x or <c>0</c> if they are equal.</returns>
  public int Compare(string? x, string? y)
  {
    if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
    {
      return 0;
    }

    // If the first version has a pre release but the second version not, consider it as newer.
    if (!string.IsNullOrWhiteSpace(x) && string.IsNullOrWhiteSpace(y))
    {
      return 1;
    }

    // If the first version doesn't have a pre release but the second version does, consider it as older.
    if (string.IsNullOrWhiteSpace(x) && !string.IsNullOrWhiteSpace(y))
    {
      return -1;
    }

    var xComponents = x?.Split('.') ?? Array.Empty<string>();
    var yComponents = y?.Split('.') ?? Array.Empty<string>();

    var minLen = Math.Min(xComponents.Length, yComponents.Length);
    for (var i = 0; i < minLen; i++)
    {
      var xComponent = xComponents[i];
      var yComponent = yComponents[i];

      var xIsNumber = int.TryParse(xComponent, out var xNumber);
      var yIsNumber = int.TryParse(yComponent, out var yNumber);

      int result;

      if (xIsNumber && yIsNumber)
      {
        result = yNumber.CompareTo(xNumber);
        if (result != 0)
        {
          return result;
        }
      }
      else if (xIsNumber)
      {
        return -1;
      }
      else if (yIsNumber)
      {
        return 1;
      }
      else
      {
        result = string.CompareOrdinal(xComponent, yComponent);
        if (result != 0)
        {
          return result;
        }
      }
    }

    return xComponents.Length.CompareTo(yComponents.Length);
  }
}
