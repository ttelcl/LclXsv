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

    [Fact]
    public void CanUseStandardXsvReader()
    {
      var datafile = "sample1.csv";
      Assert.True(File.Exists(datafile));

      var columns = new ColumnMap();
      var fooColumn = columns.Declare("foo");
      var barColumn = columns.Declare("bar");
      var bazColumn = columns.Declare("baz");

      var n = 0;
      using(var xr = Xsv.ReadXsv(datafile).AsXsvReader())
      {
        foreach(var cursor in xr.ReadCursor(columns))
        {
          Assert.True(n<3);
          switch(n)
          {
            case 0:
              Assert.Equal("0", cursor[fooColumn]);
              Assert.Equal("zero", cursor[barColumn]);
              Assert.Equal("nothing", cursor[bazColumn]);
              break;
            case 1:
              Assert.Equal("1", cursor[fooColumn]);
              Assert.Equal("one", cursor[barColumn]);
              Assert.Equal("something", cursor[bazColumn]);
              break;
            case 2:
              Assert.Equal("2", cursor[fooColumn]);
              Assert.Equal("two", cursor[barColumn]);
              Assert.Equal("many", cursor[bazColumn]);
              break;
            default:
              throw new InvalidOperationException("Unexpected state");
          }
          n++;
        }
        Assert.Equal(3, n);
      }
    }

    [Fact]
    public void CanUseStandardXsvReader2()
    {
      var datafile = "sample1.csv";
      Assert.True(File.Exists(datafile));

      var columns = new ColumnMap();
      var fooColumn = columns.Declare("foo");
      var barColumn = columns.Declare("bar");
      var bazColumn = columns.Declare("baz");

      var n = 0;
      foreach(var cursor in Xsv.ReadXsvCursor(datafile, columns))
      {
        Assert.True(n<3);
        switch(n)
        {
          case 0:
            Assert.Equal("0", cursor[fooColumn]);
            Assert.Equal("zero", cursor[barColumn]);
            Assert.Equal("nothing", cursor[bazColumn]);
            break;
          case 1:
            Assert.Equal("1", cursor[fooColumn]);
            Assert.Equal("one", cursor[barColumn]);
            Assert.Equal("something", cursor[bazColumn]);
            break;
          case 2:
            Assert.Equal("2", cursor[fooColumn]);
            Assert.Equal("two", cursor[barColumn]);
            Assert.Equal("many", cursor[bazColumn]);
            break;
          default:
            throw new InvalidOperationException("Unexpected state");
        }
        n++;
      }
      Assert.Equal(3, n);
    }

    [Fact]
    public void CanUseStandardXsvCursor()
    {
      var datafile = "sample1.csv";
      Assert.True(File.Exists(datafile));

      var cursor = new XsvCursor(null);
      
      var fooColumn = cursor.ColumnMapping.Declare("foo");
      var barColumn = cursor.ColumnMapping.Declare("bar");
      var bazColumn = cursor.ColumnMapping.Declare("baz");

      var n = 0;
      using(var xr = Xsv.ReadXsv(datafile).AsXsvReader())
      {
        foreach(var cur in xr.ReadCursor(cursor))
        {
          Assert.True(n<3);
          switch(n)
          {
            case 0:
              Assert.Equal("0", cur[fooColumn]);
              Assert.Equal("zero", cur[barColumn]);
              Assert.Equal("nothing", cur[bazColumn]);
              break;
            case 1:
              Assert.Equal("1", cur[fooColumn]);
              Assert.Equal("one", cur[barColumn]);
              Assert.Equal("something", cur[bazColumn]);
              break;
            case 2:
              Assert.Equal("2", cur[fooColumn]);
              Assert.Equal("two", cur[barColumn]);
              Assert.Equal("many", cur[bazColumn]);
              break;
            default:
              throw new InvalidOperationException("Unexpected state");
          }
          n++;
        }
        Assert.Equal(3, n);
      }
    }

    [Fact]
    public void CanUseCustomXsvCursor()
    {
      var datafile = "sample1.csv";
      Assert.True(File.Exists(datafile));

      var cursor = new CustomXsvCursor();

      var n = 0;
      using(var xr = Xsv.ReadXsv(datafile).AsXsvReader())
      {
        foreach(var cur in xr.ReadCursor(cursor))
        {
          Assert.True(n<3);
          switch(n)
          {
            case 0:
              Assert.Equal(0, cur.Foo);
              Assert.Equal("zero", cur.Bar);
              Assert.Equal("nothing", cur.Baz);
              break;
            case 1:
              Assert.Equal(1, cur.Foo);
              Assert.Equal("one", cur.Bar);
              Assert.Equal("something", cur.Baz);
              break;
            case 2:
              Assert.Equal(2, cur.Foo);
              Assert.Equal("two", cur.Bar);
              Assert.Equal("many", cur.Baz);
              break;
            default:
              throw new InvalidOperationException("Unexpected state");
          }
          n++;
        }
        Assert.Equal(3, n);
      }
    }

    [Fact]
    public void CanWriteXsv()
    {
      /*
       * This is an example of "normal" reading and writing of XSV
       * files. The API used for reading (Xsv.ReadXsvCursor()) is tuned
       * for the use case where you know in advance which columns you
       * expect in the input. For more dynamic scenarios use the underlying
       * APIs that Xsv.ReadXsvCursor() is built on.
       */

      var datafile = "sample1.csv";
      Assert.True(File.Exists(datafile));

      var outfile = "sample-out-1.csv";
      if(File.Exists(outfile))
      {
        File.Delete(outfile);
      }
      Assert.False(File.Exists(outfile));

      var incolumns = new ColumnMap();
      var fooInColumn = incolumns.Declare("foo");
      var barInColumn = incolumns.Declare("bar");
      var bazInColumn = incolumns.Declare("baz");

      using(var itrw = Xsv.WriteXsv(outfile))
      {
        var xob = new XsvOutBuffer(new[] { "baz", "foo" });
        var bazOutColumn = xob.GetColumn("baz");
        var fooOutColumn = xob.GetColumn("foo");

        itrw.WriteHeader(xob);

        foreach(var cursor in Xsv.ReadXsvCursor(datafile, incolumns))
        {
          xob[fooOutColumn] = cursor[fooInColumn];
          xob[bazOutColumn] = cursor[bazInColumn];
          itrw.WriteBuffer(xob);
        }

        itrw.FinishFile();
      }

      Assert.True(File.Exists(outfile));

    }

  }
}
