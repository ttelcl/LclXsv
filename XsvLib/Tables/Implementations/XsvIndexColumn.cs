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
  /// An XsvColumn implementation that contains a source column index
  /// for looking up a value in an IReadOnlyList{string}
  /// </summary>
  public class XsvIndexColumn: XsvColumn
  {
    /// <summary>
    /// Create a new XsvIndexColum
    /// </summary>
    public XsvIndexColumn(
      string name,
      int index)
      : base(name)
    {
      Index = index;
    }

    /// <summary>
    /// Create a list of XsvIndexColumn instances from an ordered list of column names
    /// </summary>
    public static IReadOnlyList<XsvIndexColumn> MapNames(IReadOnlyList<string> names)
    {
      return names
        .Select((name, index) => new XsvIndexColumn(name, index))
        .ToList()
        .AsReadOnly();
    }

    /// <summary>
    /// The field index of this column
    /// </summary>
    public int Index { get; }

  }
}
