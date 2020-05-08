using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//this class is for if you are connecting to the research preview
namespace FarmBeatsAccelerator.DBcollection
{
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Cosmos.Table;
    public class DBCollection<T> where T : ITableEntity, new()
    { 
        public CloudTable GetTable(string tableName,CloudStorageAccount CSA)
        {
            CloudTableClient tableClient = CSA.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);
            return table;
        }
        public async Task<CloudTable> CreateTable(string tableName, CloudStorageAccount CSA)
        {
            CloudTableClient tableClient = CSA.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}", tableName);
            }
            else
            {
                Console.WriteLine("Table {0} already exists", tableName);
            }
            return table;
        }
        public List<T> GetAll(CloudTable table)
        {
            TableQuery<T> query = new TableQuery<T>();
            return table.ExecuteQuerySegmentedAsync(query, null).GetAwaiter().GetResult().ToList();
        }

        public List<T> GetByKey(string key, CloudTable table)
        {
            TableQuery<T> query = new TableQuery<T>();
            query.Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, key));

            return table.ExecuteQuerySegmentedAsync(query, null).GetAwaiter().GetResult().ToList();
        }
        public List<T> GetByPartition(string key, CloudTable table)
        {
            TableQuery<T> query = new TableQuery<T>();
            query.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key));

            return table.ExecuteQuerySegmentedAsync(query, null).GetAwaiter().GetResult().ToList();
        }
        public List<T> GetBySkuId(string key, CloudTable table)
        {
            TableQuery<T> query = new TableQuery<T>();
            query.Where(TableQuery.GenerateFilterCondition("SkuId", QueryComparisons.Equal, key));

            return table.ExecuteQuerySegmentedAsync(query, null).GetAwaiter().GetResult().ToList();
        }
        public T Get(string key, CloudTable table)
        {
            return GetByKey(key, table).FirstOrDefault();
        }
        public void Insert(T entity, CloudTable table)
        {
            TableOperation insertOperation = TableOperation.Insert(entity);
            table.ExecuteAsync(insertOperation).GetAwaiter().GetResult();
        }

        public void Update(T updatedEntity, CloudTable table)
        {
            TableOperation updateOperation = TableOperation.Replace(updatedEntity);
            table.ExecuteAsync(updateOperation).GetAwaiter().GetResult();
        }

    }
}

