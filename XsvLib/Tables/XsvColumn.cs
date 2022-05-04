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
  /// Abstractly identifies a column in an XSV table, without 
  /// exposing the implementation of how exactly it extracts
  /// cell values from an XsvRow
  /// </summary>
  public abstract class XsvColumn
  {
    /// <summary>
    /// Create a new XsvColumn
    /// </summary>
    protected XsvColumn(
      string name)
    {
      Name = name;
    }

    /// <summary>
    /// The column's name
    /// </summary>
    public string Name { get; }

  }
}
