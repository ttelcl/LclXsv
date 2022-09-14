/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib
{
  /// <summary>
  /// Manages a group of MappedColumn instances with unique names
  /// </summary>
  public class ColumnMap
  {
    private readonly Dictionary<string, MappedColumn> _columns;

    /// <summary>
    /// Create a new ColumnMap
    /// </summary>
    /// <param name="caseSensitive">
    /// Whether column names should be treated case-sensitively or not (default: not)
    /// </param>
    public ColumnMap(bool caseSensitive = false)
    {
      _columns = new Dictionary<string, MappedColumn>(
        caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Get a previously created column or create a new column.
    /// </summary>
    /// <param name="name">
    /// The name of the column
    /// </param>
    /// <param name="create">
    /// Whether a column should be created if missing. When also optional==false,
    /// it is an error if the column was not missing.
    /// </param>
    /// <param name="optional">
    /// Whether retrieval or creation is optional. If false, the column must not
    /// exist when "create" is true and must exist when "create" is false.
    /// </param>
    /// <returns>
    /// Returns the created or retrieved existsing column. In case "optional" was
    /// true and "create" was false, this may be null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the entry did already exist, "create" is true, and "optional" is false.
    /// Also thrown when the entry did not exist, "create" is false, and "optional" is false.
    /// </exception>
    public MappedColumn? this[string name, bool create = false, bool optional = false] {
      get {
        if(create && CreationLocked)
        {
          throw new InvalidOperationException(
            $"This ColumnMap has been locked for new column creation");
        }
        if(_columns.TryGetValue(name, out MappedColumn result))
        {
          if(create && !optional)
          {
            throw new InvalidOperationException(
              $"The column is declared twice: {name}");
          }
          return result;
        }
        else
        {
          if(create)
          {
            result = new MappedColumn(this, name);
            _columns.Add(name, result);
            return result;
          }
          if(!optional)
          {
            throw new InvalidOperationException(
              $"A required column was not found: {name}");
          }
          return null;
        }
      }
    }

    /// <summary>
    /// Declare a new column. Shorthand for 
    /// "return this[name, create: true, optional: !mustNotExist];"
    /// </summary>
    public MappedColumn Declare(string name, bool mustNotExist = true)
    {
      return this[name, create: true, optional: !mustNotExist]!;
    }

    /// <summary>
    /// Find a column if it was declared (returning null if not found)
    /// </summary>
    public MappedColumn? Find(string name)
    {
      return this[name, create: false, optional: true];
    }

    /// <summary>
    /// Get a previously declared column, throwing an exception if not found.
    /// </summary>
    public MappedColumn Get(string name)
    {
      return this[name, create: false, optional: false]!;
    }

    /// <summary>
    /// Unbind all columns, then bind the columns as prescribed by the headers
    /// argument.
    /// </summary>
    /// <param name="headers">
    /// The column names in the order they will appear in the data.
    /// </param>
    /// <param name="allowEmpty">
    /// By default an exception is thrown if there are no columns declared at all.
    /// Set this to true to skip that check.
    /// </param>
    /// <returns>
    /// True if all declared columns have been bound, false if some declared columns
    /// are left unbound.
    /// </returns>
    public bool BindColumns(IReadOnlyList<string> headers, bool allowEmpty = false)
    {
      foreach(var column in _columns.Values)
      {
        column.Index = -1;
      }
      if(_columns.Count == 0 && !allowEmpty)
      {
        throw new InvalidOperationException("Cannot bind columns - there are none!");
      }
      for(var i = 0; i < headers.Count; i++)
      {
        if(_columns.TryGetValue(headers[i], out var result))
        {
          result.Index = i;
        }
      }
      return _columns.Values.All(c => c.HasIndex);
    }

    /// <summary>
    /// Re-bind columns, assigning a column index based on the current order
    /// of columns (which is declaration order, unless AllColumns(true) was called)
    /// </summary>
    public void BindColumns(bool allowEmpty = false)
    {
      if(_columns.Count == 0 && !allowEmpty)
      {
        throw new InvalidOperationException("Cannot bind columns - there are none!");
      }
      var i = 0;
      foreach(var column in _columns.Values)
      {
        column.Index = i++;
      }
    }

    /// <summary>
    /// Enumerate the columns that have no binding
    /// </summary>
    public IEnumerable<MappedColumn> UnboundColumns()
    {
      return _columns.Values.Where(c => !c.HasIndex);
    }

    /// <summary>
    /// Return all declared columns, optionally sorted by binding index
    /// </summary>
    public IReadOnlyList<MappedColumn> AllColumns(bool sort)
    {
      if(sort)
      {
        var sorted =
          from column in _columns.Values
          orderby column.Index, column.Name
          select column;
        return sorted.ToList().AsReadOnly();
      }
      else
      {
        return _columns.Values.ToList().AsReadOnly();
      }
    }

    /// <summary>
    /// Return the columns. Unless AllColumns(true) was called, the order is
    /// the declaration order.
    /// </summary>
    public IEnumerable<MappedColumn> EnumColumns()
    {
      return _columns.Values;
    }

    /// <summary>
    /// Get the number of declared columns
    /// </summary>
    public int Count { get => _columns.Count; }

    /// <summary>
    /// True if creating new columns is disabled, false if enabled.
    /// Once disabled this stays disabled. Use LockCreation to activate the lock.
    /// </summary>
    public bool CreationLocked { get; private set; }

    /// <summary>
    /// Sets CreationLocked to true, preventing further column creations
    /// </summary>
    public void LockCreation()
    {
      CreationLocked = true;
    }
  }
}
