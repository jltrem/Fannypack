using System;
using System.Collections.Generic;
using System.Text;
using Fannypack.Functional.Option;

namespace Fannypack.Functional
{
   public struct Option<T>
   {
      private readonly T _value;

      private Option(T value)
      {
         if (value == null) throw new ArgumentNullException();

         _value = value;
         IsSome = true;
      }

      public bool IsSome { get; }
      public bool IsNone => !IsSome;

      public static implicit operator Option<T>(None _) => new Option<T>();
      public static implicit operator Option<T>(Some<T> some) => new Option<T>(some.Value);
      public static implicit operator Option<T>(T value) => value == null ? Statics.None: Statics.Some(value);

      public R Match<R>(Func<T, R> Some, Func<R> None) => IsSome ? Some(_value) : None();

      public Unit Match(Action<T> Some, Action None)
      {
         if (IsSome)
         {
            Some(_value);
         }
         else
         {
            None();
         }
         return Statics.Unit;
      }

      public IEnumerable<T> AsEnumerable()
      {
         if (IsSome) yield return _value;
      }
   }

   namespace Option
   {
      public struct None
      {
         internal static readonly None Default = new None();
      }

      public struct Some<T>
      {
         internal T Value { get; }

         internal Some(T value)
         {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
         }
      }
   }
}
