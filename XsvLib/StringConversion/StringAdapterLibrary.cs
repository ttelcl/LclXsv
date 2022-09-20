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
    /// Register the adapter for the given type and each of the givven names
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
    public void Register<TData>(StringAdapter<TData> adapter, params string[] names)
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
    }

  }
}
