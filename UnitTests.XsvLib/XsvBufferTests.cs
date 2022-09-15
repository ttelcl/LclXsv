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
      var buffer = new XsvBuffer(false);
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
      var buffer = new XsvBuffer(false);
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
      var buffer = new XsvBuffer(false);
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

    [Fact]
    public void CanWriteWithXsvBuffer()
    {
      const string outname = "sample-output-1.csv";

      var buffer = new XsvBuffer(true);
      var foo = buffer.Declare("foo");
      var bar = buffer.Declare("bar");
      var baz = buffer.Declare("baz");
      buffer.Lock();
      Assert.Equal(3, buffer.Accessors.Count);
      Assert.Equal(0, buffer.FieldsWritten);

      using(var itrw = Xsv.WriteXsv(outname))
      {
        buffer.WriteHeader(itrw);
        Assert.Equal(0, buffer.FieldsWritten);
        Assert.False(foo.IsSet);
        foo.Value = "0";
        Assert.True(foo.IsSet);
        Assert.Equal(1, buffer.FieldsWritten);
        Assert.False(bar.IsSet);
        bar.Set("zero");
        Assert.True(bar.IsSet);
        Assert.Equal(2, buffer.FieldsWritten);
        baz.Set("nothing");
        Assert.Equal(3, buffer.FieldsWritten);
        buffer.WriteRow(itrw);
        Assert.Equal(0, buffer.FieldsWritten);
        foo.Set("1");
        Assert.Equal(1, buffer.FieldsWritten);
        bar.Set("one");
        Assert.Equal(2, buffer.FieldsWritten);
        baz.Set("something");
        Assert.Equal(3, buffer.FieldsWritten);
        buffer.WriteRow(itrw);
        Assert.Equal(0, buffer.FieldsWritten);
        foo.Set("2");
        bar.Set("two");
        baz.Set("many");
        buffer.WriteRow(itrw);
      }

      var lines = File.ReadAllLines(outname);
      Assert.Equal(4, lines.Length);
      Assert.Equal("foo,bar,baz", lines[0]);
      Assert.Equal("0,zero,nothing", lines[1]);
      Assert.Equal("1,one,something", lines[2]);
      Assert.Equal("2,two,many", lines[3]);
    }

  }
}