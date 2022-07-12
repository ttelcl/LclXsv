/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib
{
  /// <summary>
  /// Buffers the fields for a row to be output to XSV
  /// </summary>
  public class XsvOutBuffer
  {
    private readonly string?[] _buffer;

    /// <summary>
    /// Create a new XsvOutBuffer
    /// </summary>
    public XsvOutBuffer(
      IEnumerable<string> header,
      bool caseSensitive = false)
    {
      Header = header.ToList().AsReadOnly();
      _buffer = new string[Header.Count];
      Columns = new ColumnMap(caseSensitive);
      foreach(var cn in header)
      {
        Columns.Declare(cn);
      }
      Columns.BindColumns(Header);
    }

    /// <summary>
    /// The header (the column names in their proper order)
    /// </summary>
    public IReadOnlyList<string> Header { get; }

    /// <summary>
    /// The column map, containing the MappedColumns (pre-bound to the header)
    /// </summary>
    public ColumnMap Columns { get; }

    /// <summary>
    /// Retrieve the mapped column corresponding to one of the columns
    /// in the header
    /// </summary>
    public MappedColumn GetColumn(string name)
    {
      return Columns[name, false, false]!;
    }

    /// <summary>
    /// Get or set a field in this row buffer. Setting a column that was
    /// set already since the last Reset() or Emit() will cause an exception.
    /// </summary>
    /// <param name="c">
    /// Identifies the column to set. This must be one of the columns
    /// in Columns (obtained through GetColumn())
    /// </param>
    /// <returns>
    /// The current value of the field. This may be null if not set yet
    /// since the latest Emit() or Reset().
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when an uninitialized column is passed, or when setting
    /// a field that was already set.
    /// </exception>
    public string? this[MappedColumn c] {
      get {
        if(!c.HasIndex)
        {
          throw new InvalidOperationException(
            $"Attempt to use an unbound column '{c.Name}'");
        }
        return _buffer[c.Index];
      }
      set {
        if(!c.HasIndex)
        {
          throw new InvalidOperationException(
            $"Attempt to use an unbound column '{c.Name}'");
        }
        if(_buffer[c.Index] != null)
        {
          throw new InvalidOperationException(
            $"Column '{c.Name}' was already set for the current row.");
        }
        _buffer[c.Index] = value;
      }
    }

    /// <summary>
    /// Clear all fields in the row, preparing this buffer to receive data
    /// for the next row. This is called automatically by Emit(); calling this
    /// explicitly allows abandoning a row in progress.
    /// </summary>
    public void Reset()
    {
      for(var i = 0; i < _buffer.Length; i++)
      {
        _buffer[i] = null;
      }
    }

    /// <summary>
    /// Write the header to the given ITextRecordWriter
    /// </summary>
    public void EmitHeader(ITextRecordWriter itrw)
    {
      itrw.WriteLine(Header);
    }

    /// <summary>
    /// Verify that all fields have a value and write the row to the specified
    /// ITextRecordWriter
    /// </summary>
    /// <param name="itrw">
    /// The writer to write to
    /// </param>
    /// <param name="reset">
    /// When true (default), reset the buffer after the write. 
    /// By setting this to false you can prevent this automatic reset, for instance
    /// if you want to write this same buffer to multiple writers.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when any of the fields have not been assigned a value since the last reset.
    /// </exception>
    public void Emit(ITextRecordWriter itrw, bool reset = true)
    {
      foreach(var c in Columns.AllColumns(false))
      {
        if(_buffer[c.Index] is null)
        {
          throw new InvalidOperationException(
            $"Invalid output row: no value assigned to column '{c.Name}'");
        }
      }
      itrw.WriteLine(_buffer.Select(s => s!));
      if(reset)
      {
        Reset();
      }
    }

  }
}
