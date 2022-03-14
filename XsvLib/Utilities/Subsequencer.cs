/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Utilities
{
  /// <summary>
  /// Wraps a host sequence to expose it as smaller sub-sequences.
  /// Enumerate parts of the sequence by enumerating the result of the Rest()
  /// method. Call Break() during iteration to end the subsequence.
  /// Call Rest() again to start enumerating the next subsequence.
  /// </summary>
  public class Subsequencer<T>: IDisposable
  {
    private readonly IEnumerable<T>? _hostEnumerable;
    private IEnumerator<T>? _host;
    private bool _disposed;
    private bool _exhausted;
    private bool _breakRequest;

    /// <summary>
    /// Create a new Subsequencer. 
    /// </summary>
    public Subsequencer(IEnumerable<T> host)
    {
      if(host == null)
      {
        _disposed = true;
        _exhausted = true;
        _host = null;
        _hostEnumerable = null;
      }
      else
      {
        _disposed = false;
        _exhausted = false;
        _hostEnumerable = host;
        _host = host.GetEnumerator();
      }
    }

    /// <summary>
    /// Clean up
    /// </summary>
    public void Dispose()
    {
      if(!_disposed)
      {
        _disposed = true;
        _exhausted = true;
        _host?.Dispose();
      }
    }

    /// <summary>
    /// The host sequence
    /// </summary>
    public IEnumerable<T>? Host { get => _hostEnumerable; }

    /// <summary>
    /// True if the host sequence has been completely enumerated
    /// </summary>
    public bool IsExhausted { get { return _exhausted; } }

    /// <summary>
    /// True if this object has been disposed
    /// </summary>
    public bool IsDisposed { get { return _disposed; } }

    /// <summary>
    /// Start enumerating the remainder of the host sequence. Dispose the returned
    /// enumerable or call Break() to finish the subsequence. Calling this after the
    /// host stream is exhausted but before this Subsequencee is disposed returns an
    /// empty sequence.
    /// </summary>
    public IEnumerable<T> Rest()
    {
      if(_disposed)
      {
        throw new ObjectDisposedException("Attempt to enumerate a disposed Subsequencer");
      }
      while(!_breakRequest && !_exhausted && !_disposed)
      {
        var hasNext = _host!.MoveNext();
        if(!hasNext)
        {
          _exhausted = true;
        }
        else
        {
          yield return _host.Current;
        }
      }
      _breakRequest = false; // reset it
    }

    /// <summary>
    /// Reset to the original state after constructing, rewinding the iteration to the start
    /// </summary>
    public void Reset()
    {
      if(_disposed)
      {
        throw new ObjectDisposedException("Attempt to reset a disposed Subsequencer");
      }
      if(_hostEnumerable == null)
      {
        _disposed = true;
        _exhausted = true;
        _host = null;
      }
      else
      {
        _disposed = false;
        _exhausted = false;
        _host = _hostEnumerable.GetEnumerator();
      }
    }

    /// <summary>
    /// Finish the current subsequence, making the sequence returned by Rest() act as completed
    /// upon its next iteration. The break request is reset when that iteration finishes. Does
    /// not interact with the Next(), NextOrDefault() and TryNext() APIs.
    /// </summary>
    public void Break()
    {
      _breakRequest = true;
    }

    /// <summary>
    /// Get the next iteration value directly (bypassing the Rest() sequence). Throws an
    /// exception if there is no next iteration.
    /// Effectively equal to Rest().First(), except it ignores a pending break request.
    /// </summary>
    public T Next()
    {
      if(_disposed)
      {
        throw new ObjectDisposedException("Attempt to enumerate a disposed Subsequencer");
      }
      if(_exhausted)
      {
        throw new InvalidOperationException("The sequence has no more elements");
      }
      var hasNext = _host!.MoveNext();
      if(!hasNext)
      {
        _exhausted = true;
        throw new InvalidOperationException("The sequence has no more elements");
      }
      return _host.Current;
    }

    /// <summary>
    /// Like Next(), but returning the default for T if the sequence is finished
    /// Effectively equal to Rest().FirstOrDefault(), except it ignores a pending break request.
    /// </summary>
    public T? NextOrDefault()
    {
      if(_disposed)
      {
        throw new ObjectDisposedException("Attempt to enumerate a disposed Subsequencer");
      }
      if(_exhausted)
      {
        return default;
      }
      var hasNext = _host!.MoveNext();
      if(!hasNext)
      {
        _exhausted = true;
        return default;
      }
      return _host.Current;
    }

    /// <summary>
    /// A variant of Next() that returns success and status as two separate values
    /// (returning true on success)
    /// </summary>
    public bool TryNext(out T? value)
    {
      if(_disposed)
      {
        throw new ObjectDisposedException("Attempt to enumerate a disposed Subsequencer");
      }
      value = default;
      if(_exhausted)
      {
        return false;
      }
      var hasNext = _host!.MoveNext();
      if(!hasNext)
      {
        _exhausted = true;
        return false;
      }
      value = _host.Current;
      return true;
    }

  }

}
