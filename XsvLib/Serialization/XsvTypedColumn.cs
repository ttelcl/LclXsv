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
using XsvLib.Tables;

namespace XsvLib.Serialization
{
    /// <summary>
    /// Defines a column in an XSV table that will be mapped to
    /// another data type.
    /// This abstract base class does not define that data type.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of the row model class that will host the storage for the column
    /// </typeparam>
    public abstract class XsvTypedColumn<TModel>: XsvColumn
  {
    /// <summary>
    /// Create a new XsvTypedColumn
    /// </summary>
    protected XsvTypedColumn(string name)
      : base(name)
    {
    }

    /// <summary>
    /// Get the string representation of the column in the given row
    /// </summary>
    public abstract string GetString(TModel row);

    /// <summary>
    /// Set the value of the column in the given row from the parsed
    /// value of the given string
    /// </summary>
    public abstract void SetString(TModel row, string value);
  }

  /// <summary>
  /// Defines a column in an XSV table, the model containing the column as property, and the data type it represents
  /// </summary>
  /// <typeparam name="TModel">
  /// The model class that stores the data for this column (and other columns)
  /// </typeparam>
  /// <typeparam name="TData">
  /// The type of the data value
  /// </typeparam>
  public class XsvTypedColumn<TModel, TData>: XsvTypedColumn<TModel>
  {
    /// <summary>
    /// Create a new XsvTypedColumn
    /// </summary>
    public XsvTypedColumn(string name, StringAdapter<TData> converter, FieldAccessor<TModel, TData> accessor)
      : base(name)
    {
      Converter = converter;
      Accessor = accessor;
    }

    /// <summary>
    /// The string adapter used for converting string values to data values for this column
    /// </summary>
    public StringAdapter<TData> Converter { get; }

    /// <summary>
    /// The accessor that extracts or stores the strongly typed data field in the model instance
    /// </summary>
    public FieldAccessor<TModel, TData> Accessor { get; }

    /// <inheritdoc/>
    public override string GetString(TModel row)
    {
      return Converter.StringValue(Accessor.Get(row));
    }

    /// <inheritdoc/>
    public override void SetString(TModel row, string value)
    {
      Accessor.Set(row, Converter.ParseString(value));
    }

  }
}
