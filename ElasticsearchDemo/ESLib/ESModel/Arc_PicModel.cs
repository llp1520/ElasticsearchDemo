using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchDemo.ESLib.ESModel
{
	public class Arc_PicModel
	{
		[PropertyName("iD")]
        public int iD { get; set; }
        public int arcID { get; set; }
        public int pRI { get; set; }
        public string fileName { get; set; }
		public int pageNO { get; set; }
		public int subPageNO { get; set; }
		public int fileSize { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public int dPI { get; set; }
		public int oCRState { get; set; }
		public DateTime creatTime { get; set; }
		public DateTime updateTime { get; set; }
        public string pageType { get; set; }
        public int a4Nums { get; set; }
        public string pText { get; set; }

		//public string AllWord { get; set; }
		
	}
}
