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
using XsvLib.Tables.Cursor;

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
          return Csv.ReadCsv(reader, skipEmptyLines, separator: ',', leaveOpen: leaveOpen);
        case XsvFormat.Tsv:
          return Tsv.ReadTsv(reader, skipEmptyLines, leaveOpen: leaveOpen);
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
    /// Open an XSV style data file and read the rows in it as a sequence of
    /// XsvCursor objects. The same XsvCursor is returned for each row, bound
    /// to a different row each time.
    /// This method is recommended for scenarios where you know the columns you
    /// are interested in in advance.
    /// </summary>
    /// <param name="filename">
    /// The name of the XSV file to open
    /// </param>
    /// <param name="columns">
    /// The column mapper with the names of the columns you are interested in
    /// already Declared.
    /// </param>
    /// <param name="skipEmptyLines">
    /// Normally true. Setting this to false enables scenarios where
    /// empty input lines must be flagged as error.
    /// </param>
    /// <returns>
    /// A sequence of the same XsvCursor instance repeated for each row.
    /// </returns>
    public static IEnumerable<XsvCursor> ReadXsvCursor(
      string filename, ColumnMap columns, bool skipEmptyLines = true)
    {
      using(var xr = Xsv.ReadXsv(filename, skipEmptyLines).AsXsvReader())
      {
        foreach(var cursor in xr.ReadCursor(columns))
        {
          yield return cursor;
        }
      }
    }

    /// <summary>
    /// Open an XSV style data file and read the rows in it and bind them 
    /// one by one to the given XsvCursor or subclass object, returning
    /// that same object for each row.
    /// </summary>
    /// <typeparam name="TCursor">
    /// XsvCursor, or more likely a custom subclass that sets up specific columns
    /// </typeparam>
    /// <param name="filename">
    /// The name of the file to read
    /// </param>
    /// <param name="cursor">
    /// The cursor object, with columns already declared in its ColumnMap.
    /// The columns could for instance be declared manually, or be declared
    /// in the constructor of an XsvCursor subclass.
    /// </param>
    /// <param name="skipEmptyLines">
    /// Normally true. Setting this to false enables scenarios where
    /// empty input lines must be flagged as error.
    /// </param>
    /// <returns>
    /// A sequence of the argument "cursor" instance repeated for each data
    /// row.
    /// </returns>
    public static IEnumerable<TCursor> ReadXsvCursor<TCursor>(
      string filename, TCursor cursor, bool skipEmptyLines = true)
      where TCursor : XsvCursor
    {
      using(var xr = Xsv.ReadXsv(filename, skipEmptyLines).AsXsvReader())
      {
        foreach(var c in xr.ReadCursor(cursor))
        {
          yield return c;
        }
      }
    }

    /// <summary>
    /// Open an XSV style data source and read the rows in it and bind them 
    /// one by one to the given XsvCursor or subclass object, returning
    /// that same object for each row.
    /// </summary>
    /// <typeparam name="TCursor">
    /// XsvCursor, or more likely a custom subclass that sets up specific columns
    /// </typeparam>
    /// <param name="opener">
    /// A function that opens the data source and returns it.
    /// </param>
    /// <param name="cursor">
    /// The cursor object, with columns already declared in its ColumnMap.
    /// The columns could for instance be declared manually, or be declared
    /// in the constructor of an XsvCursor subclass.
    /// </param>
    /// <returns>
    /// A sequence of the argument "cursor" instance repeated for each data
    /// row.
    /// </returns>
    public static IEnumerable<TCursor> ReadXsvCursor<TCursor>(
      Func<ITextRecordReader> opener, TCursor cursor)
      where TCursor : XsvCursor
    {
      using(var xr = opener().AsXsvReader())
      {
        foreach(var c in xr.ReadCursor(cursor))
        {
          yield return c;
        }
      }
    }

    /// <summary>
    /// Open an XSV style data source and read the rows in it and bind them 
    /// one by one to the given XsvCursor or subclass object, returning
    /// that same object for each row. This overload is shaped as an extension method
    /// on the cursor object.
    /// </summary>
    /// <typeparam name="TCursor">
    /// XsvCursor, or more likely a custom subclass that sets up specific columns
    /// </typeparam>
    /// <param name="cursor">
    /// The cursor object, with columns already declared in its ColumnMap.
    /// The columns could for instance be declared manually, or be declared
    /// in the constructor of an XsvCursor subclass.
    /// </param>
    /// <param name="opener">
    /// A function that opens the data source and returns it.
    /// </param>
    /// <returns>
    /// A sequence of the argument "cursor" instance repeated for each data
    /// row.
    /// </returns>
    public static IEnumerable<TCursor> ReadXsvCursor<TCursor>(
      this TCursor cursor, Func<ITextRecordReader> opener)
      where TCursor : XsvCursor
    {
      return ReadXsvCursor(opener, cursor);
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
          return Csv.WriteCsv(writer, fieldCount);
        case XsvFormat.Tsv:
          return Tsv.WriteTsv(writer, fieldCount);
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
    /// <returns>
    /// An object implementing both ITextRecordWriter and IDisposable
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if the file extension is not recognized.
    /// </exception>
    public static IDisposableTextRecordWriter WriteXsv(
      string filename, int fieldCount = 0)
    {
      switch(XsvFormat.XsvFromFilename(filename, true))
      {
        case XsvFormat.Csv:
          return Csv.WriteCsv(filename, fieldCount);
        case XsvFormat.Tsv:
          return Tsv.WriteTsv(filename, fieldCount);
        default:
          throw new NotSupportedException($"Unsupported file format: '{filename}");
      }
    }
  }

}
