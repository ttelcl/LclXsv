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
  /// Composes a TextReader and a CsvParser. Does not "own" the TextReader; the caller
  /// is responsible for closing it.
  /// </summary>
  public class CsvReader: ITextRecordReader
  {
    private readonly TextReader _reader;
    private char _separator;

    /// <summary>
    /// Create a new CsvReader.
    /// </summary>
    public CsvReader(TextReader reader, bool skipEmptyLines, char separator = ',')
    {
      _reader = reader;
      _separator = separator;
      SkipEmptyLines = skipEmptyLines;
      TrimSpaces = true;
      ParserState.TestValidSeparator(separator);
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
      return CsvParser.ParseLines(ReadLines(), _separator);
    }

    /// <summary>
    /// Read all lines
    /// </summary>
    internal IEnumerable<string> ReadLines()
    {
      while(true)
      {
        var line = _reader.ReadLine();
        if(line == null)
        {
          yield break;
        }
        if(line.Length > 0 || !SkipEmptyLines)
        {
          yield return line;
        }
      }
    }

  }

}
