using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DroidSolutions.Oss.SemanticVersion;

/// <summary>
/// A model that represents a semantic version number.
/// </summary>
public class SemanticVersionObject : ISemanticVersion, IComparable, IEquatable<SemanticVersionObject>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SemanticVersionObject"/> class.
  /// </summary>
  public SemanticVersionObject()
    : this(1, 0, 0, string.Empty)
  { }

  /// <summary>
  /// Initializes a new instance of the <see cref="SemanticVersionObject"/> class.
  /// </summary>
  /// <param name="major">The major version.</param>
  public SemanticVersionObject(int major)
    : this(major, 0, 0, string.Empty)
  { }

  /// <summary>
  /// Initializes a new instance of the <see cref="SemanticVersionObject"/> class.
  /// </summary>
  /// <param name="major">The major version.</param>
  /// <param name="minor">The minor version.</param>
  public SemanticVersionObject(int major, int minor)
    : this(major, minor, 0, string.Empty)
  { }

  /// <summary>
  /// Initializes a new instance of the <see cref="SemanticVersionObject"/> class.
  /// </summary>
  /// <param name="major">The major version.</param>
  /// <param name="minor">The minor version.</param>
  /// <param name="patch">The patch version.</param>
  public SemanticVersionObject(int major, int minor, int patch)
    : this(major, minor, patch, string.Empty)
  { }

  /// <summary>
  /// Initializes a new instance of the <see cref="SemanticVersionObject"/> class.
  /// </summary>
  /// <param name="major">The major version.</param>
  /// <param name="minor">The minor version.</param>
  /// <param name="patch">The patch version.</param>
  /// <param name="prerelease">The prerelease version.</param>
  public SemanticVersionObject(int major, int minor, int patch, string prerelease)
  {
    Major = major;
    Minor = minor;
    Patch = patch;
    PreRelease = prerelease;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SemanticVersionObject"/> class.
  /// </summary>
  /// <param name="major">The major version.</param>
  /// <param name="minor">The minor version.</param>
  /// <param name="patch">The patch version.</param>
  /// <param name="prerelease">The prerelease version.</param>
  /// <param name="build">The build metadata.</param>
  public SemanticVersionObject(int major, int minor, int patch, string? prerelease, string build)
  {
    Major = major;
    Minor = minor;
    Patch = patch;
    PreRelease = prerelease;
    Build = build;
  }

  /// <summary>
  /// Gets or sets the major version number.
  /// </summary>
  public virtual int Major { get; set; }

  /// <summary>
  /// Gets or sets the minor version number.
  /// </summary>
  public virtual int Minor { get; set; }

  /// <summary>
  /// Gets or sets the patch version number.
  /// </summary>
  public virtual int Patch { get; set; }

  /// <summary>
  /// Gets or sets the additional label, like alpha, beta, pre-release or whatever.
  /// </summary>
  public virtual string? PreRelease { get; set; }

  /// <summary>
  /// Gets or sets the build metadata.
  /// </summary>
  public virtual string? Build { get; set; }

  /// <summary>
  /// Checks if both given items are equal.
  /// </summary>
  /// <param name="left">The left side of the comparison.</param>
  /// <param name="right">The right side of the comparison.</param>
  /// <returns><see langword="true"/> if both sides are equal, else <see langword="false"/>.</returns>
  public static bool operator ==(SemanticVersionObject? left, SemanticVersionObject? right)
  {
    return left?.CompareTo(right) == 0;
  }

  /// <summary>
  /// Checks if both given items are not equal.
  /// </summary>
  /// <param name="left">The left side of the comparison.</param>
  /// <param name="right">The right side of the comparison.</param>
  /// <returns><see langword="false"/> if both sides are equal, else <see langword="true"/>.</returns>
  public static bool operator !=(SemanticVersionObject? left, SemanticVersionObject? right)
  {
    return left?.CompareTo(right) != 0;
  }

  /// <summary>
  /// Checks if both given left is greater than right.
  /// </summary>
  /// <param name="left">The left side of the comparison.</param>
  /// <param name="right">The right side of the comparison.</param>
  /// <returns><see langword="true"/> if left is greater than the right side, else <see langword="false"/>.</returns>
  public static bool operator >(SemanticVersionObject? left, SemanticVersionObject? right)
  {
    return left?.CompareTo(right) < 0;
  }

  /// <summary>
  /// Checks if both given right is greater than left.
  /// </summary>
  /// <param name="left">The left side of the comparison.</param>
  /// <param name="right">The right side of the comparison.</param>
  /// <returns><see langword="true"/> if right is greater than the left side, else <see langword="false"/>.</returns>
  public static bool operator <(SemanticVersionObject? left, SemanticVersionObject? right)
  {
    return left?.CompareTo(right) > 0;
  }

  /// <summary>
  /// Checks if both given left is equal or greater than the right.
  /// </summary>
  /// <param name="left">The left side of the comparison.</param>
  /// <param name="right">The right side of the comparison.</param>
  /// <returns><see langword="true"/> if left is greater than the right side, else <see langword="false"/>.</returns>
  public static bool operator >=(SemanticVersionObject? left, SemanticVersionObject? right)
  {
    return left?.CompareTo(right) <= 0;
  }

  /// <summary>
  /// Checks if both given right is equal or greater than the left.
  /// </summary>
  /// <param name="left">The left side of the comparison.</param>
  /// <param name="right">The right side of the comparison.</param>
  /// <returns><see langword="true"/> if right is greater than the left side, else <see langword="false"/>.</returns>
  public static bool operator <=(SemanticVersionObject? left, SemanticVersionObject? right)
  {
    return left?.CompareTo(right) >= 0;
  }

  /// <summary>
  /// Creates a <see cref="SemanticVersion"/> instance by parsing the given string.
  /// </summary>
  /// <param name="version">A version string in semantic release format, where the elements are separated by dots. The version can
  /// optionally be prefixed with a "v".</param>
  /// <returns>A <see cref="SemanticVersionObject"/> instance with the parsed version numbers. </returns>
  public static SemanticVersionObject FromString(string version)
  {
    // @"^v?(\d+).(\d+).(\d+)(-(.*))?$",
    Regex regex = new(
      @"^v?(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<build>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
      RegexOptions.CultureInvariant);

    Match match = regex.Match(version);
    if (!match.Success)
    {
      throw new ArgumentException(
        $"The given string \"{version}\" is not a valid semantic version!", nameof(version));
    }

    SemanticVersionObject result = new(
      int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
      int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
      int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));

    if (match.Groups.ContainsKey("prerelease"))
    {
      result.PreRelease = match.Groups["prerelease"].Value;
    }

    if (match.Groups.ContainsKey("build"))
    {
      result.Build = match.Groups["build"].Value;
    }

    return result;
  }

  /// <summary>
  /// Returns a comparer that sorts a list of version descending by the newest first and the oldest last.
  /// </summary>
  /// <returns>An <see cref="IComparer"/>.</returns>
  public static IComparer SortVersionDescending()
  {
    return new SemanticVersionComparer();
  }

  /// <summary>
  /// Compare method that tells you if the given version is newer than this version.
  /// </summary>
  /// <param name="obj">The version to compare.</param>
  /// <returns><c>-1</c> if the given version is older, <c>1</c> if it is newer or <c>0</c> if they are equal.
  /// </returns>
  public int CompareTo(object? obj)
  {
    if (obj == null)
    {
      return -1;
    }

    if (obj is not SemanticVersionObject version)
    {
      throw new ArgumentException("Given object is not a SemanticVersionObject.", nameof(obj));
    }

    return new SemanticVersionComparer().Compare(this, version);
  }

  /// <summary>
  /// Returns a string representation of the Semantic Version.
  /// </summary>
  /// <returns>A string representing the version.</returns>
  public override string ToString()
  {
    return ToVersionString();
  }

  /// <summary>
  /// Returns a string representation of the Semantic Version.
  /// </summary>
  /// <param name="withLeadingV">If false, no leading v will be in the resulting string.</param>
  /// <returns>A string representation of the version.</returns>
  public string ToVersionString(bool withLeadingV = true)
  {
    var result = $"{Major}.{Minor}.{Patch}";

    if (withLeadingV)
    {
      result = $"v{result}";
    }

    if (!string.IsNullOrEmpty(PreRelease))
    {
      result = $"{result}-{PreRelease}";
    }

    if (!string.IsNullOrEmpty(Build))
    {
      result = $"{result}+{Build}";
    }

    return result;
  }

  /// <summary>
  /// Checks if this version is newer than the received version.
  /// </summary>
  /// <param name="compare">The version to check against.</param>
  /// <returns>A value indicating whether this instance is newer than the given value.</returns>
  public bool IsNewerThan(SemanticVersionObject? compare)
  {
    return CompareTo(compare) == -1;
  }

  /// <summary>
  /// Checks if this version is older than the received version.
  /// </summary>
  /// <param name="compare">The version to check against.</param>
  /// <returns>A value indicating whether this instance is older than the given value.</returns>
  public bool IsOlderThan(SemanticVersionObject? compare)
  {
    return CompareTo(compare) == 1;
  }

  /// <summary>
  /// Checks if the current version object is a pre-release version or not.
  /// </summary>
  /// <returns><see langword="true"/> if this version is a pre-release, else <see langword="false"/>.</returns>
  public bool IsPreRelease()
  {
    return !string.IsNullOrEmpty(PreRelease);
  }

  /// <inheritdoc/>
  public bool Equals(SemanticVersionObject? other)
  {
    return CompareTo(other) == 0;
  }

  /// <inheritdoc/>
  public override bool Equals(object? obj)
  {
    return Equals(obj as SemanticVersionObject);
  }

  /// <inheritdoc/>
  public override int GetHashCode()
  {
    unchecked
    {
      int hashCode = Major;
      hashCode = (hashCode * 397) ^ Minor;
      hashCode = (hashCode * 397) ^ Patch;
      hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(PreRelease) ? PreRelease.GetHashCode() : 0);

      return hashCode;
    }
  }
}
