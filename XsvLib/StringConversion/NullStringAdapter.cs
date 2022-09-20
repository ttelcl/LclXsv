/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.StringConversion
{
  /// <summary>
  /// A filter turning a string adapter for a non-nullable type
  /// into a string adapter for the corresponding nullable type,
  /// treating a specific marker string (default: empty string) as NULL value
  /// </summary>
  public class NullStringAdapter<TData> : StringAdapter<TData?>
  {
    /// <summary>
    /// Create a new BlankStringAdapter
    /// </summary>
    /// <param name="baseAdapter">
    /// The string adapter for the underlying non-nullable type
    /// </param>
    /// <param name="nullMarker">
    /// The string to treat as equivalent of a null data value
    /// </param>
    public NullStringAdapter(
      StringAdapter<TData> baseAdapter,
      string nullMarker = "")
    {
      BaseAdapter = baseAdapter;
      NullMarker = nullMarker;
    }

    /// <summary>
    /// The underlying non-nullable type adapter
    /// </summary>
    public StringAdapter<TData> BaseAdapter { get; }

    /// <summary>
    /// The string representing null data values
    /// </summary>
    public string NullMarker { get; }

    /// <summary>
    /// Parse a string to a TData instance, converting NullMarker to
    /// null. Also accepts null string as input (converting it to 
    /// a null return value)
    /// </summary>
    public override TData? ParseString(string value)
    {
      if(value == null || value == NullMarker)
      {
        return default;
      }
      else
      {
        return BaseAdapter.ParseString(value);
      }
    }

    /// <summary>
    /// Convert a data value to the corresponding string (and convert
    /// null to NullMarker)
    /// </summary>
    public override string StringValue(TData? value)
    {
      return value == null ? NullMarker : BaseAdapter.StringValue(value);
    }
  }
}