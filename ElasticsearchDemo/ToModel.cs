using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchDemo
{
	public static class ToModel
	{
		public static TModel DataRowToModel<TModel>(this DataRow dr)
		{
			Type type = typeof(TModel);
			TModel md = (TModel)Activator.CreateInstance(type);
			foreach (var prop in type.GetProperties())
			{
				if (dr[prop.Name] == DBNull.Value)//避免dbnull异常
				{
					prop.SetValue(md, "");
				}
				else
				{
					prop.SetValue(md, dr[prop.Name]);
				}
			}
			return md;
		}
	}
}
