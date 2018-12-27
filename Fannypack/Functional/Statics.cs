using Fannypack.Functional.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fannypack.Functional
{
   public static partial class Statics
   {
      public static Option<T> Some<T>(T value) => new Some<T>(value);
      public static None None => None.Default;

      public static Option<T> Optional<T>(T value) => value == null ? None : Some(value);

      public static Unit Unit => Unit.Default;

      public static Either.Left<L> Left<L>(L l) => new Either.Left<L>(l);
      public static Either.Right<R> Right<R>(R r) => new Either.Right<R>(r);
   }
}
