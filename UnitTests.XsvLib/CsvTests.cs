using System;
using System.IO;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

using XsvLib;
using XsvLib.Implementation;
using XsvLib.Implementation.Csv;

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
        };

      var records =
        CsvParser.ParseLines(csv1, separator: ',')
        .Select(row => row.ToArray()) // also support a volatile implementation, just in case
        .ToList();

      Assert.Equal(2, records.Count);
      Assert.Equal(3, records[0].Length);
      Assert.Equal(3, records[1].Length);
      Assert.Equal("foo", records[0][0]);
      Assert.Equal("bar", records[0][1]);
      Assert.Equal("baz", records[0][2]); // note: leading space trimmed
      Assert.Equal("1", records[1][0]);
      Assert.Equal("2", records[1][1]); // note: trailing space trimmed
      Assert.Equal("3", records[1][2]);
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
      using(var xsv = new XsvReader(Csv.ParseCsv(csv1)))
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

  }
}
