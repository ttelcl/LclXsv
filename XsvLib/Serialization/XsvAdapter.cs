/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Serialization
{
  /// <summary>
  /// The DTO adapter, defining the properties of the DTO to be
  /// serialized and providing the type converters. During reading and writing
  /// this adapter temporarily takes ownership of the DTO to serialize or
  /// deserialize.
  /// </summary>
  /// <typeparam name="T">
  /// The DTO class to serialize or deserialize
  /// </typeparam>
  public class XsvAdapter<T>
    where T : class
  {
    private readonly List<XsvTypedColumn<T>> _columns;
    private readonly Dictionary<string, XsvTypedColumn<T>> _columnMap;

    /// <summary>
    /// Create a new XsvAdapter
    /// </summary>
    protected XsvAdapter(
      IEnumerable<XsvTypedColumn<T>> columns,
      bool columnNamesCaseSensitive = false)
    {
      _columns = new List<XsvTypedColumn<T>>();
      _columnMap = new Dictionary<string, XsvTypedColumn<T>>(
        columnNamesCaseSensitive
        ? StringComparer.Ordinal
        : StringComparer.InvariantCultureIgnoreCase);
      Columns = _columns.AsReadOnly();
      _columns.AddRange(columns);
      foreach(var column in _columns)
      {
        if(_columnMap.ContainsKey(column.Name))
        {
          throw new ArgumentException(
            $"Duplicate column name '{column.Name}'");
        }
        _columnMap.Add(column.Name, column);
      }
    }

    /// <summary>
    /// The DTO to serialize or deserialize
    /// </summary>
    protected T? Target { get; set; }

    /// <summary>
    /// The columns for this adapater, in the same order as they
    /// appear in the XSV data.
    /// </summary>
    public IReadOnlyList<XsvTypedColumn<T>> Columns { get; set; }
  
    /// <summary>
    /// Get a column object by name
    /// </summary>
    public XsvTypedColumn<T> GetColumn(string name)
    {
      return _columnMap[name];
    }

    /// <summary>
    /// Reorder the columns to match the given column name order
    /// </summary>
    public void BindColumnOrder(IEnumerable<string> names)
    {
      _columns.Clear();
      var done = new HashSet<string>(_columnMap.Comparer);
      foreach(var name in names)
      {
        if(done.Contains(name))
        {
          throw new ArgumentException(
            $"Duplicate column name '{name}' in column order");
        }
        if(_columnMap.TryGetValue(name, out var column))
        {
          done.Add(name);
          _columns.Add(column);
        }
        else
        {
          throw new ArgumentException(
            $"Undeclared column '{name}' in column order");
        }
      }

    }

  }
}
