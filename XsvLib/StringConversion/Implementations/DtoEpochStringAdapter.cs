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
  /// String adapter for DateTimeOffset values represented by
  /// the configured unit since the UNIX epoch.
  /// </summary>
  internal class DtoEpochStringAdapter : StringAdapter<DateTimeOffset>
  {
    /// <summary>
    /// Create a new DtoEpochStringAdapter
    /// </summary>
    public DtoEpochStringAdapter(long ticksPerUnit = TimeSpan.TicksPerSecond)
    {
      TicksPerUnit = ticksPerUnit;
    }

    public long TicksPerUnit { get; }

    public override DateTimeOffset ParseString(string value)
    {
      var units = Int64.Parse(value);
      return EpochHelper.CountToDateTimeOffset(units, TicksPerUnit);
    }

    public override string StringValue(DateTimeOffset value)
    {
      var units = EpochHelper.DateTimeOffsetToCount(value, TicksPerUnit);
      return units.ToString();
    }
  }
}
