using ElasticsearchDemo.Elasticserach.ESModels;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using ElasticsearchDemo.ESLib.ESModel;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Diagnostics;
using System.Dynamic;
using Elasticsearch.Net;

namespace ElasticsearchDemo.ESLib.ESHelper
{
	public static class ESHelper
	{
		public static string ConString { get; set; } = "http://localhost:9200";

		private static ElasticClient elasticClient = null;

		/// <summary>
		/// 连接到ES服务，获取ES的连接客户端管理对象
		/// </summary>
		/// <param name="reSetConnStr">是否重置连接字符ConString 默认为http://localhost:9200</param>
		/// <param name="indexName">指定默认的索引名称</param>
		/// <returns>ES的连接客户端管理对象</returns>
		/// <exception cref="Exception">连接失败时候抛出异常</exception>
		public static ElasticClient ESClient(bool reSetConnStr = false)//, string indexName = "index_arc_pic")
		{
			if (!reSetConnStr && elasticClient != null) return elasticClient;
			if (string.IsNullOrEmpty(ConString))
			{
				throw new Exception("连接字符串未初始化，请检查！");
			}
			//响应的正文内容未被捕获或已被序列化完成。这可能是因为在请求中没有设置相应的选项来捕获和处理响应的内容。
			//var settings = new ConnectionSettings()
			//var client = new ElasticClient(settings);
			// 建立连接
			var node = new Uri(ConString); // Elasticsearch 服务器的地址和端口http://localhost:9200
			var settings = new ConnectionSettings(node).DisableDirectStreaming();//.DefaultIndex(indexName);
			var client = new ElasticClient(settings);
			elasticClient = client;
			if (!client.Ping().IsValid)
			{
				throw new Exception("连接异常，请检查网络或者连接配置！");
			}
			return client;
		}

		/// <summary>
		/// 判断当前ES服务是否正常连接
		/// </summary>
		/// <returns>连接状态</returns>
		public static bool HasConnected()
		{
			if (ESClient().Ping().IsValid)
			{
				return true;
			}
			return false;
		}


		//创建索引  //指定字段
		public static void CreateIndex(string indexName)
		{
			// Elasticsearch 连接配置
			/*var uri = new Uri("http://localhost:9200");
			var connectionSettings = new ConnectionSettings(uri);
			var elasticClient = new ElasticClient(connectionSettings);*/

			// 索引名称和映射定义
			var indexExistsResponse = ESClient().Indices.Exists(indexName);
			if (indexExistsResponse.Exists)
			{
				MessageBox.Show("索引已存在！");
				return;
			}

			// 创建索引
			var createIndexResponse = ESClient().Indices.Create(indexName, c => c
				.Map<Arc_PicModel>(m => m
					.Properties(p => p
						.Number(n => n.Name(x => x.iD).Index(false))
						.Number(n => n.Name(x => x.arcID))
						.Number(n => n.Name(x => x.pRI).Index(false))
						.Text(t => t.Name(x => x.fileName).Index(false))
						.Number(n => n.Name(x => x.pageNO).Index(false))
						.Number(n => n.Name(x => x.subPageNO).Index(false))
						.Number(n => n.Name(x => x.fileSize).Index(false))
						.Number(n => n.Name(x => x.width).Index(false))
						.Number(n => n.Name(x => x.height).Index(false))
						.Number(n => n.Name(x => x.dPI).Index(false))
						.Number(n => n.Name(x => x.oCRState).Index(false))
						.Date(d => d.Name(x => x.creatTime))
						.Date(d => d.Name(x => x.updateTime))
						.Text(t => t.Name(x => x.pageType).Index(false))
						.Number(n => n.Name(x => x.a4Nums).Index(false))
						.Text(t => t.Name(x => x.pText).Analyzer("ik_max_word"))
					// 添加更多的字段定义...
					)
				)
			);

			if (createIndexResponse.IsValid)
			{
				MessageBox.Show("索引创建成功");
			}
			else
			{
				MessageBox.Show("索引创建失败：" + createIndexResponse.DebugInformation);
			}
		}

		//创建索引 自动映射类
		public static void CreateAutoIndex(string indexName)
		{
			indexName = indexName.ToLower();//索引名称必须小写
			var createIndexResponse = elasticClient.Indices.Create(indexName, c => c
				.Map(m => m
					.AutoMap()
				)
			);

			if (createIndexResponse.IsValid)
			{
				// 索引创建成功
			}
			else
			{
				// 处理索引创建失败的情况
			}
		}


		/*
				public static void updeateESIndex(string indexName)
				{
					// 数据同步
					var documents = new List<Arc_Model>
					{
						new Arc_Model { },
						new Arc_Model {  },
						// 添加更多的文档...
					};
					//插入索引
					var bulkIndexResponse = ESClient.Bulk(b => b
						.Index(indexName)
						.IndexMany(documents)
					);

					if (bulkIndexResponse.IsValid)
					{
						Console.WriteLine("数据同步成功");
					}
					else
					{
						Console.WriteLine("数据同步失败：" + bulkIndexResponse.DebugInformation);
					}
				}
		*/


		// 获取索引名称列表
		public static List<string> GetIndexNames()
		{
			return ESClient(false).Cat.Indices().Records.Select(i => i.Index).ToList();
		}

		//获取指定索引的所有字段
		public static PropertyName[] GetAllFieldsFromIndex(string indexName)
		{
			var fieldList = new List<string>();
			// 获取索引的映射信息
			var response = ESClient().Indices.GetMapping<object>(m => m.Index(indexName));//获取索引
			var mappings = response.Indices[indexName].Mappings;//获取映射
			if (mappings.Properties == null)
			{
				return null;
			}
			return mappings.Properties.Keys.ToArray();//获取字段
		}

		//删除索引
		public static void DeleteIndex(string indexName)
		{
			var indexExistsResponse = ESClient().Indices.Exists(indexName);
			if (!indexExistsResponse.Exists)
			{
				MessageBox.Show("索引不存在，无法删除！");
				return;
			}

			var deleteIndexResponse = ESClient().Indices.Delete(indexName);
			if (deleteIndexResponse.IsValid)
			{
				MessageBox.Show("删除成功！");
				// 索引删除成功
			}
			else
			{
				// 处理索引删除失败的情况
			}
		}




		//自定义分词器
		/*public void fenci()
		{
			// Elasticsearch 连接配置
			var uri = new Uri("http://localhost:9200");
			var connectionSettings = new ConnectionSettings(uri);
			var elasticClient = new ElasticClient(connectionSettings);

			// 定义自定义分词器
			var customTokenizer = new PatternTokenizer
			{
				Name = "my_tokenizer",
				Pattern = @"[\s]+"
			};

			// 创建索引模板
			var createIndexTemplateResponse = elasticClient.Indices.PutTemplate("my_index_template", c => c
				.IndexPatterns("my_index")
				.Settings(s => s
					.Analysis(a => a
						.Tokenizers(t => t
							.Add("my_tokenizer", customTokenizer)
						)
						.Analyzers(analyzer => analyzer
							.Custom("my_analyzer", custom => custom
								.Tokenizer("my_tokenizer")
							)
						)
					)
				)
			);

			if (createIndexTemplateResponse.IsValid)
			{
				Console.WriteLine("索引模板创建成功");
			}
			else
			{
				Console.WriteLine("索引模板创建失败：" + createIndexTemplateResponse.DebugInformation);
				return;
			}
		}
*/



		//匹配索引数据
		public static DataTable MatchAll(string indexName,string searchTerm)//此处指定了类型，无法检索Arc_PicModel以为的索引
		{




			var searchResponse = ESClient().Search<object>(s => s
				.Index(indexName)
				.Size(100)//可以在此指定返回数量 es默认不能超过10000
				.Query(q => q.MatchAll())
				);
			/*var searchResponse = elasticClient.Search<Arc_PicModel>(s => s
			.Index(indexName)
			.Size(100)//返回10条数据
			.Query(q => q
				.Match(mm => mm
					.Query(searchTerm)
						.Field(f1 => f1.pText) // 设置要搜索的字段
						//.Field(f2=>f2.arcID)//可以加上多个字段
						.Analyzer("ik_max_word")//设置分词器
					  )
				  )
			);*/
			return ConvertToDataTable(searchResponse.Documents);
		}


		//匹配指定数据
		public static DataTable MatchAll(string indexName,string fieldName,string searchText)
		{

			var searchResponse = ESClient().Search<object>(s => s
				.Index(indexName)
				.Query(q => q
				.Match(m => m
				.Field(fieldName)
				.Query(searchText)
						)
					)
				);


			if (searchResponse.IsValid)
			{
				/*var hits = searchResponse.Hits;

				foreach (var hit in hits)
				{
					var document = hit.Source;
					// 处理搜索结果
				}*/
			}
			else
			{
				// 处理搜索失败的情况
			}
			return ConvertToDataTable(searchResponse.Documents);
		}


		//没有实体类检索es
		public static IEnumerable<dynamic> SearchElasticsearch(string indexName, string fieldName, string searchText)
		{
			var searchResponse = ESClient().Search<DynamicResponse>(s => s
				.Index(indexName)
				.Query(q => q
					.Match(m => m
						.Field(fieldName)
						.Query(searchText)
					)
				)
			);

			if (searchResponse.IsValid)
			{
				foreach (var hit in searchResponse.Hits)
				{
					dynamic dynamicResponse = hit.Source;
					if (dynamicResponse.LoginID != null)
					{
						var loginId = dynamicResponse.LoginID;
						// 处理 loginId
					}
					else
					{
						// 处理属性不存在的情况
					}
					yield return hit.Source;//返回处理 再回来迭代 不消耗过多内存
				}
			}
			else
			{
				Console.WriteLine("搜索请求失败");
			}
		}


		//将泛型集合（IEnumerable<T>）转换为 DataTable 对象
		public static DataTable ConvertToDataTable<T>(IEnumerable<T> data)
		{
			DataTable table = new DataTable();
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

			foreach (PropertyDescriptor prop in props)
			{
				table.Columns.Add(prop.Name, prop.PropertyType);
			}

			foreach (T item in data)
			{
				DataRow row = table.NewRow();
				foreach (PropertyDescriptor prop in props)
				{
					row[prop.Name] = prop.GetValue(item);
				}
				table.Rows.Add(row);
			}

			return table;
		}


		//将es返回的searchResponse.Documents转换成datable
		public static DataTable ConvertDocToDataTable<T>(IEnumerable<T> data)
		{
			DataTable table = new DataTable();
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

			foreach (PropertyDescriptor prop in props)
			{
				table.Columns.Add(prop.Name, prop.PropertyType);
			}

			foreach (T item in data)
			{
				DataRow row = table.NewRow();
				foreach (PropertyDescriptor prop in props)
				{
					row[prop.Name] = prop.GetValue(item);
				}
				table.Rows.Add(row);
			}

			return table;
		}





		//将数据库的数据插入到es中
		public static void ImportData()
		{
			DataTable dt = SqlHelper.ExecuteTable("SELECT * FROM Suite_Arc_Pic");
			int amount = dt.Rows.Count;
			int count = 0;

			List<Arc_PicModel> documents = new List<Arc_PicModel>();

			// 创建 Stopwatch 实例
			//Stopwatch stopwatch = new Stopwatch();
			foreach (DataRow dr in dt.Rows)
			{
				var md = dr.DataRowToModel<Arc_PicModel>();
				documents.Add(md);

				if (documents.Count % 5000 == 0)//1000条插入一次
				{
					//遍历10000条数据0.2497570秒
					//stopwatch.Start();

					// 批量插入文档
					var bulkResponse = ESClient().Bulk(b => b     //10000条插入到索引耗时2.6654912秒
						.Index("index_arc_pic")
						.IndexMany(documents)
					);
					//stopwatch.Stop();
					//TimeSpan elapsedTime = stopwatch.Elapsed;


					string info = bulkResponse.ApiCall.DebugInformation;
					if (!bulkResponse.IsValid)
					{
						count += documents.Count;
						UpdateProgress(count);
					}
					else
					{
						MessageBox.Show("批量插入失败！");
					}

					// 清空文档列表
					documents.Clear();
				}
			}

			// 处理剩余的文档
			if (documents.Count > 0)
			{
				var bulkResponse = ESClient().Bulk(b => b
					.Index("index_arc_pic")
					.IndexMany(documents)
				);

				if (!bulkResponse.IsValid)
				{
					count += documents.Count;
					UpdateProgress(count);
				}
				else
				{
					MessageBox.Show("批量插入失败！");
				}
			}

			MessageBox.Show("插入完成！");
		}







		//查询示例
		public static void SearchDocument()
		{
			// 创建 Elasticsearch 客户端连接
			var connectionSettings = new ConnectionSettings(new Uri("http://localhost:9200"))
				.DefaultIndex("your_index_name"); // 设置默认索引名称

			var elasticClient = new ElasticClient(connectionSettings);

			// 构建查询请求
			var searchRequest = new SearchRequest<Arc_PicModel>
			{
				Query = new MatchQuery
				{
					Field = Infer.Field<Arc_PicModel>(d => d.pText), // 设置查询字段
					Query = "search_keyword" // 设置搜索关键字
				}
			};

			// 执行查询请求
			var searchResponse = elasticClient.Search<Arc_PicModel>(searchRequest);

			// 处理查询结果
			if (searchResponse.IsValid)
			{
				// 获取命中的文档
				var documents = searchResponse.Documents;

				// 处理文档结果
				foreach (var document in documents)
				{
					// 处理每个文档
					// ...
				}
			}
			else
			{
				// 查询出现错误，处理错误信息
				var errorMessage = searchResponse.DebugInformation;
				// ...
			}
		}


		//获取文档数量
		public static long GetIndexDocumentCount(string indexName)
		{
			var countResponse = ESClient().Count<object>(c => c.Index(indexName));
			if (countResponse.IsValid)
			{
				return countResponse.Count;
			}
			else
			{
				// 处理错误情况
				throw new Exception("Failed to get document count: " + countResponse.DebugInformation);
			}
		}




		#region 进度通知
		public static event EventHandler<int> ProgressUpdated;


		public static void UpdateProgress(int progress)
		{
			// 触发进度更新事件
			ProgressUpdated?.Invoke(null, progress);
		}


		#endregion



		//不定义实体类的前提下将数据库的数据插入到es索引中
		public static void ImportToESFromDataTable(DataTable dt, string indexName)
		{
			var client = ESClient();
			indexName = indexName.ToLower();
			if (!client.Indices.Exists(indexName).Exists)
			{
				CreateAutoIndex(indexName);
			}

			var bulkDescriptor = new BulkDescriptor();

			foreach (DataRow row in dt.Rows)
			{
				var document = new Dictionary<string, object>();

				foreach (DataColumn column in dt.Columns)
				{
					document[column.ColumnName] = row[column.ColumnName];
				}

				bulkDescriptor.Index<object>(i => i
					.Index(indexName)
					.Document(document)
				);
			}

			var bulkResponse = client.Bulk(bulkDescriptor);

			if (bulkResponse.IsValid)
			{
				// 批量插入成功
			}
			else
			{
				// 处理批量插入失败的情况
			}
		}

		public static void SearchDocuments(string indexName, string searchTerm)
		{
			
			var client = ESClient();

			var searchResponse = client.Search<object>(s => s
				.Index(indexName.ToLower())
				.Query(q => q
					.Match(m => m
						.Field("_all")
						.Query(searchTerm)
					)
				)
			);

			if (searchResponse.IsValid)
			{
				var hits = searchResponse.Hits;

				foreach (var hit in hits)
				{
					var document = hit.Source;
					// 处理查询结果的逻辑
				}
			}
			else
			{
				// 处理查询失败的情况
			}
		}
	}
}
