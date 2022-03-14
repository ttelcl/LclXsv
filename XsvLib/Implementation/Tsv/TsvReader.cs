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

using XsvLib;

namespace XsvLib.Implementation.Tsv
{
  /// <summary>
  /// Implements ITextRecordReader for TSV files
  /// </summary>
  public class TsvReader: ITextRecordReader
  {
    private readonly ILinesReader _reader;

    /// <summary>
    /// Create a new TsvReader
    /// </summary>
    public TsvReader(ILinesReader reader)
    {
      _reader = reader;
    }

    /// <summary>
    /// Create a new TsvReader
    /// </summary>
    public TsvReader(TextReader reader, bool skipEmptyLines)
      : this(reader.LinesFromTextReader(skipEmptyLines))
    {
    }

    /// <summary>
    /// Read all records. Note that this interface has no concept of "headers"; there may or may
    /// not be header records included.
    /// </summary>
    public IEnumerable<IReadOnlyList<string>> ReadRecords()
    {
      foreach(var line in _reader.ReadLines())
      {
        var fields = line.Split('\t');
        yield return fields;
      }
    }

  }
}
