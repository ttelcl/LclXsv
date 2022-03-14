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
    /// to be a valiud file name, only to have a valid file extension.
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
    /// Thrown when the file extension is not recognized.
    /// </exception>
    public static IDisposableTextRecordReader OpenXsv(
      TextReader reader, string formatname, bool skipEmptyLines = true, bool leaveOpen = false)
    {
      switch(XsvFormat.XsvFromFilename(formatname))
      {
        case XsvFormat.Csv:
          return OpenCsv(reader, skipEmptyLines, separator: ',', leaveOpen: leaveOpen);
        case XsvFormat.Tsv:
          return OpenTsv(reader, skipEmptyLines, leaveOpen: leaveOpen);
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
    public static IDisposableTextRecordReader OpenXsv(
      string filename, bool skipEmptyLines = true)
    {
      var reader = File.OpenText(filename);
      return OpenXsv(reader, filename, skipEmptyLines, leaveOpen: false);
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
    public static IDisposableTextRecordReader OpenCsv(
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
    public static IDisposableTextRecordReader OpenCsv(
      string filename, bool skipEmptyLines = true, char separator = ',')
    {
      var reader = File.OpenText(filename);
      return OpenCsv(reader, skipEmptyLines, separator, false);
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
    public static IDisposableTextRecordReader OpenTsv(
      TextReader tr, bool skipEmptyLines = true, bool leaveOpen = false)
    {
      return leaveOpen
        ? new TextRecordReaderWrapper(new TsvReader(tr, skipEmptyLines))
        : new TextRecordReaderWrapper(new TsvReader(tr, skipEmptyLines), tr);
    }

    /// <summary>
    /// Open the file as TSV data file
    /// </summary>
    public static IDisposableTextRecordReader OpenTsv(
      string filename, bool skipEmptylines = true)
    {
      return OpenTsv(File.OpenText(filename), skipEmptylines);
    }

    /// <summary>
    /// Expose an in-memory collection of CSV formatted lines as an ITextRecordReader
    /// </summary>
    public static ITextRecordReader ParseCsv(ICollection<string> lines, char separator = ',')
    {
      return new DelegateTextRecordReader(
        () => CsvParser.ParseLines(lines, separator));
    }

  }

}
