using System;
using System.Collections.Generic;
using System.Text;

namespace Fannypack
{
   public static class DateTimeExt
   {
      public static string ToInvertedTicksString(this DateTimeOffset timestamp) =>
         string.Format("{0:D19}", DateTimeOffset.MaxValue.Ticks - timestamp.Ticks);
   }
}
