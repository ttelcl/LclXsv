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
  internal class Int64StringAdapter: StringAdapter<long>
  {
    public override long ParseString(string value)
    {
      return Int64.Parse(value);
    }

    public override string StringValue(long value)
    {
      return value.ToString();
    }

  }
}