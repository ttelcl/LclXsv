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

namespace XsvLib.Implementation.Tsv
{
  /// <summary>
  /// Implements ITextRecordWriter for TSV files
  /// </summary>
  public class TsvWriter: ITextRecordWriter
  {
    private readonly TextWriter _writer;
    private int _fieldsThisLine;
    private char[] _badChars;

    /// <summary>
    /// Create a new TsvWriter
    /// </summary>
    public TsvWriter(
      TextWriter writer,
      int fieldCount = 0)
    {
      _writer = writer;
      FieldCount = fieldCount;
      _badChars = "\t\r\n".ToCharArray();
    }

    /// <summary>
    /// If larger than 0, the precise number of fields in each line. If 0, no
    /// length check is performed
    /// </summary>
    public int FieldCount { get; }

    /// <summary>
    /// Start a new line (verifying we were not in the middle of another one)
    /// </summary>
    public void StartLine()
    {
      if(_fieldsThisLine!=0)
      {
        throw new InvalidOperationException("Another line was already in progress");
      }
    }

    /// <summary>
    /// Write the next field in the current line
    /// </summary>
    public void WriteField(string field)
    {
      field = field??String.Empty;
      if(FieldCount>0 && _fieldsThisLine>=FieldCount)
      {
        throw new InvalidOperationException("Too many fields in TSV line");
      }
      if(_fieldsThisLine>0)
      {
        _writer.Write('\t');
      }
      _fieldsThisLine++;
      if(field.IndexOfAny(_badChars)>=0)
      {
        throw new ArgumentException("Field contains character that is not supported by TSV (tab or newline)");
      }
      _writer.Write(field);
    }

    /// <summary>
    /// Finish writing the current line
    /// </summary>
    public void FinishLine()
    {
      if(FieldCount>0 && _fieldsThisLine<FieldCount)
      {
        throw new InvalidOperationException("Too few fields in TSV line");
      }
      Trace.Assert(FieldCount==0 || _fieldsThisLine==FieldCount);
      _fieldsThisLine = 0;
      _writer.WriteLine();
    }

    /// <summary>
    /// Finish the file, as far as TSV writing is concerned (does not close the underlying TextWriter)
    /// </summary>
    public void FinishFile()
    {
      if(_fieldsThisLine>0)
      {
        throw new InvalidOperationException("Incomplete last TSV line");
      }
    }
  }


}