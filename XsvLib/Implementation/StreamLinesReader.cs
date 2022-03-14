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

namespace XsvLib.Implementation
{
  /// <summary>
  /// Wraps a TextReader as ILinesReader
  /// </summary>
  public class StreamLinesReader: ILinesReader
  {
    private TextReader? _reader;

    /// <summary>
    /// Create a new StreamLinesReader
    /// </summary>
    public StreamLinesReader(TextReader reader, bool skipEmptyLines)
    {
      _reader = reader;
      SkipEmptyLines = skipEmptyLines;
    }

    /// <summary>
    /// When true, ReadLines() skips empty lines
    /// </summary>
    public bool SkipEmptyLines { get; }

    /// <summary>
    /// Implements ILinesReader
    /// </summary>
    public IEnumerable<string> ReadLines()
    {
      if(_reader == null)
      {
        throw new ObjectDisposedException(GetType().FullName);
      }
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
