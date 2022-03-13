/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XsvLib.Implementation.Csv;
using XsvLib.Implementation;

namespace XsvLib
{
  /// <summary>
  /// Static class acting as API entrypoint for this library
  /// </summary>
  public static class Xsv
  {
    /// <summary>
    /// Open a CSV file and return it as an object that implements IDisposable and ITextRecordReader
    /// </summary>
    /// <param name="filename">
    /// The file to open
    /// </param>
    /// <param name="skipEmptyLines">
    /// Whether or not to skip empty lines. Default true.
    /// </param>
    /// <param name="separator">
    /// The CSV separator character to use. Default ','. This method does not attempt to
    /// automatically guess the separator.
    /// </param>
    /// <returns>
    /// An object implementing both IDisposable and ITextRecordReader
    /// </returns>
    public static IDisposableTextRecordReader OpenCsv(
      string filename, bool skipEmptyLines = true, char separator = ',')
    {
      var reader = File.OpenText(filename);
      return new TextRecordReaderWrapper(new CsvReader(reader, skipEmptyLines, separator), reader);
    }

    /// <summary>
    /// Expose an in-memory collection of CSV formatted lines as an ITextRecordReader
    /// </summary>
    public static ITextRecordReader ParseCsv(ICollection<string> lines, char separator = ',')
    {
      return new DelegateTextRecordReader(
        () => CsvParser.ParseLines(lines, separator));
    }

  }

}
