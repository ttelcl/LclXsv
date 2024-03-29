﻿/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using System.Diagnostics.CodeAnalysis;

namespace XsvLib.StringConversion
{
  /// <summary>
  /// Defines the logic for converting the given type to or from a string,
  /// including specific formatting rules.
  /// </summary>
  public abstract class StringAdapter<TData>  : IStringAdapter<TData>
  {
    /// <summary>
    /// Create a new StringAdapter
    /// </summary>
    protected StringAdapter()
    {
    }

    ///// <summary>
    ///// Try to parse the string
    ///// </summary>
    ///// <param name="s">
    ///// The string to parse
    ///// </param>
    ///// <param name="value">
    ///// The output value on success
    ///// </param>
    ///// <returns>
    ///// True on success, false on failure
    ///// </returns>
    //public abstract bool TryParse(string s, [MaybeNullWhen(returnValue: false)] out TData value);

    /// <summary>
    /// Parse a string to a TData instance
    /// </summary>
    public abstract TData ParseString(string value);

    /// <summary>
    /// Convert a data value to a string
    /// </summary>
    /// <param name="value">
    /// The data value to serialize
    /// </param>
    /// <returns>
    /// A string representing the data value
    /// </returns>
    public abstract string StringValue(TData value);

  }
}