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

using XsvLib.Implementation;
using XsvLib.Tables;
using XsvLib.Tables.Cursor;

namespace XsvLib
{
  /// <summary>
  /// Extension methods on interfaces in this library
  /// </summary>
  public static class XsvExtensions
  {
    /// <summary>
    /// Load all records from the reader into a new list, optionally cloning the records
    /// </summary>
    /// <param name="itrr">
    /// The ITextRecordReader to read the records from
    /// </param>
    /// <param name="clone">
    /// If true: clone each record. This supports ITextRecordReader implementations that
    /// reuse the returned record object
    /// </param>
    /// <returns></returns>
    public static List<IReadOnlyList<string>> LoadAll(this ITextRecordReader itrr, bool clone)
    {
      var records = itrr.ReadRecords();
      if(clone)
      {
        records = records.Select(row => row.ToArray());
      }
      return records.ToList();
    }

    /// <summary>
    /// Wrap a TextReader as an object implementing ILinesReader
    /// </summary>
    public static ILinesReader LinesFromTextReader(this TextReader tr, bool skipEmptyLines)
    {
      return new StreamLinesReader(tr, skipEmptyLines);
    }

    /// <summary>
    /// Write a full line of fields
    /// </summary>
    public static void WriteLine(this ITextRecordWriter itrw, IEnumerable<string> fields)
    {
      itrw.StartLine();
      itrw.WriteFields(fields);
      itrw.FinishLine();
    }

    /// <summary>
    /// Write multiple fields (possibly as part of a larger line)
    /// </summary>
    public static void WriteFields(this ITextRecordWriter itrw, IEnumerable<string> fields)
    {
      foreach(var field in fields)
      {
        itrw.WriteField(field);
      }
    }

    /// <summary>
    /// Write (Emit) the current row in the buffer and reset it.
    /// </summary>
    public static void WriteBuffer(this ITextRecordWriter itrw, XsvOutBuffer buffer)
    {
      buffer.Emit(itrw);
    }

    /// <summary>
    /// Write (Emit) the header of the buffer.
    /// </summary>
    public static void WriteHeader(this ITextRecordWriter itrw, XsvOutBuffer buffer)
    {
      buffer.EmitHeader(itrw);
    }

    /// <summary>
    /// Wrap the ITextRecordReader as an XsvReader (and peel off the header line)
    /// </summary>
    /// <param name="itrr">
    /// The ITextRecordReader to wrap
    /// </param>
    /// <param name="leaveOpen">
    /// False (default) to have XsvReader close the wrapped ITextRecordReader upon disposal
    /// (if it supports IDisposable). True to leave it open.
    /// </param>
    /// <returns>
    /// A new XsvReader
    /// </returns>
    public static XsvReader AsXsvReader(this ITextRecordReader itrr, bool leaveOpen=false)
    {
      return new XsvReader(itrr, leaveOpen);
    }

    /// <summary>
    /// Repeatedly bind each raw row to the given XsvRow subclass instance, returning that
    /// same instance on each iteration.
    /// </summary>
    /// <typeparam name="TRow">
    /// The XsvRow subclass to bind the raw rows to. The row buffer type must be 
    /// IReadOnlyList{string}
    /// </typeparam>
    /// <typeparam name="TCol">
    /// The column type used by the XsvRow subclass
    /// </typeparam>
    /// <typeparam name="TBuf">
    /// The raw row buffer type (normally IReadOnlyList{string})
    /// </typeparam>
    /// <param name="rawrows">
    /// The input rows
    /// </param>
    /// <param name="rowcursor"></param>
    /// <returns>
    /// A sequence returning the rowcursor argument repeatedly, once for each raw row.
    /// </returns>
    public static IEnumerable<TRow> PlayRows<TRow, TCol, TBuf>(
      this IEnumerable<TBuf> rawrows,
      TRow rowcursor)
      where TRow : XsvRow<TCol, TBuf> 
      where TCol : XsvColumn
      where TBuf: class
    {
      foreach(var rawrow in rawrows)
      {
        rowcursor.SetRow(rawrow);
        yield return rowcursor;
      }
      rowcursor.SetRow(null);
    }

    /// <summary>
    /// Repeatedly bind each raw row to the given XsvCursor, returning that
    /// same XsvCursor instance on each iteration.
    /// </summary>
    /// <param name="rawrows">
    /// The raw input rows
    /// </param>
    /// <param name="cursor">
    /// The cursor object that will be bound to each raw input row
    /// </param>
    /// <returns>
    /// The same cursor object repeated, bound once to each raw row.
    /// </returns>
    public static IEnumerable<XsvCursor> PlayRows(
      this IEnumerable<IReadOnlyList<string>> rawrows,
      XsvCursor cursor)
    {
      return rawrows.PlayRows<XsvCursor, MappedColumn, IReadOnlyList<string>>(cursor);
    }
  }
}
