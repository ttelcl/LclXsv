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
  /// Describes a column with a specific name that can be mapped
  /// to a specific column index. 
  /// </summary>
  public class MappedColumn : XsvColumn
  {
    /// <summary>
    /// Create a new MappedColumn. This is called indirectly by the
    /// indexer of ColumnMap
    /// </summary>
    internal MappedColumn(
      string name)
      : base(name)
    {
      Index = -1;
    }

    /// <summary>
    /// The index for this column, or -1 if not mapped.
    /// </summary>
    public int Index { get; internal set; }

    /// <summary>
    /// True if an index has been bound to this column
    /// </summary>
    public bool HasIndex => Index >= 0;

  }
}
