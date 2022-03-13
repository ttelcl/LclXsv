/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Implementation
{
  /// <summary>
  /// Combines an ITextRecordReader instance together with resources that should
  /// be disposed when done with it.
  /// </summary>
  public class TextRecordReaderWrapper: IDisposableTextRecordReader, IDisposable, ITextRecordReader
  {
    private IDisposable[]? _disposables;

    /// <summary>
    /// Create a new TextRecordReaderWrapper
    /// </summary>
    public TextRecordReaderWrapper(
      ITextRecordReader rdr,
      params IDisposable[] disposables)
    {
      Reader = rdr;
      _disposables = disposables;
    }

    /// <summary>
    /// The wrapped Reader
    /// </summary>
    public ITextRecordReader Reader { get; }

    /// <summary>
    /// Read the records
    /// </summary>
    public IEnumerable<IReadOnlyList<string>> ReadRecords()
    {
      return Reader.ReadRecords();
    }

    /// <summary>
    /// Disposes the disposables in the order given
    /// </summary>
    public void Dispose()
    {
      if(_disposables!=null)
      {
        var disposables = _disposables;
        _disposables = null;
        foreach(var disposable in disposables)
        {
          disposable.Dispose();
        }
      }
    }
  }

}
