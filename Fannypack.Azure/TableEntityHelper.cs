using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fannypack.Azure
{
   public static class TableEntityHelper
   {
      public static async Task<List<T>> Query<T>(CloudTable table) where T : TableEntity, new()
      {
         var results = new List<T>();

         TableContinuationToken continuationToken = null;
         while (true)
         {
            var segment = await table.ExecuteQuerySegmentedAsync(new TableQuery<T>(), continuationToken);
            results.AddRange(segment.Results);

            continuationToken = segment.ContinuationToken;
            if (continuationToken == null)
            {
               break;
            }
         }

         return results;
      }

      public static async Task<List<T>> QueryPartition<T>(CloudTable table, string partitionKey) where T : TableEntity, new()
      {
         var partition = new PartitionQuery(partitionKey);
         return await partition.GetRows<T>(table);
      }
   }
}
