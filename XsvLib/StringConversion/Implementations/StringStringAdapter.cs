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
  /// A dummy string adapter implementing StringAdapter{string}
  /// </summary>
  internal class StringStringAdapter: StringAdapter<string>
  {
    public override string ParseString(string value)
    {
      return value;
    }

    public override string StringValue(string value)
    {
      return value;
    }
  }
}
