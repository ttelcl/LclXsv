/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XsvLib.StringConversion;

namespace XsvLib.Buffers
{
  /// <summary>
  /// Combines an XsvColumnAccessor and a StringAdapter, providing a typed accessor
  /// for an XsvBuffer column value.
  /// </summary>
  public class XsvTypedAccessor<T>
  {
    /// <summary>
    /// Create a new XsvTypedAccessor
    /// </summary>
    public XsvTypedAccessor(
      XsvColumnAccessor stringAccessor,
      IStringAdapter<T> adapter)
    {
      Accessor = stringAccessor;
      Adapter = adapter;
    }

    /// <summary>
    /// The underlying accessor, accessing the column as a string
    /// </summary>
    public XsvColumnAccessor Accessor { get; }

    /// <summary>
    /// The adapter converting bidirectionally between the raw string value
    /// and the typed equivalent
    /// </summary>
    public IStringAdapter<T> Adapter { get; }

    /// <summary>
    /// Get or set the typed value for the XsvBuffer column
    /// </summary>
    public T Value {
      get => Get();
      set => Set(value);
    }

    /// <summary>
    /// Get the typed value of the column
    /// </summary>
    public T Get()
    {
      return Adapter.ParseString(Accessor.Get());
    }

    /// <summary>
    /// Set the typed value of the column
    /// </summary>
    public void Set(T v)
    {
      Accessor.Set(Adapter.StringValue(v));
    }

  }
}
