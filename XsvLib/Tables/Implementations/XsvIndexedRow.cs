/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Tables.Implementations
{
  /// <summary>
  /// Implements XsvRow to wrap an IReadOnlyList{string}
  /// </summary>
  public class XsvIndexedRow: XsvRow<XsvIndexColumn, IReadOnlyList<string>>
  {
    /// <summary>
    /// Create a new XsvIndexedRow
    /// </summary>
    public XsvIndexedRow(
      )
    {
      SetRow(null);
    }

    /// <summary>
    /// Retrieve the field in the current row buffer indexed by
    /// the given column, returning null if there is no current row,
    /// or the index is invalid
    /// </summary>
    /// <param name="column">
    /// The column identigying the field index to return
    /// </param>
    public override string? this[XsvIndexColumn column] { 
      get {
        if(CurrentRow != null && column.Index>=0 && column.Index < CurrentRow.Count)
        {
          return CurrentRow[column.Index];
        }
        else
        {
          return null;
        }
      }
    }

  }
}