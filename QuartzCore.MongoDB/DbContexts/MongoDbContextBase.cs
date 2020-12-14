﻿using MongoDB.Driver;
using QuartzCore.Common.Attributes;
using QuartzCore.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace QuartzCore.MongoDB.DbContexts
{
    public abstract class MongoDbContextBase : IDisposable
    {
        private readonly MongoDbContextOptions _options;
        public MongoDbContextBase([NotNull] MongoDbContextOptions options)
        {
            _options = options;
        }

        private string ConnectionString => _options.ConnectionString;

        public IMongoDatabase Database => GetDbContext();

        public IMongoCollection<TEntity> Collection<TEntity>()
        {
            return Database.GetCollection<TEntity>(GetTableName<TEntity>()); //获取表名
        }

        private string GetTableName<TEntity>()
        {
            Type t = typeof(TEntity);
            var table = t.GetAttribute<MongoDBTableAttribute>();

            if (table == null)
            {
                return t.Name;
            }
            if (string.IsNullOrEmpty(table.TableName))
            {
                throw new Exception("Table name does not exist!");
            }
            return table.TableName;
        }


        private IMongoDatabase GetDbContext()
        {
            var mongoUrl = new MongoUrl(ConnectionString);
            var databaseName = mongoUrl.DatabaseName;
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new Exception($"{mongoUrl}不存DatabaseName名!!!");
            }

            var database = new MongoClient(mongoUrl).GetDatabase(databaseName);
            return database;
        }

        public void Dispose()
        {

        }
    }
}
