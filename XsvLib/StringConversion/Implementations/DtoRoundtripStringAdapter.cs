/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.StringConversion.Implementations
{
  /// <summary>
  /// StringAdapter for DateTimeOffset using roundtrip serialization
  /// </summary>
  internal class DtoRoundtripStringAdapter: StringAdapter<DateTimeOffset>
  {
    public override DateTimeOffset ParseString(string value)
    {
      // Recommended implementation per
      // https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-round-trip-date-and-time-values#round-trip-a-datetimeoffset-value
      return DateTimeOffset.Parse(value, null, DateTimeStyles.RoundtripKind);
    }

    public override string StringValue(DateTimeOffset value)
    {
      return value.ToString("o");
    }
  }
}
