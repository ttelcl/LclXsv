/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Tables.Cursor
{
  /// <summary>
  /// Implements XsvRow for use with MappedColumn and IReadOnlyList{string}
  /// </summary>
  public class XsvCursor : XsvRow<MappedColumn, IReadOnlyList<string>>
  {
    /// <summary>
    /// Create a new XsvCursor
    /// </summary>
    /// <param name="columnMapping">
    /// The ColumnMap that contains or will contain the column definitions.
    /// If null, a new empty ColumnMap will be created
    /// </param>
    public XsvCursor(
      ColumnMap? columnMapping)
      : base()
    {
      ColumnMapping = columnMapping ?? new ColumnMap();
    }

    /// <summary>
    /// The ColumnMap that owns the MappedColumn instances to be passed
    /// as indexes.
    /// </summary>
    public ColumnMap ColumnMapping { get; }

    /// <summary>
    /// Get the value for a specific column from the current row,
    /// or null if there is no current row or the column is not mapped
    /// </summary>
    /// <param name="column">
    /// The column to look up
    /// </param>
    /// <returns>
    /// The string value of the column, or null if not known.
    /// </returns>
    public override string? this[MappedColumn column] { 
      get {
        return CurrentRow!=null && column.Index >= 0 && column.Index < CurrentRow.Count
          ? CurrentRow[column.Index]
          : null;
      }
    }
  }
}
