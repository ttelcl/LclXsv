/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.IO;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

using XsvLib;
using XsvLib.Buffers;

namespace UnitTests.XsvLib
{
  public class XsvBufferTests
  {
    private readonly ITestOutputHelper _output;

    public XsvBufferTests(ITestOutputHelper output)
    {
      _output = output;
    }

    const string sampleFileName = "sample1.csv";

    [Fact]
    public void CanReadXsvWithXsvBufferBasic()
    {
      var buffer = new XsvBuffer();
      var foo = buffer.Declare("foo");
      var bar = buffer.Declare("bar");
      var baz = buffer.Declare("baz");
      
      using(var reader = Xsv.ReadXsv(sampleFileName))
      {
        Assert.False(buffer.IsLocked);
        var row = 0;
        foreach(var b in reader.ReadXsvBuffer(buffer))
        {
          Assert.True(buffer.IsLocked);
          if(row == 0)
          {
            Assert.Equal("0", foo.Value);
            Assert.Equal("zero", bar.Value);
            Assert.Equal("nothing", baz.Value);
          }
          if(row == 1)
          {
            Assert.Equal("1", foo.Value);
            Assert.Equal("one", bar.Value);
            Assert.Equal("something", baz.Value);
          }
          if(row == 2)
          {
            Assert.Equal("2", foo.Value);
            Assert.Equal("two", bar.Value);
            Assert.Equal("many", baz.Value);
          }
          row++;
        }
        Assert.Equal(3, row);
      }
    }

    [Fact]
    public void CanReadXsvWithXsvBufferReordered()
    {
      var buffer = new XsvBuffer();
      var bar = buffer.Declare("bar");
      var baz = buffer.Declare("baz");
      var foo = buffer.Declare("foo");

      using(var reader = Xsv.ReadXsv(sampleFileName))
      {
        Assert.False(buffer.IsLocked);
        var row = 0;
        foreach(var b in reader.ReadXsvBuffer(buffer))
        {
          Assert.True(buffer.IsLocked);
          if(row == 0)
          {
            Assert.Equal("0", foo.Value);
            Assert.Equal("zero", bar.Value);
            Assert.Equal("nothing", baz.Value);
          }
          if(row == 1)
          {
            Assert.Equal("1", foo.Value);
            Assert.Equal("one", bar.Value);
            Assert.Equal("something", baz.Value);
          }
          if(row == 2)
          {
            Assert.Equal("2", foo.Value);
            Assert.Equal("two", bar.Value);
            Assert.Equal("many", baz.Value);
          }
          row++;
        }
        Assert.Equal(3, row);
      }
    }

    [Fact]
    public void CanReadXsvWithXsvBufferOnlySomeColumns()
    {
      var buffer = new XsvBuffer();
      var foo = buffer.Declare("foo");
      var baz = buffer.Declare("baz");

      using(var reader = Xsv.ReadXsv(sampleFileName))
      {
        Assert.False(buffer.IsLocked);
        var row = 0;
        foreach(var b in reader.ReadXsvBuffer(buffer))
        {
          Assert.True(buffer.IsLocked);
          if(row == 0)
          {
            Assert.Equal("0", foo.Value);
            Assert.Equal("nothing", baz.Value);
          }
          if(row == 1)
          {
            Assert.Equal("1", foo.Value);
            Assert.Equal("something", baz.Value);
          }
          if(row == 2)
          {
            Assert.Equal("2", foo.Value);
            Assert.Equal("many", baz.Value);
          }
          row++;
        }
        Assert.Equal(3, row);
      }
    }

  }
}