using Nest;
using System;

namespace ElasticsearchDemo.Elasticserach.ESModels
{
	public class Arc_Model
	{
		[PropertyName("id")]
		public int ID { get; set; }

		[PropertyName("associatedcode")]
		public string AssociatedCode { get; set; }

		[PropertyName("owner")]
		public string Owner { get; set; }

		[PropertyName("rightno")]
		public string RightNO { get; set; }

		[PropertyName("houselocated")]
		public string HouseLocated { get; set; }

		[PropertyName("arcrollno")]
		public string ArcRollNo { get; set; }

		[PropertyName("arcsubtype")]
		public string ArcSubType { get; set; }

		[PropertyName("oldbusinessno")]
		public string OldBusinessNO { get; set; }

		[PropertyName("arcmergeno")]
		public string ArcMergeNO { get; set; }

		[PropertyName("project")]
		public string PROJECT { get; set; }

		[PropertyName("chcompany")]
		public string CHCompany { get; set; }

		[PropertyName("chxzqh")]
		public string CHXZQH { get; set; }
		[PropertyName("chbglx")]
		public string CHBGLX { get; set; }

		[PropertyName("remark")]
		public string Remark { get; set; }

		/*
				public string ArcMainType { get; set; }
				public string ArcType { get; set; }
				public int RegType { get; set; }
				public int RegKind { get; set; }
				public int RegCause { get; set; }
				public DateTime RegDate { get; set; }
				public int ScanRegID { get; set; }
				public string BusinessNO { get; set; }
				public int ArcNO { get; set; }
				public int AssociatedIndex { get; set; }
				public int AssociatedPri { get; set; }
				public string TXRightNO { get; set; }
				public string AllOwner { get; set; }
				public string AllRightNO { get; set; }
				public string AllIDCard { get; set; }
				public string AllHouseLocated { get; set; }
				public string OldArcNO { get; set; }
				public string ArcPlaceNO { get; set; }
				public DateTime CreateTime { get; set; }
				public DateTime UpdateTime { get; set; }
				public DateTime EdocTime { get; set; }
				public int EdocRepeats { get; set; }
				public DateTime InfoTime { get; set; }
				public int PageNums { get; set; }
				public int DocBackup { get; set; }
				public int ManagerID { get; set; }
				public int EnterType { get; set; }
				public int EnterState { get; set; }
				public int ScanUser { get; set; }
				public int ScanState { get; set; }
				public int Compare { get; set; }
				public int ArcHaveError { get; set; }
				public int QC { get; set; }
				public int QCUser { get; set; }
				public int ReQC { get; set; }
				public int ReQCUser { get; set; }
				public int FinalQC { get; set; }
				public int FinalQCUser { get; set; }
				public int SampleQC { get; set; }
				public int SampleQCUser { get; set; }
				public int ImgCheck { get; set; }
				public int ImgZip { get; set; }
				public int ImgDel { get; set; }
				public int UpDataToServer { get; set; }
				public int UpToServer { get; set; }
				public string SuffixList { get; set; }
				public DateTime UpToServerTime { get; set; }
				public string UpToServerRemark { get; set; }
				public int PreArcID { get; set; }
				public int RelationUser { get; set; }
				public int RelationState { get; set; }
				public int RelationQC { get; set; }
				public int RelationQCUser { get; set; }
				public int UpToServerPageNums { get; set; }
				public int DelMidServer { get; set; }
				public int HouseNums { get; set; }
				public int CanEdit { get; set; }
				public int DelOurServer { get; set; }
				public string Fix1017 { get; set; }
				public string MortOwner { get; set; }
				public string MortRightNo { get; set; }
				public string DEBTTYPE { get; set; }
				public int tmpUP { get; set; }
				public string SCOPE { get; set; }
				public string WTCompany { get; set; }
				public int UpMergeNO { get; set; }
				public int ChkArcRollNO { get; set; }
				public string AllBusiNO { get; set; }
				public string ChkInfo { get; set; }
				public DateTime CHGDDate { get; set; }
				public string CHGSNO { get; set; }
				public double CHArea { get; set; }
				public string ZJDASJH { get; set; }
				public string DCHYRightNO { get; set; }
				public string DCHYOldArcNO { get; set; }*/
	}
}
