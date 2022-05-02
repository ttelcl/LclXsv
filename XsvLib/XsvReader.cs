/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XsvLib.Utilities;

namespace XsvLib
{
  /// <summary>
  /// Wraps an ITextRecordReader and separates its header line
  /// </summary>
  public class XsvReader: IDisposableTextRecordReader
  {
    private readonly ITextRecordReader _reader;
    private bool _disposed;

    /// <summary>
    /// Create a new XsvReader and read the header line from the source
    /// </summary>
    public XsvReader(
      ITextRecordReader itrr,
      bool ownsReader = true)
    {
      _reader = itrr;
      OwnsReader = ownsReader;
      Sequencer = new Subsequencer<IReadOnlyList<string>>(itrr.ReadRecords());
      Header = Sequencer.Next().ToList().AsReadOnly();
    }

    /// <summary>
    /// The header
    /// </summary>
    public IReadOnlyList<string> Header { get; }

    /// <summary>
    /// Implements ITextRecordReader, returning the remainder of
    /// the records (after extracting the header).
    /// Implemented as "return Sequencer.Rest();"
    /// </summary>
    public IEnumerable<IReadOnlyList<string>> ReadRecords()
    {
      return Sequencer.Rest();
    }

    /// <summary>
    /// The sequencer providing the content lines after the header (for
    /// advanced use cases)
    /// </summary>
    public Subsequencer<IReadOnlyList<string>> Sequencer { get; }
    
    /// <summary>
    /// True if this XsvReader "owns" the wrapped ITextRecordReader.
    /// In that case Disposing this XsvReader also disposes the TextRecordReader
    /// if it supports IDisposable
    /// </summary>
    public bool OwnsReader { get; }

    /// <summary>
    /// Clean up
    /// </summary>
    public void Dispose()
    {
      if(!_disposed)
      {
        _disposed = true;
        Sequencer.Dispose();
        if(OwnsReader && _reader is IDisposable disp)
        {
          disp.Dispose();
        }
      }
    }
  }
}
