using System;

namespace Fannypack
{
   public static class IntExt
   {
      public static Func<int> MakeCounter(this int initialValue)
      {
         int count = initialValue;
         return () => ++count;
      }
   }
}
