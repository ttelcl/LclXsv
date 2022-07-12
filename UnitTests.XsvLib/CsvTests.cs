using System;
using System.IO;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

using XsvLib;
using XsvLib.Implementation;
using XsvLib.Implementation.Csv;
using XsvLib.Tables.Cursor;

namespace UnitTests.XsvLib
{
  public class CsvTests
  {
    private readonly ITestOutputHelper _output;

    public CsvTests(ITestOutputHelper output)
    {
      _output = output;
    }

    [Fact]
    public void CanParseCsv()
    {
      var csv1 =
        new[] {
          "foo,bar, baz",
          "1,2 ,3",
          "\"hello, world!\",\"hello\",\"\"\"world\"\"\""
        };

      var records =
        CsvParser.ParseLines(csv1, separator: ',')
        .Select(row => row.ToArray()) // also support a volatile implementation, just in case
        .ToList();

      Assert.Equal(3, records.Count);
      Assert.Equal(3, records[0].Length);
      Assert.Equal(3, records[1].Length);
      Assert.Equal(3, records[2].Length);

      Assert.Equal("foo", records[0][0]);
      Assert.Equal("bar", records[0][1]);
      Assert.Equal("baz", records[0][2]); // note: leading space trimmed

      Assert.Equal("1", records[1][0]);
      Assert.Equal("2", records[1][1]); // note: trailing space trimmed
      Assert.Equal("3", records[1][2]);

      Assert.Equal("hello, world!", records[2][0]); // Embedded comma in content
      Assert.Equal("hello", records[2][1]); // Superfluous quotes removed
      Assert.Equal("\"world\"", records[2][2]); // Embedded quotes in content via quote doubling

    }

    [Fact]
    public void CanParseCsvViaReader()
    {
      var csv1 =
        new[] {
          "foo,bar, baz",
          "1,2 ,3",
        };
      
      var itrr = Csv.ParseCsv(csv1, separator: ',');

      var records = itrr.LoadAll(true);

      Assert.Equal(2, records.Count);
      Assert.Equal(3, records[0].Count);
      Assert.Equal(3, records[1].Count);
      Assert.Equal("foo", records[0][0]);
      Assert.Equal("bar", records[0][1]);
      Assert.Equal("baz", records[0][2]); // note: leading space trimmed
      Assert.Equal("1", records[1][0]);
      Assert.Equal("2", records[1][1]); // note: trailing space trimmed
      Assert.Equal("3", records[1][2]);
    }

    [Fact]
    public void CanParseCsvWithHeaderViaXsvReader()
    {
      var csv1 =
        new[] {
          "foo,bar, baz",
          "1,2,3",
          "4,5,6",
        };
      // var xsv = new XsvReader(Csv.ParseCsv(csv1))
      using(var xsv = Csv.ParseCsv(csv1).AsXsvReader())
      {
        Assert.NotNull(xsv.Header);
        var records = xsv.LoadAll(true);

        Assert.Equal(3, xsv.Header.Count);
        Assert.Equal("foo", xsv.Header[0]);
        Assert.Equal("bar", xsv.Header[1]);
        Assert.Equal("baz", xsv.Header[2]);

        Assert.Equal(2, records.Count);
        Assert.Equal("1", records[0][0]);
        Assert.Equal("2", records[0][1]);
        Assert.Equal("3", records[0][2]);
        Assert.Equal("4", records[1][0]);
        Assert.Equal("5", records[1][1]);
        Assert.Equal("6", records[1][2]);
      }
    }

    [Fact]
    public void ColumnMappingTest()
    {
      var cm = new ColumnMap();

      var c1 = cm.Declare("c");
      var c2 = cm.Declare("e");
      var c3 = cm.Declare("g");
      var c4 = cm.Declare("a");
      var c5 = cm.Declare("b");

      // Note: "d" is deliberately missing

      var r = cm.BindColumns(new[] { "b", "c", "d", "e" });

      Assert.False(r);
      var unbound = cm.UnboundColumns().ToList();
      var unboundNames = unbound.Select(c => c.Name).ToList();

      Assert.Equal(2, unbound.Count);
      Assert.Contains("a", unboundNames);
      Assert.Contains("g", unboundNames);

      Assert.Equal(1, c1.Index);
      Assert.Equal(3, c2.Index);
      Assert.Equal(-1, c3.Index);
      Assert.Equal(-1, c4.Index);
      Assert.Equal(0, c5.Index);

      var all = cm.AllColumns(true);
      Assert.Equal(5, all.Count);

      Assert.Equal(new[] { "a", "g", "b", "c", "e" }, all.Select(c => c.Name));

      var r2 = cm.BindColumns(new[] { "a", "b", "c", "e", "g" });
      Assert.True(r2);
    }

  }
}
