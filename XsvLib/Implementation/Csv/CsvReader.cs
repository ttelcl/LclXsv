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

namespace XsvLib.Implementation.Csv
{
  /// <summary>
  /// Wraps a CsvParser and a source of text lines.
  /// Does not "own" the line source; the caller is responsible for closing it.
  /// </summary>
  public class CsvReader: ITextRecordReader
  {
    private readonly ILinesReader _reader;
    private char _separator;

    /// <summary>
    /// Create a new CsvReader.
    /// </summary>
    public CsvReader(ILinesReader reader, char separator = ',')
    {
      _reader = reader;
      _separator = separator;
      TrimSpaces = true;
      ParserState.TestValidSeparator(separator);
    }

    /// <summary>
    /// Create a new CsvReader.
    /// </summary>
    public CsvReader(TextReader reader, bool skipEmptyLines, char separator = ',')
      : this(reader.LinesFromTextReader(skipEmptyLines), separator)
    {
    }

    /// <summary>
    /// When true: skip empty lines
    /// </summary>
    public bool SkipEmptyLines { get; set; }

    /// <summary>
    /// When true (default): trim unescaped whitespace surrounding column values
    /// </summary>
    public bool TrimSpaces { get; set; }

    /// <summary>
    /// Change the separator for subsequent calls to ReadRecords
    /// </summary>
    public void ChangeSeparator(char separator)
    {
      ParserState.TestValidSeparator(separator);
      _separator = separator;
    }

    /// <summary>
    /// Read all records. Note that this interface has no concept of "headers"; there may or may
    /// not be header records included.
    /// </summary>
    public IEnumerable<IReadOnlyList<string>> ReadRecords()
    {
      return CsvParser.ParseLines(_reader.ReadLines(), _separator);
    }

  }

}
