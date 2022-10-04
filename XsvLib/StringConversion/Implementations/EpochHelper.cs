/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.StringConversion.Implementations
{
  /// <summary>
  /// Static methods for epoch-based time stamp calculations
  /// (going beyond the standard APIs)
  /// </summary>
  public static class EpochHelper
  {
    /// <summary>
    /// The epoch time stamp (1970-01-01 00:00:00 Z) as DateTime
    /// </summary>
    public static readonly DateTime Epoch =
      new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// The epoch time stamp (1970-01-01 00:00:00 Z) as DateTimeOffset
    /// </summary>
    public static readonly DateTimeOffset EpochDto =
      new DateTimeOffset(Epoch, TimeSpan.Zero);

    /// <summary>
    /// The epoch time stamp expressed in ticks
    /// </summary>
    public static readonly long EpochTicks = Epoch.Ticks;

    /// <summary>
    /// Convert an integer count of time units since the epoch to a DateTime.
    /// The returned value will always be an UTC DateTime.
    /// </summary>
    /// <param name="count">
    /// The number of time units
    /// </param>
    /// <param name="ticksPerUnit">
    /// The size of each time unit expressed in ticks.
    /// Default is TimeSpan.TicksPerSecond (that is: time units are seconds)
    /// </param>
    /// <returns>
    /// The DateTime corresponding to the parameters (in the UTC timezone)
    /// </returns>
    public static DateTime CountToDateTime(long count, long ticksPerUnit = TimeSpan.TicksPerSecond)
    {
      var tickDelta = count*ticksPerUnit;
      return Epoch.AddTicks(tickDelta);
    }

    /// <summary>
    /// Convert a DateTime to an integer count of time units (rounded down if not
    /// exactly representable).
    /// </summary>
    /// <param name="stamp">
    /// The time stamp to convert, which must have a Kind of UTC or Local
    /// (local stamps are converted first to UTC)
    /// </param>
    /// <param name="ticksPerUnit">
    /// The size of each time unit expressed in ticks.
    /// Default is TimeSpan.TicksPerSecond (that is: time units are seconds)
    /// </param>
    /// <returns>
    /// The time unit count corresponding to the parameters
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="stamp"/> has Kind = DateTimeKind.Unspecified.
    /// </exception>
    public static long DateTimeToCount(DateTime stamp, long ticksPerUnit = TimeSpan.TicksPerSecond)
    {
      if(stamp.Kind == DateTimeKind.Unspecified)
      {
        throw new ArgumentException(
          "The argument time stamp must be UTC or local, not unspecified");
      }
      if(stamp.Kind == DateTimeKind.Local)
      {
        stamp = stamp.ToUniversalTime();
      }
      var tickDelta = stamp.Ticks - EpochTicks;
      return tickDelta / ticksPerUnit;
    }

    /// <summary>
    /// Convert an integer count of time units since the epoch to a DateTimeOffset.
    /// </summary>
    /// <param name="count">
    /// The number of time units
    /// </param>
    /// <param name="ticksPerUnit">
    /// The size of each time unit expressed in ticks.
    /// Default is TimeSpan.TicksPerSecond (that is: time units are seconds)
    /// </param>
    /// <returns>
    /// The DateTimeOffset corresponding to the parameters (in the UTC timezone)
    /// </returns>
    public static DateTimeOffset CountToDateTimeOffset(long count, long ticksPerUnit = TimeSpan.TicksPerSecond)
    {
      return EpochDto.AddTicks(count*ticksPerUnit);
    }

    /// <summary>
    /// Convert a DateTimeOffset to an integer count of time units (rounded down if not
    /// exactly representable).
    /// </summary>
    /// <param name="stamp">
    /// The time stamp to convert (interpreted as UTC)
    /// </param>
    /// <param name="ticksPerUnit">
    /// The size of each time unit expressed in ticks.
    /// Default is TimeSpan.TicksPerSecond (that is: time units are seconds)
    /// </param>
    /// <returns>
    /// The time unit count corresponding to the parameters
    /// </returns>
    public static long DateTimeOffsetToCount(DateTimeOffset stamp, long ticksPerUnit = TimeSpan.TicksPerSecond)
    {
      var tickDelta = stamp.UtcTicks - EpochTicks;
      return tickDelta / ticksPerUnit;
    }

  }
}