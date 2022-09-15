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

    /// <summary>
    /// Clear the column value to an empty string, and mark the
    /// column as not-set in case Write Tracking is enabled on the
    /// backing XsvBuffer
    /// </summary>
    public void Clear()
    {
      if(Column.Index < 0)
      {
        throw new InvalidOperationException(
          $"The column '{Column.Name}' has not been bound yet");
      }
      Owner.ClearField(Column.Index);
    }

    /// <summary>
    /// Check if a value has been set for this column. This will always
    /// be false if the XsvBuffer isn't set to track writes.
    /// </summary>
    public bool IsSet {
      get {
        if(Column.Index < 0)
        {
          throw new InvalidOperationException(
            $"The column '{Column.Name}' has not been bound yet");
        }
        return Owner.IsSet(Column.Index);
      }
    }

  }
}
