/*
 * (c) 2022  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XsvLib
{
  /// <summary>
  /// Abstraction for an object that can be enumerated to produce
  /// a series of lines
  /// </summary>
  public interface ILinesReader
  {

    /// <summary>
    /// Enumerate the lines represented by this object
    /// </summary>
    IEnumerable<string> ReadLines();

  }

}
