/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XsvLib
{
  /// <summary>
  /// Implemented by objects that can produce a series of string-valued records.
  /// </summary>
  public interface ITextRecordReader
  {
    /// <summary>
    /// Enumerates the sequence of string-valued records (shaped as
    /// IReadOnlyList{string}).
    /// </summary>
    /// <remarks>
    /// <para>
    /// A typical implementation could return string[] instances, but may
    /// choose a different IReadOnlyList implementation.
    /// </para>
    /// <para>
    /// Clients must assume that returned values are only valid until the next
    /// iteration (implementations may re-use their IReadOnlyList instance).
    /// </para>
    /// </remarks>
    IEnumerable<IReadOnlyList<string>> ReadRecords();
  }

  /// <summary>
  /// An ITextRecordReader that is also IDisposable
  /// </summary>
  public interface IDisposableTextRecordReader : ITextRecordReader, IDisposable
  {
  }

}

