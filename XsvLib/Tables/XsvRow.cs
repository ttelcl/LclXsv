/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Tables
{
  /// <summary>
  /// A buffer abstractly storing a row of an XSV table.
  /// This class is indexed by an XsvColumn subclass, and
  /// its implementation is tied to that XsvColumn implementation
  /// </summary>
  /// <typeparam name="TColumn">
  /// The subclass of XsvColumn to be used with this implementation
  /// </typeparam>
  /// <typeparam name="TRowBuffer">
  /// The implementation class for the underlying buffer
  /// </typeparam>
  public abstract class XsvRow<TColumn, TRowBuffer> 
    where TColumn : XsvColumn
    where TRowBuffer: class
  {
    /// <summary>
    /// Create a new XsvCursor
    /// </summary>
    protected XsvRow()
    {
      HasData = false;
    }

    /// <summary>
    /// Whether or not the row actually has data
    /// </summary>
    public bool HasData { get; protected set; }

    /// <summary>
    /// Get or set the current row
    /// </summary>
    public TRowBuffer? CurrentRow { get; protected set; }

    /// <summary>
    /// Change the value of CurrentRow. Subclasses can override this
    /// to perform additional updates. The default implementation
    /// changes HasData based on whether or not the buffer is null.
    /// </summary>
    public virtual void SetRow(TRowBuffer? buffer)
    {
      CurrentRow = buffer;
      HasData = buffer is not null;
    }

    /// <summary>
    /// Get the cell value for the identified column in the currently loaded
    /// row. Returns null if there is no value.
    /// </summary>
    /// <param name="column">
    /// The column to retrieve
    /// </param>
    public abstract string? this[TColumn column] { get; }

    /// <summary>
    /// Get the non-null string value of the column, throwing an exception
    /// if there is no value
    /// </summary>
    /// <param name="column">
    /// The column to retrieve
    /// </param>
    /// <returns>
    /// The string value for the column (which can be empty, but not null)
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no data is loaded, or the column has no valid mapping
    /// </exception>
    public string GetString(TColumn column)
    {
      if(HasData)
      {
        var value = this[column];
        if(value != null)
        {
          return value;
        }
      }
      throw new InvalidOperationException(
        $"No data loaded for column {column.Name}");
    }

    /// <summary>
    /// Get the value of the column, parsed as integer
    /// </summary>
    public int GetInt32(TColumn column)
    {
      return Int32.Parse(GetString(column));
    }

  }
}
