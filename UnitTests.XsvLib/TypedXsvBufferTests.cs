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
using XsvLib.StringConversion;

namespace UnitTests.XsvLib
{
  public class TypedXsvBufferTests
  {
    private readonly ITestOutputHelper _output;

    public TypedXsvBufferTests(ITestOutputHelper output)
    {
      _output = output;
    }

    [Fact]
    public void StringAdapterTest()
    {
      var lib = new StringAdapterLibrary();

      Assert.Null(lib.Find<string>(""));
      Assert.Null(lib.Find<int>(""));
      Assert.Null(lib.Find<int?>(""));

      lib.RegisterStandard();

      Assert.NotNull(lib.Find<string>(""));
      Assert.NotNull(lib.Find<int>(""));
      Assert.NotNull(lib.Find<int?>(""));

    }

    [Fact]
    public void CanBufferTyped()
    {
      var buffer = new XsvBuffer(false);
      buffer.AdapterLibrary
        .RegisterTimestampsRoundtrip("roundtrip")
        .RegisterTimestampsEpoch("seconds", TimeSpan.TicksPerSecond)
        .RegisterTimestampsEpoch("millis", TimeSpan.TicksPerMillisecond)
        .RegisterBool("nope", "yep", "yepnope")
        .RegisterNullable<bool>("maybe", "yepnopemaybe", "yepnope")
        ;
      var colStr = buffer.Declare<string>("str");
      var colInt = buffer.Declare<int>("int");
      var colLong = buffer.Declare<long>("long");
      var colBool = buffer.Declare<bool>("bool");
      var colInt2 = buffer.Declare<int?>("int2");
      var colLong2 = buffer.Declare<long?>("long2");
      var colBool2 = buffer.Declare<bool?>("bool2");
      var colDtr = buffer.Declare<DateTime>("dt_r", "roundtrip");
      var colDts = buffer.Declare<DateTime>("dt_s", "seconds");
      var colBool3 = buffer.Declare<bool?>("yip", "yepnopemaybe");
      buffer.Lock();

      var time = new DateTime(2022, 10, 4, 17, 2, 00, DateTimeKind.Utc);

      colStr.Value = "string";
      Assert.Equal("string", colStr.Value);
      Assert.Equal("string", colStr.Accessor.Value);

      colInt.Value = 42;
      Assert.Equal(42, colInt.Value);
      Assert.Equal("42", colInt.Accessor.Value);

      colLong.Value = 4242;
      Assert.Equal(4242L, colLong.Value);
      Assert.Equal("4242", colLong.Accessor.Value);

      colBool.Value = true;
      Assert.True(colBool.Value);
      Assert.Equal("true", colBool.Accessor.Value);

      colInt2.Value = 43;
      Assert.Equal(43, colInt2.Value);
      Assert.Equal("43", colInt2.Accessor.Value);
      colInt2.Value = null;
      Assert.Equal((int?)null, colInt2.Value);
      Assert.Equal("", colInt2.Accessor.Value);

      colLong2.Value = 4343;
      Assert.Equal(4343L, colLong2.Value);
      Assert.Equal("4343", colLong2.Accessor.Value);
      colLong2.Value = null;
      Assert.Equal((long?)null, colInt2.Value);
      Assert.Equal("", colInt2.Accessor.Value);

      colBool2.Value = false;
      Assert.False(colBool2.Value);
      Assert.Equal("false", colBool2.Accessor.Value);
      colBool2.Value = null;
      Assert.Null(colBool2.Value);
      Assert.Equal("", colBool2.Accessor.Value);

      colDtr.Value = time;
      Assert.Equal(time, colDtr.Value);
      Assert.Equal("2022-10-04T17:02:00.0000000Z", colDtr.Accessor.Value);

      colDts.Value = time;
      Assert.Equal(time, colDts.Value);
      Assert.Equal("1664902920", colDts.Accessor.Value);

      colBool3.Value = null;
      Assert.Null(colBool3.Value);
      Assert.Equal("maybe", colBool3.Accessor.Value);
      colBool3.Value = true;
      Assert.True(colBool3.Value);
      Assert.Equal("yep", colBool3.Accessor.Value);
      colBool3.Value = false;
      Assert.False(colBool3.Value);
      Assert.Equal("nope", colBool3.Accessor.Value);

      colBool3.Accessor.Value = "yep";
      Assert.True(colBool3.Value);
      colBool3.Accessor.Value = "foo";
      Assert.Throws<ArgumentOutOfRangeException>(() => {
        var _ = colBool3.Value;
      });
    }

  }

}
