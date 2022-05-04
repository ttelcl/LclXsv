﻿/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Tables
{
  /// <summary>
  /// A buffer abstractly storing a row of an XSV table.
  /// This class is indexed by an XsvColumn subclass, and
  /// its implementation is tied to that XsvColumn implementation
  /// </summary>
  /// <typeparam name="TColumn">
  /// The subclass of XsvColumn to be used with this implementation
  /// </typeparam>
  /// <typeparam name="TRowBuffer">
  /// The implementation class for the underlying buffer
  /// </typeparam>
  public abstract class XsvRow<TColumn, TRowBuffer> 
    where TColumn : XsvColumn
    where TRowBuffer: class
  {
    /// <summary>
    /// Create a new XsvCursor
    /// </summary>
    protected XsvRow()
    {
    }

    /// <summary>
    /// Whether or not the row actually has data
    /// </summary>
    public bool HasData { get; protected set; }

    /// <summary>
    /// Get or set the current row
    /// </summary>
    public TRowBuffer? CurrentRow { get; protected set; }

    /// <summary>
    /// Change the value of CurrentRow. Subclasses can override this
    /// to perform additional updates
    /// </summary>
    public virtual void SetRow(TRowBuffer? buffer)
    {
      CurrentRow = buffer;
    }

    /// <summary>
    /// Get the cell value for the identified column in the currently loaded
    /// row. Returns null if there is no value.
    /// </summary>
    /// <param name="column">
    /// The column to retrieve
    /// </param>
    public abstract string? this[TColumn column] { get; }

  }
}
