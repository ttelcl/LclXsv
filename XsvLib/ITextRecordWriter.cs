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
  /// Writes records consisting of a sequence of text fields
  /// </summary>
  public interface ITextRecordWriter
  {
    /// <summary>
    /// If larger than 0: the precise number of fields in each line.
    /// If 0, no length check is performed
    /// </summary>
    int FieldCount { get; }

    /// <summary>
    /// Write the next field in the current line
    /// </summary>
    void WriteField(string field);

    /// <summary>
    /// Start a new record (line)
    /// </summary>
    void StartLine();

    /// <summary>
    /// Finish the current record (line)
    /// </summary>
    void FinishLine();

    /// <summary>
    /// Finish the file
    /// </summary>
    void FinishFile();

  }


  /// <summary>
  /// An ITextRecordWriter that is also IDisposable
  /// </summary>
  public interface IDisposableTextRecordWriter: ITextRecordWriter, IDisposable
  {
  }


}

