using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fannypack.Functional;
using static Fannypack.Functional.Statics;

namespace Fannypack.Azure
{
   public class PartitionQuery
   {
      private readonly string _partitionKey;

      public PartitionQuery(string partitionKey)
      {
         _partitionKey = partitionKey ?? throw new ArgumentNullException("partitionKey");
      }

      public async Task<List<T>> FindWhereRowKeyStartsWith<T>(CloudTable table, string rowKeyStartsWith) where T : TableEntity, new()
      {
         if (table == null) throw new ArgumentNullException("table");
         if (string.IsNullOrEmpty(rowKeyStartsWith)) throw new ArgumentNullException("rowKeyStartsWith");

         var query = new TableQuery<T>();

         var lastCharNdx = rowKeyStartsWith.Length - 1;
         var lastChar = rowKeyStartsWith[lastCharNdx];
         var nextLastChar = (char)(lastChar + 1);

         var startsWithEndPattern = rowKeyStartsWith.Substring(0, lastCharNdx) + nextLastChar;

         var prefixCondition = TableQuery.CombineFilters(
             TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, rowKeyStartsWith),
             TableOperators.And,
             TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, startsWithEndPattern));

         var filterString = TableQuery.CombineFilters(
            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionKey),
            TableOperators.And,
            prefixCondition);

         return await ExecuteAsync(table, query.Where(filterString));
      }

      public async Task<List<T>> GetRows<T>(CloudTable table) where T : TableEntity, new()
      {
         if (table == null) throw new ArgumentNullException("table");

         var query = new TableQuery<T>();
         var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionKey);

         return await ExecuteAsync(table, query.Where(filter));
      }

      public async Task<Option<T>> GetTopRow<T>(CloudTable table) where T : TableEntity, new()
      {
         if (table == null) throw new ArgumentNullException("table");

         var query = new TableQuery<T>().Take(1);
         var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionKey);

         var list = await ExecuteAsync(table, query.Where(filter));
         return list.FirstOrDefault();
      }

      public async static Task<List<T>> ExecuteAsync<T>(CloudTable table, TableQuery<T> query) where T : TableEntity, new()
      {
         List<T> results = new List<T>();

         TableContinuationToken continuationToken = null;
         while (true)
         {
            var segment = await table.ExecuteQuerySegmentedAsync(query, continuationToken);

            results.AddRange(segment.Results);

            continuationToken = segment.ContinuationToken;
            if (continuationToken == null)
            {
               break;
            }
         }

         return results;
      }
   }
}
