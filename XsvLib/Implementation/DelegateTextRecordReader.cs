/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Implementation
{
  /// <summary>
  /// A simple ITextRecordReader implementation that defers implementation to a delegate
  /// </summary>
  public class DelegateTextRecordReader: ITextRecordReader
  {
    private readonly Func<IEnumerable<IReadOnlyList<string>>> _factory;

    /// <summary>
    /// Create a new DelegateTextRecordReader
    /// </summary>
    public DelegateTextRecordReader(
      Func<IEnumerable<IReadOnlyList<string>>> factory)
    {
      _factory = factory;
    }

    /// <summary>
    /// Implements the ITextRecordReader interface via the delegate provided
    /// in the constructor
    /// </summary>
    public IEnumerable<IReadOnlyList<string>> ReadRecords()
    {
      return _factory();
    }
  }
}