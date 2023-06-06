using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace ElasticsearchDemo
{
	internal class SqlHelper
	{
		public static string ConnStr { get; set; } //连接数据库字符串

		/// <summary>
		/// 数据库连接测试
		/// </summary>
		public static bool IsConnection()
		{
			using (SqlConnection conn = new SqlConnection(ConnStr))
			{
				try
				{
					//打开数据库成功
					conn.Open();
					return true;
				}
				catch
				{
					return false;
				}
			}
		}

		/// <summary>
		/// 获取所有数据库
		/// </summary>
		/// <returns>所有数据库名称的字符集合</returns>
		public static List<string> GetDataBaseNames()
		{
			var names = new List<string>();
			using (SqlConnection connection = new SqlConnection(ConnStr))
			{
				connection.Open();
				DataTable databases = connection.GetSchema("Databases");
				foreach (DataRow database in databases.Rows)
				{
					names.Add(database["database_name"].ToString());
				}
			}
			return names;
		}

		//获取所有模式名称
		public static List<string> GetAllSchemas(string databaseName)
		{
			List<string> schemaNames = new List<string>();

			using (var connection = new SqlConnection(ConnStr))
			{
				connection.Open();

				string query = $"SELECT name FROM {databaseName}.sys.schemas";

				using (var command = new SqlCommand(query, connection))
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						string schemaName = reader.GetString(0);
						schemaNames.Add(schemaName);
					}
				}
			}

			return schemaNames;
		}


		//获取指定数据库指定模式下的所有表名
		public static List<string> GetTablesInSchema(string databaseName, string schemaName)
		{
			List<string> tableNames = new List<string>();

			using (var connection = new SqlConnection(ConnStr))
			{
				connection.Open();

				DataTable tableSchema = connection.GetSchema("Tables", new[] { databaseName, schemaName });

				foreach (DataRow row in tableSchema.Rows)
				{
					string tableName = row["TABLE_NAME"].ToString();
					tableNames.Add(tableName);
				}
			}
			return tableNames;
		}

		//获取所有字段名
		public static List<string> GetTableColumns(string databaseName, string schemaName, string tableName)
		{
			List<string> columnNames = new List<string>();

			using (var connection = new SqlConnection(ConnStr))
			{
				connection.Open();

				string query = $@"
                SELECT COLUMN_NAME
                FROM {databaseName}.INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = '{schemaName}' AND TABLE_NAME = '{tableName}'";

				using (var command = new SqlCommand(query, connection))
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						string columnName = reader.GetString(0);
						columnNames.Add(columnName);
					}
				}
			}

			return columnNames;
		}

		//模糊查询
		public static DataTable FuzzySearch(string databaseName, string schemaName, string tableName, string columnName, string searchKeyword)
		{
			// 构建 SQL 查询语句
			string sql = $"SELECT * FROM [{databaseName}].[{schemaName}].[{tableName}] WHERE [{columnName}] LIKE @SearchKeyword";

			// 创建数据库连接
			using (SqlConnection connection = new SqlConnection(ConnStr))
			{
				// 创建 SqlCommand 对象
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					// 添加查询参数
					command.Parameters.AddWithValue("@SearchKeyword", $"%{searchKeyword}%");

					// 打开数据库连接
					connection.Open();

					// 执行查询并获取结果
					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataTable resultTable = new DataTable();
						adapter.Fill(resultTable);
						return resultTable;
					}
				}
			}
		}





		/// <summary>
		/// 操作数据库执行SQL语句
		/// </summary>
		/// <param name="cmdText">要执行的SQL语句</param>
		/// <param name="sqlParameters">可选参数</param>
		/// <returns></returns>
		public static DataTable ExecuteTable(string cmdText, params SqlParameter[] sqlParameters)
		{
			using (SqlConnection conn = new SqlConnection(ConnStr))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(cmdText, conn);
				cmd.Parameters.AddRange(sqlParameters);
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				sqlDataAdapter.Fill(ds);
				return ds.Tables[0];
			}
		}

		/// <summary>
		/// 获取服务器时间
		/// </summary>
		public static DateTime GetSerTime()
		{
			using (SqlConnection conn = new SqlConnection(ConnStr))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand("SELECT GETDATE()", conn);
				return (DateTime)cmd.ExecuteScalar();
			}
		}

	}
}
