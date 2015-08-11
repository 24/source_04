using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
//using PB_Ado;
using pb.Data.Xml;
using pb.Text;

//////////////////   Debug pb channel_id

// à finir gestion de gEditRepository

//namespace PB_Grid
namespace pb.Windows.Forms
{
    #region class GridException
    public class GridException : Exception
    {
        public GridException(string sMessage) : base(sMessage) { }
        public GridException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public GridException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public GridException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    public interface IAdo
    {
        IDbDataAdapter CreateDataAdapter(IDbCommand cmdSelect);
        void DataAdapter_Dispose(IDbDataAdapter da);
        void DataAdapter_FillSchema(IDbDataAdapter da, DataTable dt);
        void DataAdapter_Fill(IDbDataAdapter da, DataTable dt);
        int DataAdapter_Update(IDbDataAdapter da, DataTable dt);
        int DataAdapter_Update_DeletedRows(IDbDataAdapter da, DataTable dt);
        IDbCommand CreateCmd(IDbConnection con, string cmd);
        IDbCommand CreateCmd(IDbConnection con, string cmd, CommandType cmdType);
        IDbCommand CreateCmd(IDbConnection con, string cmd, CommandType cmdType, DataTable dt);
        DataTable ExeCmd(IDbConnection con, string cmd, params object[] prm);
        CommandType GetCommandType(string cmdType);
        object CreateCommandBuilder(IDbDataAdapter da);
        IDbCommand CreateUpdateCommand(object cb);
        IDbCommand CreateInsertCommand(object cb);
        IDbCommand CreateDeleteCommand(object cb);
        DbType GetDbType(Type type);
    }

    #region class cGrid
    public class cGrid
	{
		#region maj
		// maj du 05/12/2005 PB
		//   correction de la libération des ressources (Dispose())
		// maj du 29/12/2005 PB
		//   gestion de la suppression des enregistrements sélectionnés, par défaut l'option est activée, pour désactiver l'option mettre Option = "NoDeleteSelected"
		//   correction de Dispose() pour désactiver les event de la grille et effacer les RepositoryItems
		// maj du 10/10/2005 PB
		//   correction du bug sur les champs identity CreateDataSource()
		//   force AutoIncrementSeed = -1 et AutoIncrementStep = -1
		#endregion

		#region doc
        /***
		 * <def
		 *		def                     = "nom de la source de données (DataTable)"
		 *		Table                   = "nom du DataTable"
		 *		Cmd                     = "commande sql de la source de données"
		 *		CmdType                 = "Text|StoredProcedure|TableDirect"
		 *		Option                  = "ReadOnly, NoUpdate, NoInsert, NoInsertButton, NoDelete, NoDeleteSelected, NoDeleteButton, NoMultiSelect,
		 *                                 AllCommand, UpdateCommand, InsertCommand, DeleteCommand,
		 *                                 DetailDynamic, NoBestFitColumns, NoNavigator, ColumnAutoWidth, NoIndicator, ShowGroupPanel,
		 *                                 NoShowColumnHeaders, NoShowFilterPanel,
		 *                                 NoAllowFilter, NoAllowGroup, NoAllowRowSizing, NoAllowSort,
		 *                                 MasterViewMode, AllowZoomDetail, SmartDetailExpand, AllowExpandEmptyDetails, AutoZoomDetail,
		 *                                 EnableDetailToolTip, ShowDetailTabs, SmartDetailHeight,
         *                                 ExpandAllMasterRows"
		 *		Sort                    = "column [asc|desc], column [asc|desc], ..."
		 *		Column                  = "column [caption], column [caption], ..."
		 *                                liste ordonnée des colonnes visibles
		 *		ColumnCaption           = "column caption, column caption, ..."
		 *                                liste des titres des colonnes
         *      Update                  = "column, column, ..."
         *                                liste des colonnes qui peuvent être mis à jour, toutes les autres colonnes sont en ReadOnly
         *      ReadOnly                = "column, column, ..."
         *                                liste des colonnes qui sont en ReadOnly
         *      Hide                    = "column, column, ..."
         *                                liste des colonnes qui sont cachées
         *      Filter                  = "column : filter, column : filter, ..."
		 *		StyleFocusedRow         = <StyleOptions>
		 *		StyleSelectedRow        = <StyleOptions>
		 *		StyleHideSelectionRow   = <StyleOptions>
		 *		Font					= "font name, size, Bold, Italic, Strikeout, Underline"
		 *		RowHeight				= "row height"
		 * >
		 * 
		 *		<StyleOptions>          = "None, StyleEnabled, UseBackColor, UseDrawEndEllipsis, UseDrawFocusRect, UseFont, UseForeColor,
		 *                                 UseHorzAlignment, UseImage, UseVertAlignment, UseWordWrap"
		 * 
		 * <col
		 *		Name                    = "nom de la colonne"
		 *		Caption                 = "titre de la colonne"
		 *		Option                  = "AutoIncrement, PrimaryKey, ReadOnly, Hide"
		 *		Style                   = "HAlignNear, HAlignCenter, HAlignFar, VAlignTop, VAlignCenter, VAlignBottom"
		 *		Width                   = "largeur de la colonne"
		 *		Default                 = "valeur par défaut"
		 *		FormatDate              = ".net format"
		 *		FormatNum               = ".net format"
		 *		Edit                    = "LookUp | Combo | Mask | Memo | HeureSec | Second"
		 * 
		 *		LookUp ...
         *		LookUpName              = "nom du LookUp pour le générer une seule fois"
		 *		Cmd                     = "commande sql de la source de données"
		 *		Column                  = "column [caption], column [caption], ..."
		 *                                liste des colonnes visibles
		 *		Display                 = "colonne à afficher"
		 *		Value                   = "colonne de la valeur à enregistrer"
		 *		NullText                = "affichage d'une valeur nulle"
		 * 
		 *		Combo ...
		 *		Cmd                     = "commande sql de la source de données"
		 *		Option                  = "ValueInListOnly"
		 *		NullText                = "affichage d'une valeur nulle"
		 * 
		 *		Mask ...
		 *		Mask                    = "masque de saisie"
		 *		MaskBlank               = "blanc"
		 *		MaskOption              = "Regular, NoSaveLiteral, NoIgnoreMaskBlank, BeepOnError"
		 * 
		 *		Memo ...
		 * 
		 *		HeureSec ou Second ...
		 *		Format                  = "hh:mm:ss" combinaison h, m et s
		 *		
		 * >
		 *		LookUp ou Combo ... liste des valeurs (pas de Cmd)
		 * <value
		 *		Column_display          = "display value"
		 *		Column_value            = "value"
		 * />
		 * </col>
		 * 
		 * <detail
		 *		def                     = ...
		 *		Table                   = ...
		 *		Cmd                     = ...
		 *		CmdType                 = ...
		 *		Option                  = ...
		 *		MasterId                = "nom de la colonne clé master"
		 *		DetailId                = "nom de la colonne clé détail"
		 * >
		 * 
		 * <detail_col
		 * />
		 * 
		 * </def>
		 * 
		 * 
		 * 
		 
            <def def="b_daypart"
				Cmd="select dp.* from b_daypart dp, b_client cl where cl.client_id = dp.client_id order by cl.longname, isnull(dp.order_no, 9999), dp.longname">
                <col Name="daypart_id" Option="ReadOnly, Hide"/>
                <col Name="client_id" Caption="client" Edit="LookUp" Cmd="select client_id, longname from b_client order by longname"/>
				<col Name="import_type" Caption="import type" Edit="LookUp" Display="Display" Default="type1" Column="Display">
					<value Display="Type 1" import_type="type1"/>
					<value Display="Type 2" import_type="type2"/>
				</col>
				<detail def="b_daypart_detail" MasterId="daypart_id" Cmd="select * from b_daypart_detail">
					<detail_col Name="daypart_id" Option="Hide"/>
					<detail_col Name="begin_hour" Edit="HeureSec"/>
					<detail_col Name="end_hour" Edit="HeureSec"/>
				</detail>
            </def>
		***/
        #endregion

        #region variable
        private static bool gbTrace = false;
        private static Regex grxGridFilter = new Regex(@"([^,]+),\s*");

        private static CultureInfo gCulture = CultureInfo.CurrentCulture;

        private string gsFormName = null;
        private bool gbSaveParameter = true;

        private bool gbDataAdapterFillInProgress = false;
        private bool gbSelectFocusedRow = false;

        private static IAdo _ado = null;

		// Master
        private string gsMasterName = null;
        private GridControl gGridMaster = null;
        private GridView gViewMaster = null;
        private GridControl gGridMaster0 = null;
        private GridView gViewMaster0 = null;
        private SortedList<string, GridControl> gStaticGridControl = new SortedList<string, GridControl>();
        //private GridControl gStaticMasterGrid = null;
        //private GridView gStaticViewMaster = null;
        private DataTable gdtMaster = null;
        private IDbDataAdapter gdaMaster = null;
        private string gsMasterId = null;
        private XtraGridOption gOptionMaster = null;

		// Detail
        private string gsDetailName = null;
        private GridControl gGridDetail = null;
        private GridView gViewDetail = null;
        private DataTable gdtDetail = null;
        private IDbDataAdapter gdaDetail = null;
        private string gsDetailId = null;
        private bool gbDetailDynamic = false;
        private XtraGridOption gOptionDetail = null;
		#endregion

        #region constructor
		#region cGrid(GridControl gridMaster)
        public cGrid(GridControl gridMaster)
        {
            gGridMaster = gridMaster;
            Init();
        }
		#endregion

		#region cGrid(GridControl gridMaster, GridControl gridDetail)
        public cGrid(GridControl gridMaster, GridControl gridDetail)
        {
            gGridMaster = gridMaster;
            gGridDetail = gridDetail;
            Init();
        }
        #endregion

        #region cGrid(GridControl gridMaster, GridControl gridDetail, string sFormName, bool bSaveParameter)
        public cGrid(GridControl gridMaster, GridControl gridDetail, string sFormName, bool bSaveParameter)
        {
            gGridMaster = gridMaster;
            gGridDetail = gridDetail;
            gsFormName = sFormName;
            gbSaveParameter = bSaveParameter;
            Init();
        }
        #endregion
        #endregion

        #region Init
        private void Init()
        {
            if (gGridMaster != null)
			{
                gGridMaster0 = gGridMaster;
                gViewMaster = (GridView)gGridMaster.MainView;
                gViewMaster0 = gViewMaster;
                SetMasterGridEvent(gGridMaster);
			}

            if (gGridDetail != null)
			{
				gViewDetail = (GridView)gGridDetail.MainView;
				////gviewDetail.LostFocus += new EventHandler(GridView_LostFocus);
				//ggridDetail.Leave += new EventHandler(GridControl_Leave);
				gViewDetail.InitNewRow += new DevExpress.Data.InitNewRowEventHandler(GridViewDetail_InitNewRow);
                gViewDetail.FocusedRowChanged += new FocusedRowChangedEventHandler(GridView_SelectFocusedRow);
                ////view = (ColumnView)ggridDetail.KeyboardFocusView;
				////view.InvalidRowException += new InvalidRowExceptionEventHandler(GridView_InvalidRowException);
			}
		}
		#endregion

        #region SetMasterGridEvent
        private void SetMasterGridEvent(GridControl grid)
        {
            GridView view = (GridView)grid.MainView;
            ////gviewMaster.LostFocus += new EventHandler(GridView_LostFocus);
            //ggridMaster.Leave += new EventHandler(GridControl_Leave);
            view.FocusedRowChanged += new FocusedRowChangedEventHandler(GridViewMaster_FocusedRowChanged);
            view.FocusedRowChanged += new FocusedRowChangedEventHandler(GridView_SelectFocusedRow);
            view.Layout += new System.EventHandler(GridViewMaster_Layout);
            ////view = (ColumnView)gGridMaster.KeyboardFocusView;
            ////view.InvalidRowException += new InvalidRowExceptionEventHandler(GridView_InvalidRowException);
        }
        #endregion

        #region RemoveMasterGridEvent
        private void RemoveMasterGridEvent(GridControl grid)
        {
            GridView view = (GridView)grid.MainView;
            ////gviewMaster.LostFocus -= new EventHandler(GridView_LostFocus);
            //ggridMaster.Leave -= new EventHandler(GridControl_Leave);
            view.FocusedRowChanged -= new FocusedRowChangedEventHandler(GridViewMaster_FocusedRowChanged);
            view.FocusedRowChanged -= new FocusedRowChangedEventHandler(GridView_SelectFocusedRow);
            view.Layout -= new System.EventHandler(GridViewMaster_Layout);
            ////view = (ColumnView)gGridMaster.KeyboardFocusView;
            ////view.InvalidRowException -= new InvalidRowExceptionEventHandler(GridView_InvalidRowException);
        }
        #endregion

        #region Dispose
        public void Dispose()
		{
            ClearDataSource();
			if (gGridMaster != null)
			{
                RemoveMasterGridEvent(gGridMaster);
			}
			if (gGridDetail != null)
			{
				////gviewDetail.LostFocus -= new EventHandler(GridView_LostFocus);
				//ggridDetail.Leave -= new EventHandler(GridControl_Leave);
				gViewDetail.InitNewRow -= new DevExpress.Data.InitNewRowEventHandler(GridViewDetail_InitNewRow);
                gViewDetail.FocusedRowChanged -= new FocusedRowChangedEventHandler(GridView_SelectFocusedRow);
                ////view = (ColumnView)ggridDetail.KeyboardFocusView;
				////view.InvalidRowException -= new InvalidRowExceptionEventHandler(GridView_InvalidRowException);
			}
		}
		#endregion

		#region property ...
        #region Trace
        public static bool Trace
        {
            get { return gbTrace; }
            set
            {
                gbTrace = value;
                XtraGridTools.Trace = value;
            }
        }
        #endregion

        #region Culture
        public static CultureInfo Culture
        {
            get { return gCulture; }
            set { gCulture = value; }
        }
        #endregion

        #region SelectFocusedRow
        public bool SelectFocusedRow
        {
            get { return gbSelectFocusedRow; }
            set { gbSelectFocusedRow = value; }
        }
        #endregion

		#region MasterDataTable
		public DataTable MasterDataTable
		{
			get { return gdtMaster; }
			set { gdtMaster = value; }
		}
		#endregion

		#region MasterDataAdapter
		public IDbDataAdapter MasterDataAdapter
		{
			get { return gdaMaster; }
			set { gdaMaster = value; }
		}
		#endregion

		#region DetailDataTable
		public DataTable DetailDataTable
		{
			get { return gdtDetail; }
			set { gdtDetail = value; }
		}
		#endregion

		#region DetailDataAdapter
		public IDbDataAdapter DetailDataAdapter
		{
			get { return gdaDetail; }
			set { gdaDetail = value; }
		}
		#endregion

		#region MasterGrid
		public GridControl MasterGrid
		{
			get { return gGridMaster; }
			set { gGridMaster = value; }
		}
		#endregion

        #region //HasStaticMasterGrid
        //public bool HasStaticMasterGrid
        //{
        //    get { return (gStaticMasterGrid != null); }
        //}
        #endregion

        #region //StaticMasterGrid
        //public GridControl StaticMasterGrid
        //{
        //    get { return gStaticMasterGrid; }
        //}
        #endregion

        #region DetailGrid
		public GridControl DetailGrid
		{
			get { return gGridDetail; }
			set { gGridDetail = value; }
		}
		#endregion

		#region MasterName
		public string MasterName
		{
            get { return gsMasterName; }
        }
		#endregion

		#region DetailName
		public string DetailName
		{
            get { return gsDetailName; }
        }
		#endregion

		#region HasMaster
		public bool HasMaster
		{
			get { return (gdaMaster != null); }
		}
		#endregion

		#region HasDetail
		public bool HasDetail
		{
			get { return (gdaDetail != null); }
		}
		#endregion

		#region Master_CurrentId
		public object Master_CurrentId
		{
			get
			{
                object id = null;
                DataRow row = gViewMaster.GetDataRow(gViewMaster.FocusedRowHandle);
				if (row != null && gsMasterId != null) id = row[gsMasterId];
				return id;
			}
		}
		#endregion

		#region Detail_CurrentId
		public object Detail_CurrentId
		{
			get
			{
                object id = null;
                DataRow row = gViewDetail.GetDataRow(gViewDetail.FocusedRowHandle);
				if (row != null && gsDetailId != null) id = row[gsDetailId];
				return id;
			}
		}
		#endregion

		#region Master_CurrentDataRow
		public DataRow Master_CurrentDataRow
		{
			get
            {
                if (gViewMaster == null) return null;
                return gViewMaster.GetDataRow(gViewMaster.FocusedRowHandle);
            }
		}
		#endregion

        #region Detail_CurrentDataRow
        public DataRow Detail_CurrentDataRow
		{
			get
            {
                if (gViewDetail == null) return null;
                return gViewDetail.GetDataRow(gViewDetail.FocusedRowHandle);
            }
		}
		#endregion
        #endregion

		#region Method ...
		#region Master_Refresh
		public void Master_Refresh()
		{
			if (gdtMaster == null) return;
            if (gbDataAdapterFillInProgress)
            {
                //cTrace.Trace("Master_Refresh : impossible to refresh data ({0}), another refresh data is already in progress", gsMasterName);
                return;
            }

            //cTrace.StartNestedLevel("Master_Refresh");

            //SaveCurrentChange(gGridMaster);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridMaster);

            //Ado.DataAdapter_Update(gdaMaster, gdtMaster);
            _ado.DataAdapter_Update(gdaMaster, gdtMaster);

            if (gdtMaster.PrimaryKey.Length == 0)
            {
                if (gdtDetail != null) gdtDetail.Rows.Clear();
                gdtMaster.Rows.Clear();
            }
            gbDataAdapterFillInProgress = true;
            //Ado.DataAdapter_Fill(gdaMaster, gdtMaster);
            _ado.DataAdapter_Fill(gdaMaster, gdtMaster);
            gbDataAdapterFillInProgress = false;

            //cTrace.StopLevel("Master_Refresh");

            Detail_Refresh();
		}
		#endregion

		#region Detail_Refresh
		public void Detail_Refresh()
		{
			if (gdtDetail == null) return;
            if (gbDataAdapterFillInProgress)
            {
                //cTrace.Trace("Detail_Refresh : impossible to refresh data ({0}), another refresh data is already in progress", gsDetailName);
                return;
            }

            //cTrace.StartNestedLevel("Detail_Refresh");

            //SaveCurrentChange(gGridDetail);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridDetail);

            //Update(gdaDetail, gdtDetail);
            //Ado.DataAdapter_Update(gdaDetail, gdtDetail);
            _ado.DataAdapter_Update(gdaDetail, gdtDetail);

			if (gbDetailDynamic)
				Detail_Query();
			else
			{
                if (gdtDetail.PrimaryKey.Length == 0) gdtDetail.Rows.Clear();
                gbDataAdapterFillInProgress = true;
                //Ado.DataAdapter_Fill(gdaDetail, gdtDetail);
                _ado.DataAdapter_Fill(gdaDetail, gdtDetail);
                gbDataAdapterFillInProgress = false;
            }

            //cTrace.StopLevel("Detail_Refresh");
        }
		#endregion
		
		#region Master_SaveCurrentChange
		public void Master_SaveCurrentChange()
		{
            //SaveCurrentChange(gGridMaster);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridMaster);
        }
		#endregion

		#region Detail_SaveCurrentChange
		public void Detail_SaveCurrentChange()
		{
            //SaveCurrentChange(gGridDetail);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridDetail);
        }
		#endregion
		#endregion

		#region SetDataSource ...
        #region SetMasterDataSource ...
        #region SetMasterDataSource(IDbConnection con, string sXmlParam)
        public void SetMasterDataSource(IDbConnection con, string sXmlParam)
		{
            SetMasterDataSource(con, XDocument.Parse(sXmlParam).Element("def"));
        }
		#endregion

        #region SetMasterDataSource(IDbConnection con, XElement xe)
        public void SetMasterDataSource(IDbConnection con, XElement xe)
        {
            SetMasterDataSource(con, null, xe);
        }
        #endregion

        #region SetMasterDataSource(IDbConnection con, DataList dataList, XElement xe)
        public void SetMasterDataSource(IDbConnection con, DataList dataList, XElement xe)
        {
            if (xe == null) return;
            string sMasterName = GetSourceName(xe);
            if (sMasterName == gsMasterName) return;

            //string sGridName = xe.zAttribValue("GridName");
            //if (sGridName != null)
            //{
            //    GridControl grid;
            //    if (gStaticGridControl.ContainsKey(sGridName))
            //    {
            //        grid = gStaticGridControl[sGridName];
            //        SetMasterGrid(grid);
            //        return;
            //    }
            //    else
            //    {
            //        grid = CreateGridControl();
            //        SetMasterGridEvent(grid);
            //        gStaticGridControl.Add(sGridName, grid);
            //        SetMasterGrid(grid);
            //    }
            //}
            //else
            //    SetMasterGrid(gGridMaster0);

            //cTrace.StartNestedLevel("cGrid_SetMasterDataSource");
            DataTable dt;
            IDbDataAdapter da;
            CreateDataSource(con, dataList, xe, out da, out dt);
            SetMasterDataSource(con, dataList, dt, da, xe);
            //cTrace.StopNestedLevel("cGrid_SetMasterDataSource");
        }
        #endregion

        #region SetMasterGrid
        private void SetMasterGrid(GridControl grid)
        {
            gGridMaster = grid;
            if (grid != null)
                gViewMaster = (GridView)grid.MainView;
            else
                gViewMaster = null;
        }
        #endregion

        #region CreateGridControl
        private static GridControl CreateGridControl()
        {
            GridControl grid = new GridControl();
            ((System.ComponentModel.ISupportInitialize)(grid)).BeginInit();
            //this.pan_data1.Controls.Add(this.grid_data1);
            //this.grid_data1.Dock = System.Windows.Forms.DockStyle.Fill;

            grid.EmbeddedNavigator.Name = "";
            grid.Location = new System.Drawing.Point(0, 0);
            //grid.MainView = view;
            //grid.Name = "grid_data1";
            grid.Size = new System.Drawing.Size(100, 100); //Size(1008, 390);
            //this.grid_data1.TabIndex = 0;
            grid.Text = "Data";
            //this.grid_data1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grid_EditorKeyDown);
            //this.grid_data1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grid_KeyDown);

            GridView view = new GridView();
            ((System.ComponentModel.ISupportInitialize)(view)).BeginInit();
            view.GridControl = grid;
            view.Name = "gridView";


            grid.MainView = view;
            ((System.ComponentModel.ISupportInitialize)(grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(view)).EndInit();

            return grid;
        }
        #endregion

        #region SetMasterDataSource(IDbConnection con, DataList dataList, DataTable dt, IDbDataAdapter da, string sXmlParam)
        public void SetMasterDataSource(IDbConnection con, DataList dataList, DataTable dt, IDbDataAdapter da, string sXmlParam)
        {
            SetMasterDataSource(con, dataList, dt, da, XDocument.Parse(sXmlParam).Element("def"));
        }
        #endregion

        #region SetMasterDataSource(IDbConnection con, DataList dataList, DataTable dt, IDbDataAdapter da, XElement xe)
        public void SetMasterDataSource(IDbConnection con, DataList dataList, DataTable dt, IDbDataAdapter da, XElement xe)
        {
            //cTrace.StartNestedLevel("cGrid_SetMasterDataSource");

            //string sDataName = Xml.GetAttribValue(xe, "def");
            string sDataName = xe.zAttribValue("def");
            if (sDataName == gsMasterName) return;
            //Update();
            ClearDataSource();
            //gsMasterId = null;
            //gsDetailId = null;
            //gbDetailDynamic = false;
            if (xe == null) return;
            gdaMaster = da;
            gdtMaster = dt;
            XtraGridOption option = gOptionMaster = new XtraGridOption(xe.zAttribValue("Option"));
            GridSetDataSource(gGridMaster, dt, xe, con, dataList, option);
            gdtMaster.RowDeleting += new DataRowChangeEventHandler(DataTableMaster_RowDeleting);
            gsMasterName = sDataName;
            //LoadMasterParameters();

            //cTrace.StopNestedLevel("cGrid_SetMasterDataSource");
        }
        #endregion
        #endregion

        #region AddDetailDataSource ...
        #region AddDetailDataSource(IDbConnection con, string sXmlParam)
        public void AddDetailDataSource(IDbConnection con, string sXmlParam)
        {
            AddDetailDataSource(con, null, sXmlParam);
        }
        #endregion

        #region AddDetailDataSource(IDbConnection con, DataList dataList, string sXmlParam)
        public void AddDetailDataSource(IDbConnection con, DataList dataList, string sXmlParam)
		{
            AddDetailDataSource(con, dataList, XDocument.Parse(sXmlParam).Element("def"));
        }
		#endregion

        #region AddDetailDataSource(IDbConnection con, XElement xe)
        public void AddDetailDataSource(IDbConnection con, XElement xe)
        {
            AddDetailDataSource(con, null, xe);
        }
        #endregion

        #region AddDetailDataSource(IDbConnection con, DataList dataList, XElement xe)
        public void AddDetailDataSource(IDbConnection con, DataList dataList, XElement xe)
        {
            string sDetailName = GetSourceName(xe);
            if (sDetailName == gsDetailName) return;

            //if (gbTrace) cTrace.StartNestedLevel("cGrid_AddDetailDataSource_1");

            IDbDataAdapter da;
            DataTable dt;
            CreateDataSource(con, dataList, xe, out da, out dt, gdtMaster);
            AddDetailDataSource(con, dataList, da, dt, xe);

            //if (gbTrace) cTrace.StopNestedLevel("cGrid_AddDetailDataSource_1");
        }
        #endregion

        #region AddDetailDataSource(IDbConnection con, DataList dataList, IDbDataAdapter da, DataTable dt, string sXmlParam)
        public void AddDetailDataSource(IDbConnection con, DataList dataList, IDbDataAdapter da, DataTable dt, string sXmlParam)
		{
            AddDetailDataSource(con, dataList, da, dt, XDocument.Parse(sXmlParam).Element("def"));
        }
		#endregion

        #region AddDetailDataSource(IDbConnection con, DataList dataList, IDbDataAdapter da, DataTable dt, XElement xe)
        public void AddDetailDataSource(IDbConnection con, DataList dataList, IDbDataAdapter da, DataTable dt, XElement xe)
        {
            //string sDataName = Xml.GetAttribValue(xe, "def");
            string sDataName = xe.zAttribValue("def");
            if (sDataName == gsDetailName) return;

            //if (gbTrace) cTrace.StartNestedLevel("cGrid_AddDetailDataSource_2");

            Update();
            int iRow = 0;
            if (gGridMaster != null && gGridMaster.MainView != null) iRow = ((GridView)gGridMaster.MainView).FocusedRowHandle;

            ClearDetailDataSource();
            if (xe == null) return;

            string sOption = xe.zAttribValue("Option");
            XtraGridOption option = gOptionDetail = new XtraGridOption(sOption);
            gdaDetail = da; gdtDetail = dt;
            gbDetailDynamic = option.DetailDynamic;


            gsMasterId = xe.zAttribValue("MasterId");
            gsDetailId = xe.zAttribValue("DetailId", gsMasterId);

            GridSetDataSource(gGridDetail, gdtDetail, xe, con, dataList, option);
            gViewDetail.OptionsView.ShowFilterPanel = false;

            DetailUpdateList();
            gsDetailName = sDataName;
            //LoadDetailParameters();
            if (gGridMaster != null && gGridMaster.MainView != null) ((GridView)gGridMaster.MainView).FocusedRowHandle = iRow;

            //if (gbTrace) cTrace.StopNestedLevel("cGrid_AddDetailDataSource_2");
        }
        #endregion
        #endregion

        #region ClearDataSource
        private void ClearDataSource()
        {
            //if (gbTrace) cTrace.StartNestedLevel("cGrid_ClearDataSource");
            Update();
            ClearDetailDataSource();
            ClearMasterDataSource();
            //if (gbTrace) cTrace.StopNestedLevel("cGrid_ClearDataSource");
        }
        #endregion

        #region ClearMasterDataSource
        private void ClearMasterDataSource()
        {
            //SaveMasterParameters();
            _ado.DataAdapter_Dispose(gdaMaster);
            gdaMaster = null;

            GridClearDataSource(gGridMaster);
            if (gdtMaster != null)
            {
                gdtMaster.Dispose();
                gdtMaster = null;
            }
            gsMasterId = null;
            gbDetailDynamic = false;
            gsMasterName = null;
        }
        #endregion

        #region ClearDetailDataSource
        public void ClearDetailDataSource()
		{
            //SaveDetailParameters();
            _ado.DataAdapter_Dispose(gdaDetail);
            gdaDetail = null;
			if ( gGridDetail != null && gGridDetail.DataSource != null)
			{
                DataTable dt = (DataTable)gGridDetail.DataSource;
				dt.ParentRelations.Clear();
				dt.Constraints.Clear();
				dt.Dispose();
			}
            GridClearDataSource(gGridDetail);
            gdtDetail = null;
			gsDetailId = null;
			gbDetailDynamic = false;
            gsDetailName = null;
        }
		#endregion

        #region GetSourceName(XElement xe)
        private static string GetSourceName(XElement xe)
        {
            if (xe == null) return null;
            XAttribute xa = xe.Attribute("def");
            if (xa != null) return xa.Value;
            return null;
        }
        #endregion

        //private void LoadMasterParameters()
        //{
        //    if (gsFormName == null || gViewMaster == null || gsMasterName == null) return;
        //    XtraGridTools.LoadParameters(gViewMaster, gsFormName, gsMasterName);
        //}

        //private void SaveMasterParameters()
        //{
        //    if (gsFormName == null || gViewMaster == null || gsMasterName == null) return;
        //    XtraGridTools.SaveParameters(gViewMaster, gsFormName, gsMasterName);
        //}

        //private void LoadDetailParameters()
        //{
        //    if (gsFormName == null || gViewDetail == null || gsDetailName == null) return;
        //    XtraGridTools.LoadParameters(gViewDetail, gsFormName, gsDetailName);
        //}

        //private void SaveDetailParameters()
        //{
        //    if (gsFormName == null || gViewDetail == null || gsDetailName == null) return;
        //    XtraGridTools.SaveParameters(gViewDetail, gsFormName, gsDetailName);
        //}

        //public void Master_SetGridColumn(string sXmlParam)
        //{
        //    SetGridColumnXml(gViewMaster, sXmlParam);
        //}

        //public void Detail_SetGridColumn(string sXmlParam)
        //{
        //    SetGridColumnXml(gViewDetail, sXmlParam);
        //}
		#endregion

        #region CreateDataSource (static) ...
        #region CreateDataSource(IDbConnection con, string sXmlParam, out IDbDataAdapter da, out DataTable dt)
        public static void CreateDataSource(IDbConnection con, string sXmlParam, out IDbDataAdapter da, out DataTable dt)
        {
            CreateDataSource(con, null, sXmlParam, out da, out dt);
        }
        #endregion

        #region CreateDataSource(IDbConnection con, DataList dataList, string sXmlParam, out IDbDataAdapter da, out DataTable dt)
        public static void CreateDataSource(IDbConnection con, DataList dataList, string sXmlParam, out IDbDataAdapter da, out DataTable dt)
        {
            CreateDataSource(con, dataList, XDocument.Parse(sXmlParam).Element("def"), out da, out dt);
        }
        #endregion

        #region CreateDataSource(IDbConnection con, DataList dataList, XElement xe, out IDbDataAdapter da, out DataTable dt)
        public static void CreateDataSource(IDbConnection con, DataList dataList, XElement xe, out IDbDataAdapter da, out DataTable dt)
        {
            CreateDataSource(con, dataList, xe, out da, out dt, null);
        }
        #endregion

        #region CreateDataSource(IDbConnection con, DataList dataList, XElement xe, out IDbDataAdapter da, out DataTable dt, DataTable dtMaster)
        /****************
		<def
			def="b_channel"
			Caption="Channel"
			Cmd="commande sql (select * from b_channel"
			CmdType="StoredProcedure | TableDirect | Text"
			UpdateCmd="update b_channel set shortname = @shortname, longname = @longname where channel_id = @channel_id"
			UpdateCmdType="StoredProcedure | TableDirect | Text"
			Option=""
		>
			<col Name="client_id" Option="ReadOnly, Hide"/>
			<!--<col Name="code" Edit="LookUp" Display="code" Value="code" Cmd="select code, longname channel from b_channel order by isnull(order_no, 9999), longname"/>-->
		</def>
		****************/
        public static void CreateDataSource(IDbConnection con, DataList dataList, XElement xe, out IDbDataAdapter da, out DataTable dt, DataTable dtMaster)
        {
            da = null;
            dt = null;
            bool bDetailDynamic = false;

            if (xe == null) return;

            //if (gbTrace) cTrace.StartNestedLevel("cGrid_CreateDataSource");

            string sOption = xe.zAttribValue("Option");
            XtraGridOption option = new XtraGridOption(sOption);
            bDetailDynamic = option.DetailDynamic;

            string sCmdType, sCmd;
            IDbCommand cmd = null;
            string sTableDef = xe.zAttribValue("TableDef");
            if (sTableDef != null)
            {
                DataContainer data = dataList[sTableDef];
                cmd = data.Command;
                dt = data.DataTable;
                //da = Ado.CreateDataAdapter(cmd);
                da = _ado.CreateDataAdapter(cmd);
            }
            else
            {
                sCmdType = xe.zAttribValue("CmdType", CommandType.Text.ToString());
                sCmd = xe.zAttribValue("Cmd");
                //cmd = Ado.CreateCmd(con, sCmd, Ado.GetCommandType(sCmdType));
                cmd = _ado.CreateCmd(con, sCmd, _ado.GetCommandType(sCmdType));

                if (bDetailDynamic)
                {
                    string sMasterId = xe.zAttribValue("MasterId");
                    AddMasterIdParameter(cmd, dtMaster, sMasterId);
                }

                string sTable = GetTableName(xe);
                dt = new DataTable(sTable);

                //da = Ado.CreateDataAdapter(cmd);
                da = _ado.CreateDataAdapter(cmd);

                //Ado.DataAdapter_FillSchema(da, dt);
                _ado.DataAdapter_FillSchema(da, dt);

                string sPrimaryKey = xe.zAttribValue("PrimaryKey");
                SetPrimaryKey(dt, sPrimaryKey);

                SetDataTableColumnOption(dt, xe);
            }

            //////////////////   Debug pb channel_id
            //if (dt.Columns.Contains("channel_id"))
            //{
            //    cTrace.Trace("PB_Grid.DataContainer.CreateDataTable() : après DataAdapter_FillSchema");
            //    DataColumn col = dt.Columns["channel_id"];
            //    cTrace.Trace("PB_Grid.cGrid.CreateDataSource() : Table {0}, col {1}, AutoIncrementSeed = {2}, AutoIncrementStep = {3}", dt.TableName, col.ColumnName, col.AutoIncrementSeed, col.AutoIncrementStep);

            //    dt.TableNewRow -= new DataTableNewRowEventHandler(Test_TableNewRow_Event);
            //    dt.TableNewRow += new DataTableNewRowEventHandler(Test_TableNewRow_Event);

            //    DataRow row = dt.NewRow();
            //    //col.AutoIncrementSeed = -20;
            //    //cTrace.Trace("PB_Grid.cGrid.CreateDataSource() : Table {0}, col {1}, AutoIncrementSeed = {2}, AutoIncrementStep = {3}", dt.TableName, col.ColumnName, col.AutoIncrementSeed, col.AutoIncrementStep);
            //    //row = dt.NewRow();
            //    //col.AutoIncrementSeed = -1;
            //    //cTrace.Trace("PB_Grid.cGrid.CreateDataSource() : Table {0}, col {1}, AutoIncrementSeed = {2}, AutoIncrementStep = {3}", dt.TableName, col.ColumnName, col.AutoIncrementSeed, col.AutoIncrementStep);
            //    //row = dt.NewRow();
            //}

            sCmdType = xe.zAttribValue("UpdateCmdType", CommandType.Text.ToString());
            sCmd = xe.zAttribValue("UpdateCmd");
            if (sCmd != null)
            {
                //da.UpdateCommand = Ado.CreateCmd(con, sCmd, Ado.GetCommandType(sCmdType), dt);
                da.UpdateCommand = _ado.CreateCmd(con, sCmd, _ado.GetCommandType(sCmdType), dt);
                da.UpdateCommand.UpdatedRowSource = UpdateRowSource.Both;
            }

            bool bInsertCmdDefined = false;
            sCmdType = xe.zAttribValue("InsertCmdType", CommandType.Text.ToString());
            sCmd = xe.zAttribValue("InsertCmd");
            if (sCmd != null)
            {
                //da.InsertCommand = Ado.CreateCmd(con, sCmd, Ado.GetCommandType(sCmdType), dt);
                da.InsertCommand = _ado.CreateCmd(con, sCmd, _ado.GetCommandType(sCmdType), dt);
                da.InsertCommand.UpdatedRowSource = UpdateRowSource.Both;
                bInsertCmdDefined = true;
            }

            sCmdType = xe.zAttribValue("DeleteCmdType", CommandType.Text.ToString());
            sCmd = xe.zAttribValue("DeleteCmd");
            if (sCmd != null)
            {
                //da.DeleteCommand = Ado.CreateCmd(con, sCmd, Ado.GetCommandType(sCmdType), dt);
                da.DeleteCommand = _ado.CreateCmd(con, sCmd, _ado.GetCommandType(sCmdType), dt);
                da.DeleteCommand.UpdatedRowSource = UpdateRowSource.Both;
            }

            //object cb = Ado.CreateCommandBuilder(da);
            object cb = _ado.CreateCommandBuilder(da);
            //if (((!option.ReadOnly && !option.NoUpdate) || option.UpdateCommand) && da.UpdateCommand == null) da.UpdateCommand = Ado.CreateUpdateCommand(cb);
            if (((!option.ReadOnly && !option.NoUpdate) || option.UpdateCommand) && da.UpdateCommand == null) da.UpdateCommand = _ado.CreateUpdateCommand(cb);
            //if (((!option.ReadOnly && !option.NoInsert) || option.InsertCommand) && da.InsertCommand == null) da.InsertCommand = Ado.CreateInsertCommand(cb);
            if (((!option.ReadOnly && !option.NoInsert) || option.InsertCommand) && da.InsertCommand == null) da.InsertCommand = _ado.CreateInsertCommand(cb);
            //if (((!option.ReadOnly && !option.NoDelete) || option.DeleteCommand) && da.DeleteCommand == null) da.DeleteCommand = Ado.CreateDeleteCommand(cb);
            if (((!option.ReadOnly && !option.NoDelete) || option.DeleteCommand) && da.DeleteCommand == null) da.DeleteCommand = _ado.CreateDeleteCommand(cb);

            if (da.InsertCommand != null && !bInsertCmdDefined)
            {
                //string sPrimaryKeyTrace;
                //if (dt.PrimaryKey == null) sPrimaryKeyTrace = "null"; else sPrimaryKeyTrace = dt.PrimaryKey.zToStringValues();
                //cTrace.Trace("PB_Grid.cGrid.CreateDataSource() : Table {0}, PrimaryKey {1}", dt.TableName, sPrimaryKeyTrace);

                IDbCommand cmdInsert = GetCommandWithReturn(da.InsertCommand, dt.PrimaryKey);
                da.InsertCommand.Dispose();
                da.InsertCommand = cmdInsert;
            }
            //if (sTableDef == null && !option.DetailDynamic) Ado.DataAdapter_Fill(da, dt);
            if (sTableDef == null && !option.DetailDynamic) _ado.DataAdapter_Fill(da, dt);

            //string sDataName = Xml.GetAttribValue(xe, "def");
            string sDataName = xe.zAttribValue("def");
            //if (gbTrace) cTrace.StopNestedLevel("cGrid_CreateDataSource");
        }
        #endregion

        #region //Test_TableNewRow_Event
        //////////////////   Debug pb channel_id
        //public static void Test_TableNewRow_Event(object sender, DataTableNewRowEventArgs e)
        //{
        //    if (e.Row.Table.Columns.Contains("channel_id"))
        //    {
        //        DataTable dt = e.Row.Table;
        //        object channel_id = e.Row["channel_id"];
        //        DataColumn col = dt.Columns["channel_id"];
        //        cTrace.Trace("new row in table {0} : channel_id = {1}, AutoIncrement {2}, AutoIncrementSeed = {3}, AutoIncrementStep = {4}", dt.TableName, channel_id, col.AutoIncrement, col.AutoIncrementSeed, col.AutoIncrementStep);
        //        //cTrace.Trace("new row in table {0}, col {1}, AutoIncrement {2}, AutoIncrementSeed = {3}, AutoIncrementStep = {4}", dt.TableName, col.ColumnName, col.AutoIncrement, col.AutoIncrementSeed, col.AutoIncrementStep);
        //    }
        //}
        #endregion

        #region SetPrimaryKey
        public static void SetPrimaryKey(DataTable dt, string sPrimaryKey)
        {
            if (sPrimaryKey == null) return;
            string[] sColumns = zsplit.Split(sPrimaryKey, ',', true, false, true);
            DataColumn[] columns = new DataColumn[sColumns.Length];
            for (int i = 0; i < sColumns.Length; i++)
                columns[i] = dt.Columns[sColumns[i]];
            dt.PrimaryKey = columns;
        }
        #endregion

        #region GetTableName(XElement xe)
        public static string GetTableName(XElement xe)
        {
            //string sTableName = Xml.GetAttribValue(xe, "Table");
            string sTableName = xe.zAttribValue("Table");
            if (sTableName == null)
                //sTableName = Xml.GetAttribValue(xe, "def");
                sTableName = xe.zAttribValue("def");
            return sTableName;
        }
        #endregion

        #region //GridClearDataSource (old version)
        //public static void GridClearDataSource(GridControl grid)
        //{
        //    BaseView baseView;
        //    GridView gridView;

        //    if (grid == null) return;
        //    grid.DataSource = null;
        //    gridView = (GridView)grid.MainView;

        //    if (gbTrace) cTrace.StartNestedLevel("Columns_Clear");
        //    gridView.Columns.Clear();
        //    if (gbTrace) cTrace.StopNestedLevel("Columns_Clear");

        //    grid.EmbeddedNavigator.ButtonClick -= new NavigatorButtonClickEventHandler(EmbeddedNavigator_DeleteSelection);
        //    grid.EmbeddedNavigator.NavigatorException -= new NavigatorExceptionEventHandler(EmbeddedNavigator_NavigatorException);

        //    if (gbTrace) cTrace.StartNestedLevel("RepositoryItems_Clear");
        //    //grid.RepositoryItems.Clear();
        //    for (int i = grid.RepositoryItems.Count - 1; i >= 0; i--)
        //    {
        //        RepositoryItem item = grid.RepositoryItems[i];
        //        if (!gStaticEditRepository.ContainsKey(item.Name))
        //            grid.RepositoryItems.RemoveAt(i);
        //    }
        //    if (gbTrace) cTrace.StopNestedLevel("RepositoryItems_Clear");

        //    if (gbTrace) cTrace.StartNestedLevel("View_Dispose");
        //    for (int i = grid.Views.Count - 1; i >= 1; i--)
        //    {
        //        baseView = grid.Views[i];
        //        baseView.Dispose();
        //    }
        //    foreach (BaseView view in grid.LevelDefaults.Values) view.Dispose();
        //    if (gbTrace) cTrace.StopNestedLevel("View_Dispose");

        //    if (gbTrace) cTrace.StartNestedLevel("LevelDefaults_Clear");
        //    grid.LevelDefaults.Clear();
        //    if (gbTrace) cTrace.StopNestedLevel("LevelDefaults_Clear");

        //    grid.Leave -= new EventHandler(GridControl_Leave);
        //}
        #endregion

        #region GridClearDataSource
        public static void GridClearDataSource(GridControl grid)
        {
            //BaseView baseView;
            //GridView gridView;

            //if (grid == null) return;
            //grid.DataSource = null;
            //gridView = (GridView)grid.MainView;

            //if (gbTrace) cTrace.StartNestedLevel("Columns_Clear");
            //gridView.Columns.Clear();
            //if (gbTrace) cTrace.StopNestedLevel("Columns_Clear");

            //grid.EmbeddedNavigator.ButtonClick -= new NavigatorButtonClickEventHandler(EmbeddedNavigator_DeleteSelection);
            //grid.EmbeddedNavigator.NavigatorException -= new NavigatorExceptionEventHandler(EmbeddedNavigator_NavigatorException);

            //if (gbTrace) cTrace.StartNestedLevel("RepositoryItems_Clear");
            ////grid.RepositoryItems.Clear();
            //for (int i = grid.RepositoryItems.Count - 1; i >= 0; i--)
            //{
            //    RepositoryItem item = grid.RepositoryItems[i];
            //    if (!gEditRepository.ContainsKey(item.Name))
            //        grid.RepositoryItems.RemoveAt(i);
            //}
            //if (gbTrace) cTrace.StopNestedLevel("RepositoryItems_Clear");

            //if (gbTrace) cTrace.StartNestedLevel("View_Dispose");
            //for (int i = grid.Views.Count - 1; i >= 1; i--)
            //{
            //    baseView = grid.Views[i];
            //    baseView.Dispose();
            //}
            //foreach (BaseView view in grid.LevelDefaults.Values) view.Dispose();
            //if (gbTrace) cTrace.StopNestedLevel("View_Dispose");

            //if (gbTrace) cTrace.StartNestedLevel("LevelDefaults_Clear");
            //grid.LevelDefaults.Clear();
            //if (gbTrace) cTrace.StopNestedLevel("LevelDefaults_Clear");

            //grid.Leave -= new EventHandler(GridControl_Leave);

            grid.EmbeddedNavigator.ButtonClick -= new NavigatorButtonClickEventHandler(XtraGridTools.EmbeddedNavigator_DeleteSelection);
            grid.EmbeddedNavigator.NavigatorException -= new NavigatorExceptionEventHandler(XtraGridTools.EmbeddedNavigator_NavigatorException);
            grid.Leave -= new EventHandler(GridControl_Leave);

            XtraGridTools.ClearGrid(grid, gStaticEditRepository);
        }
        #endregion

        #region SetDataTableColumnOption(DataTable dt, XElement xe)
        public static void SetDataTableColumnOption(DataTable dt, XElement xe)
        {
            SetDataTableAutoIncrementOption(dt);
            SetDataTableColumnDefaultValue(dt);

            if (xe == null) return;
            foreach (XElement col in xe.Elements("col"))
            {
                DataColumn colTable = dt.Columns[col.zAttribValue("Name")];
                SetDataColumnOption(colTable, col.zAttribValue("Option", ""));
                SetDataTableColumnDefaultValue(colTable, col.zAttribValue("Default", ""));
            }
        }
        #endregion

        #region SetDataTableAutoIncrementOption
        public static void SetDataTableAutoIncrementOption(DataTable dt)
        {
            int i;
            DataColumn colTable;

            for (i = 0; i < dt.Columns.Count; i++)
            {
                colTable = dt.Columns[i];
                if (colTable.AutoIncrement)
                {
                    //cTrace.Trace("PB_Grid.cGrid.SetDataTableAutoIncrementOption() : table {0}, col {1}, AutoIncrementSeed = -1, AutoIncrementStep = -1", dt.TableName, colTable.ColumnName);
                    colTable.AutoIncrementSeed = -1;
                    colTable.AutoIncrementStep = -1;
                }
            }
        }
        #endregion

        #region SetDataColumnOption
        private static void SetDataColumnOption(DataColumn col, string sOption)
        {
            int i;
            string[] sOptions;

            if (col != null && sOption != null && sOption != "")
            {
                sOptions = zsplit.Split(sOption.ToLower(), ',', true);
                for (i = 0; i < sOptions.Length; i++)
                {
                    switch (sOptions[i])
                    {
                        case "autoincrement":
                            col.AutoIncrement = true;
                            break;
                        case "primarykey":
                            col.Table.Constraints.Add("PrimaryKey", col, true);
                            break;
                    }
                }
            }
        }
        #endregion

        #region SetDataTableColumnDefaultValue
        private static void SetDataTableColumnDefaultValue(DataTable dt)
        {
            foreach (DataColumn col in dt.Columns)
            {
                //if (!col.AllowDBNull && !col.AutoIncrement && col.DataType == typeof(bool)) col.DefaultValue = zconvert.DefaultValue(col.DataType);
                if (!col.AllowDBNull && !col.AutoIncrement && col.DataType == typeof(bool))
                    col.DefaultValue = zconvert.DefaultValue(col.DataType);
            }
        }
        #endregion

        #region SetDataTableColumnDefaultValue
        private static void SetDataTableColumnDefaultValue(DataColumn col, string defaultValue)
        {
            if (defaultValue != "" && defaultValue != null)
            {
                //object oDefaultValue = zconvert.TryParse(col.DataType, sDefaultValue);
                col.DefaultValue = defaultValue.zObjectTryParseAs(col.DataType);
            }
        }
        #endregion

        #region CreateDataAdapter
        private static IDbDataAdapter CreateDataAdapter(IDbCommand cmd, XtraGridOption option)
        {
            //IDbDataAdapter da = Ado.CreateDataAdapter(cmd);
            IDbDataAdapter da = _ado.CreateDataAdapter(cmd);
            //object cb = Ado.CreateCommandBuilder(da);
            object cb = _ado.CreateCommandBuilder(da);
            //if ((!option.ReadOnly && !option.NoUpdate) || option.UpdateCommand) da.UpdateCommand = Ado.CreateUpdateCommand(cb);
            if ((!option.ReadOnly && !option.NoUpdate) || option.UpdateCommand) da.UpdateCommand = _ado.CreateUpdateCommand(cb);
            //if ((!option.ReadOnly && !option.NoInsert) || option.InsertCommand) da.InsertCommand = Ado.CreateInsertCommand(cb);
            if ((!option.ReadOnly && !option.NoInsert) || option.InsertCommand) da.InsertCommand = _ado.CreateInsertCommand(cb);
            //if ((!option.ReadOnly && !option.NoDelete) || option.DeleteCommand) da.DeleteCommand = Ado.CreateDeleteCommand(cb);
            if ((!option.ReadOnly && !option.NoDelete) || option.DeleteCommand) da.DeleteCommand = _ado.CreateDeleteCommand(cb);
            return da;
        }
        #endregion

        #region AddMasterIdParameter
        private static void AddMasterIdParameter(IDbCommand cmdSelect, DataTable dtMaster, string sMasterId)
        {
            IDbDataParameter prm = cmdSelect.CreateParameter();
            prm.ParameterName = sMasterId;
            prm.SourceColumn = sMasterId;
            prm.Direction = ParameterDirection.Input;
            DataColumn col = dtMaster.Columns[sMasterId];
            //prm.DbType = Ado.GetDbType(col.DataType);
            prm.DbType = _ado.GetDbType(col.DataType);
            cmdSelect.Parameters.Add(prm);
        }
        #endregion

        #region GetCommandWithReturn
        private static IDbCommand GetCommandWithReturn(IDbCommand cmd, DataColumn[] PrimaryKey)
        {
            int i;
            string s;
            IDbDataParameter prm, prmKey;
            DataColumn key;
            IDbCommand cmdNew;

            cmdNew = null;
            if (cmd != null)
            {
                s = cmd.CommandText;
                if (PrimaryKey != null && PrimaryKey.Length == 1)
                {
                    key = PrimaryKey[0];
                    if (key.AutoIncrement && key.Unique)
                        s += string.Format("; select * from {0} where ({1} = @@identity)", key.Table.TableName, key.ColumnName);
                }

                //cmdNew = Ado.CreateCmd(cmd.Connection, s);
                cmdNew = _ado.CreateCmd(cmd.Connection, s);
                cmdNew.UpdatedRowSource = UpdateRowSource.Both;
                for (i = 0; i < cmd.Parameters.Count; i++)
                {
                    prmKey = (IDbDataParameter)cmd.Parameters[i];
                    prm = null;
                    if (cmd is OleDbCommand)
                    {
                        prm = new OleDbParameter();
                        ((OleDbParameter)prm).OleDbType = ((OleDbParameter)prmKey).OleDbType;
                    }
                    else if (cmd is SqlCommand)
                    {
                        prm = new SqlParameter();
                        ((SqlParameter)prm).SqlDbType = ((SqlParameter)prmKey).SqlDbType;
                    }
                    else if (cmd is OdbcCommand)
                    {
                        prm = new OdbcParameter();
                        ((OdbcParameter)prm).OdbcType = ((OdbcParameter)prmKey).OdbcType;
                    }
                    if (prm != null)
                    {
                        prm.DbType = prmKey.DbType;
                        prm.Direction = prmKey.Direction;
                        prm.ParameterName = prmKey.ParameterName;
                        prm.Precision = prmKey.Precision;
                        prm.Scale = prmKey.Scale;
                        prm.Size = prmKey.Size;
                        prm.SourceColumn = prmKey.SourceColumn;
                        prm.SourceVersion = prmKey.SourceVersion;
                        cmdNew.Parameters.Add(prm);
                    }
                }
            }
            return cmdNew;
        }
        #endregion
        #endregion

		#region GridSetDataSource (static) ...
        #region GridSetDataSource ...
        #region GridSetDataSource(GridControl dxGrid, DataTable dt, string sXmlParam)
        public static void GridSetDataSource(GridControl dxGrid, DataTable dt, string sXmlParam)
		{
			GridSetDataSource(dxGrid, dt, sXmlParam, null, null);
		}
		#endregion

        #region GridSetDataSource(GridControl dxGrid, DataTable dt, string sXmlParam, IDbConnection con, DataList dataList)
        public static void GridSetDataSource(GridControl dxGrid, DataTable dt, string sXmlParam, IDbConnection con, DataList dataList)
		{
            XElement xe = null;
            if (sXmlParam != null) xe = XDocument.Parse(sXmlParam).Element("def");
            GridSetDataSource(dxGrid, dt, xe, con, dataList);
        }
		#endregion

        #region GridSetDataSource(GridControl dxGrid, DataTable dt, XElement xe, IDbConnection con, DataList dataList)
        public static void GridSetDataSource(GridControl dxGrid, DataTable dt, XElement xe, IDbConnection con, DataList dataList)
        {
            //if (xe == null) return;
            string sOption = null;
            if (xe != null)
                sOption = xe.zAttribValue("Option");
            XtraGridOption option = new XtraGridOption(sOption);
            GridSetDataSource(dxGrid, dt, xe, con, dataList, option);
        }
        #endregion

        #region GridSetDataSource(GridControl grid, DataTable dt, XElement xe, IDbConnection con, DataList dataList, cGridOption option)
        private static void GridSetDataSource(GridControl grid, DataTable dt, XElement xe, IDbConnection con, DataList dataList, XtraGridOption option)
        {
            //if (gbTrace) cTrace.StartNestedLevel("cGrid_GridSetDataSource");

            //if (gbTrace) cTrace.StartNestedLevel("EndEvent");
            if (grid.Tag is FormGridEvent)
                ((FormGridEvent)grid.Tag).End();
            //if (gbTrace) cTrace.StopNestedLevel("EndEvent");

            ((GridView)grid.MainView).CollapseAllDetails();
            GridClearDataSource(grid);

            //if (option.MasterViewMode) GridCreateDetailViews(grid, dt);
            if (option.MasterViewMode) XtraGridTools.GridCreateDetailViews(grid, dt);

            //if (gbTrace) cTrace.StartNestedLevel("SetDataSource");
            grid.DataSource = dt;
            grid.ForceInitialize();
            //if (gbTrace) cTrace.StopNestedLevel("SetDataSource");

            //GridSetNavigatorOption(grid, option);
            XtraGridTools.SetNavigatorOption(grid, option);

            GridSetViewOption((GridView)grid.MainView, xe, dt, con, dataList, option);

            grid.Leave += new EventHandler(GridControl_Leave);

            if (xe != null && option.MasterViewMode)
            {
                IEnumerator<XElement> xeDetailEnum = xe.Elements("def").GetEnumerator();
                foreach (DataRelation r in dt.ChildRelations)
                {
                    if (!xeDetailEnum.MoveNext()) break;
                    XElement xeDetail = xeDetailEnum.Current;
                    string sRelation = r.RelationName;
                    if (sRelation == null) break;
                    GridView view = (GridView)grid.LevelDefaults[sRelation];
                    GridSetViewOption(view, xeDetail, r.ChildTable, con, dataList);
                }
            }

            //if (option.ExpandAllMasterRows) GridExpandAllMasterRows(grid);
            if (option.ExpandAllMasterRows) XtraGridTools.GridExpandAllMasterRows((GridView)grid.MainView);

            grid.Tag = new FormGridEvent(grid, xe);

            //if (gbTrace) cTrace.StopNestedLevel("cGrid_GridSetDataSource");
        }
        #endregion
        #endregion

        #region //GridCreateDetailViews
        //private static void GridCreateDetailViews(GridControl grid, DataTable dt)
        //{
        //    foreach (DataRelation r in dt.ChildRelations)
        //    {
        //        string sRelation = r.RelationName;
        //        if (sRelation == null) break;
        //        if (grid.LevelDefaults.Contains(sRelation))
        //        {
        //            GridView oldView = (GridView)grid.LevelDefaults[sRelation];
        //            grid.LevelDefaults.Remove(sRelation);
        //            oldView.Dispose();
        //        }
        //        GridView view = new GridView();
        //        grid.LevelDefaults.Add(sRelation, view);
        //    }
        //}
        #endregion

        #region GridSetOption ...
        #region GridSetOption(GridControl dxGrid, string sXmlParam)
        public static void GridSetOption(GridControl dxGrid, string sXmlParam)
        {
            XElement xe = null;
            if (sXmlParam != null) xe = XDocument.Parse(sXmlParam).Element("def");
            GridSetOption(dxGrid, xe, null, null, null);
        }
        #endregion

        #region GridSetOption(GridControl dxGrid, string sXmlParam, DataTable dt, IDbConnection con, DataList dataList)
        public static void GridSetOption(GridControl dxGrid, string sXmlParam, DataTable dt, IDbConnection con, DataList dataList)
        {
            XElement xe = null;
            if (sXmlParam != null) xe = XDocument.Parse(sXmlParam).Element("def");
            GridSetOption(dxGrid, xe, dt, con, dataList);
        }
        #endregion

        #region GridSetOption(GridControl dxGrid, XElement xe)
        public static void GridSetOption(GridControl dxGrid, XElement xe)
        {
            GridSetOption(dxGrid, xe, null, null, null);
        }
        #endregion

        #region GridSetOption(GridControl dxGrid, XElement xe, DataTable dt, IDbConnection con, DataList dataList)
        public static void GridSetOption(GridControl dxGrid, XElement xe, DataTable dt, IDbConnection con, DataList dataList)
        {
            string sOption = xe.zAttribValue("Option");
            XtraGridOption option = new XtraGridOption(sOption);
            dxGrid.ForceInitialize();

            //GridSetNavigatorOption(dxGrid, option);
            XtraGridTools.SetNavigatorOption(dxGrid, option);

            GridSetViewOption((GridView)dxGrid.MainView, xe, dt, con, dataList, option);
            dxGrid.Leave += new EventHandler(GridControl_Leave);
        }
        #endregion
        #endregion

        #region //GridSetNavigatorOption
        //private static void GridSetNavigatorOption(GridControl dxGrid, GridOption option)
        //{
        //    //if (gbTrace) cTrace.StartNestedLevel("GridSetNavigatorOption");

        //    dxGrid.UseEmbeddedNavigator = !option.NoNavigator;

        //    dxGrid.EmbeddedNavigator.Buttons.Append.Enabled = false;
        //    dxGrid.EmbeddedNavigator.Buttons.Edit.Enabled = false;
        //    dxGrid.EmbeddedNavigator.Buttons.EndEdit.Enabled = false;
        //    dxGrid.EmbeddedNavigator.Buttons.CancelEdit.Enabled = false;
        //    dxGrid.EmbeddedNavigator.Buttons.Remove.Enabled = false;

        //    if (!option.ReadOnly)
        //    {
        //        if (!option.NoUpdate)
        //        {
        //            dxGrid.EmbeddedNavigator.Buttons.Edit.Enabled = true;
        //            dxGrid.EmbeddedNavigator.Buttons.EndEdit.Enabled = true;
        //            dxGrid.EmbeddedNavigator.Buttons.CancelEdit.Enabled = true;
        //        }
        //        if (!option.NoInsert && !option.NoInsertButton)
        //        {
        //            dxGrid.EmbeddedNavigator.Buttons.Append.Enabled = true;
        //        }
        //        if (!option.NoDelete && !option.NoDeleteButton)
        //        {
        //            dxGrid.EmbeddedNavigator.Buttons.Remove.Enabled = true;
        //            if (!option.NoDeleteSelected)
        //            {
        //                dxGrid.EmbeddedNavigator.ButtonClick += new NavigatorButtonClickEventHandler(XtraGridTools.EmbeddedNavigator_DeleteSelection);
        //            }
        //        }
        //    }

        //    dxGrid.EmbeddedNavigator.NavigatorException += new NavigatorExceptionEventHandler(XtraGridTools.EmbeddedNavigator_NavigatorException);

        //    //if (gbTrace) cTrace.StopNestedLevel();
        //}
		#endregion

        #region //EmbeddedNavigator_NavigatorException
        //private static void EmbeddedNavigator_NavigatorException(object sender, NavigatorExceptionEventArgs e)
        //{
        //    string s;

        //    Debug.WriteLine("EmbeddedNavigator_NavigatorException");
        //    e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        //    s = cError.GetErrorMessage(e.Exception, false, false);
        //    MessageBox.Show(s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //}
		#endregion

        #region GridCreateDetailView ...
        #region GridCreateDetailView(GridControl dxGrid, DataTable dt, string sRelation, string sXmlParam)
        public static void GridCreateDetailView(GridControl dxGrid, DataTable dt, string sRelation, string sXmlParam)
		{
			GridCreateDetailView(dxGrid, dt, sRelation, sXmlParam, null, null);
		}
		#endregion

        #region GridCreateDetailView(GridControl dxGrid, DataTable dt, string sRelation, string sXmlParam, IDbConnection con, DataList dataList)
        public static void GridCreateDetailView(GridControl dxGrid, DataTable dt, string sRelation, string sXmlParam, IDbConnection con, DataList dataList)
		{
            GridCreateDetailView(dxGrid, dt, sRelation, XDocument.Parse(sXmlParam).Element("def"), con, dataList);
        }
		#endregion

        #region GridCreateDetailView(GridControl dxGrid, DataTable dt, string sRelation, XElement xe, IDbConnection con, DataList dataList)
        public static void GridCreateDetailView(GridControl dxGrid, DataTable dt, string sRelation, XElement xe, IDbConnection con, DataList dataList)
        {
            string sOption = xe.zAttribValue("Option");
            XtraGridOption option = new XtraGridOption(sOption);

            GridView view = new GridView();
            view.GridControl = dxGrid;
            dxGrid.LevelDefaults.Add(sRelation, view);
            GridSetViewOption(view, xe, dt, con, dataList, option);
        }
        #endregion
        #endregion

        #region GridSetViewOption ...
        #region GridSetViewOption(GridView view, string sXmlParam)
        public static void GridSetViewOption(GridView view, string sXmlParam)
        {
            GridSetViewOption(view, sXmlParam, null, null, null);
        }
        #endregion

        #region GridSetViewOption(GridView view, string sXmlParam, DataTable dt, IDbConnection con, DataList dataList)
        public static void GridSetViewOption(GridView view, string sXmlParam, DataTable dt, IDbConnection con, DataList dataList)
        {
            XElement xe = null;
            if (sXmlParam != null) xe = XDocument.Parse(sXmlParam).Element("def");
            GridSetViewOption(view, xe, dt, con, dataList);
        }
        #endregion

        #region GridSetViewOption(GridView view, XElement xe)
        public static void GridSetViewOption(GridView view, XElement xe)
        {
            GridSetViewOption(view, xe, null, null, null);
        }
        #endregion

        #region GridSetViewOption(GridView view, XElement xe, DataTable dt, IDbConnection con, DataList dataList)
        public static void GridSetViewOption(GridView view, XElement xe, DataTable dt, IDbConnection con, DataList dataList)
        {
            string sOption = xe.zAttribValue("Option");
            XtraGridOption option = new XtraGridOption(sOption);
            GridSetViewOption(view, xe, dt, con, dataList, option);
        }
        #endregion

        #region GridSetViewOption(GridView view, XElement xe, DataTable dt, IDbConnection con, DataList dataList, cGridOption option)
        private static void GridSetViewOption(GridView view, XElement xe, DataTable dt, IDbConnection con, DataList dataList, XtraGridOption option)
        {
            //if (gbTrace) cTrace.StartNestedLevel("GridSetViewOption");

            //if (gbTrace) cTrace.StartNestedLevel("Columns_Clear");
            view.Columns.Clear();
            //if (gbTrace) cTrace.StopNestedLevel("Columns_Clear");

            //if (gbTrace) cTrace.StartNestedLevel("PopulateColumns");
            if (dt != null)
                view.PopulateColumns(dt);
            else
                view.PopulateColumns();
            //if (gbTrace) cTrace.StopNestedLevel("PopulateColumns");

            //((System.ComponentModel.ISupportInitialize)(dxGrid)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(view)).BeginInit();

            string sName = xe.zAttribValue("def");
            if (sName != null) view.Name = sName;

            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumn***");
            string sColumn = xe.zAttribValue("Column");
            //SetGridColumn(view, sColumn);
            XtraGridTools.SetGridColumn(view, sColumn);

            sColumn = xe.zAttribValue("ColumnCaption");
            //SetGridColumnCaption(view, sColumn);
            XtraGridTools.SetGridColumnCaption(view, sColumn);

            sColumn = xe.zAttribValue("Update");
            //SetGridColumnUpdate(view, sColumn);
            XtraGridTools.SetGridColumnUpdate(view, sColumn);

            sColumn = xe.zAttribValue("ReadOnly");
            //SetGridColumnReadOnly(view, sColumn);
            XtraGridTools.SetGridColumnReadOnly(view, sColumn);

            sColumn = xe.zAttribValue("Hide");
            //SetGridColumnHide(view, sColumn);
            XtraGridTools.SetGridColumnHide(view, sColumn);
            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumn***");

            // Filter
            string sFilter = xe.zAttribValue("Filter");
            //SetGridFilter(view, sFilter);
            XtraGridTools.SetGridFilter(view, sFilter);

            view.OptionsView.ColumnAutoWidth = option.ColumnAutoWidth;
            view.OptionsView.ShowGroupPanel = option.ShowGroupPanel;
            view.OptionsView.ShowIndicator = !option.NoIndicator;
            view.OptionsView.ShowFilterPanel = !option.NoShowFilterPanel;
            view.OptionsView.ShowColumnHeaders = !option.NoShowColumnHeaders;

            view.OptionsCustomization.AllowFilter = !option.NoAllowFilter;
            view.OptionsCustomization.AllowGroup = !option.NoAllowGroup;
            view.OptionsCustomization.AllowRowSizing = !option.NoAllowRowSizing;
            view.OptionsCustomization.AllowSort = !option.NoAllowSort;

            //RowHeight
            //SetRowHeight(view, xe.zAttribValue("RowHeight"));
            XtraGridTools.SetRowHeight(view, xe.zAttribValue("RowHeight"));

            //Font font = GetFont(xe.zAttribValue("Font"));
            Font font = XtraGridTools.GetFont(xe.zAttribValue("Font"));
            if (font != null)
            {
                view.ViewStylesInfo.Row.Font = font;
                view.ViewStylesInfo.FocusedRow.Font = font;
                view.ViewStylesInfo.FocusedCell.Font = font;
            }

            StyleOptions style;
            //if (GetStyleOptions(xe.zAttribValue("StyleFocusedRow"), out style))
            if (XtraGridTools.GetStyleOptions(xe.zAttribValue("StyleFocusedRow"), out style))
                view.ViewStylesInfo.FocusedRow.Options = style;
            //if (GetStyleOptions(xe.zAttribValue("StyleSelectedRow"), out style))
            if (XtraGridTools.GetStyleOptions(xe.zAttribValue("StyleSelectedRow"), out style))
                view.ViewStylesInfo.SelectedRow.Options = style;
            //if (GetStyleOptions(xe.zAttribValue("StyleHideSelectionRow"), out style))
            if (XtraGridTools.GetStyleOptions(xe.zAttribValue("StyleHideSelectionRow"), out style))
                view.ViewStylesInfo.HideSelectionRow.Options = style;

            //OptionsDetail
            view.OptionsDetail.EnableMasterViewMode = option.MasterViewMode;
            view.OptionsDetail.AllowZoomDetail = option.AllowZoomDetail;
            view.OptionsDetail.SmartDetailExpand = option.SmartDetailExpand;
            view.OptionsDetail.AllowExpandEmptyDetails = option.AllowExpandEmptyDetails;
            view.OptionsDetail.AutoZoomDetail = option.AutoZoomDetail;
            view.OptionsDetail.EnableDetailToolTip = option.EnableDetailToolTip;
            view.OptionsDetail.ShowDetailTabs = option.ShowDetailTabs;
            view.OptionsDetail.SmartDetailHeight = option.SmartDetailHeight;

            view.OptionsBehavior.Editable = false;

            view.OptionsView.ShowNewItemRow = false;
            if (!option.ReadOnly)
            {
                if (!option.NoUpdate) view.OptionsBehavior.Editable = true;
                if (!option.NoInsert && !option.NoInsertButton) view.OptionsView.ShowNewItemRow = true;
            }

            if (!option.NoMultiSelect) view.OptionsSelection.MultiSelect = true;

            //if (gbTrace) cTrace.StartNestedLevel("GridSetViewOption_2");
            IEnumerable<XElement> cols = null;
            if (xe != null)
            {
                cols = xe.Elements("col");
                foreach (XElement col in cols)
                {
                    string sColName = col.zAttribValue("Name");
                    GridColumn colGrid = view.Columns[sColName];
                    if (colGrid == null)
                    {
                        colGrid = view.Columns.Add();
                        colGrid.Name = sColName;
                        colGrid.Caption = sColName;
                        colGrid.FieldName = sColName;
                        colGrid.VisibleIndex = view.VisibleColumns.Count;
                    }
                    if (colGrid != null)
                    {
                        string sOption = col.zAttribValue("Option", "");
                        string sCaption = col.zAttribValue("Caption");
                        if (sCaption != null) colGrid.Caption = sCaption;
                        //SetGridColumnOption(colGrid, sOption);
                        XtraGridTools.SetGridColumnOption(colGrid, sOption);
                        //SetGridColumnStyle(colGrid, col.zAttribValue("Style"));
                        XtraGridTools.SetGridColumnStyle(colGrid, col.zAttribValue("Style"));
                        //SetGridColumnFormatDate(colGrid, col.zAttribValue("FormatDate"));
                        XtraGridTools.SetGridColumnFormatDate(colGrid, col.zAttribValue("FormatDate"));
                        //SetGridColumnFormatNum(colGrid, col.zAttribValue("FormatNum"));
                        XtraGridTools.SetGridColumnFormatNum(colGrid, col.zAttribValue("FormatNum"));
                        //SetGridColumnEdit(view, con, dataList, colGrid, col.zAttribValue("Edit"), col);
                        XtraGridTools gridTools = new XtraGridTools(con, dataList, gStaticEditRepository);
                        gridTools.SetColumnEdit(colGrid, col);
                    }
                }
            }
            //SetGridColumnsDefaultEdit(view);
            XtraGridTools.SetGridColumnsDefaultEdit(view);
            //if (gbTrace) cTrace.StopNestedLevel("GridSetViewOption_2");

            //((System.ComponentModel.ISupportInitialize)(view)).EndInit();

            //if (gbTrace) cTrace.StartNestedLevel("BestFitColumns");
            if (!option.NoBestFitColumns)
            {
                view.BestFitMaxRowCount = 50;
                view.BestFitColumns();
            }
            //if (gbTrace) cTrace.StopNestedLevel("BestFitColumns");

            if (cols != null)
            {
                foreach (XElement col in cols)
                {
                    GridColumn colGrid = view.Columns[col.zAttribValue("Name")];
                    //XtraGridTools.SetGridColumnWidth(colGrid, col.zAttribValueInt("Width", -1));
                    XtraGridTools.SetGridColumnWidth(colGrid, col.zAttribValue("Width").zTryParseAs<int>(-1));
                }
            }

            string sSort = xe.zAttribValue("Sort");
            //SetGridSort(view, sSort);
            XtraGridTools.SetGridSort(view, sSort);

            //LoadParameters(view, xe);
            //XtraGridTools.LoadParameters(view, xe);

            //if (gbTrace) cTrace.StopNestedLevel("GridSetViewOption");
        }
        #endregion
        #endregion

        public static void SetGridColumnXml(GridView view, string sXmlParam)
		{
            XtraGridTools.SetGridColumn(view, XDocument.Parse(sXmlParam).Element("def").zAttribValue("Column"));
        }

        #region SetGridColumnEditLookUp ...
        private static SortedList<string, RepositoryItem> gStaticEditRepository = new SortedList<string, RepositoryItem>();
		#endregion
        #endregion

		#region Update ...
		#region Update()
		public void Update()
		{
            //if (gbTrace) cTrace.StartNestedLevel("cGrid_Update");

            //SaveCurrentChange(gGridMaster);
            //SaveCurrentChange(gGridDetail);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridMaster);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridDetail);

            int nb = Update(gdaMaster, gdtMaster, gdaDetail, gdtDetail);

            if (gOptionMaster != null && gOptionMaster.AutoRefresh) Master_Refresh();
            if (gOptionDetail != null && gOptionDetail.AutoRefresh) Detail_Refresh();

			if (nb > 0) DetailUpdateList();

            //if (gbTrace) cTrace.StopNestedLevel("cGrid_Update");
		}
		#endregion

		#region Update(IDbDataAdapter daMaster, GridControl gridMaster, IDbDataAdapter daDetail, GridControl gridDetail)
		public static int Update(IDbDataAdapter daMaster, GridControl gridMaster, IDbDataAdapter daDetail, GridControl gridDetail)
		{
			return Update(daMaster, (DataTable)gridMaster.DataSource, daDetail, (DataTable)gridDetail.DataSource);
		}
		#endregion

		#region Update(IDbDataAdapter daMaster, DataTable dtMaster, IDbDataAdapter daDetail, DataTable dtDetail)
		public static int Update(IDbDataAdapter daMaster, DataTable dtMaster, IDbDataAdapter daDetail, DataTable dtDetail)
		{
            //UpdateDeletedRows(daDetail, dtDetail);
            //Ado.DataAdapter_Update_DeletedRows(daDetail, dtDetail);
            _ado.DataAdapter_Update_DeletedRows(daDetail, dtDetail);

            //Update(daMaster, dtMaster);
            //return Update(daDetail, dtDetail);
            //Ado.DataAdapter_Update(daMaster, dtMaster);
            _ado.DataAdapter_Update(daMaster, dtMaster);
            //return Ado.DataAdapter_Update(daDetail, dtDetail);
            return _ado.DataAdapter_Update(daDetail, dtDetail);
        }
		#endregion

		#region Update(IDbDataAdapter da, GridControl grid)
		public static int Update(IDbDataAdapter da, GridControl grid)
		{
            //return Update(da, (DataTable)grid.DataSource);
            //return Ado.DataAdapter_Update(da, (DataTable)grid.DataSource);
            return _ado.DataAdapter_Update(da, (DataTable)grid.DataSource);
        }
		#endregion

        #region //Update(IDbDataAdapter da, DataTable dt)
        //public static int Update(IDbDataAdapter da, DataTable dt)
        //{
        //    if (da != null && dt != null)
        //    {
        //        if (da is OleDbDataAdapter)
        //            return ((OleDbDataAdapter)da).Update(dt);
        //        if (da is SqlDataAdapter)
        //            return ((SqlDataAdapter)da).Update(dt);
        //        if (da is OdbcDataAdapter)
        //            return ((OdbcDataAdapter)da).Update(dt);
        //    }
        //    return 0;
        //}
		#endregion

		#region UpdateDetail
		public void UpdateDetail()
		{
            //SaveCurrentChange(gGridDetail);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridDetail);

            //Update(gdaDetail, gdtDetail);
            //Ado.DataAdapter_Update(gdaDetail, gdtDetail);
            _ado.DataAdapter_Update(gdaDetail, gdtDetail);
        }
		#endregion

		#region UpdateDeletedRows()
		public void UpdateDeletedRows()
		{
            //SaveCurrentChange(gGridMaster);
            //SaveCurrentChange(gGridDetail);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridMaster);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridDetail);

            //UpdateDeletedRows(gdaDetail, gdtDetail);
            //UpdateDeletedRows(gdaMaster, gdtMaster);
            //Ado.DataAdapter_Update_DeletedRows(gdaDetail, gdtDetail);
            _ado.DataAdapter_Update_DeletedRows(gdaDetail, gdtDetail);
            //Ado.DataAdapter_Update_DeletedRows(gdaMaster, gdtMaster);
            _ado.DataAdapter_Update_DeletedRows(gdaMaster, gdtMaster);
        }
		#endregion

		#region UpdateDeletedRows(IDbDataAdapter da, GridControl grid)
		public static int UpdateDeletedRows(IDbDataAdapter da, GridControl grid)
		{
            //return UpdateDeletedRows(da, (DataTable)grid.DataSource);
            //return Ado.DataAdapter_Update_DeletedRows(da, (DataTable)grid.DataSource);
            return _ado.DataAdapter_Update_DeletedRows(da, (DataTable)grid.DataSource);
        }
		#endregion

        #region //UpdateDeletedRows(IDbDataAdapter da, DataTable dt)
        //public static int UpdateDeletedRows(IDbDataAdapter da, DataTable dt)
        //{
        //    int i, nb;
        //    DataRow[] rows;

        //    if (da != null && dt != null)
        //    {
        //        nb = 0;
        //        for (i = 0; i < dt.Rows.Count; i++)
        //        {
        //            if (dt.Rows[i].RowState == DataRowState.Deleted) nb++;
        //        }
        //        rows = new DataRow[nb];
        //        nb = 0;
        //        for (i = 0; i < dt.Rows.Count; i++)
        //        {
        //            if (dt.Rows[i].RowState == DataRowState.Deleted) rows[nb++] = dt.Rows[i];
        //        }
        //        if (da is OleDbDataAdapter)
        //            return ((OleDbDataAdapter)da).Update(rows);
        //        if (da is SqlDataAdapter)
        //            return ((SqlDataAdapter)da).Update(rows);
        //        if (da is OdbcDataAdapter)
        //            return ((OdbcDataAdapter)da).Update(rows);
        //    }
        //    return 0;
        //}
		#endregion

		#region RejectChanges
		public void RejectChanges()
		{
            //SaveCurrentChange(gGridMaster);
            //SaveCurrentChange(gGridDetail);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridMaster);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridDetail);

			if (gdtMaster != null) gdtMaster.RejectChanges();
			if (gdtDetail != null) gdtDetail.RejectChanges();
		}
		#endregion
		#endregion

		#region Detail ...
		#region DetailUpdateList
		public void DetailUpdateList()
		{
			if (gbDetailDynamic) Detail_Query(); else Detail_Filter();
		}
		#endregion

		#region Detail_Filter
		private void Detail_Filter()
		{
			object idMaster;
			ColumnFilterInfo f;
			GridColumn col;

			if (gdaDetail != null)
			{
                //cGrid.SaveCurrentChange(gGridDetail);
                XtraGridTools.SaveCurrentChangeToDataTable(gGridDetail);

				idMaster = Master_CurrentId;
				col = gViewDetail.Columns[gsDetailId];
				if (col != null && col.FilterInfo.Value != idMaster)
				{
					f = new ColumnFilterInfo(idMaster);
					col.FilterInfo = f;
					if (idMaster != null)
					{
						if (!gOptionDetail.NoInsert && !gOptionDetail.NoInsertButton)
                            //cGrid.GridInsertEnable(gGridDetail);
                            XtraGridTools.NavigatorInsertEnable(gGridDetail);
                    }
					else
                        //cGrid.GridInsertDisable(gGridDetail);
                        XtraGridTools.NavigatorInsertDisable(gGridDetail);
                }
			}
		}
		#endregion

		#region Detail_Query
		private void Detail_Query()
		{
            if (gdaDetail == null) return;
            if (gbDataAdapterFillInProgress)
            {
                //cTrace.Trace("Detail_Query : impossible to refresh data ({0}), another refresh data is already in progress", gsDetailName);
                return;
            }

            object idMaster = Master_CurrentId;
            if (gdaDetail.SelectCommand.Parameters.Count == 0) return;
            IDbDataParameter prm = (IDbDataParameter)gdaDetail.SelectCommand.Parameters[0];
            if (prm == null || prm.Value == idMaster) return;

            //SaveCurrentChange(gGridDetail);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridDetail);

            //Update(gdaDetail, gdtDetail);
            //Ado.DataAdapter_Update(gdaDetail, gdtDetail);
            _ado.DataAdapter_Update(gdaDetail, gdtDetail);

			prm.Value = idMaster;
			gdtDetail.Rows.Clear();
            if (idMaster == null) return;
            gbDataAdapterFillInProgress = true;
            try
            {
                //Ado.DataAdapter_Fill(gdaDetail, gdtDetail);
                _ado.DataAdapter_Fill(gdaDetail, gdtDetail);
            }
            catch
            {
            }
            gbDataAdapterFillInProgress = false;
		}
		#endregion

		#region Detail_InitNewRow
		public void Detail_InitNewRow()
		{
			object Id;
			GridColumn col;

			if (gdaDetail != null)
			{
				Id = Master_CurrentId;
				col = gViewDetail.Columns[gsDetailId];
				gViewDetail.SetRowCellValue(gViewDetail.FocusedRowHandle, col, Id);
			}
		}
		#endregion
		#endregion

		#region Event ...
		#region GridControl_Leave
		private static void GridControl_Leave(object sender, EventArgs e)
		{
            //SaveCurrentChange((GridControl)sender);
            XtraGridTools.SaveCurrentChangeToDataTable((GridControl)sender);
        }
		#endregion

		#region GridView_InvalidRowException
		private static void GridView_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
		{
			e.ExceptionMode = ExceptionMode.Ignore;
		}
		#endregion

		#region GridViewMaster_FocusedRowChanged
		private void GridViewMaster_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
		{
			DetailUpdateList();
		}
		#endregion

        #region GridView_SelectFocusedRow
        //private int giGridView_SelectFocusedRow_nb = 0;
        private void GridView_SelectFocusedRow(object sender, FocusedRowChangedEventArgs e)
        {
            if (!gbSelectFocusedRow) return;
            GridView view = (GridView)sender;
            view.UnselectRow(e.PrevFocusedRowHandle);
            view.SelectRow(view.FocusedRowHandle);
            //cTrace.Trace("{0} GridView_FocusedRowChanged {1} : from {2} to {3} real {4}", giGridView_SelectFocusedRow_nb++, view.Name, e.PrevFocusedRowHandle, e.FocusedRowHandle, view.FocusedRowHandle);
        }
        #endregion

		#region GridViewMaster_Layout
		private void GridViewMaster_Layout(object sender, System.EventArgs e)
		{
			DetailUpdateList();
		}
		#endregion

		#region GridViewDetail_InitNewRow
		private void GridViewDetail_InitNewRow(object sender, DevExpress.Data.InitNewRowEventArgs e)
		{
			Detail_InitNewRow();
		}
		#endregion

		#region DataTableMaster_RowDeleting
		private void DataTableMaster_RowDeleting(object sender, DataRowChangeEventArgs e)
		{
			int i, j;
			DataTable dt, dtMaster;
			DataSet ds;
			ForeignKeyConstraint ct;

			dtMaster = e.Row.Table;
			ds = dtMaster.DataSet;
            if (ds == null) return;
			for (i = 0; i < ds.Tables.Count; i++)
			{
				dt = ds.Tables[i];
				if (dt != dtMaster)
				{
					for (j = 0; j < dt.Constraints.Count; j++)
					{
						ct = dt.Constraints[j] as ForeignKeyConstraint;
						if (ct != null)
						{
							if (ct.RelatedTable == dtMaster)
							{
								if ( gGridDetail != null && gGridDetail.DataSource == dt && (gViewDetail.RowCount > 1 || gViewDetail.GetDataRow(0) != null))
									throw new GridException("You can't remove this record because he as child record");
							}
						}
					}
				}
			}
		}
		#endregion

        #region //EmbeddedNavigator_DeleteSelection
        //private static void EmbeddedNavigator_DeleteSelection(object sender, DevExpress.XtraEditors.NavigatorButtonClickEventArgs e)
        //{
        //    int i;
        //    int[] iSelect;
        //    DevExpress.XtraGrid.GridControlNavigator nav;
        //    DevExpress.XtraGrid.GridControl grid;
        //    DevExpress.XtraGrid.Views.Grid.GridView gridView;
        //    DataRow dr;

        //    nav = (GridControlNavigator)sender;
        //    grid = (GridControl)nav.Parent;
        //    if (e.Button.ButtonType == NavigatorButtonType.Remove && !e.Handled)
        //    {
        //        gridView = (GridView)grid.MainView;
        //        iSelect = gridView.GetSelectedRows();
        //        if (iSelect != null)
        //        {
        //            for (i = 0; i < iSelect.Length; i++)
        //            {
        //                dr = gridView.GetDataRow(iSelect[i]);
        //                if (dr != null) DeleteRow(dr);
        //            }
        //        }
        //        else
        //        {
        //            dr = gridView.GetDataRow(gridView.FocusedRowHandle);
        //            if (dr != null) DeleteRow(dr);
        //        }
        //        e.Handled = true;
        //    }
        //}
        #endregion

        #region //DeleteRow
        //private static void DeleteRow(DataRow dr)
        //{
        //    try
        //    {
        //        dr.Delete();
        //    }
        //    catch (Exception ex)
        //    {
        //        string s;

        //        s = cError.GetErrorMessage(ex, false, false);
        //        MessageBox.Show(s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
		#endregion
        #endregion

		#region Form ...
        #region //SaveCurrentChange
        //public static void SaveCurrentChange(GridControl grid)
        //{
        //    ColumnView view;

        //    if (grid != null)
        //    {
        //        //Save the latest changes to the bound DataTable
        //        view = (ColumnView)grid.KeyboardFocusView;

        //        cTrace.StartNestedLevel("CloseEditor");
        //        view.CloseEditor();
        //        cTrace.StopLevel("CloseEditor");

        //        cTrace.StartNestedLevel("UpdateCurrentRow");
        //        view.UpdateCurrentRow();
        //        cTrace.StopLevel("UpdateCurrentRow");
        //    }
        //}
        #endregion

        #region NewGrid
		public static GridControl NewGrid()
		{
			GridControl dxGrid;
			GridView dxGridView;

			dxGrid = new GridControl();
			dxGridView = new GridView();
			((System.ComponentModel.ISupportInitialize)(dxGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(dxGridView)).BeginInit();

			dxGrid.EmbeddedNavigator.Name = "";
			dxGrid.Location = new System.Drawing.Point(32, 40);
			dxGrid.MainView = dxGridView;
			dxGrid.Name = "Grid";
			dxGrid.Size = new System.Drawing.Size(1032, 480);
			dxGrid.TabIndex = 0;
			dxGrid.Text = "gridControl1";

			dxGridView.GridControl = dxGrid;
			dxGridView.Name = "gdxGridView";

			((System.ComponentModel.ISupportInitialize)(dxGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(dxGridView)).EndInit();

			return dxGrid;
		}
		#endregion

        #region //GridInsertEnable
        //public static void GridInsertEnable(GridControl dxGrid)
        //{
        //    ((GridView)dxGrid.MainView).OptionsView.ShowNewItemRow = true;
        //    dxGrid.EmbeddedNavigator.Buttons.Append.Enabled = true;
        //}
        #endregion

        #region //GridInsertDisable
        //public static void GridInsertDisable(GridControl dxGrid)
        //{
        //    SaveCurrentChange(dxGrid);
        //    ((GridView)dxGrid.MainView).OptionsView.ShowNewItemRow = false;
        //    dxGrid.EmbeddedNavigator.Buttons.Append.Enabled = false;
        //}
        #endregion

        #region UnselectAll
        public static void UnselectAll(GridView view)
        {
            int[] iRows = view.GetSelectedRows();
            if (iRows == null) return;
            foreach (int iRow in iRows) view.UnselectRow(iRow);
        }
        #endregion
        #endregion

        #region Export ...
		#region ExportDataSetXmlSchema
		public static void ExportDataSetXmlSchema(DataSet ds)
		{
			string sPath;

			if (ds != null)
			{
				sPath = @"c:\aa\";
				if (ds.DataSetName != null && ds.DataSetName != "") sPath += ds.DataSetName; else sPath += "DataSet";
				sPath += ".xsd";
				ds.WriteXmlSchema(sPath);
			}
		}
		#endregion

		#region ExportParameters
		private static void ExportParameters(IDataParameterCollection Parameters)
		{
			int i;

			Export("  Parameters :");
			for (i = 0; i < Parameters.Count; i++)
			{
				if (i != 0) Export(",");
				Export(" {0}={1}", i, ((IDbDataParameter)Parameters[i]).ParameterName);
			}
			ExportLine();
		}
		#endregion

		#region ExportDataSet
		private static void ExportDataSet(DataSet ds)
		{
			int i;

			ExportLine("Export DataSet : {0}", ds.DataSetName);
			for (i = 0; i < ds.Tables.Count; i++)
			{
				ExportDataTable(ds.Tables[i]);
			}

		}
		#endregion

		#region ExportDataTable
		private static void ExportDataTable(DataTable dt)
		{
			int i;

			ExportLine("  DataTable : {0}", dt.TableName);
			for (i = 0; i < dt.Columns.Count; i++)
			{
			}
			for (i = 0; i < dt.Constraints.Count; i++)
			{
			}
		}
		#endregion

		#region Export
		private static void Export(string sFormat, params object[] oPrm)
		{
			string s;

			if (sFormat != null)
				s = string.Format(sFormat, oPrm);
			else
				s = "";
			Debug.Write(s);
		}
		#endregion

		#region ExportLine()
		private static void ExportLine()
		{
			ExportLine(null);
		}
		#endregion

		#region ExportLine(string sFormat, params object[] oPrm)
		private static void ExportLine(string sFormat, params object[] oPrm)
		{
			string s;

			if (sFormat != null)
				s = string.Format(sFormat, oPrm);
			else
				s = "";
			Debug.WriteLine(s);
		}
		#endregion
		#endregion

		#region Save and load grid param ...
        #region //LoadParameters(GridView view, XElement xe)
        //public static void LoadParameters(GridView view, XElement xe)
        //{
        //    string sPrmFile = xe.zAttribValue("PrmFile");
        //    string sPrmName = xe.zAttribValue("Prm");
        //    if (sPrmName == null) sPrmName = xe.zAttribValue("def");
        //    LoadParameters(view, sPrmFile, sPrmName);
        //}
        #endregion

        #region //LoadParameters(GridView view, string sParameterName, string sGridName)
        //public static void LoadParameters(GridView view, string sParameterName, string sGridName)
        //{
        //    if (sParameterName == null || sGridName == null) return;

        //    if (gbTrace) cTrace.StartNestedLevel("cGrid_LoadParameters");

        //    XmlParameter xp = new XmlParameter(sParameterName);
        //    xp.Deserialize();
        //    cGrid.GridLoadColumnWidth(xp, view, sGridName);

        //    if (gbTrace) cTrace.StopNestedLevel("cGrid_LoadParameters");
        //}
        #endregion

        #region //SaveParameters(GridView view, XElement xe)
        //public static void SaveParameters(GridView view, XElement xe)
        //{
        //    string sPrmFile = xe.zAttribValue("PrmFile");
        //    string sPrmName = xe.zAttribValue("Prm");
        //    if (sPrmName == null) sPrmName = xe.zAttribValue("def");

        //    //SaveParameters(view, sPrmFile, sPrmName);
        //    XtraGridTools.SaveParameters(view, sPrmFile, sPrmName);
        //}
        #endregion

        #region //SaveParameters(GridView view, string sParameterName)
        //public static void SaveParameters(GridView view, string sParameterName)
        //{
        //    SaveParameters(view, sParameterName, null);
        //}
        #endregion

        #region //SaveParameters(GridView view, string sParameterName, string sGridName)
        //public static void SaveParameters(GridView view, string sParameterName, string sGridName)
        //{
        //    if (sParameterName == null) return;
        //    if (sGridName == null) sGridName = view.Name;
        //    if (sGridName == null) return;

        //    //if (gbTrace) cTrace.StartNestedLevel("SaveParameters");

        //    XmlParameter xp = new XmlParameter(sParameterName);
        //    cGrid.GridSaveColumnWidth(xp, view, sGridName);
        //    foreach (BaseView v in view.GridControl.LevelDefaults.Values)
        //    {
        //        if (!(v is GridView)) continue;
        //        GridView gv = (GridView)v;
        //        cGrid.GridSaveColumnWidth(xp, gv, gv.Name);
        //    }

        //    if (gbTrace) cTrace.StartNestedLevel("SaveParameters_Serialize");
        //    xp.Serialize();
        //    if (gbTrace) cTrace.StopNestedLevel("SaveParameters_Serialize");

        //    //if (gbTrace) cTrace.StopNestedLevel("SaveParameters");
        //}
        #endregion

        #region //GridLoadColumnWidth
        //public static void GridLoadColumnWidth(XmlParameter xp, GridView view, string sGrid)
        //{
        //    if (sGrid == null) return;
        //    sGrid += "_";
        //    foreach (GridColumn col in view.Columns)
        //    {
        //        object oWidth = xp.Get(sGrid + col.FieldName);
        //        //if (oWidth != null && oWidth is int) SetGridColumnWidth(col, (int)oWidth);
        //        if (oWidth != null && oWidth is int) XtraGridTools.SetGridColumnWidth(col, (int)oWidth);
        //    }
        //}
        #endregion

		#region //GridSaveColumnWidth
        //public static void GridSaveColumnWidth(XmlParameter xp, GridView view, string sGrid)
        //{
        //    if (sGrid == null) return;

        //    //if (gbTrace) cTrace.StartNestedLevel("GridSaveColumnWidth");

        //    sGrid += "_";
        //    foreach(GridColumn col in view.Columns)
        //    {
        //        xp.Add(sGrid + col.FieldName, col.Width);
        //    }

        //    //if (gbTrace) cTrace.StopNestedLevel("GridSaveColumnWidth");
        //}
		#endregion
		#endregion
	}
	#endregion

    #region class FormGridEvent
    public class FormGridEvent : IDisposable
    {
        #region variable
        private GridControl gGrid = null;
        private XElement gxeGridFormat = null;
        #endregion

        #region constructor
        public FormGridEvent(GridControl grid, XElement xeGridFormat)
        {
            gGrid = grid;
            gxeGridFormat = xeGridFormat;
            Init();
        }
        #endregion

        #region Dispose (IDisposable Members)
        public void Dispose()
        {
            Form form = gGrid.FindForm();
            if (form != null)
                form.FormClosing -= new FormClosingEventHandler(FormClosingEvent);
        }
        #endregion

        #region Init
        private void Init()
        {
            if (gGrid == null || gxeGridFormat == null) return;
            Form form = gGrid.FindForm();
            if (form != null)
                form.FormClosing += new FormClosingEventHandler(FormClosingEvent);
        }
        #endregion

        public void End()
        {
            Form form = gGrid.FindForm();
            if (form != null)
                form.FormClosing -= new FormClosingEventHandler(FormClosingEvent);
            //XtraGridTools.SaveParameters((GridView)gGrid.MainView, gxeGridFormat);
        }

        private void FormClosingEvent(object sender, FormClosingEventArgs e)
        {
            //XtraGridTools.SaveParameters((GridView)gGrid.MainView, gxeGridFormat);
        }
    }
    #endregion

    #region class DataContainer
    public class DataContainer
    {
        #region variable
        private IDbConnection gCon = null;
        private XElement gXmlDataDef = null;
        private string gsName = null;
        private bool gbCommandReaded = false;
        private IDbCommand gCommand = null;
        private DataTable gdt = null;
        private static IAdo _ado = null;
        #endregion

        #region constructor
        public DataContainer(IDbConnection con, XElement xmlDataDef)
        {
            gCon = con;
            gXmlDataDef = xmlDataDef;
            gsName = xmlDataDef.Attribute("Name").Value;
        }
        #endregion

        #region Name
        public string Name
        {
            get { return gsName; }
        }
        #endregion

        #region Command
        public IDbCommand Command
        {
            get
            {
                if (!gbCommandReaded) CreateCommand();
                return gCommand;
            }
        }
        #endregion

        #region DataTable
        public DataTable DataTable
        {
            get
            {
                if (gdt == null) CreateDataTable();
                return gdt;
            }
        }
        #endregion

        #region CreateCommand
        private void CreateCommand()
        {
            string sCmdType = gXmlDataDef.zAttribValue("CmdType", CommandType.Text.ToString());
            string sCmd = gXmlDataDef.zAttribValue("Cmd");
            //if (sCmd != null) gCommand = Ado.CreateCmd(gCon, sCmd, Ado.GetCommandType(sCmdType));
            if (sCmd != null) gCommand = _ado.CreateCmd(gCon, sCmd, _ado.GetCommandType(sCmdType));
            gbCommandReaded = true;
        }
        #endregion

        private void CreateDataTable()
        {
            if (Command != null)
            {
                string sTable = cGrid.GetTableName(gXmlDataDef);
                gdt = new DataTable(sTable);

                //IDbDataAdapter da = Ado.CreateDataAdapter(Command);
                IDbDataAdapter da = _ado.CreateDataAdapter(Command);

                //Ado.DataAdapter_FillSchema(da, gdt);
                _ado.DataAdapter_FillSchema(da, gdt);

                //string sPrimaryKeyTrace;
                //if (gdt.PrimaryKey == null) sPrimaryKeyTrace = "null"; else sPrimaryKeyTrace = gdt.PrimaryKey.zToStringValues();
                //cTrace.Trace("PB_Grid.DataContainer.CreateDataTable() : Table {0}, PrimaryKey {1}", sTable, sPrimaryKeyTrace);

                string sPrimaryKey = gXmlDataDef.zAttribValue("PrimaryKey");
                cGrid.SetPrimaryKey(gdt, sPrimaryKey);

                // Attention il faut appeler SetDataTableColumnOption() avant DataAdapter_Fill() sinon la clé primaire avec auto incrément n'est pas bien initialisé. (bug channel_id)
                cGrid.SetDataTableColumnOption(gdt, gXmlDataDef);

                //////////////////   Debug pb channel_id
                //if (gdt.Columns.Contains("channel_id"))
                //{
                //    cTrace.Trace("PB_Grid.DataContainer.CreateDataTable() : après SetDataTableColumnOption");
                //    DataColumn col = gdt.Columns["channel_id"];
                //    cTrace.Trace("PB_Grid.DataContainer.CreateDataTable() : Table {0}, col {1}, AutoIncrementSeed = {2}, AutoIncrementStep = {3}", gdt.TableName, col.ColumnName, col.AutoIncrementSeed, col.AutoIncrementStep);

                //    gdt.TableNewRow -= new DataTableNewRowEventHandler(cGrid.Test_TableNewRow_Event);
                //    gdt.TableNewRow += new DataTableNewRowEventHandler(cGrid.Test_TableNewRow_Event);

                //    DataRow row = gdt.NewRow();
                //    //col.AutoIncrementSeed = -20;
                //    //cTrace.Trace("PB_Grid.DataContainer.CreateDataTable() : Table {0}, col {1}, AutoIncrementSeed = {2}, AutoIncrementStep = {3}", gdt.TableName, col.ColumnName, col.AutoIncrementSeed, col.AutoIncrementStep);
                //    //row = gdt.NewRow();
                //    //col.AutoIncrementSeed = -1;
                //    //cTrace.Trace("PB_Grid.DataContainer.CreateDataTable() : Table {0}, col {1}, AutoIncrementSeed = {2}, AutoIncrementStep = {3}", gdt.TableName, col.ColumnName, col.AutoIncrementSeed, col.AutoIncrementStep);
                //    //row = gdt.NewRow();
                //}

                //Ado.DataAdapter_Fill(da, gdt);
                _ado.DataAdapter_Fill(da, gdt);

                string sAddNullRow = gXmlDataDef.zAttribValue("AddNullRow");
                if (sAddNullRow != null)
                {
                    AddNullRow(gdt, sAddNullRow);
                }

                //cGrid.SetDataTableColumnOption(gdt, gXmlDataDef);
            }
            else
            {
                gdt = gXmlDataDef.zXmlToDataTable("value");
                //var q = from item in gXmlDataDef.Elements("col") select new { Name = item.zAttribValue("Name"), Type = zreflection.GetType(item.zAttribValue("Type")) };
                var q = from item in gXmlDataDef.Elements("col") select new { Name = item.zAttribValue("Name"), Type = Reflection.GetTypeFromName(item.zAttribValue("Type")) };
                foreach (var col in q)
                    //gdt = zdt.ChangeColumnType(gdt, col.Name, col.Type);
                    gdt = gdt.zChangeColumnType(col.Name, col.Type);
            }
        }

        #region AddNullRow
        private static void AddNullRow(DataTable dt, string sAddNullRow)
        {
            DataRow row = dt.NewRow();
            char[,] cCharZone = new char[,] { { '"', '"' }, { '\'', '\'' } };
            string[] sValues = zsplit.Split(sAddNullRow, ',', cCharZone, true);
            foreach (string sValue in sValues)
            {
                string[] sValues2 = zsplit.Split(sValue, '=', cCharZone, true);
                if (sValues2.Length == 2)
                {
                    string sValue2 = sValues2[1];
                    if ((sValue2.StartsWith("'") && sValue2.EndsWith("'")) || (sValue2.StartsWith("\"") && sValue2.EndsWith("\"")))
                        sValue2 = sValue2.Substring(1, sValue2.Length - 2);
                    row[sValues2[0]] = sValue2;
                }
            }
            dt.Rows.InsertAt(row, 0);
        }
        #endregion

        #region RefreshData
        public void RefreshData()
        {
            //IDbDataAdapter da = Ado.CreateDataAdapter(Command);
            IDbDataAdapter da = _ado.CreateDataAdapter(Command);
            //Ado.DataAdapter_Update(da, DataTable);
            _ado.DataAdapter_Update(da, DataTable);
            //Ado.DataAdapter_Fill(da, DataTable);
            _ado.DataAdapter_Fill(da, DataTable);
        }
        #endregion
    }
    #endregion

    #region class DataList
    public class DataList
    {
        #region variable
        private SortedList<string, DataContainer> gDataList = new SortedList<string, DataContainer>();
        #endregion

        #region constructor
        public DataList(IDbConnection con, XElement xmlDataList)
        {
            InitData(con, xmlDataList);
        }
        #endregion

        #region index
        public DataContainer this[string sDataName]
        {
            get { if (gDataList.ContainsKey(sDataName)) return gDataList[sDataName]; else return null; }
        }
        #endregion

        #region InitData
        public void InitData(IDbConnection con, XElement xmlDataList)
        {
            foreach (XElement xmlData in xmlDataList.Elements("data_table"))
            {
                DataContainer data = new DataContainer(con, xmlData);
                if (!gDataList.ContainsKey(data.Name))
                    gDataList.Add(data.Name, data);
            }
        }
        #endregion
    }
    #endregion
}
