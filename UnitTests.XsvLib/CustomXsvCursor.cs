/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XsvLib;
using XsvLib.Tables.Cursor;

namespace UnitTests.XsvLib
{
  /// <summary>
  /// Example custom XsvCursor subclass
  /// </summary>
  public class CustomXsvCursor: XsvCursor
  {
    /// <summary>
    /// Create a new CustomXsvCursor
    /// </summary>
    public CustomXsvCursor()
      : base(null)
    {
      FooColumn = ColumnMapping.Declare("foo");
      BarColumn = ColumnMapping.Declare("bar");
      BazColumn = ColumnMapping.Declare("baz");
    }

    /// <summary>
    /// The column key for the "foo" column
    /// </summary>
    public MappedColumn FooColumn { get; }

    /// <summary>
    /// The column key for the "bar" column
    /// </summary>
    public MappedColumn BarColumn { get; }

    /// <summary>
    /// The column key for the "baz" column
    /// </summary>
    public MappedColumn BazColumn { get; }

    public int Foo => GetInt32(FooColumn);

    public string Bar => GetString(BarColumn);

    public string Baz => GetString(BazColumn);


  }
}
