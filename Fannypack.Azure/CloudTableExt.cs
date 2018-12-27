using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Fannypack.Functional;
using static Fannypack.Functional.Statics;

namespace Fannypack.Azure
{
   public static class CloudTableExt
   {
      public static async Task<TableResult> Insert(this CloudTable table, ITableEntity entity)
      {
         var operation = TableOperation.Insert(entity);
         return await table.ExecuteAsync(operation);
      }

      public static async Task<TableResult> InsertOrReplace(this CloudTable table, ITableEntity entity)
      {
         var operation = TableOperation.InsertOrReplace(entity);
         return await table.ExecuteAsync(operation);
      }

      public static async Task<Option<T>> Retrieve<T>(this CloudTable table, string partitionKey, string rowKey) where T : TableEntity
      {
         var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
         TableResult retrievedResult = await table.ExecuteAsync(operation);
         return (retrievedResult.Result == null)
            ? None
            : Some(retrievedResult.Result as T);
      }

      public static async Task<T> RetrieveOrCreate<T>(this CloudTable table, string partitionKey, string rowKey, Func<T> create) where T : TableEntity
      {
         var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
         TableResult retrievedResult = await table.ExecuteAsync(operation);
         if (retrievedResult.Result == null)
         {
            return await CreateAndSave(table, partitionKey, rowKey, create);
         }
         return retrievedResult.Result as T;
      }

      private static async Task<T> CreateAndSave<T>(CloudTable table, string partitionKey, string rowKey, Func<T> create) where T : TableEntity
      {
         await Insert(table, create());

         var res = await table.Retrieve<T>(partitionKey, rowKey);
         return res.Match(
            Some: x => x,
            None: () => throw new Exception());
      }
   }
}
