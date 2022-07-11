/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib
{
  /// <summary>
  /// Identifier constants for supported formats of XSV-like files
  /// </summary>
  public static class XsvFormat
  {
    /// <summary>
    /// Indicates a CSV file
    /// </summary>
    public const string Csv = ".csv";

    /// <summary>
    /// Indicates a TSV file
    /// </summary>
    public const string Tsv = ".tsv";

    /// <summary>
    /// Determines the XSV format based on a file name or partial file name
    /// (only the end of the name, including the full extension, is needed)
    /// </summary>
    /// <param name="filename">
    /// The file name or partial file name to determine the format of.
    /// It is sufficient if this contains just the file extension (including
    /// the leading '.').
    /// </param>
    /// <param name="supportTmp">
    /// Default false. If true, also file extensions with an additional
    /// ".tmp" are recognized ("*.csv.tmp", "*.tsv.tmp")
    /// </param>
    /// <returns>
    /// The determined format constant, or null if not recognized.
    /// </returns>
    public static string? XsvFromFilename(string filename, bool supportTmp = false)
    {
      foreach(var format in new[] { Csv, Tsv, })
      {
        if(filename.EndsWith(format, StringComparison.OrdinalIgnoreCase))
        {
          return format;
        }
        if(supportTmp && filename.EndsWith(format + ".tmp", StringComparison.OrdinalIgnoreCase))
        {
          return format;
        }
      }
      return null;
    }

  }
}
