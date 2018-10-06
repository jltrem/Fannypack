using System;
using Xunit;
using Fannypack;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Fannypack.Tests
{
   public class IntExtTests
   {

      [Fact]
      public void can_create_counter_that_returns_incremental_values()
      {
         var counter = 0.MakeCounter();
         Assert.NotNull(counter);

         for (int i = 1; i <= 10; i++)
         {
            Assert.Equal(i, counter());
         }
      }

      [Fact]
      public void can_multiple_counters_that_maintain_their_own_count()
      {
         object locker = new object();
         int testCount = 0;
         void IncrementTestCount()
         {
            lock (locker)
            {
               testCount++;
            }
         }

         List<Task> tasks = new List<Task>();
         (int InitialValue, Func<int> Counter) New(int initVal) => (initVal, initVal.MakeCounter());

         for (int taskNum = 1; taskNum <= 100; taskNum++)
         {
            tasks.Add(Task.Run(() => 
            {
               var c = New(taskNum);
               for (int countNum = 1; countNum <= 10; countNum++)
               {
                  Assert.Equal(c.InitialValue + countNum, c.Counter());
                  IncrementTestCount();
               }
            }));
         }

         Task.WaitAll(tasks.ToArray());
         Assert.Equal(100 * 10, testCount);
      }

   }
}
