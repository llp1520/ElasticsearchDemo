using ElasticsearchDemo.Elasticserach.ESModels;
using ElasticsearchDemo.ESLib.ESHelper;
using ElasticsearchDemo.ESLib.ESModel;
using ElasticsearchDemo.Properties;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ElasticsearchDemo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			Control.CheckForIllegalCrossThreadCalls = false;//启用线程间的调用
			txt_DBPwd.TextBox.PasswordChar = '*';// 将ToolStripTextBox控件设置为密码输入框
			txt_ESPwd.TextBox.PasswordChar = '*';// 将ToolStripTextBox控件设置为密码输入框
			cob_ESIp.Text = "http://localhost";
			cob_SqlType.Text = "SQL Server";
			cob_DataCount.Text = "---查询所有---";
			cob_ESDataCount.Text = "---查询所有---";
			cob_SearchMode.Text = "全文查询(Full-Text Query)";
			cob_DBSearchMode.Text = "AND";
		}
		ElasticClient elasticClient = null;
		bool isConnected = false;
		private void button1_Click(object sender, EventArgs e)
		{
			//ESHelper.CreateIndex();
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
		   .DefaultIndex("index_arc_pic"); // 设置默认索引名称

			elasticClient = new ElasticClient(settings);

			if (elasticClient.Ping().IsValid)
			{
				tsl_ESConnStatus.Text = "ES服务已连接";
				tsl_ESConnStatus.ForeColor = Color.White;
				tsl_ESConnStatus.BackColor = Color.Green;
				var response = elasticClient.Indices.GetMapping<object>(m => m.Index("index_arc_pic"));//获取索引
				var mappings = response.Indices["index_arc_pic"].Mappings;//获取映射
				var fieldMappings = mappings.Properties.Keys.ToArray();//获取字段

				cb_IndexFields.DataSource = fieldMappings;
				//indexNames.Items.AddRange(ESHelper.GetIndexNames().ToArray());
				isConnected = true;
			}
			else
			{
				tsl_ESConnStatus.Text = "ES服务连接失败";
				tsl_ESConnStatus.ForeColor = Color.Red;
				tsl_ESConnStatus.BackColor = Color.Gold;

				isConnected = false;
			}

			/*ElasticClient client = null;
			try
			{
				client = ESHelper.ESClient;
				label3.Text = "连接成功";
				label3.ForeColor = Color.White;	
				label3.BackColor = Color.Green;
				//MessageBox.Show("连接成功！");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				label3.Text = "连接失败";
				label3.ForeColor = Color.Red;
				throw;
			}
			var indexName = "index_suite_arc"; // 索引名称

			var response = client.Indices.GetMapping<object>(m => m.Index(indexName));//获取索引
			var mappings = response.Indices[indexName].Mappings;//获取映射
			var fieldMappings = mappings.Properties.Keys.ToList();//获取字段
	//===========================================================================================================		
	//===========================================================================================================		
	//===========================================================================================================		
			comboBox1.DataSource=fieldMappings;
			comboBox3.DataSource = ESHelper.GetIndexName();


			dataGridView1.DataSource= ESHelper.MatchAll();
			// 设置自动调整模式为所有列
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

			// 调整列宽度
			dataGridView1.AutoResizeColumns();
*/
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (elasticClient == null) { return; }
			string searchTerm = ""; // 设置要搜索的关键词
			if (!isConnected)
			{
				return;
			}
			List<string> searchFields = GetSearchFields<Arc_Model>();//将对象的所有字段转换成搜索字段 

			var searchResponse = elasticClient.Search<Arc_PicModel>(s => s
			.Size(100)//返回100条数据
			.Query(q => q
				.Match(mm => mm
					.Query(searchTerm)
						.Field(f1 => f1.pText) // 设置要搜索的字段
						.Analyzer("ik_max_word")//设置分词器
					  )
				  )
			);



			/*var searchResponse = elasticClient.Search<Arc_Model>(s => s
			.Query(q => q
				.MultiMatch(mm => mm
					.Query(searchTerm)
					.Fields(f => f
						.Field(f1=>f1.OldBusinessNO) // 设置要搜索的字段
						.Field(f1=>f1.AssociatedCode) 
						.Field(f1=>f1.Owner) 
						.Field(f1=>f1.RightNO) 
						.Field(f1=>f1.HouseLocated) 
						.Field(f1=>f1.ArcSubType) 
						.Field(f1=>f1.ArcMergeNO) 
						.Field(f1=>f1.PROJECT) 
						.Field(f1=>f1.CHCompany) 
						.Field(f1=>f1.CHXZQH) 
						.Field(f1=>f1.CHBGLX) 
						.Field(f1=>f1.Remark) 
						)
					)
				)
			);*/



			if (searchResponse.IsValid)
			{
				List<Arc_PicModel> searchResults = (List<Arc_PicModel>)searchResponse.Documents;

				//dataGridView1.DataSource = ConvertToDataTable(searchResults);
			}
			else
			{
				// 处理搜索错误
			}

			// 设置自动调整模式为所有列
			//dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			// 调整列宽度
			//dataGridView1.AutoResizeColumns();
		}

		//将对象属性转换成检索字段
		public static List<string> GetSearchFields<T>()
		{
			List<string> searchFields = new List<string>();

			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo property in properties)
			{
				searchFields.Add(property.Name);
			}

			return searchFields;
		}

		//将泛型list对象转换成datatable
		static DataTable ConvertToDataTable<T>(IEnumerable<T> data)
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

		private void btn_CreateIndex_Click(object sender, EventArgs e)
		{
			ESHelper.CreateIndex("index_arc_pic");
		}

		private void insertDocument_Click(object sender, EventArgs e)
		{
			Task.Run(() => ESHelper.ImportData());
			// 订阅进度更新事件
			ESHelper.ProgressUpdated += ProgressUpdated;
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void ProgressUpdated(object sender, int progress)
		{
			// 在这里更新进度条的值
			lblCount.Text = progress.ToString();
		}


		private void link_DBConnTest_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			string connStr = $"server={txt_DBIp.Text.Trim()};uid={txt_DBUid.Text.Trim()};pwd={txt_DBPwd.Text.Trim()}";
			SqlHelper.ConnStr = connStr;
			try
			{
				SqlHelper.IsConnection();
				tsl_DBConnStatus.Text = "数据库已连接";
				tsl_DBConnStatus.BackColor = Color.Green;
				tsl_DBConnStatus.ForeColor = Color.White;
				cb_DataBaseNames.DataSource = SqlHelper.GetDataBaseNames();//获取所有数据库

			}
			catch (Exception ex)
			{
				tsl_DBConnStatus.Text = "数据库连接失败";
				tsl_DBConnStatus.BackColor = Color.Red;
				tsl_DBConnStatus.ForeColor = Color.White;
				cb_DataBaseNames.DataSource = null;
				cob_TableNames.DataSource = null;
				cb_DataBaseNames.Items.Clear();
				cob_TableNames.Items.Clear();
				MessageBox.Show(ex.Message, "错误提示");
			}

			Cursor = Cursors.Default;
		}



		private void cb_DataBases_SelectedIndexChanged(object sender, EventArgs e)
		{
			var databaseName = cb_DataBaseNames.Text.Trim();
			SqlHelper.ConnStr = $"server={txt_DBIp.Text.Trim()};database={databaseName};uid={txt_DBUid.Text.Trim()};pwd={txt_DBPwd.Text.Trim()}";
			var allSchemas = SqlHelper.GetAllSchemas(databaseName);
			if (allSchemas.Count == 0)
			{
				cob_SchemaNames.DataSource = null;
				cob_SchemaNames.Items.Clear();
				return;
			}
			cob_SchemaNames.DataSource = allSchemas;
		}

		private void cob_SchemaNames_TextChanged(object sender, EventArgs e)
		{
			string databaseName = cb_DataBaseNames.Text.Trim();
			string schemaName = cob_SchemaNames.Text.Trim();
			var allTables = SqlHelper.GetTablesInSchema(databaseName, schemaName);
			if (allTables.Count == 0)//没有表
			{
				cob_TableNames.DataSource = null;
				cob_TableNames.Items.Clear();
				return;
			}
			cob_TableNames.DataSource = allTables;
		}

		List<string> columnNames = new List<string>();//所有的列名
		private void cob_TableNames_TextChanged(object sender, EventArgs e)
		{
			string databaseName = cb_DataBaseNames.Text.Trim();
			string schemaName = cob_SchemaNames.Text.Trim();
			string tableName = cob_TableNames.Text.Trim();
			if (tableName == "")
			{
				cob_TableColumnNames.DataSource = null;
				cob_TableColumnNames.Items.Clear();
				return;
			}
			string cmdText = $"SELECT COUNT(*) FROM [{databaseName}].[{schemaName}].[{tableName}]";
			tsl_TableRowsCount.Text = SqlHelper.ExecuteTable(cmdText).Rows[0][0].ToString();
			var allTables = SqlHelper.GetTablesInSchema(databaseName, schemaName);
			columnNames = SqlHelper.GetTableColumns(databaseName, schemaName, tableName);
			cob_TableColumnNames.DataSource = columnNames;
			tsl_TableRowsCount.ForeColor = Color.Red;
		}


		private void cob_SqlType_SelectedIndexChanged(object sender, EventArgs e)
		{
			cob_SqlType.Text = "SQL Server";
		}

		int getCount = 50;//默认只查前50条
		private void btn_SearchInDB_Click(object sender, EventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();//总运行时间监视
			string databaseName = cb_DataBaseNames.Text.Trim();
			string schemaName = cob_SchemaNames.Text.Trim();
			string tableName = cob_TableNames.Text.Trim();
			string columnName = cob_TableColumnNames.Text.Trim();
			string keyword = txt_DBKeywords.Text.Trim();
			if (keyword == "" || columnName == "" || tableName == "" || schemaName == "" || databaseName == "") return;
			//getCount = int.Parse(cob_DataCount.Text);//此处暂未作异常处理
			//处理关键词
			//获取所有的关键字
			string[] words = keyword.Split(' ');
			//showMessage(rtb_DBSearchInfo, rtb_ESSearchInfo.Text, 0, false, words);
			// 构建 SQL 查询语句
			StringBuilder sb = new StringBuilder();
			string sql = "";
			if (cob_DataCount.Text == "---查询所有---")
			{
				sql = $"SELECT TOP 100 [{columnName}] FROM [{databaseName}].[{schemaName}].[{tableName}] WHERE [{columnName}] LIKE '%{words[0]}%' ";
			}
			else
			{
				getCount = int.Parse(cob_DataCount.Text);
				sql = $"SELECT TOP {getCount} [{columnName}] FROM [{databaseName}].[{schemaName}].[{tableName}] WHERE [{columnName}] LIKE '%{words[0]}%' ";
			}

			sb.Append(sql);
			//此处暂不处理sql注入
			for (int i = 1; i < words.Count(); i++)
			{
				sb.Append($" {cob_DBSearchMode.Text} [{columnName}] LIKE '%{words[i]}%' ");
			}
			sql = sb.ToString();
			Cursor = Cursors.WaitCursor;
			Stopwatch sw2 = Stopwatch.StartNew();//数据库执行时间监视
			DataTable dt = SqlHelper.ExecuteTable(sql);
			sw2.Stop();
			rtb_DBSearchInfo.Clear();
			if (dt.Rows.Count > 0)
			{
				if (cob_DataCount.Text == "---查询所有---" || //获取所有数据
					cob_DataCount.Text != dt.Rows.Count.ToString()) //如果想要获取的数据与实际的不一样，获取实际的
				{
					getCount = dt.Rows.Count;
				}
				for (int i = 0; i < getCount; i++)
				{
					ShowMessage(rtb_DBSearchInfo, $"第{i + 1}条数据:", 1, false, $"第{i + 1}条数据");
					ShowMessage(rtb_DBSearchInfo, dt.Rows[i][0].ToString(), 2, false, words);
				}
			}
			else
			{
				getCount = 0;
			}
			Cursor = Cursors.Default;
			sw.Stop();
			var endTip = $"=============返回{getCount}条数据=============\r\n" +
				$"总耗时：{sw.Elapsed}\r\n" +
				$"数据库查询并返回数据耗时：{sw2.Elapsed}" +
				$"\r\n=============================================";

			ShowMessage(rtb_DBSearchInfo, endTip, 3, true);
		}

		private void cob_DataCount_TextChanged(object sender, EventArgs e)
		{
			string input = cob_DataCount.Text;
			if (Tools.IsNumeric(input))
			{
				getCount = int.Parse(input);
				return;
			}
			cob_DataCount.Text = "---查询所有---";
		}


		private void link_ESConnTest_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			string ipStr = $"{cob_ESIp.Text.Trim()}:{txt_ESPort.Text.Trim()}";
			ESHelper.ConString = ipStr;
			try
			{
				var client = ESHelper.ESClient(true);//已经做了连接异常处理
				tsl_ESConnStatus.Text = "ES服务已连接";
				tsl_ESConnStatus.ForeColor = Color.White;
				tsl_ESConnStatus.BackColor = Color.Green;
			}
			catch (Exception ex)
			{
				tsl_ESConnStatus.Text = "ES服务连接失败";
				tsl_ESConnStatus.ForeColor = Color.White;
				tsl_ESConnStatus.BackColor = Color.Red;
				MessageBox.Show(ex.Message);
			}
			Cursor = Cursors.Default;
			cb_IndexNames.DataSource = ESHelper.GetIndexNames();//参数为默认的索引名称  获取索引名
		}
		private void cb_IndexNames_TextChanged(object sender, EventArgs e)
		{
			var indexName = cb_IndexNames.Text.Trim();
			if (!ESHelper.ESClient().Indices.Exists(indexName).Exists) return;
			tsl_DocCount.ForeColor = Color.Red;
			tsl_DocCount.Text = ESHelper.GetIndexDocumentCount(indexName).ToString();
			cb_IndexFields.DataSource = ESHelper.GetAllFieldsFromIndex(indexName);//获取字段名
		}

		private void btn_SearchInES_Click(object sender, EventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();
			Cursor = Cursors.WaitCursor;
			var indexName = cb_IndexNames.Text.Trim();
			var fieldName = cb_IndexFields.Text.Trim();
			var searchText = txt_ESKeywords.Text.Trim();
			if (indexName == "" || fieldName == "" || searchText == "")
			{
				return;
			}
			string[] word = searchText.Split(' ')
							.Distinct()
							.Select(c => c.ToString().Trim())
							.Where(s => !string.IsNullOrWhiteSpace(s.Trim()))
							.ToArray();//暂时自己处理关键字词 进行高亮显示
			string[] words = searchText
							.Distinct()
							.Select(c => c.ToString().Trim())
							.Where(s => !string.IsNullOrWhiteSpace(s.Trim()))
							.ToArray();//暂时自己处理关键字词 进行高亮显示
			words = words.Concat(word).ToArray();
			//DataTable dt= ESHelper.MatchAll(indexName,fieldName, searchText);
			Stopwatch sw2 = Stopwatch.StartNew();
			ESHelper.SearchDocuments(indexName, searchText);
			DataTable dt = ESHelper.MatchAll(indexName, searchText);

			sw2.Stop();
			int realCount = dt.Rows.Count;
			// 遍历原始 DataTable 的每一行
			foreach (DataRow row in dt.Rows)
			{
				// 判断行是否为空白
				bool isBlankRow = true;
				foreach (var item in row.ItemArray)
				{
					if (item != null && !string.IsNullOrWhiteSpace(item.ToString()))
					{
						isBlankRow = false;
						break;
					}
				}

				// 如果行不为空白，则将其添加到新的 DataTable 中
				if (isBlankRow)
				{
					realCount--;
				}
			}
			int returnCount = dt.Rows.Count;
			if (realCount <= 0)
			{
				ShowMessage(rtb_Message, $"{DateTime.Now.ToString("G")}:\t没有查到数据！", 1);
				Cursor = Cursors.Default;
				return;
			}

			// 获取指定列的索引
			int columnIndex = dt.Columns.IndexOf(fieldName);

			rtb_ESSearchInfo.Clear();
			// 遍历DataTable的每一行，并获取指定列的值
			/*foreach (DataRow row in dt.Rows)//此处耗时较多，性能交叉 需要重点优化
			{
				count++;
				// 使用索引获取指定列的值
				object columnValue = row[columnIndex];

				// 将值转换为适当的类型（如果需要）
				string stringValue = columnValue?.ToString();

				// 处理列值
				showMessage(rtb_ESSearchInfo, $"第{count}条数据:", 1, false, $"第{count}条数据");
				showMessage(rtb_ESSearchInfo, stringValue, 2, false, words);

			}*/
			//最多显示100条数据
			int showCount = 0;
			if (dt.Rows.Count>100)
			{
				showCount = 100;
			}
			else
			{
				showCount= dt.Rows.Count;
			}
			//优化后
			var columnValues = dt.Rows
					.Cast<DataRow>()
					.Select(row => row[columnIndex]?.ToString())
					.ToList();
			for (int i = 0; i < showCount; i++)
			{
				// 处理列值
				ShowMessage(rtb_ESSearchInfo, $"第{i + 1}条数据:", 1, false, $"第{i + 1}条数据");
				ShowMessage(rtb_ESSearchInfo, columnValues[i], 2, false, words);
			}
			sw.Stop();
			var endTip = $"=============返回{returnCount}条数据=============\r\n" +
				$"总耗时：{sw.Elapsed}\r\n" +
				$"ES服务器返回检索数据耗时：{sw2.Elapsed}" +
				$"\r\n=============================================";

			ShowMessage(rtb_ESSearchInfo, endTip, 3, true);
			Cursor = Cursors.Default;
		}
		private void btn_CreateIndexFromDB_Click(object sender, EventArgs e)
		{
			string databaseName = cb_DataBaseNames.Text.Trim();
			string schemaName = cob_SchemaNames.Text.Trim();
			string tableName = cob_TableNames.Text.Trim();
			if (tableName == "")
			{
				return;
			}
			Cursor = Cursors.WaitCursor;
			var sql = $"SELECT * FROM [{databaseName}].[{schemaName}].[{tableName}]";//此处未处理sql注入
			DataTable dt = SqlHelper.ExecuteTable(sql);
			ESHelper.ImportToESFromDataTable(dt, tableName);
			Cursor = Cursors.Default;
			cb_IndexNames.DataSource = ESHelper.GetIndexNames();//参数为默认的索引名称  获取索引名
			ShowMessage(rtb_Message, $"{DateTime.Now.ToString("G")}:\t导入成功！", 1);
		}

		private void btn_CreateIndexFromDBCol_Click(object sender, EventArgs e)
		{

		}



		//=====================方法类============================

		/// <summary>
		/// 文本显示
		/// </summary>
		/// <param name="richTextBox">显示文本的控件</param>
		/// <param name="message">文本内容</param>
		/// <param name="lineNum">需要换行的行数，默认为0</param>
		/// 此方法有待优化
		public void ShowMessage(RichTextBox richTextBox, string message, int lineNum = 0, bool atStart = false, params string[] keywords)
		{
			Stopwatch sw = Stopwatch.StartNew();
			if (atStart)//是否插入到最开始的位置
			{
				// 将插入点移到文本框的开始位置
				richTextBox.SelectionStart = 0;

				// 对选中文本的进行上色放大加粗
				richTextBox.SelectionColor = Color.Red;
				// 获取当前字体
				Font currentFont = richTextBox.Font;
				// 设置大号的字体
				Font largerFont = new Font(currentFont.FontFamily, currentFont.Size + 4, FontStyle.Bold);
				// 设置选中文本的字体
				richTextBox.SelectionFont = largerFont;
				// 在插入点处插入文本
				richTextBox.SelectedText = message;
				//换行
				for (int i = 0; i < lineNum; i++)
				{
					richTextBox.SelectedText = "\r\n";
				}
				return;
			}

			//找出关键字并高亮
			int temp = richTextBox.TextLength;
			richTextBox.AppendText(message);
			for (int i = 0; i < keywords.Count(); i++)
			{
				int startIndex = temp;//这一步很关键
				Color color = Tools.GenerateRandomColor();
				while (startIndex < richTextBox.TextLength)
				{
					startIndex = richTextBox.Find(keywords[i], startIndex, RichTextBoxFinds.None);

					if (startIndex == -1)
					{
						break;
					}
					// 设置选中文本的起始位置和长度
					richTextBox.Select(startIndex, keywords[i].Length);
					// 对选中文本的进行上色放大加粗
					richTextBox.SelectionColor = color;
					// 获取当前字体
					Font currentFont = richTextBox.Font;
					// 设置大一号的字体
					Font largerFont = new Font(currentFont.FontFamily, currentFont.Size + 3, FontStyle.Bold);
					// 设置选中文本的字体
					richTextBox.SelectionFont = largerFont;
					startIndex += keywords[i].Length;
				}
			}

			//换行
			for (int i = 0; i < lineNum; i++)
			{
				richTextBox.AppendText("\r\n");
			}
			sw.Stop();
			string a = sw.Elapsed.ToString();
		}

		/*	public void showMessage(RichTextBox richTextBox, string message, int lineNum = 0, bool atStart = false, params string[] keywords)
			{
				Stopwatch sw = Stopwatch.StartNew();

				if (atStart)//是否插入到最开始的位置
				{
					// 将插入点移到文本框的开始位置
					richTextBox.SelectionStart = 0;

					// 在插入点处插入文本
					richTextBox.SelectedText = message;
					//换行
					for (int i = 0; i < lineNum; i++)
					{
						richTextBox.SelectedText = "\r\n";
					}
				}
				else
				{
					richTextBox.AppendText(message);

				}
				for (int i = 0; i < keywords.Count(); i++)
				{
					int startIndex = 0;
					Color color = Tools.GenerateRandomColor();
					while (startIndex < richTextBox.TextLength)
					{
						startIndex = richTextBox.Find(keywords[i], startIndex, RichTextBoxFinds.None);

						if (startIndex == -1)
						{
							break;
						}
						// 设置选中文本的起始位置和长度
						richTextBox.Select(startIndex, keywords[i].Length);
						// 对选中文本的进行上色放大加粗
						richTextBox.SelectionColor = color;
						// 获取当前字体
						Font currentFont = richTextBox.Font;
						// 设置大一号的字体
						Font largerFont = new Font(currentFont.FontFamily, currentFont.Size + 2, FontStyle.Bold);
						// 设置选中文本的字体
						richTextBox.SelectionFont = largerFont;
						startIndex += keywords[i].Length;
					}
				}
				//换行
				for (int i = 0; i < lineNum; i++)
				{
					richTextBox.AppendText("\r\n");
				}
				sw.Stop();
				string a = sw.Elapsed.ToString();
			}

	*/
		private void cob_ESIp_TextChanged(object sender, EventArgs e)
		{
			if (!cob_ESIp.Text.StartsWith("http://"))
			{
				cob_ESIp.Text = "http://";
				cob_ESIp.Select(cob_ESIp.Text.Length, 0);//将光标设置到输入框的末尾位置：
			}
		}

		private void btn_DelIndex_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(cb_IndexNames.Text))
			{
				return;
			}
			ESHelper.DeleteIndex(cb_IndexNames.Text);
			ShowMessage(rtb_Message, $"{DateTime.Now.ToString("G")}:\t删除索引【{cb_IndexNames.Text}】成功！", 1);
			cb_IndexNames.DataSource = ESHelper.GetIndexNames();//参数为默认的索引名称  获取索引名
		}
	}
}
