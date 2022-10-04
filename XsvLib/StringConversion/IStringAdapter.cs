/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XsvLib.StringConversion
{
  /// <summary>
  /// Defines the logic for converting the given type to or from a string,
  /// including specific formatting rules.
  /// </summary>
  public interface IStringAdapter<TData>
  {

    /// <summary>
    /// Parse a string to a TData instance
    /// </summary>
    TData ParseString(string value);

    /// <summary>
    /// Convert a data value to a string
    /// </summary>
    /// <param name="value">
    /// The data value to serialize
    /// </param>
    /// <returns>
    /// A string representing the data value
    /// </returns>
    string StringValue(TData value);
  }
}

