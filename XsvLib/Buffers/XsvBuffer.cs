﻿/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Buffers
{
  /// <summary>
  /// Provides a buffer for an XSV row, for reading or writing
  /// </summary>
  /// <remarks>
  /// <para>
  /// Usage has two phases: first declare columns, then use Lock()
  /// or Lock(headers) to freeze the column setup and use the buffer
  /// </para>
  /// <para>
  /// In the use phase you can optionally attach an external backing
  /// buffer via any of the "Attach()" methods, and detach it
  /// again via "Detach()".
  /// </para>
  /// </remarks>
  public class XsvBuffer
  {
    private readonly ColumnMap _columnMap;
    private readonly List<XsvColumnAccessor> _accessors;
    private string[]? _buffer;
    private IReadOnlyList<string>? _attachedReadonlyList;
    private string[]? _attachedArray;
    private List<string>? _attachedList;

    /// <summary>
    /// Create a new XsvBuffer.
    /// </summary>
    public XsvBuffer(bool caseSensitive = false)
    {
      _columnMap = new ColumnMap(caseSensitive);
      _accessors = new List<XsvColumnAccessor>();
      Accessors = _accessors.AsReadOnly();
      _buffer = null;
    }

    /// <summary>
    /// Get an accessor for the named column. If the column
    /// does not exist yet, it is created, or fails if this
    /// buffer has been locked. If created, a column index is
    /// assigned (in declaration order).
    /// </summary>
    /// <param name="name">
    /// The column name tor retrieve or create
    /// </param>
    /// <param name="declare">
    /// (default false). When true, it is an error if the column already exists
    /// </param>
    /// <returns>
    /// An XsvColumnAccessor
    /// </returns>
    public XsvColumnAccessor GetColumn(string name, bool declare = false)
    {
      var mco = _columnMap.Find(name);
      if(mco != null)
      {
        if(declare)
        {
          throw new InvalidOperationException(
            $"Duplicate column name '{name}'");
        }
        return new XsvColumnAccessor(this, mco);
      }
      else
      {
        if(IsLocked)
        {
          throw new InvalidOperationException(
            $"Cannot create new columns after locking this XsvBuffer ({name})");
        }
        var mc = _columnMap.Declare(name, declare);
        var xca = new XsvColumnAccessor(this, mc);
        mc.Index = _accessors.Count;
        _accessors.Add(xca);
        return xca;
      }
    }

    /// <summary>
    /// Shorthand for GetColumn(name, true)
    /// </summary>
    /// <param name="name">
    /// The column name to register
    /// </param>
    public XsvColumnAccessor Declare(string name)
    {
      return GetColumn(name, true);
    }

    /// <summary>
    /// The column accessors (initially in declaration order, but possibly
    /// reordered by Lock(headers))
    /// </summary>
    public IReadOnlyList<XsvColumnAccessor> Accessors { get; }

    /// <summary>
    /// Lock this buffer, preventing any further column changes.
    /// Also assigns column indices in declaration order.
    /// </summary>
    public void Lock()
    {
      if(!IsLocked)
      {
        _buffer = new string[_accessors.Count];
        for(var i = 0; i < _buffer.Length; i++)
        {
          _buffer[i] = String.Empty;
        }
      }
    }

    /// <summary>
    /// Lock this buffer, preventing any further column changes.
    /// Adds any values in "headers" as columns, if they were missing.
    /// Each previously declared column must occur in "headers".
    /// Reorders columns and accessors, and assigns indices, to match "headers".
    /// </summary>
    public void Lock(IEnumerable<string> headers)
    {
      var hdr = headers.ToList();
      foreach(var h in hdr)
      {
        GetColumn(h); // add if not already present, abort if already locked
      }
      _columnMap.BindColumns(hdr);
      var unbound = _columnMap.UnboundColumns().ToList();
      if(unbound.Count > 0)
      {
        var unboundNames = String.Join(", ", unbound.Select(x => x.Name));
        throw new InvalidOperationException(
          $"Not all declared column names appeared in the headers. Missing: {unboundNames}");
      }
      var sortedAccessors =
        (from accessor in _accessors
        orderby accessor.Column.Index
        select accessor).ToList();
      _accessors.Clear();
      _accessors.AddRange(sortedAccessors);
      if(!IsLocked)
      {
        _buffer = new string[_accessors.Count];
        for(var i = 0; i < _buffer.Length; i++)
        {
          _buffer[i] = String.Empty;
        }
      }
    }

    /// <summary>
    /// Shortcut for Lock(reader.Header)
    /// </summary>
    public void Lock(XsvReader source)
    {
      Lock(source.Header);
    }

    /// <summary>
    /// True if this buffer is locked (columns can no longer changed)
    /// </summary>
    public bool IsLocked { get => _buffer != null; }

    /// <summary>
    /// The number of columns.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Get or set a value in the buffer. Not usable before
    /// calling Lock() or Lock(headers).
    /// If an external buffer is attached that is used as backing,
    /// otherwise an internal buffer is used
    /// </summary>
    /// <param name="i">
    /// The column index
    /// </param>
    /// <returns>
    /// The column value in the buffer
    /// </returns>
    public string this[int i] {
      get {
        CheckLocked();
        if(i < 0 || i >= _buffer!.Length)
        {
          throw new IndexOutOfRangeException($"Invalid column index");
        }
        if(_attachedArray!=null)
        {
          return _attachedArray[i];
        }
        else if(_attachedList!=null)
        {
          return _attachedList[i];
        }
        else if(_attachedReadonlyList!=null)
        {
          return _attachedReadonlyList[i];
        }
        else
        {
          return _buffer[i];
        }
      }
      set {
        CheckLocked();
        if(i < 0 || i >= _buffer!.Length)
        {
          throw new IndexOutOfRangeException($"Invalid column index");
        }
        if(_attachedArray!=null)
        {
          _attachedArray[i] = value;
        }
        else if(_attachedList!=null)
        {
          _attachedList[i] = value;
        }
        else if(_attachedReadonlyList!=null)
        {
          throw new InvalidOperationException("The external buffer is read-only");
        }
        else
        {
          _buffer[i] = value;
        }
      }
    }

    /// <summary>
    /// Return the buffer as an IReadOnlyList. The actual buffer depends
    /// on which buffer is attached, if any.
    /// </summary>
    public IReadOnlyList<string> Buffer {
      get {
        CheckLocked();
        if(_attachedArray!=null)
        {
          return _attachedArray;
        }
        else if(_attachedList!=null)
        {
          return _attachedList;
        }
        else if(_attachedReadonlyList!=null)
        {
          return _attachedReadonlyList;
        }
        else
        {
          return _buffer!;
        }
      }
    }

    /// <summary>
    /// Detach any external buffers
    /// </summary>
    public void Detach()
    {
      _attachedArray = null;
      _attachedList = null;
      _attachedReadonlyList = null;
    }

    /// <summary>
    /// Attach an array as external buffer and detach other external buffers
    /// </summary>
    public void Attach(string[] buffer)
    {
      CheckLocked();
      if(buffer.Length != _buffer!.Length)
      {
        throw new ArgumentException("Incorrect buffer length");
      }
      _attachedArray = buffer;
      _attachedList = null;
      _attachedReadonlyList = null;
    }

    /// <summary>
    /// Attach an IReadOnlyList as external buffer and detach other external buffers.
    /// Until it is detached this XsvBuffer will be readonly!
    /// </summary>
    public void Attach(IReadOnlyList<string> buffer)
    {
      CheckLocked();
      if(buffer.Count != _buffer!.Length)
      {
        throw new ArgumentException("Incorrect buffer length");
      }
      _attachedArray = null;
      _attachedList = null;
      _attachedReadonlyList = buffer;
    }

    /// <summary>
    /// Attach an IList as external buffer and detach other external buffers
    /// </summary>
    public void Attach(List<string> buffer)
    {
      CheckLocked();
      if(buffer.Count != _buffer!.Length)
      {
        throw new ArgumentException("Incorrect buffer length");
      }
      _attachedArray = null;
      _attachedList = buffer;
      _attachedReadonlyList = null;
    }

    /// <summary>
    /// Write the current row to an ITextRecordWriter
    /// </summary>
    public void WriteRow(ITextRecordWriter itrw)
    {
      CheckLocked();
      itrw.StartLine();
      foreach(var a in Accessors)
      {
        itrw.WriteField(a.Value);
      }
      itrw.FinishLine();
    }

    /// <summary>
    /// Write the header to an ITextRecordWriter
    /// </summary>
    public void WriteHeader(ITextRecordWriter itrw)
    {
      CheckLocked();
      itrw.StartLine();
      foreach(var a in Accessors)
      {
        itrw.WriteField(a.Column.Name);
      }
      itrw.FinishLine();
    }

    private void CheckLocked()
    {
      if(!IsLocked)
      {
        throw new InvalidOperationException(
          $"Expecting the buffer to be locked before this operation");
      }
    }
  }
}
