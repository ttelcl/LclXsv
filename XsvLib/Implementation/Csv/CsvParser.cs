/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.Implementation.Csv
{

  /// <summary>
  /// Static class for parsing CSV content
  /// </summary>
  public static class CsvParser
  {
    /// <summary>
    /// Parse a text reader as CSV style text into a sequence of string records.
    /// </summary>
    /// <param name="lines">
    /// The lines to parse
    /// </param>
    /// <param name="separator">
    /// The separator character to use (default ','). Accepted separator characters are the 
    /// characters in the string ",; :|/*\#+_"
    /// </param>
    /// <param name="skipEmptyFields">
    /// Default false. When true, empty unquoted fields are removed (usually a bad idea, but there are some
    /// rare use cases)
    /// </param>
    /// <param name="trimSpace">
    /// When true (default), whitespace surrounding unquoted fields is trimmed
    /// </param>
    /// <param name="supportSingleQuote">
    /// Default false. When true, fields can be quoted in single quotes as well as double quotes
    /// </param>
    /// <remarks>
    /// <para>
    /// This CSV parser supports:
    /// </para>
    /// <list type="bullet">
    /// <item>Parsing CSV text using the specified separator character (default ',')</item>
    /// <item>Recognizes quoted fields (quoted by double quotes, '"'), and supports quotes inside
    /// such quoted fields by doubling the quote character</item>
    /// <item>Quoted fields are allowed to contain line breaks</item>
    /// <item>By default whitespace surrounding field values is trimmed (use a quoted field
    /// for explicit leading or trailing spaces)</item>
    /// </list>
    /// </remarks>
    public static IEnumerable<IReadOnlyList<string>> ParseLines(
      IEnumerable<string> lines,
      char separator = ',',
      bool skipEmptyFields = false,
      bool trimSpace = true,
      bool supportSingleQuote = false)
    {
      var state = new ParserState(separator, skipEmptyFields, trimSpace, supportSingleQuote);
      IReadOnlyList<string>? fields;

      foreach(var line in lines)
      {
        foreach(var ch in line)
        {
          state.ParseChar(ch); // output can only be null
        }
        state.ParseChar('\r');
        fields = state.ParseChar('\n');
        if(fields != null)
        {
          // it is possible that a line break occurs in a quoted field,
          // so this condition does not always trigger
          yield return fields;
        }
      }
      fields = state.ParseEof();
      if(fields != null)
      {
        yield return fields;
      }
    }

  }

}
