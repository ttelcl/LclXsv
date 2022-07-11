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

using XsvLib.Implementation.Csv;
using XsvLib.Implementation;
using XsvLib.Implementation.Tsv;

namespace XsvLib
{
  /// <summary>
  /// Static class acting as API entrypoint for this library
  /// </summary>
  public static class Xsv
  {
    /// <summary>
    /// Wraps a TextReader to read data in some kind of XSV-style data format.
    /// </summary>
    /// <param name="reader">
    /// The reader to read from
    /// </param>
    /// <param name="formatname">
    /// The name of the file (or pseudo-file), whose file extension determines
    /// the format the file is in. Supported formats include .csv and .tsv.
    /// This argument is only used to determine the data format and doesn't need
    /// to be a valid file name, only to have a valid file extension.
    /// </param>
    /// <param name="skipEmptyLines">
    /// Whether or not to skip empty lines. Default true.
    /// </param>
    /// <param name="leaveOpen">
    /// When false (default), disposing the returned IDisposableTextRecordReader also
    /// disposes the TextReader.
    /// </param>
    /// <returns>
    /// An object implementing both IDisposable and ITextRecordReader
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if the file extension is not recognized.
    /// </exception>
    public static IDisposableTextRecordReader ReadXsv(
      TextReader reader, string formatname, bool skipEmptyLines = true, bool leaveOpen = false)
    {
      switch(XsvFormat.XsvFromFilename(formatname, false))
      {
        case XsvFormat.Csv:
          return ReadCsv(reader, skipEmptyLines, separator: ',', leaveOpen: leaveOpen);
        case XsvFormat.Tsv:
          return ReadTsv(reader, skipEmptyLines, leaveOpen: leaveOpen);
        default:
          throw new NotSupportedException($"Unsupported file format: {formatname}");
      }
    }

    /// <summary>
    /// Open an XSV-style data file. Supported formats include .csv and .tsv
    /// </summary>
    /// <param name="filename">
    /// The name of the file to open. The file extension determines the format.
    /// </param>
    /// <param name="skipEmptyLines">
    /// Whether or not to skip empty lines. Default true.
    /// </param>
    /// <returns>
    /// An object implementing ITextRecordReader and IDisposable
    /// </returns>
    public static IDisposableTextRecordReader ReadXsv(
      string filename, bool skipEmptyLines = true)
    {
      var reader = File.OpenText(filename);
      return ReadXsv(reader, filename, skipEmptyLines, leaveOpen: false);
    }

    /// <summary>
    /// Wrap a TextWriter in in ITextRecordWriter implementation for writing to
    /// one of the supported XSV formats (CSV or TSV).
    /// </summary>
    /// <param name="writer">
    /// The TextWriter to wrap. The lifetime of that TextWriter is the caller's 
    /// responsibility.
    /// </param>
    /// <param name="formatname">
    /// The pseudo-filename to derive the format from. Only the file extension matters.
    /// In addition to the standard format names (*.csv, *.tsv) also the extensions with
    /// an additional ".tmp" are recognized (*.csv.tmp, *.tsv.tmp)
    /// </param>
    /// <param name="fieldCount">
    /// If not 0: the number of fields expected in each row. If 0 (default) no
    /// field count check is performed.
    /// </param>
    /// <returns>
    /// An ITextRecordWriter implementation
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if the file extension is not recognized.
    /// </exception>
    public static ITextRecordWriter WriteXsv(
      TextWriter writer, string formatname, int fieldCount=0)
    {
      switch(XsvFormat.XsvFromFilename(formatname, true))
      {
        case XsvFormat.Csv:
          return WriteCsv(writer, fieldCount);
        case XsvFormat.Tsv:
          return WriteTsv(writer, fieldCount);
        default:
          throw new NotSupportedException($"Unsupported file format: {formatname}");
      }
    }

    /// <summary>
    /// Create a new file for writing a supported XSV format (CSV or TSV)
    /// </summary>
    /// <param name="filename">
    /// The name of the file, with a supported file extension, or a supported file extension
    /// plus an additional ".tmp".
    /// </param>
    /// <param name="fieldCount">
    /// If not 0: the number of fields expected in each row. If 0 (default) no
    /// field count check is performed.
    /// </param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if the file extension is not recognized.
    /// </exception>
    public static IDisposableTextRecordWriter WriteXsv(
      string filename, int fieldCount = 0)
    {
      switch(XsvFormat.XsvFromFilename(filename, true))
      {
        case XsvFormat.Csv:
          return WriteCsv(filename, fieldCount);
        case XsvFormat.Tsv:
          return WriteTsv(filename, fieldCount);
        default:
          throw new NotSupportedException($"Unsupported file format: '{filename}");
      }
    }

    /// <summary>
    /// Open a reader for reading CSV lines from the given TextReader, and 
    /// return it as an object that implements ITextRecordReader and IDisposable
    /// </summary>
    /// <param name="tr">
    /// The text reader to read from
    /// </param>
    /// <param name="skipEmptyLines">
    /// Whether or not to skip empty lines. Default true.
    /// </param>
    /// <param name="separator">
    /// The CSV separator character to use. Default ','. This method does not attempt to
    /// automatically guess the separator.
    /// </param>
    /// <param name="leaveOpen">
    /// When false (default), disposing the returned object does also dispose the input
    /// TextReader
    /// </param>
    /// <returns>
    /// An object implementing both IDisposable and ITextRecordReader
    /// </returns>
    public static IDisposableTextRecordReader ReadCsv(
      TextReader tr, bool skipEmptyLines = true, char separator = ',', bool leaveOpen = false)
    {
      return leaveOpen
        ? new TextRecordReaderWrapper(new CsvReader(tr, skipEmptyLines, separator))
        : new TextRecordReaderWrapper(new CsvReader(tr, skipEmptyLines, separator), tr);
    }

    /// <summary>
    /// Open a CSV file and return it as an object that implements IDisposable and ITextRecordReader
    /// </summary>
    /// <param name="filename">
    /// The file to open
    /// </param>
    /// <param name="skipEmptyLines">
    /// Whether or not to skip empty lines. Default true.
    /// </param>
    /// <param name="separator">
    /// The CSV separator character to use. Default ','. This method does not attempt to
    /// automatically guess the separator.
    /// </param>
    /// <returns>
    /// An object implementing both IDisposable and ITextRecordReader
    /// </returns>
    public static IDisposableTextRecordReader ReadCsv(
      string filename, bool skipEmptyLines = true, char separator = ',')
    {
      var reader = File.OpenText(filename);
      return ReadCsv(reader, skipEmptyLines, separator, false);
    }

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
    /// Expose an in-memory collection of CSV formatted lines as an ITextRecordReader
    /// </summary>
    public static ITextRecordReader ParseCsv(ICollection<string> lines, char separator = ',')
    {
      return new DelegateTextRecordReader(
        () => CsvParser.ParseLines(lines, separator));
    }

    /// <summary>
    /// Create an ITextRecordWriter instance for writing CSV.
    /// </summary>
    /// <param name="writer">
    /// The TextWriter to write to. Remember to close it after you finish writing the data
    /// (ITextRecordWriter does not do that)
    /// </param>
    /// <param name="fieldCount">
    /// The number of columns to be written, or 0 (default) to not verify column counts
    /// </param>
    /// <param name="separator">
    /// The separator character to use (default ',')
    /// </param>
    /// <returns>
    /// An object implementing ITextRecordWriter
    /// </returns>
    public static ITextRecordWriter WriteCsv(TextWriter writer, int fieldCount = 0, char separator = ',')
    {
      return new CsvWriter(writer, fieldCount, separator);
    }

    /// <summary>
    /// Create an ITextRecordWriter instance for writing CSV to the given file
    /// </summary>
    /// <param name="filename">
    /// The name of the file to write to
    /// </param>
    /// <param name="fieldCount">
    /// The number of columns in the file, or 0 (default) to not verify column counts
    /// </param>
    /// <param name="separator">
    /// The separator character to use (default ',')
    /// </param>
    /// <returns>
    /// An object implementing both ITextRecordWriter and IDisposable
    /// </returns>
    public static IDisposableTextRecordWriter WriteCsv(string filename, int fieldCount = 0, char separator = ',')
    {
      var writer = File.CreateText(filename);
      return new TextRecordWriterWrapper(WriteCsv(writer, fieldCount, separator), writer);
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
