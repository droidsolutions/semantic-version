namespace DroidSolutions.Oss.SemanticVersion;

/// <summary>
/// An interface representing a Semantic Version.
/// </summary>
public interface ISemanticVersion
{
  /// <summary>
  /// Gets the major version.
  /// </summary>
  int Major { get; }

  /// <summary>
  /// Gets the minor version.
  /// </summary>
  int Minor { get; }

  /// <summary>
  /// Gets a patch version.
  /// </summary>
  int Patch { get; }

  /// <summary>
  /// Gets the prerelease version.
  /// </summary>
  string? PreRelease { get; }
}
