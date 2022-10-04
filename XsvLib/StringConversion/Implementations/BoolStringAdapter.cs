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
  internal class BoolStringAdapter: StringAdapter<bool>
  {
    public BoolStringAdapter(
      string falseString = "false",
      string trueString = "true")
    {
      FalseString = falseString;
      TrueString = trueString; 
    }

    public string FalseString { get; }

    public string TrueString { get; }

    public override bool ParseString(string value)
    {
      if(StringComparer.InvariantCultureIgnoreCase.Equals(value, FalseString))
      {
        return false;
      }
      else if(StringComparer.InvariantCultureIgnoreCase.Equals(value, TrueString))
      {
        return true;
      }
      else
      {
        throw new ArgumentOutOfRangeException(
          $"Expecting '{FalseString}' or '{TrueString}' but received '{(value ?? "")}'");
      }
    }

    public override string StringValue(bool value)
    {
      return value ? TrueString : FalseString;
    }
  }
}