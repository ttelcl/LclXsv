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

namespace XsvLib.Implementation.Csv
{
  /// <summary>
  /// Wrapper around a TextWriter to write CSV streams. This writer does
  /// not "own" the wrapped TextWriter.
  /// </summary>
  public class CsvWriter: ITextRecordWriter
  {
    private readonly TextWriter _writer;
    private int _fieldsThisLine;
    private char[] _triggerQuoteChars;

    /// <summary>
    /// Create a new CsvWriter
    /// </summary>
    /// <param name="writer">
    /// The TextWriter to write to
    /// </param>
    /// <param name="fieldCount">
    /// The number of columns to expect, or 0 (default) to not check column counts
    /// </param>
    /// <param name="separator">
    /// The separator character to use (default ',')
    /// </param>
    /// <param name="quoteAlways">
    /// When true: emit all fields quoted. When false (default): only emit fields
    /// quoted if necessary.
    /// </param>
    public CsvWriter(
      TextWriter writer,
      int fieldCount = 0,
      char separator = ',',
      bool quoteAlways = false)
    {
      _writer = writer;
      FieldCount = fieldCount;
      QuoteAlways = quoteAlways;
      Separator = separator;
      var tqc = ",;\"'\r\n";
      if(tqc.IndexOf(Separator)<0)
      {
        tqc = Separator.ToString() + tqc;
      }
      _triggerQuoteChars = tqc.ToCharArray();
    }

    /// <summary>
    /// If larger than 0: the number of fields each line must have
    /// </summary>
    public int FieldCount { get; }

    /// <summary>
    /// If true, fields are always quoted, even if not needed
    /// </summary>
    public bool QuoteAlways { get; set; }

    /// <summary>
    /// The CSV separator character (normally ",")
    /// </summary>
    public char Separator { get; }

    /// <summary>
    /// Write the next field (quoting it if necessary)
    /// </summary>
    public void WriteField(string field)
    {
      field = field??String.Empty;
      if(FieldCount>0 && _fieldsThisLine>=FieldCount)
      {
        throw new InvalidOperationException("Too many fields in CSV line");
      }
      if(_fieldsThisLine>0)
      {
        _writer.Write(Separator);
      }
      _fieldsThisLine++;
      if(FieldNeedsQuoting(field))
      {
        // Write Quoted
        _writer.Write('"');
        foreach(var ch in field)
        {
          if(ch=='"')
          {
            _writer.Write("\"\"");
          }
          else
          {
            _writer.Write(ch);
          }
        }
        _writer.Write('"');
      }
      else
      {
        _writer.Write(field);
      }
    }

    /// <summary>
    /// Finish the current line
    /// </summary>
    public void FinishLine()
    {
      if(FieldCount>0 && _fieldsThisLine<FieldCount)
      {
        throw new InvalidOperationException("Too few fields in CSV line");
      }
      Trace.Assert(FieldCount==0 || _fieldsThisLine==FieldCount);
      _fieldsThisLine = 0;
      _writer.WriteLine();
    }

    /// <summary>
    /// Start a new line (verifies that no unfinished line is in progress)
    /// </summary>
    public void StartLine()
    {
      if(_fieldsThisLine!=0)
      {
        throw new InvalidOperationException("Another line was already in progress");
      }
    }

    /// <summary>
    /// Verifies the current state is a valid end of a CSV file.
    /// </summary>
    public void FinishFile()
    {
      if(_fieldsThisLine>0)
      {
        throw new InvalidOperationException("Incomplete last CSV line");
      }
    }

    /// <summary>
    /// Check if a cell value requires quoting (takes QuoteAlways and Separator into account).
    /// </summary>
    public bool FieldNeedsQuoting(string field)
    {
      if(QuoteAlways)
      {
        return true;
      }
      if(String.IsNullOrEmpty(field))
      {
        return false;
      }
      if(Char.IsWhiteSpace(field[0]) || Char.IsWhiteSpace(field[field.Length-1]))
      {
        return true;
      }
      if(field.IndexOfAny(_triggerQuoteChars)>=0)
      {
        return true;
      }
      return false;
    }

  }

}
