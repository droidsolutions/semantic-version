using System.Collections;

namespace DroidSolutions.Oss.SemanticVersion;

/// <summary>
/// A comparer for Semantic Version objects.
/// </summary>
public class SemanticVersionComparer : IComparer, IComparer<ISemanticVersion>
{
  /// <summary>
  /// Compares both objects as <see cref="ISemanticVersion"/>.
  /// </summary>
  /// <param name="x">The first version.</param>
  /// <param name="y">The second version.</param>
  /// <returns><c>-1</c> if y is older than x, <c>1</c> if y is newer than x or <c>0</c> if they are equal.</returns>
  int IComparer.Compare(object? x, object? y)
  {
    return Compare((ISemanticVersion?)x, (ISemanticVersion?)y);
  }

  /// <summary>
  /// Compares two <see cref="ISemanticVersion"/> instances and returns an integer representing if y is older, newer or equal to x.
  /// </summary>
  /// <param name="x">The first version.</param>
  /// <param name="y">The second version.</param>
  /// <returns><c>-1</c> if y is older than x, <c>1</c> if y is newer than x or <c>0</c> if they are equal.</returns>
  public int Compare(ISemanticVersion? x, ISemanticVersion? y)
  {
    if (x == null && y == null)
    {
      return 0;
    }

    int nullCompare = CheckOneIsNull(x, y);
    if (nullCompare != 0)
    {
      return nullCompare;
    }

    var majorCompare = y!.Major.CompareTo(x!.Major);
    if (majorCompare != 0)
    {
      return majorCompare;
    }

    var minorCompare = y.Minor.CompareTo(x.Minor);
    if (minorCompare != 0)
    {
      return minorCompare;
    }

    var patchCompare = y.Patch.CompareTo(x.Patch);
    if (patchCompare != 0)
    {
      return patchCompare;
    }

    return new SemanticVersionPrereleaseComparer().Compare(x.PreRelease, y.PreRelease);
  }

  private static int CheckOneIsNull(ISemanticVersion? x, ISemanticVersion? y)
  {
    if (x == null && y != null)
    {
      return -1;
    }

    if (x != null && y == null)
    {
      return 1;
    }

    return 0;
  }
}
