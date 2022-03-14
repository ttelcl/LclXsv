/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XsvLib.Implementation;

namespace XsvLib
{
  /// <summary>
  /// Extension methods on interfaces in this library
  /// </summary>
  public static class XsvExtensions
  {
    /// <summary>
    /// Load all records from the reader into a new list, optionally cloning the records
    /// </summary>
    /// <param name="itrr">
    /// The ITextRecordReader to read the records from
    /// </param>
    /// <param name="clone">
    /// If true: clone each record. This supports ITextRecordReader implementations that
    /// reuse the returned record object
    /// </param>
    /// <returns></returns>
    public static List<IReadOnlyList<string>> LoadAll(this ITextRecordReader itrr, bool clone)
    {
      var records = itrr.ReadRecords();
      if(clone)
      {
        records = records.Select(row => row.ToArray());
      }
      return records.ToList();
    }

    /// <summary>
    /// Wrap a TextReader as an object implementing ILinesReader
    /// </summary>
    public static ILinesReader LinesFromTextReader(this TextReader tr, bool skipEmptyLines)
    {
      return new StreamLinesReader(tr, skipEmptyLines);
    }
  }
}
