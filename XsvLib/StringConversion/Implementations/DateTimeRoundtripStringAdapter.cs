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
  internal class DateTimeRoundtripStringAdapter: StringAdapter<DateTime>
  {
    public DateTimeRoundtripStringAdapter()
    {
    }

    public override DateTime ParseString(string value)
    {
      return DateTime.Parse(value, null, DateTimeStyles.RoundtripKind);
    }

    public override string StringValue(DateTime value)
    {
      return value.ToString("o");
    }
  }
}
