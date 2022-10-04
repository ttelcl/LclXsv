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
  /// StringAdapter{DateTime} using epoch units for serialization
  /// </summary>
  internal class DateTimeEpochStringAdapter : StringAdapter<DateTime>
  {
    /// <summary>
    /// Create a new DtoEpochStringAdapter
    /// </summary>
    public DateTimeEpochStringAdapter(long ticksPerUnit = TimeSpan.TicksPerSecond)
    {
      TicksPerUnit = ticksPerUnit;
    }

    public long TicksPerUnit { get; }

    public override DateTime ParseString(string value)
    {
      var units = Int64.Parse(value);
      return EpochHelper.CountToDateTime(units, TicksPerUnit);
    }

    public override string StringValue(DateTime value)
    {
      var units = EpochHelper.DateTimeToCount(value, TicksPerUnit);
      return units.ToString();
    }

  }
}
