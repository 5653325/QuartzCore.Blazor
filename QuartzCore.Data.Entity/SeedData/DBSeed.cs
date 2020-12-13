﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzCore.Data.Entity.SeedData
{
    public class DBSeed
    {
        private const string Sqlite_ExistTableSql = "SELECT count(1) FROM sqlite_master WHERE type='table' AND name = 'app'";
        private const string Mysql_ExistTableSql = " SELECT count(1) FROM information_schema.TABLES WHERE table_name ='app'";
        private const string SqlServer_ExistTableSql = "SELECT COUNT(1) FROM dbo.SYSOBJECTS WHERE ID = object_id(N'[dbo].[app]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
        private const string Oracle_ExistTableSql = "select count(1) from user_tables where table_name = 'app'";
        private const string PostgreSql_ExistTableSql = "select count(1) from pg_class where relname = 'app'";

        public static bool ExistTable(IFreeSql instance)
        {
            //sqlite exist table?
            string sql = "";
            switch (instance.Ado.DataType)
            {
                case FreeSql.DataType.Sqlite:
                    sql = Sqlite_ExistTableSql;
                    break;
                case FreeSql.DataType.MySql:
                    sql = Mysql_ExistTableSql;
                    break;
                case FreeSql.DataType.SqlServer:
                    sql = SqlServer_ExistTableSql;
                    break;
                case FreeSql.DataType.Oracle:
                    sql = Oracle_ExistTableSql;
                    break;
                case FreeSql.DataType.PostgreSQL:
                    sql = PostgreSql_ExistTableSql;
                    break;
                default:
                    sql = Sqlite_ExistTableSql;
                    break;
            }

            dynamic count = instance.Ado.ExecuteScalar(sql);

            return count > 0;
        }

        /// <summary>
        /// 先判断是否建过表了，如果没有则新建表
        /// </summary>
        /// <param name="instance"></param>
        public static void Ensure(IFreeSql instance)
        {
            if (!ExistTable(instance))
            {
                if (instance.Ado.DataType == FreeSql.DataType.Oracle)
                {
                    instance.CodeFirst.IsSyncStructureToUpper = true;
                }
                try
                {
                    instance.CodeFirst.SyncStructure<AppEntity>();
                    instance.CodeFirst.SyncStructure<TasksQzEntity>();
                    instance.CodeFirst.SyncStructure<QzRunLogEntity>();
                    
                }
                catch (Exception ex)
                {
                    //var logger = Global.LoggerFactory.CreateLogger<EnsureTables>();
                    //logger.LogError(ex, "Ensure Tables failed .");
                }
            }
        }
    }
}
