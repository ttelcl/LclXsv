/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XsvLib.StringConversion.Implementations;

namespace XsvLib.StringConversion
{
  /// <summary>
  /// A library of StringAdapters identified by their type and optionally
  /// a name to distinguish multiple adapters for the same type.
  /// </summary>
  public class StringAdapterLibrary
  {
    private readonly Dictionary<Type, Dictionary<string, object>> _adapters;

    /// <summary>
    /// Create a new StringAdapterLibrary. Initially the library is empty
    /// </summary>
    public StringAdapterLibrary()
    {
      _adapters = new Dictionary<Type, Dictionary<string, object>>();
    }

    /// <summary>
    /// Return the registered string adapter for the given type and name, if it exists
    /// (null otherwise)
    /// </summary>
    public StringAdapter<TData>? Find<TData>(string name = "")
    {
      var type = typeof(TData);
      if(_adapters.TryGetValue(type, out var innerMap))
      {
        if(innerMap.TryGetValue(name, out var adapter))
        {
          return (StringAdapter<TData>)(adapter!);
        }
      }
      return null;
    }

    /// <summary>
    /// Return the registered string adapter for the given type and name, if it exists.
    /// (throwing a KeyNotFoundException otherwise)
    /// </summary>
    /// <typeparam name="TData">
    /// The type to get the converter for
    /// </typeparam>
    /// <param name="name">
    /// The name of the adapter (to disambiguate multiple adapters for the same type)
    /// (default is an empty string)
    /// </param>
    /// <returns>
    /// The requested StringAdapter
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// When no matching adapter has been registered
    /// </exception>
    public StringAdapter<TData> Get<TData>(string name = "")
    {
      var adapter = Find<TData>(name);
      if(adapter == null)
      {
        throw new KeyNotFoundException(
          $"No string adapter named '{name}' registered for type {typeof(TData).FullName}");
      }
      else
      {
        return adapter;
      }
    }

    /// <summary>
    /// Register the adapter for the given type and each of the given names
    /// (possibly overwriting existing adapters)
    /// </summary>
    /// <typeparam name="TData">
    /// The type the stringadapter converts
    /// </typeparam>
    /// <param name="adapter">
    /// The string adapter instance to register
    /// </param>
    /// <param name="names">
    /// Zero or more names to register the adapter as. Not prividing any names
    /// is treated as if the empty string was specified as the sole name.
    /// </param>
    /// <returns>
    /// This StringAdapterLibrary itself.
    /// </returns>
    public StringAdapterLibrary Register<TData>(StringAdapter<TData> adapter, params string[] names)
    {
      if(names.Length == 0)
      {
        names = new[] { "" };
      }
      var type = typeof(TData);
      if(!_adapters.TryGetValue(type, out var innerMap))
      {
        innerMap = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        _adapters.Add(type, innerMap);
      }
      foreach(var name in names)
      {
        innerMap[name] = adapter;
      }
      return this;
    }

    /// <summary>
    /// Register one or more aliases for a converter
    /// </summary>
    /// <typeparam name="TData">
    /// The data type of the converter to alias
    /// </typeparam>
    /// <param name="originalName">
    /// The existing name of the stringadapter to alias. Use "" to alias the default converter for TData
    /// </param>
    /// <param name="names">
    /// The alias name(s)
    /// </param>
    /// <returns>
    /// This StringAdapterLibrary itself.
    /// </returns>
    public StringAdapterLibrary RegisterAlias<TData>(string originalName, params string[] names)
    {
      return Register(Get<TData>(originalName), names);
    }

    /// <summary>
    /// Register a collection of common string adapters, all using the default adapter name.
    /// The registered types are int, long, bool and string. Optionally also register
    /// nullable versions of the value type converters. The registered bool converter represents
    /// true and false with the strings "true" and "false".
    /// </summary>
    /// <param name="includeNullable">
    /// Default true. When true, also register converters for int?, long? and bool? Null
    /// values are represented by blank strings
    /// </param>
    /// <returns>
    /// This library itself, enabling chaining of more registrations
    /// </returns>
    public StringAdapterLibrary RegisterStandard(bool includeNullable = true)
    {
      RegisterString();
      RegisterInt32();
      RegisterInt64();
      RegisterBool();
      if(includeNullable)
      {
        RegisterNullable<int>();
        RegisterNullable<long>();
        RegisterNullable<bool>();
      }
      return this;
    }

    /// <summary>
    /// Register a integer - string converter, using standard decimal integer notation.
    /// </summary>
    /// <param name="name">
    /// Default "". The name for the converter (useful for scenarios where you need
    /// to distinguish between multiple converters for the same type)
    /// </param>
    /// <returns>
    /// This StringAdapterLibrary itself.
    /// </returns>
    public StringAdapterLibrary RegisterInt32(string name = "")
    {
      return Register(new Int32StringAdapter(), name);
    }

    /// <summary>
    /// Register a long integer - string converter, using standard decimal integer notation.
    /// </summary>
    /// <param name="name">
    /// Default "". The name for the converter (useful for scenarios where you need
    /// to distinguish between multiple converters for the same type)
    /// </param>
    /// <returns>
    /// This StringAdapterLibrary itself.
    /// </returns>
    public StringAdapterLibrary RegisterInt64(string name = "")
    {
      return Register(new Int64StringAdapter(), name);
    }

    /// <summary>
    /// Register a bool - string converter, using the given representation strings for
    /// 'false' and 'true.
    /// </summary>
    /// <param name="falseValue">
    /// The representation for the value 'false'. When parsing this is case insensitive.
    /// Default "false".
    /// </param>
    /// <param name="trueValue">
    /// The representation for the value 'true'. When parsing this is case insensitive.
    /// Default "true"
    /// </param>
    /// <param name="name">
    /// Default "". The name for the converter (useful for scenarios where you need
    /// to distinguish between multiple converters for the same type)
    /// </param>
    /// <returns>
    /// This StringAdapterLibrary itself.
    /// </returns>
    public StringAdapterLibrary RegisterBool(string falseValue="false", string trueValue="true", string name = "")
    {
      return Register(new BoolStringAdapter(), falseValue, trueValue, name);
    }

    /// <summary>
    /// Register the dummy string-to-string converter (for which both parse and tostring are
    /// the identity function).
    /// </summary>
    /// <param name="name">
    /// Default "". The name of the converter
    /// </param>
    /// <returns>
    /// This StringAdapterLibrary itself
    /// </returns>
    public StringAdapterLibrary RegisterString(string name = "")
    {
      return Register(new StringStringAdapter(), name);
    }

    /// <summary>
    /// Register a string converter for converting nullable instances of type T, using the
    /// given 'nullValue' as string representation of nulls.
    /// </summary>
    /// <typeparam name="T">
    /// The base type to convert. A converter for this type with the given name must have been 
    /// registered already.
    /// </typeparam>
    /// <param name="nullValue">
    /// Default "". The string representing null values.
    /// </param>
    /// <param name="name">
    /// Default "". The name of the converter. This is both the name for the base converter
    /// as well as the nullable wrapper converter registered by this call.
    /// </param>
    /// <returns>
    /// This StringAdapterLibrary itself.
    /// </returns>
    public StringAdapterLibrary RegisterNullable<T>(string nullValue = "", string name="")
    {
      var cvt = Get<T>(name);
      return Register(new NullStringAdapter<T>(cvt, nullValue), name);
    }

  }
}
