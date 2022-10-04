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
  /// Implements StringAdapter{int}
  /// </summary>
  internal class Int32StringAdapter: StringAdapter<int>
  {
    public override int ParseString(string value)
    {
      return Int32.Parse(value);
    }

    public override string StringValue(int value)
    {
      return value.ToString();
    }
  }
}