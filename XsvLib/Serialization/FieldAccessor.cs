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
  /// Gets or sets a specific "property" of a backing object
  /// (the "property" does not need to be an actual property).
  /// </summary>
  /// <typeparam name="TModel">
  /// The type of the backing object that stores the value
  /// </typeparam>
  /// <typeparam name="TData">
  /// The type of the value to store or retrieve
  /// </typeparam>
  public abstract class FieldAccessor<TModel, TData>
  {
    /// <summary>
    /// Create a new FieldAccessor
    /// </summary>
    protected FieldAccessor()
    {
    }

    /// <summary>
    /// Get the "property" value for the given model object.
    /// </summary>
    public abstract TData Get(TModel model);

    /// <summary>
    /// Set the "property" value for the given model object.
    /// This may throw an exception in case the model is immutable.
    /// </summary>
    public abstract void Set(TModel model, TData value);

  }

  /// <summary>
  /// Implements FieldAccessor as a pair of delegates
  /// </summary>
  public class DelegateFieldAccessor<TModel, TData>
    : FieldAccessor<TModel, TData>
  {
    Func<TModel, TData> _getter;
    Action<TModel, TData>? _setter;

    /// <summary>
    /// Create a new DelegateFieldAccessor
    /// </summary>
    public DelegateFieldAccessor(
      Func<TModel, TData> getter,
      Action<TModel, TData>? setter)
    {
      _getter = getter;
      _setter = setter;
    }

    /// <inheritdoc/>
    public override TData Get(TModel model)
    {
      return _getter(model);
    }

    /// <inheritdoc/>
    public override void Set(TModel model, TData value)
    {
      if(_setter == null)
      {
        throw new NotSupportedException(
          "This property is immutable");
      }
      _setter(model, value);
    }
  }
}
