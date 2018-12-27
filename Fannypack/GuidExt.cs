using System;
using System.Collections.Generic;
using System.Text;

namespace Fannypack
{
   public static class GuidExt
   {
      public static string ToDashless(this Guid guid) =>
         guid.ToString().Replace("-", "").ToUpper();
   }
}
