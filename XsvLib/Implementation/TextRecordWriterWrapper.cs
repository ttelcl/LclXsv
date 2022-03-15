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
  /// Combines an ITextRecordWriter and resources to dispose upon being done with it
  /// </summary>
  public class TextRecordWriterWrapper: IDisposableTextRecordWriter, IDisposable, ITextRecordWriter
  {
    private IDisposable[]? _disposables;

    /// <summary>
    /// Create a new TextRecordReaderWrapper
    /// </summary>
    public TextRecordWriterWrapper(
      ITextRecordWriter wtr,
      params IDisposable[] disposables)
    {
      Writer = wtr;
      _disposables = disposables;
    }

    /// <summary>
    /// The wrapped Reader
    /// </summary>
    public ITextRecordWriter Writer { get; }


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

    /// <summary>
    /// Implements ITextRecordWriter by redirecting to the implementation in Writer
    /// </summary>
    public int FieldCount {
      get { return Writer.FieldCount; }
    }

    /// <summary>
    /// Implements ITextRecordWriter by redirecting to the implementation in Writer
    /// </summary>
    public void StartLine()
    {
      Writer.StartLine();
    }

    /// <summary>
    /// Implements ITextRecordWriter by redirecting to the implementation in Writer
    /// </summary>
    public void WriteField(string field)
    {
      Writer.WriteField(field);
    }

    /// <summary>
    /// Implements ITextRecordWriter by redirecting to the implementation in Writer
    /// </summary>
    public void FinishLine()
    {
      Writer.FinishLine();
    }

    /// <summary>
    /// Implements ITextRecordWriter by redirecting to the implementation in Writer
    /// </summary>
    public void FinishFile()
    {
      Writer.FinishFile();
    }

  }

}
