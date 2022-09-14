/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Buffers
{
  /// <summary>
  /// Adapter for accessing a single column of an XsvBuffer
  /// </summary>
  public class XsvColumnAccessor
  {
    /// <summary>
    /// Create a new XsvColumnAccessor
    /// </summary>
    internal XsvColumnAccessor(
      XsvBuffer owner,
      MappedColumn column)
    {
      Owner = owner;
      Column = column;
    }

    /// <summary>
    /// The XsvBuffer this accessor accesses
    /// </summary>
    public XsvBuffer Owner { get; }

    /// <summary>
    /// The column of the XsvBuffer to access
    /// </summary>
    public MappedColumn Column { get; }

    /// <summary>
    /// Get or set the column value (alternative to using Get() or Set())
    /// </summary>
    public string Value {
      get { return Get(); }
      set { Set(value); }
    }

    /// <summary>
    /// Get the column value
    /// </summary>
    public string Get()
    {
      if(Column.Index < 0)
      {
        throw new InvalidOperationException(
          $"The column '{Column.Name}' has not been bound yet");
      }
      return Owner[Column.Index];
    }

    /// <summary>
    /// Set the column value
    /// </summary>
    public void Set(string value)
    {
      if(Column.Index < 0)
      {
        throw new InvalidOperationException(
          $"The column '{Column.Name}' has not been bound yet");
      }
      Owner[Column.Index] = value;
    }

  }
}
