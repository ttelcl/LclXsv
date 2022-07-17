/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XsvLib.Tables.Cursor;
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
    /// <param name="itrr">
    /// The ITextRecordReader instance to wrap. The implementation may
    /// also implement IDisposable, in which case this XsvReader can take
    /// ownership of it depending on the 'leaveOpen' flag.
    /// </param>
    /// <param name="leaveOpen">
    /// When false (default), disposing this XsvReader also disposes the
    /// wrapped ITextRecordReader if it implements IDisposable.
    /// When true, the caller is responsible for disposing the wrapped
    /// reader's resources.
    /// </param>
    public XsvReader(
      ITextRecordReader itrr,
      bool leaveOpen = false)
    {
      _reader = itrr;
      LeaveOpen = leaveOpen;
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
    /// Bind this XsvReader's Header to the ColumnMap and iterate all data rows,
    /// returning an XsvCursor for each data row. The same XsvCursor instance 
    /// (bound to a different raw row) is returned for each iteration.
    /// </summary>
    /// <param name="columnMap">
    /// The ColumnMap that declares the columns you are interested in. Each
    /// MappedColumn that you declared in this ColumnMap is a valid index for
    /// the returned XsvCursor.
    /// </param>
    /// <returns>
    /// An iteration that returns the same XsvCursor on each iteration, bound
    /// to each raw data row
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when any columns declared in the ColumnMap are missing from the data.
    /// Also thrown when no columns are declared in the ColumnMap
    /// </exception>
    public IEnumerable<XsvCursor> ReadCursor(ColumnMap columnMap)
    {
      var cursor = new XsvCursor(columnMap);
      var ok = cursor.ColumnMapping.BindColumns(Header);
      if(!ok)
      {
        var missing = String.Join(", ", cursor.ColumnMapping.UnboundColumns());
        throw new InvalidOperationException(
          $"The following column(s) were expected but are missing from the input: {missing}");
      }
      foreach(var rawrow in ReadRecords())
      {
        cursor.SetRow(rawrow);
        yield return cursor;
      }
      cursor.SetRow(null);
    }

    /// <summary>
    /// Bind this XsvReader's Header to the Cursor's ColumnMap and iterate all data rows,
    /// returning the cursor argument for each data row. The same instance 
    /// (bound to a different raw row) is returned for each iteration.
    /// </summary>
    /// <param name="cursor">
    /// An instance of XsvCursor or a subclass.
    /// </param>
    /// <returns>
    /// An iteration that returns the "cursor" argument on each iteration, bound
    /// to each raw data row.
    /// </returns>
    public IEnumerable<TCursor> ReadCursor<TCursor>(TCursor cursor)
      where TCursor : XsvCursor
    {
      var ok = cursor.ColumnMapping.BindColumns(Header);
      if(!ok)
      {
        var missing = String.Join(", ", cursor.ColumnMapping.UnboundColumns());
        throw new InvalidOperationException(
          $"The following column(s) were expected but are missing from the input: {missing}");
      }
      foreach(var rawrow in ReadRecords())
      {
        cursor.SetRow(rawrow);
        yield return cursor;
      }
      cursor.SetRow(null);
    }

    /// <summary>
    /// False if this XsvReader "owns" the wrapped ITextRecordReader.
    /// In that case Disposing this XsvReader also disposes the TextRecordReader
    /// if it supports IDisposable.
    /// </summary>
    public bool LeaveOpen { get; }

    /// <summary>
    /// Clean up
    /// </summary>
    public void Dispose()
    {
      if(!_disposed)
      {
        _disposed = true;
        Sequencer.Dispose();
        if(!LeaveOpen && _reader is IDisposable disp)
        {
          disp.Dispose();
        }
      }
    }
  }
}
