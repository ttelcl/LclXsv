/*
 * (c) 2023  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib.StringConversion.Implementations;

/// <summary>
/// String Adapter converting DateTime instances to and from ISO UTC
/// format with 1 second precision.
/// </summary>
internal class DateTimeIsoSecondsStringAdapter: StringAdapter<DateTime>
{

  public DateTimeIsoSecondsStringAdapter(bool includeSuffix)
  {
    IncludeSuffix=includeSuffix;
  } 

  public bool IncludeSuffix { get; init; }

  public override DateTime ParseString(string value)
  {
    // always allow suffix on parsing
    if(value.EndsWith("Z"))
    {
      value = value[..^1];
    }
    return DateTime.ParseExact(value, "s", CultureInfo.InvariantCulture, 
      DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
  }

  public override string StringValue(DateTime value)
  {
    if(value.Kind != DateTimeKind.Utc)
    { 
      value = value.ToUniversalTime();
    }
    return IncludeSuffix ? value.ToString("s") + "Z" : value.ToString("s");
  }
}
