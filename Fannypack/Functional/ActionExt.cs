using System;
using System.Collections.Generic;
using System.Text;

namespace Fannypack.Functional
{
   public static class ActionExt
   {
      public static Func<Unit> ToFunc(this Action action) => 
         () => 
         {
            action();
            return Statics.Unit;
         };

      public static Func<T, Unit> ToFunc<T>(this Action<T> action) =>
         x => 
         {
            action(x);
            return Statics.Unit;
         };
   }
}
