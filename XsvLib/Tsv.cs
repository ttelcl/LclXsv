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
using XsvLib.Implementation.Tsv;

using XsvLib.Implementation;

namespace XsvLib
{
  /// <summary>
  /// Static methods specific to TSV (not generic XSV) files
  /// </summary>
  public static class Tsv
  {

    /// <summary>
    /// Open a reader for reading TSV lines from the given TextReader, and
    /// return it as an object that implements ITextRecordReader and IDisposable
    /// </summary>
    /// <param name="tr">
    /// The TextReader to read from
    /// </param>
    /// <param name="skipEmptyLines">
    /// Whether or not to skip empty lines. Default true.
    /// </param>
    /// <param name="leaveOpen">
    /// When false (default), disposing the returned object does also dispose the input
    /// TextReader
    /// </param>
    /// <returns>
    /// An object implementing both IDisposable and ITextRecordReader
    /// </returns>
    public static IDisposableTextRecordReader ReadTsv(
      TextReader tr, bool skipEmptyLines = true, bool leaveOpen = false)
    {
      return leaveOpen
        ? new TextRecordReaderWrapper(new TsvReader(tr, skipEmptyLines))
        : new TextRecordReaderWrapper(new TsvReader(tr, skipEmptyLines), tr);
    }

    /// <summary>
    /// Open the file as TSV data file
    /// </summary>
    public static IDisposableTextRecordReader ReadTsv(
      string filename, bool skipEmptylines = true)
    {
      return ReadTsv(File.OpenText(filename), skipEmptylines);
    }

    /// <summary>
    /// Create an ITextRecordWriter instance for writing TSV.
    /// </summary>
    /// <param name="writer">
    /// The TextWriter to write to. Remember to close it after you finish writing the data
    /// (ITextRecordWriter does not do that)
    /// </param>
    /// <param name="fieldCount">
    /// The number of columns to be written, or 0 (default) to not verify column counts
    /// </param>
    /// <returns>
    /// An object implementing ITextRecordWriter
    /// </returns>
    public static ITextRecordWriter WriteTsv(TextWriter writer, int fieldCount = 0)
    {
      return new TsvWriter(writer, fieldCount);
    }

    /// <summary>
    /// Create an ITextRecordWriter instance for writing TSV to the given file
    /// </summary>
    /// <param name="filename">
    /// The name of the file to write to
    /// </param>
    /// <param name="fieldCount">
    /// The number of columns in the file, or 0 (default) to not verify column counts
    /// </param>
    /// <returns>
    /// An object implementing both ITextRecordWriter and IDisposable
    /// </returns>
    public static IDisposableTextRecordWriter WriteTsv(string filename, int fieldCount = 0)
    {
      var writer = File.CreateText(filename);
      return new TextRecordWriterWrapper(WriteTsv(writer, fieldCount), writer);
    }

  }
}
