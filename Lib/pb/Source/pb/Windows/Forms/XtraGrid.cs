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
using System.Threading;
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
using pb.Data.Xml;
using pb.Text;

//namespace PB_Grid
namespace pb.Windows.Forms
{
    #region class XtraGridException
    public class XtraGridException : Exception
    {
        public XtraGridException(string sMessage) : base(sMessage) { }
        public XtraGridException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public XtraGridException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public XtraGridException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    public class XtraGridFormManager : IDisposable
    {
        #region variable
        private static bool gbTrace = false;

        private string gsFormName = null;
        private SortedList<string, XtraGridControlManager> gGridControls = new SortedList<string, XtraGridControlManager>();
        private bool gbSaveParameter = true;
        private IDbConnection gCon = null;
        private DataList gDataList = null;
        private SortedList<string, RepositoryItem> gStaticEditRepositoryList = new SortedList<string, RepositoryItem>();
        #endregion

        #region Dispose
        public void Dispose()
        {
            //cTrace.Trace("XtraGridFormManager.Dispose : {0}", gsFormName);
            foreach (XtraGridControlManager grid in gGridControls.Values)
            {
                grid.Dispose();
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
                XtraGridControlManager.Trace = value;
                XtraGridTools.Trace = value;
            }
        }
        #endregion

        #region FormName
        public string FormName
        {
            get { return gsFormName; }
            set { gsFormName = value; }
        }
        #endregion

        #region SaveParameter
        public bool SaveParameter
        {
            get { return gbSaveParameter; }
            set
            {
                gbSaveParameter = value;
                foreach (XtraGridControlManager grid in gGridControls.Values)
                    grid.SaveParameter = value;
            }
        }
        #endregion

        #region Connection
        public IDbConnection Connection
        {
            get { return gCon; }
            set { gCon = value; }
        }
        #endregion

        #region DataList
        public DataList DataList
        {
            get { return gDataList; }
            set { gDataList = value; }
        }
        #endregion

        #region StaticEditRepositoryList
        public SortedList<string, RepositoryItem> StaticEditRepositoryList
        {
            get { return gStaticEditRepositoryList; }
            set { gStaticEditRepositoryList = value; }
        }
        #endregion
        #endregion

        #region GetGrid(string name, GridControl grid)
        public XtraGridControlManager GetGrid(string name, GridControl grid)
        {
            //if (name == null || name == "") throw new XtraGridException("error creating grid name is not defined");
            //if (gGridControls.ContainsKey(name)) return gGridControls[name];
            //XtraGridControlManager gridManager = new XtraGridControlManager(this, name, grid);
            //gGridControls.Add(name, gridManager);
            //return gridManager;
            return GetGrid(name, grid, null);
        }
        #endregion

        #region GetGrid(string name, GridControl grid, XtraGridControlManager parentGrid)
        public XtraGridControlManager GetGrid(string name, GridControl grid, XtraGridControlManager parentGrid)
        {
            if (name == null || name == "") throw new XtraGridException("error creating grid name is not defined");
            if (gGridControls.ContainsKey(name)) return gGridControls[name];
            XtraGridControlManager gridManager = new XtraGridControlManager(this, name, grid, parentGrid);
            gGridControls.Add(name, gridManager);
            return gridManager;
        }
        #endregion

        #region CreateGrid(XElement xe)
        public XtraGridControlManager GetGrid(XElement xe)
        {
            //if (xe == null) return null;
            //string sName = xe.zAttribValue("Name");
            //if (sName == null || sName == "") throw new XtraGridException("error creating grid name is not defined");
            //XtraGridControlManager gridManager = new XtraGridControlManager(this, xe);
            //gGridControls.Add(sName, gridManager);
            //return gridManager;
            return GetGrid(xe, null, null);
        }
        #endregion

        #region GetGrid(XElement xe, GridControl grid)
        public XtraGridControlManager GetGrid(XElement xe, GridControl grid)
        {
            //string sName = xe.zAttribValue("Name");
            //if (sName == null || sName == "") throw new XtraGridException("error creating grid name is not defined");
            //XtraGridControlManager gridManager = new XtraGridControlManager(this, xe, grid);
            //gGridControls.Add(sName, gridManager);
            //return gridManager;
            return GetGrid(xe, grid, null);
        }
        #endregion

        #region GetGrid(XElement xe, XtraGridControlManager parentGrid)
        public XtraGridControlManager GetGrid(XElement xe, XtraGridControlManager parentGrid)
        {
            //string sName = xe.zAttribValue("Name");
            //if (sName == null || sName == "") throw new XtraGridException("error creating grid name is not defined");
            //XtraGridControlManager gridManager = new XtraGridControlManager(this, xe, parentGrid);
            //gGridControls.Add(sName, gridManager);
            //return gridManager;
            return GetGrid(xe, null, parentGrid);
        }
        #endregion

        public XtraGridControlManager GetGrid(XElement xe, GridControl grid, XtraGridControlManager parentGrid)
        {
            if (xe == null) return null;
            string sName = xe.zAttribValue("Name");
            if (sName == null || sName == "") throw new XtraGridException("error creating grid name is not defined");
            if (gGridControls.ContainsKey(sName)) return gGridControls[sName];
            XtraGridControlManager gridManager = new XtraGridControlManager(this, xe, grid, parentGrid);
            gGridControls.Add(sName, gridManager);
            return gridManager;
        }
    }

    public class XtraGridControlManager : IDisposable
    {
        #region variable
        private static bool gbTrace = false;
        private static bool gbDataAdapterFillInProgress = false;

        private XtraGridFormManager gFormManager = null;
        private XtraGridControlManager gParentGrid = null;
        private List<XtraGridControlManager> gChildGrid = new List<XtraGridControlManager>();
        private string gsName = null;
        private XElement gGridDefinition = null;
        private GridControl gGridControl = null;
        private GridView gGridView = null;
        private DataTable gdt = null;
        private IDbDataAdapter gda = null;
        private string gsParentIdFieldName = null;
        private string gsChildIdFieldName = null;
        private XtraGridOption gGridOption = null;
        private bool gbDynamicDataUpdate = false;
        private bool gbSaveParameter = true;
        private bool gbSelectFocusedRow = false;
        private Control gParentControl = null;
        private static IAdo _ado = null;
        #endregion

        #region property ...
        #region Trace
        public static bool Trace
        {
            get { return gbTrace; }
            set { gbTrace = value; }
        }
        #endregion

        #region FormManager
        public XtraGridFormManager FormManager
        {
            get { return gFormManager; }
        }
        #endregion

        #region ParentGrid
        public XtraGridControlManager ParentGrid
        {
            get { return gParentGrid; }
        }
        #endregion

        #region ChildGrid
        public List<XtraGridControlManager> ChildGrid
        {
            get { return gChildGrid; }
        }
        #endregion

        #region Name
        public string Name
        {
            get { return gsName; }
        }
        #endregion

        #region GridControl
        public GridControl GridControl
        {
            get { return gGridControl; }
        }
        #endregion

        #region GridView
        public GridView GridView
        {
            get { return gGridView; }
        }
        #endregion

        #region DataTable
        public DataTable DataTable
        {
            get { return gdt; }
        }
        #endregion

        #region DataAdapter
        public IDbDataAdapter DataAdapter
        {
            get { return gda; }
        }
        #endregion

        #region ParentIdFieldName
        public string ParentIdFieldName
        {
            get { return gsParentIdFieldName; }
        }
        #endregion

        #region ChildIdFieldName
        public string ChildIdFieldName
        {
            get { return gsChildIdFieldName; }
        }
        #endregion

        #region GridOption
        public XtraGridOption GridOption
        {
            get { return gGridOption; }
        }
        #endregion

        #region DynamicDataUpdate
        public bool DynamicDataUpdate
        {
            get { return gbDynamicDataUpdate; }
            set { gbDynamicDataUpdate = value; }
        }
        #endregion

        #region SaveParameter
        public bool SaveParameter
        {
            get { return gbSaveParameter; }
            set { gbSaveParameter = value; }
        }
        #endregion

        #region SelectFocusedRow
        public bool SelectFocusedRow
        {
            get { return gbSelectFocusedRow; }
            set { gbSelectFocusedRow = value; }
        }
        #endregion
        #endregion

        #region constructor ...
        #region //XtraGridControlManager(GridControl grid)
        //public XtraGridControlManager(GridControl grid)
        //{
        //    gGrid = grid;
        //    Init();
        //}
		#endregion

        #region XtraGridControlManager(XtraGridFormManager formManager, string name, GridControl grid)
        public XtraGridControlManager(XtraGridFormManager formManager, string name, GridControl grid)
        {
            gFormManager = formManager;
            gsName = name;
            gGridControl = grid;
            Init();
        }
        #endregion

        #region XtraGridControlManager(XtraGridFormManager formManager, XElement xe)
        public XtraGridControlManager(XtraGridFormManager formManager, XElement xe)
        {
            gFormManager = formManager;
            gsName = xe.zAttribValue("Name");
            gGridDefinition = xe;
            Init();
        }
        #endregion

        #region XtraGridControlManager(XtraGridFormManager formManager, XElement xe, GridControl grid)
        public XtraGridControlManager(XtraGridFormManager formManager, XElement xe, GridControl grid)
        {
            gFormManager = formManager;
            gsName = xe.zAttribValue("Name");
            gGridDefinition = xe;
            gGridControl = grid;
            Init();
        }
        #endregion

        #region XtraGridControlManager(XtraGridFormManager formManager, string name, GridControl grid, XtraGridControlManager parentGrid)
        public XtraGridControlManager(XtraGridFormManager formManager, string name, GridControl grid, XtraGridControlManager parentGrid)
        {
            gFormManager = formManager;
            gsName = name;
            gGridControl = grid;
            gParentGrid = parentGrid;
            Init();
        }
        #endregion

        #region XtraGridControlManager(XtraGridFormManager formManager, XElement xe, XtraGridControlManager parentGrid)
        public XtraGridControlManager(XtraGridFormManager formManager, XElement xe, XtraGridControlManager parentGrid)
        {
            gFormManager = formManager;
            gsName = xe.zAttribValue("Name");
            gGridDefinition = xe;
            gParentGrid = parentGrid;
            Init();
        }
        #endregion

        #region XtraGridControlManager(XtraGridFormManager formManager, XElement xe, GridControl grid, XtraGridControlManager parentGrid)
        public XtraGridControlManager(XtraGridFormManager formManager, XElement xe, GridControl grid, XtraGridControlManager parentGrid)
        {
            gFormManager = formManager;
            gsName = xe.zAttribValue("Name");
            gGridDefinition = xe;
            gGridControl = grid;
            gParentGrid = parentGrid;
            Init();
        }
        #endregion
        #endregion

        private void Init()
        {
            //cTrace.Trace("XtraGridControlManager constructor");
            if (gParentGrid != null) gParentGrid.gChildGrid.Add(this);
            if (gGridControl == null) gGridControl = XtraGridTools.CreateGridControl(gsName);
            gParentControl = gGridControl.Parent;
            gGridView = (GridView)gGridControl.MainView;
            SetGridOption();
            CreateDataSource();
            SetGridDataSource();
            //LoadParameters();
            SetGridEvent();
        }

        #region Dispose
        public void Dispose()
        {
            //cTrace.Trace("XtraGridControlManager.Dispose : {0}", gsName);
            // Mise à jour des données de la grille et de ses filles dans la base
            UpdateData();
            ClearDataSource_WithChild();

        }
        #endregion

        #region ClearDataSource_WithChild
        private void ClearDataSource_WithChild()
        {
            foreach (XtraGridControlManager grid in gChildGrid)
                grid.ClearDataSource_WithChild();
            ClearDataSource();
        }
        #endregion

        private void ClearDataSource()
        {
            //SaveParameters();
            _ado.DataAdapter_Dispose(gda);
            gda = null;
            ClearDataTable();
            RemoveGridEvent();
            SortedList<string, RepositoryItem> editRepositoryList = GetEditRepositoryList();
            XtraGridTools.ClearGrid(gGridControl, editRepositoryList);
            if (gParentGrid != null) gParentGrid.gChildGrid.Remove(this);
        }

        #region ClearDataTable
        private void ClearDataTable()
        {
            if (gdt == null) return;
            gdt.ParentRelations.Clear();
            gdt.Constraints.Clear();
            gdt.Dispose();
            gdt = null;
        }
        #endregion

        #region Property ...
        #region ParentDataRow
        public DataRow ParentDataRow
        {
            get
            {
                if (gParentGrid == null) return null;
                GridView view = gParentGrid.gGridView;
                return view.GetDataRow(view.FocusedRowHandle);
            }
        }
        #endregion

        #region CurrentParentId
        public object CurrentParentId
        {
            get
            {
                if (gsParentIdFieldName == null) return null;
                DataRow row = ParentDataRow;
                if (row == null) return null;
                return row[gsParentIdFieldName];
            }
        }
        #endregion
        #endregion

        #region SetGridEvent
        private void SetGridEvent()
        {
            //gParentGrid
            //gChildGrid

            // ******************************************** Parent and child event **********************************
            gGridView.FocusedRowChanged += new FocusedRowChangedEventHandler(GridView_SelectFocusedRow);
            gGridControl.Leave += new EventHandler(GridControl_Leave);
            ((ColumnView)gGridControl.KeyboardFocusView).InvalidRowException += new InvalidRowExceptionEventHandler(GridView_InvalidRowException);
            gGridControl.ParentChanged += new EventHandler(GridControl_ParentChanged);

            // ******************************************** Parent event ********************************************
            gGridView.FocusedRowChanged += new FocusedRowChangedEventHandler(GridViewParent_FocusedRowChanged);
            gGridView.Layout += new System.EventHandler(GridViewParent_Layout);

            // ******************************************** Child event *********************************************
            gGridView.InitNewRow += new DevExpress.Data.InitNewRowEventHandler(GridViewChild_InitNewRow);
        }
        #endregion

        #region RemoveGridEvent
        private void RemoveGridEvent()
        {
            // ******************************************** Parent and child event **********************************
            gGridView.FocusedRowChanged -= new FocusedRowChangedEventHandler(GridView_SelectFocusedRow);
            gGridControl.Leave -= new EventHandler(GridControl_Leave);
            ((ColumnView)gGridControl.KeyboardFocusView).InvalidRowException -= new InvalidRowExceptionEventHandler(GridView_InvalidRowException);
            gGridControl.EmbeddedNavigator.ButtonClick -= new NavigatorButtonClickEventHandler(XtraGridTools.EmbeddedNavigator_DeleteSelection);
            gGridControl.EmbeddedNavigator.NavigatorException -= new NavigatorExceptionEventHandler(XtraGridTools.EmbeddedNavigator_NavigatorException);
            gGridControl.ParentChanged -= new EventHandler(GridControl_ParentChanged);

            // ******************************************** Parent event ********************************************
            gGridView.FocusedRowChanged -= new FocusedRowChangedEventHandler(GridViewParent_FocusedRowChanged);
            gGridView.Layout -= new System.EventHandler(GridViewParent_Layout);

            // ******************************************** Child event *********************************************
            gGridView.InitNewRow -= new DevExpress.Data.InitNewRowEventHandler(GridViewChild_InitNewRow);
        }
        #endregion

        #region Event ...
        #region GridControl_ParentChanged
        private void GridControl_ParentChanged(object sender, EventArgs e)
        {
            //string sParent;
            //Control parent = gGridControl.Parent;
            //if (parent != null) sParent = parent.Name; else sParent = "no parent";
            //cTrace.Trace("gGridControl_ParentChanged : {0}, parent {1}", gsName, sParent);
            if (gParentControl != null) GridControl_Disable();
            if (gGridControl.Parent != null) GridControl_Enable();
            gParentControl = gGridControl.Parent;
        }
        #endregion

        #region GridControl_Enable
        private void GridControl_Enable()
        {
            //cTrace.Trace("GridControl_Enable : {0}", gsName);
        }
        #endregion

        private void GridControl_Disable()
        {
            //SaveParameters();
        }

        private void GridControl_Leave(object sender, EventArgs e)
        {
            XtraGridTools.SaveCurrentChangeToDataTable((GridControl)sender);
            //SaveParameters();
        }

        #region GridView_SelectFocusedRow
        //private int giGridView_SelectFocusedRow_nb = 0;
        private void GridView_SelectFocusedRow(object sender, FocusedRowChangedEventArgs e)
        {
            // Event : FocusedRowChanged

            if (!gbSelectFocusedRow) return;
            GridView view = (GridView)sender;
            view.UnselectRow(e.PrevFocusedRowHandle);
            view.SelectRow(view.FocusedRowHandle);
            //cTrace.Trace("{0} GridView_FocusedRowChanged {1} : from {2} to {3} real {4}", giGridView_SelectFocusedRow_nb++, view.Name, e.PrevFocusedRowHandle, e.FocusedRowHandle, view.FocusedRowHandle);
        }
        #endregion

        #region GridViewParent_FocusedRowChanged
        private void GridViewParent_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            // Event : FocusedRowChanged
            ChildDataUpdate();
        }
        #endregion

        #region GridViewParent_Layout
        private void GridViewParent_Layout(object sender, System.EventArgs e)
        {
            // Event : Layout
            ChildDataUpdate();
        }
        #endregion

        #region GridViewChild_InitNewRow
        private void GridViewChild_InitNewRow(object sender, DevExpress.Data.InitNewRowEventArgs e)
        {
            // Event : InitNewRow
            Child_InitNewRow();
        }
        #endregion

        #region GridView_InvalidRowException
        private static void GridView_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.Ignore;
        }
        #endregion
        #endregion

        #region SetGridOption
        public void SetGridOption()
        {
            string sOption = gGridDefinition.zAttribValue("Option");
            gGridOption = new XtraGridOption(sOption);
        }
        #endregion

        #region CreateDataSource ...
        #region CreateDataSource
        #region doc
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
        #endregion
        public void CreateDataSource()
        {
            if (gGridDefinition == null) return;

            //if (gbTrace) cTrace.StartNestedLevel("CreateDataSource");

            //string sOption = xe.zAttribValue("Option");
            //GridOption option = new GridOption(sOption);
            gbDynamicDataUpdate = gGridOption.DetailDynamic;

            IDbConnection con = null;

            string sCmdType, sCmd;
            IDbCommand cmd = null;
            string sTableDef = gGridDefinition.zAttribValue("TableDef");
            if (sTableDef != null)
            {
                //DataContainer data = dataList[sTableDef];
                DataContainer data = GetDataContainer(sTableDef);
                cmd = data.Command;
                gdt = data.DataTable;
                //gda = Ado.CreateDataAdapter(cmd);
                gda = _ado.CreateDataAdapter(cmd);
            }
            else
            {
                sCmdType = gGridDefinition.zAttribValue("CmdType", CommandType.Text.ToString());
                sCmd = gGridDefinition.zAttribValue("Cmd");
                con = GetConnection();
                //cmd = Ado.CreateCmd(con, sCmd, Ado.GetCommandType(sCmdType));
                cmd = _ado.CreateCmd(con, sCmd, _ado.GetCommandType(sCmdType));

                if (gbDynamicDataUpdate)
                {
                    string sMasterId = gGridDefinition.zAttribValue("MasterId");
                    DataTable dtParent = GetParentDataTable();
                    AddMasterIdParameter(cmd, dtParent, sMasterId);
                }

                string sTable = GetTableName();
                gdt = new DataTable(sTable);

                //gda = Ado.CreateDataAdapter(cmd);
                gda = _ado.CreateDataAdapter(cmd);

                //Ado.DataAdapter_FillSchema(gda, gdt);
                _ado.DataAdapter_FillSchema(gda, gdt);

                string sPrimaryKey = gGridDefinition.zAttribValue("PrimaryKey");
                SetPrimaryKey(gdt, sPrimaryKey);

                SetDataTableColumnOption(gdt, gGridDefinition);
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

            sCmdType = gGridDefinition.zAttribValue("UpdateCmdType", CommandType.Text.ToString());
            sCmd = gGridDefinition.zAttribValue("UpdateCmd");
            if (sCmd != null)
            {
                if (con == null) con = GetConnection();
                //gda.UpdateCommand = Ado.CreateCmd(con, sCmd, Ado.GetCommandType(sCmdType), gdt);
                gda.UpdateCommand = _ado.CreateCmd(con, sCmd, _ado.GetCommandType(sCmdType), gdt);
                gda.UpdateCommand.UpdatedRowSource = UpdateRowSource.Both;
            }

            bool bInsertCmdDefined = false;
            sCmdType = gGridDefinition.zAttribValue("InsertCmdType", CommandType.Text.ToString());
            sCmd = gGridDefinition.zAttribValue("InsertCmd");
            if (sCmd != null)
            {
                if (con == null) con = GetConnection();
                //gda.InsertCommand = Ado.CreateCmd(con, sCmd, Ado.GetCommandType(sCmdType), gdt);
                gda.InsertCommand = _ado.CreateCmd(con, sCmd, _ado.GetCommandType(sCmdType), gdt);
                gda.InsertCommand.UpdatedRowSource = UpdateRowSource.Both;
                bInsertCmdDefined = true;
            }

            sCmdType = gGridDefinition.zAttribValue("DeleteCmdType", CommandType.Text.ToString());
            sCmd = gGridDefinition.zAttribValue("DeleteCmd");
            if (sCmd != null)
            {
                if (con == null) con = GetConnection();
                //gda.DeleteCommand = Ado.CreateCmd(con, sCmd, Ado.GetCommandType(sCmdType), gdt);
                gda.DeleteCommand = _ado.CreateCmd(con, sCmd, _ado.GetCommandType(sCmdType), gdt);
                gda.DeleteCommand.UpdatedRowSource = UpdateRowSource.Both;
            }

            //object cb = Ado.CreateCommandBuilder(gda);
            object cb = _ado.CreateCommandBuilder(gda);
            //if (((!gGridOption.ReadOnly && !gGridOption.NoUpdate) || gGridOption.UpdateCommand) && gda.UpdateCommand == null) gda.UpdateCommand = Ado.CreateUpdateCommand(cb);
            if (((!gGridOption.ReadOnly && !gGridOption.NoUpdate) || gGridOption.UpdateCommand) && gda.UpdateCommand == null) gda.UpdateCommand = _ado.CreateUpdateCommand(cb);
            //if (((!gGridOption.ReadOnly && !gGridOption.NoInsert) || gGridOption.InsertCommand) && gda.InsertCommand == null) gda.InsertCommand = Ado.CreateInsertCommand(cb);
            if (((!gGridOption.ReadOnly && !gGridOption.NoInsert) || gGridOption.InsertCommand) && gda.InsertCommand == null) gda.InsertCommand = _ado.CreateInsertCommand(cb);
            //if (((!gGridOption.ReadOnly && !gGridOption.NoDelete) || gGridOption.DeleteCommand) && gda.DeleteCommand == null) gda.DeleteCommand = Ado.CreateDeleteCommand(cb);
            if (((!gGridOption.ReadOnly && !gGridOption.NoDelete) || gGridOption.DeleteCommand) && gda.DeleteCommand == null) gda.DeleteCommand = _ado.CreateDeleteCommand(cb);

            if (gda.InsertCommand != null && !bInsertCmdDefined)
            {
                //string sPrimaryKeyTrace;
                //if (dt.PrimaryKey == null) sPrimaryKeyTrace = "null"; else sPrimaryKeyTrace = dt.PrimaryKey.zToStringValues();
                //cTrace.Trace("PB_Grid.cGrid.CreateDataSource() : Table {0}, PrimaryKey {1}", dt.TableName, sPrimaryKeyTrace);

                IDbCommand cmdInsert = GetCommandWithReturn(gda.InsertCommand, gdt.PrimaryKey);
                gda.InsertCommand.Dispose();
                gda.InsertCommand = cmdInsert;
            }
            //if (sTableDef == null && !gGridOption.DetailDynamic) Ado.DataAdapter_Fill(gda, gdt);
            if (sTableDef == null && !gGridOption.DetailDynamic) _ado.DataAdapter_Fill(gda, gdt);

            //string sDataName = Xml.GetAttribValue(gGridDefinition, "def");
            //if (gbTrace) cTrace.StopNestedLevel("CreateDataSource");
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
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn colTable = dt.Columns[i];
                if (colTable.AutoIncrement)
                {
                    //cTrace.Trace("PB_Grid.cGrid.SetDataTableAutoIncrementOption() : table {0}, col {1}, AutoIncrementSeed = -1, AutoIncrementStep = -1", dt.TableName, colTable.ColumnName);
                    colTable.AutoIncrementSeed = -1;
                    colTable.AutoIncrementStep = -1;
                }
            }
        }
        #endregion

        #region SetDataTableColumnDefaultValue
        private static void SetDataTableColumnDefaultValue(DataTable dt)
        {
            foreach (DataColumn col in dt.Columns)
            {
                if (!col.AllowDBNull && !col.AutoIncrement && col.DataType == typeof(bool)) col.DefaultValue = zconvert.DefaultValue(col.DataType);
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

        #region GetTableName()
        public string GetTableName()
        {
            string sTableName = gGridDefinition.zAttribValue("Table");
            if (sTableName == null) sTableName = gGridDefinition.zAttribValue("Name");
            return sTableName;
        }
        #endregion

        #region GetParentDataTable
        private DataTable GetParentDataTable()
        {
            if (gParentGrid == null) throw new XtraGridException("error ParentGrid is not defined impossible to get parent DataTable {0}", gsName);
            if (gParentGrid.gdt == null) throw new XtraGridException("error DataTable of ParentGrid is not defined {0}", gsName);
            return gParentGrid.gdt;
        }
        #endregion

        #region GetFormName
        private string GetFormName()
        {
            if (gFormManager == null) throw new XtraGridException("error FormManager is not defined impossible to get form name {0}", gsName);
            return gFormManager.FormName;
        }
        #endregion

        #region GetConnection
        private IDbConnection GetConnection()
        {
            if (gFormManager == null) throw new XtraGridException("error FormManager is not defined impossible to get bdd connection {0}", gsName);
            if (gFormManager.Connection == null) throw new XtraGridException("error bdd connection is not defined in FormManager {0}", gsName);
            return gFormManager.Connection;
        }
        #endregion

        #region GetDataList
        private DataList GetDataList()
        {
            if (gFormManager == null) throw new XtraGridException("error FormManager is not defined impossible to get DataList {0}", gsName);
            return gFormManager.DataList;
        }
        #endregion

        #region GetDataContainer
        private DataContainer GetDataContainer(string sTableDef)
        {
            if (sTableDef == null) return null;
            if (gFormManager == null) throw new XtraGridException("error FormManager is not defined impossible to get DataContainer {0}", sTableDef);
            if (gFormManager.DataList == null) throw new XtraGridException("error DataList is not defined impossible to get DataContainer {0}", sTableDef);
            return gFormManager.DataList[sTableDef];
        }
        #endregion

        #region GetEditRepositoryList
        private SortedList<string, RepositoryItem> GetEditRepositoryList()
        {
            if (gFormManager == null) throw new XtraGridException("error FormManager is not defined impossible to get EditRepositoryList {0}", gsName);
            return gFormManager.StaticEditRepositoryList;
        }
        #endregion
        #endregion

        #region SetGridDataSource ...
        #region SetGridDataSource
        public void SetGridDataSource()
        {
            //GridSetDataSource(gGridMaster, dt, xe, con, dataList, option);
            //gdtMaster.RowDeleting += new DataRowChangeEventHandler(DataTableMaster_RowDeleting);
            //gsMasterName = sDataName;
            //LoadMasterParameters();

            //if (gbTrace) cTrace.StartNestedLevel("SetGridDataSource");

            //if (gbTrace) cTrace.StartNestedLevel("EndEvent");
            //if (grid.Tag is FormGridEvent)
            //    ((FormGridEvent)grid.Tag).End();
            //if (gbTrace) cTrace.StopNestedLevel("EndEvent");

            gGridView.CollapseAllDetails();
            //GridClearDataSource(grid);
            if (gGridOption.MasterViewMode) XtraGridTools.GridCreateDetailViews(gGridControl, gdt);

            //if (gbTrace) cTrace.StartNestedLevel("SetDataSource");
            gGridControl.DataSource = gdt;
            gGridControl.ForceInitialize();
            //if (gbTrace) cTrace.StopNestedLevel("SetDataSource");

            XtraGridTools.SetNavigatorOption(gGridControl, gGridOption);
            //GridSetViewOption((GridView)grid.MainView, xe, dt, con, dataList, option);
            GridSetViewOption(gGridView, gGridOption, gGridDefinition, gdt);

            //grid.Leave += new EventHandler(GridControl_Leave);

            if (gGridDefinition != null && gGridOption.MasterViewMode)
            {
                IEnumerator<XElement> xeDetailEnum = gGridDefinition.Elements("def").GetEnumerator();
                foreach (DataRelation r in gdt.ChildRelations)
                {
                    if (!xeDetailEnum.MoveNext()) break;
                    XElement xeDetailDefinition = xeDetailEnum.Current;
                    string sRelation = r.RelationName;
                    if (sRelation == null) break;
                    GridView view = (GridView)gGridControl.LevelDefaults[sRelation];
                    string sOption = xeDetailDefinition.zAttribValue("Option");
                    XtraGridOption option = new XtraGridOption(sOption);
                    GridSetViewOption(view, option, xeDetailDefinition, r.ChildTable);
                }
            }

            if (gGridOption.ExpandAllMasterRows) XtraGridTools.GridExpandAllMasterRows(gGridView);

            //grid.Tag = new FormGridEvent(grid, gGridDefinition);

            //if (gbTrace) cTrace.StopNestedLevel("SetGridDataSource");

        }
        #endregion

        #region GridSetViewOption
        private void GridSetViewOption(GridView view, XtraGridOption option, XElement definition, DataTable dt)
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

            //string sName = definition.zAttribValue("def");
            string sName = definition.zAttribValue("Name");
            if (sName != null) view.Name = sName;
            view.Name = sName;

            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumn***");
            string sColumn = definition.zAttribValue("Column");
            XtraGridTools.SetGridColumn(view, sColumn);

            sColumn = definition.zAttribValue("ColumnCaption");
            XtraGridTools.SetGridColumnCaption(view, sColumn);

            sColumn = definition.zAttribValue("Update");
            XtraGridTools.SetGridColumnUpdate(view, sColumn);

            sColumn = definition.zAttribValue("ReadOnly");
            XtraGridTools.SetGridColumnReadOnly(view, sColumn);

            sColumn = definition.zAttribValue("Hide");
            XtraGridTools.SetGridColumnHide(view, sColumn);
            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumn***");

            // Filter
            string sFilter = definition.zAttribValue("Filter");
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
            XtraGridTools.SetRowHeight(view, definition.zAttribValue("RowHeight"));

            Font font = XtraGridTools.GetFont(definition.zAttribValue("Font"));
            if (font != null)
            {
                view.ViewStylesInfo.Row.Font = font;
                view.ViewStylesInfo.FocusedRow.Font = font;
                view.ViewStylesInfo.FocusedCell.Font = font;
            }

            StyleOptions style;
            if (XtraGridTools.GetStyleOptions(definition.zAttribValue("StyleFocusedRow"), out style))
                view.ViewStylesInfo.FocusedRow.Options = style;
            if (XtraGridTools.GetStyleOptions(definition.zAttribValue("StyleSelectedRow"), out style))
                view.ViewStylesInfo.SelectedRow.Options = style;
            if (XtraGridTools.GetStyleOptions(definition.zAttribValue("StyleHideSelectionRow"), out style))
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
            IEnumerable<XElement> colDefinitions = null;
            if (definition != null)
            {
                colDefinitions = definition.Elements("col");
                foreach (XElement colDefinition in colDefinitions)
                {
                    string sColName = colDefinition.zAttribValue("Name");
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
                        string sOption = colDefinition.zAttribValue("Option", "");
                        string sCaption = colDefinition.zAttribValue("Caption");
                        if (sCaption != null) colGrid.Caption = sCaption;
                        XtraGridTools.SetGridColumnOption(colGrid, sOption);
                        XtraGridTools.SetGridColumnStyle(colGrid, colDefinition.zAttribValue("Style"));
                        XtraGridTools.SetGridColumnFormatDate(colGrid, colDefinition.zAttribValue("FormatDate"));
                        XtraGridTools.SetGridColumnFormatNum(colGrid, colDefinition.zAttribValue("FormatNum"));
                        //////////SetGridColumnEdit(view, con, dataList, colGrid, col.zAttribValue("Edit"), col);
                        IDbConnection con = GetConnection();
                        DataList dataList = GetDataList();
                        SortedList<string, RepositoryItem> editRepositoryList = GetEditRepositoryList();
                        XtraGridTools gridTools = new XtraGridTools(con, dataList, editRepositoryList);
                        gridTools.SetColumnEdit(colGrid, colDefinition);
                    }
                }
            }
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

            if (colDefinitions != null)
            {
                foreach (XElement col in colDefinitions)
                {
                    GridColumn colGrid = view.Columns[col.zAttribValue("Name")];
                    //XtraGridTools.SetGridColumnWidth(colGrid, col.zAttribValueInt("Width", -1));
                    XtraGridTools.SetGridColumnWidth(colGrid, col.zAttribValue("Width").zTryParseAs<int>(-1));
                }
            }

            string sSort = definition.zAttribValue("Sort");
            XtraGridTools.SetGridSort(view, sSort);

            //XtraGridTools.LoadParameters(view, definition);
            //LoadParameters(view);

            //if (gbTrace) cTrace.StopNestedLevel("GridSetViewOption");
        }
        #endregion
        #endregion

        //public void LoadParameters()
        //{
        //    if (gGridView == null || gsName == null) return;
        //    string sFormName = GetFormName();
        //    if (sFormName == null) return;
        //    XtraGridTools.LoadParameters(gGridView, sFormName, gsName);
        //}

        //public void SaveParameters()
        //{
        //    if (!gbSaveParameter || gGridView == null || gsName == null) return;
        //    string sFormName = GetFormName();
        //    if (sFormName == null) return;
        //    XtraGridTools.SaveParameters(gGridView, sFormName, gsName);
        //}

        #region SaveCurrentChangeToDataTable
        /// <summary>enregistre les données en cours de modification dans le DataTable pour la grille et toutes ses filles</summary>
        public void SaveCurrentChangeToDataTable()
        {
            XtraGridTools.SaveCurrentChangeToDataTable(gGridControl);
            foreach (XtraGridControlManager grid in gChildGrid)
                grid.SaveCurrentChangeToDataTable();
        }
        #endregion

        #region UpdateData
        /// <summary>Mise à jour des données de la grille et de ses filles dans la base</summary>
        public void UpdateData()
        {
            //if (gbTrace) cTrace.StartNestedLevel("UpdateData");

            //XtraGridTools.SaveCurrentChangeToDataTable(gGridMaster);
            //XtraGridTools.SaveCurrentChangeToDataTable(gGridDetail);
            SaveCurrentChangeToDataTable();
            //int nb = Update(gdaMaster, gdtMaster, gdaDetail, gdtDetail);
            //UpdateDeletedRows(daDetail, dtDetail);
            foreach (XtraGridControlManager grid in gChildGrid)
                grid.UpdateData_DeletedRows_WithChild();
            //Ado.DataAdapter_Update(gda, gdt);
            _ado.DataAdapter_Update(gda, gdt);
            //int nb = Ado.DataAdapter_Update(daDetail, dtDetail);
            int nb = 0;
            foreach (XtraGridControlManager grid in gChildGrid)
                nb += grid.UpdateData_WithChild();

            //if (gOptionMaster != null && gOptionMaster.AutoRefresh) Master_Refresh();
            //if (gOptionDetail != null && gOptionDetail.AutoRefresh) Detail_Refresh();
            AutoRefreshData_WithChild();

            if (nb > 0) ChildDataUpdate();

            //if (gbTrace) cTrace.StopNestedLevel("UpdateData");
        }
        #endregion

        #region RefreshData
        public void RefreshData()
        {
            if (gdt == null) return;
            if (gbDataAdapterFillInProgress)
            {
                //cTrace.Trace("RefreshData : impossible to refresh data ({0}), another refresh data is already in progress", gsName);
                return;
            }

            //cTrace.StartNestedLevel("RefreshData");

            XtraGridTools.SaveCurrentChangeToDataTable(gGridControl);
            //Ado.DataAdapter_Update(gda, gdt);
            _ado.DataAdapter_Update(gda, gdt);
            if (gdt.PrimaryKey.Length == 0)
            {
                // s'il n'y a pas de clé primaire il faut supprimer manuellement les lignes du DataTable
                //if (gdtDetail != null) gdtDetail.Rows.Clear();
                //gdtMaster.Rows.Clear();
                ClearDataTableRows_WithChild();
            }
            gbDataAdapterFillInProgress = true;
            //Ado.DataAdapter_Fill(gda, gdt);
            _ado.DataAdapter_Fill(gda, gdt);
            gbDataAdapterFillInProgress = false;

            //cTrace.StopLevel("RefreshData");
        }
        #endregion

        #region AutoRefreshData_WithChild
        public void AutoRefreshData_WithChild()
        {
            if (gGridOption != null && gGridOption.AutoRefresh) RefreshData();
            foreach (XtraGridControlManager grid in gChildGrid)
                grid.AutoRefreshData_WithChild();
        }
        #endregion

        #region ClearDataTableRows_WithChild
        public void ClearDataTableRows_WithChild()
        {
            foreach (XtraGridControlManager grid in gChildGrid)
                grid.ClearDataTableRows_WithChild();
            if (gdt != null) gdt.Rows.Clear();
        }
        #endregion

        #region UpdateData_WithChild
        public int UpdateData_WithChild()
        {
            //int nb = Ado.DataAdapter_Update(gda, gdt);
            int nb = _ado.DataAdapter_Update(gda, gdt);
            foreach (XtraGridControlManager grid in gChildGrid)
                nb += grid.UpdateData_WithChild();
            return nb;
        }
        #endregion

        #region UpdateData_DeletedRows_WithChild
        public int UpdateData_DeletedRows_WithChild()
        {
            //int nb = Ado.DataAdapter_Update_DeletedRows(gda, gdt);
            int nb = _ado.DataAdapter_Update_DeletedRows(gda, gdt);
            foreach (XtraGridControlManager grid in gChildGrid)
                nb += grid.UpdateData_DeletedRows_WithChild();
            return nb;
        }
        #endregion

        #region Child ...
        #region ChildDataUpdate
        public void ChildDataUpdate()
        {
            // un event FocusedRowChanged ou Layout a été déclenché il faut mettre à jour les grilles filles
            foreach (XtraGridControlManager grid in gChildGrid)
            {
                grid.DataUpdate();
            }
        }
        #endregion

        #region DataUpdate
        public void DataUpdate()
        {
            // un event FocusedRowChanged ou Layout a été déclenché sur le parent il faut mettre à jour la grille
            if (gbDynamicDataUpdate) DataUpdate_Query(); else DataUpdate_Filter();
        }
        #endregion

        #region DataUpdate_Filter
        private void DataUpdate_Filter()
        {
            if (gda == null) return;
            if (gParentGrid == null) throw new XtraGridException("error in Child_Filter there is no parent defined (\"{0}\")", gsName);
            XtraGridTools.SaveCurrentChangeToDataTable(gGridControl);
            object parentId = CurrentParentId;
            GridColumn col = gGridView.Columns[gsChildIdFieldName];
            if (col != null && col.FilterInfo.Value != parentId)
            {
                ColumnFilterInfo f = new ColumnFilterInfo(parentId);
                col.FilterInfo = f;
                if (parentId != null)
                {
                    if (!gGridOption.NoInsert && !gGridOption.NoInsertButton)
                        XtraGridTools.NavigatorInsertEnable(gGridControl);
                }
                else
                    XtraGridTools.NavigatorInsertDisable(gGridControl);
            }
        }
        #endregion

        #region DataUpdate_Query
        private void DataUpdate_Query()
		{
            if (gda == null) return;
            if (gbDataAdapterFillInProgress)
            {
                //cTrace.Trace("UpdateList_Query : impossible to refresh data ({0}), another refresh data is already in progress", gsName);
                return;
            }

            object parentId = CurrentParentId;
            if (gda.SelectCommand.Parameters.Count == 0) return;
            IDbDataParameter prm = (IDbDataParameter)gda.SelectCommand.Parameters[0];
            if (prm == null || prm.Value == parentId) return;
            XtraGridTools.SaveCurrentChangeToDataTable(gGridControl);
            //Ado.DataAdapter_Update(gda, gdt);
            _ado.DataAdapter_Update(gda, gdt);
            prm.Value = parentId;
			gdt.Rows.Clear();
            if (parentId == null) return;
            gbDataAdapterFillInProgress = true;
            try
            {
                //Ado.DataAdapter_Fill(gda, gdt);
                _ado.DataAdapter_Fill(gda, gdt);
            }
            catch
            {
            }
            gbDataAdapterFillInProgress = false;
		}
		#endregion

        #region Child_InitNewRow
        public void Child_InitNewRow()
        {
            if (gda != null)
            {
                object parentId = CurrentParentId;
                GridColumn col = gGridView.Columns[gsChildIdFieldName];
                gGridView.SetRowCellValue(gGridView.FocusedRowHandle, col, parentId);
            }
        }
        #endregion
        #endregion
    }

    public class XtraGridTools
    {
        #region variable
        private static bool gbTrace = false;

        private IDbConnection gCon = null;
        private DataList gDataList = null;
        private SortedList<string, RepositoryItem> gStaticEditRepositoryList = null;
        private static IAdo _ado = null;
        #endregion

        #region constructor ...
        #region XtraGridTools()
        public XtraGridTools()
        {
        }
        #endregion

        #region XtraGridTools(IDbConnection con)
        public XtraGridTools(IDbConnection con)
        {
            gCon = con;
        }
        #endregion

        #region XtraGridTools(IDbConnection con, DataList dataList)
        public XtraGridTools(IDbConnection con, DataList dataList)
        {
            gCon = con;
            gDataList = dataList;
        }
        #endregion

        #region XtraGridTools(IDbConnection con, DataList dataList, SortedList<string, RepositoryItem> editRepositoryList)
        public XtraGridTools(IDbConnection con, DataList dataList, SortedList<string, RepositoryItem> editRepositoryList)
        {
            gCon = con;
            gDataList = dataList;
            gStaticEditRepositoryList = editRepositoryList;
        }
        #endregion
        #endregion

        #region property ...
        #region Trace
        public static bool Trace
        {
            get { return gbTrace; }
            set { gbTrace = value; }
        }
        #endregion

        #region Connection
        public IDbConnection Connection
        {
            get { return gCon; }
            set { gCon = value; }
        }
        #endregion

        #region DataList
        public DataList DataList
        {
            get { return gDataList; }
            set { gDataList = value; }
        }
        #endregion

        #region StaticEditRepositoryList
        public SortedList<string, RepositoryItem> StaticEditRepositoryList
        {
            get { return gStaticEditRepositoryList; }
            set { gStaticEditRepositoryList = value; }
        }
        #endregion
        #endregion

        #region static function ...
        #region CreateGridControl ...
        #region CreateGridControl()
        public static GridControl CreateGridControl()
        {
            return CreateGridControl(null);
        }
        #endregion

        #region CreateGridControl(string sName)
        public static GridControl CreateGridControl(string sName)
        {
            GridControl grid = new GridControl();
            ((System.ComponentModel.ISupportInitialize)(grid)).BeginInit();

            grid.EmbeddedNavigator.Name = "";
            grid.Location = new System.Drawing.Point(0, 0);
            grid.Name = sName;
            grid.Size = new System.Drawing.Size(100, 100);
            //this.grid_data1.TabIndex = 0;
            //grid.Text = "Data";

            GridView view = new GridView();
            ((System.ComponentModel.ISupportInitialize)(view)).BeginInit();
            view.GridControl = grid;
            //view.Name = "gridView";

            grid.MainView = view;
            ((System.ComponentModel.ISupportInitialize)(grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(view)).EndInit();

            return grid;
        }
        #endregion
        #endregion

        #region SaveCurrentChangeToDataTable
        public static void SaveCurrentChangeToDataTable(GridControl grid)
        {
            if (grid != null)
            {
                //Save the latest changes to the bound DataTable
                ColumnView view = (ColumnView)grid.KeyboardFocusView;

                //cTrace.StartNestedLevel("CloseEditor");
                view.CloseEditor();
                //cTrace.StopLevel("CloseEditor");

                //cTrace.StartNestedLevel("UpdateCurrentRow");
                view.UpdateCurrentRow();
                //cTrace.StopLevel("UpdateCurrentRow");
            }
        }
        #endregion

        #region Load and Save parameters ...
        //public static void LoadParameters(GridView view, XElement xe)
        //{
        //    string sPrmFile = xe.zAttribValue("PrmFile");
        //    string sPrmName = xe.zAttribValue("Prm");
        //    if (sPrmName == null) sPrmName = xe.zAttribValue("Name");
        //    LoadParameters(view, sPrmFile, sPrmName);
        //}

        //public static void LoadParameters(GridView view, string sFormName, string sGridName)
        //{
        //    if (sFormName == null) return;
        //    if (sGridName == null) sGridName = view.Name;
        //    if (sGridName == null) return;
        //    string sParameterName = sFormName + "_" + sGridName;

        //    XmlParameters_v1 xp = new XmlParameters_v1(sParameterName);
        //    LoadColumnWidth(xp, view, sGridName);
        //}

        //public static void SaveParameters(GridView view, XElement xe)
        //{
        //    string sPrmFile = xe.zAttribValue("PrmFile");
        //    string sPrmName = xe.zAttribValue("Prm");
        //    if (sPrmName == null) sPrmName = xe.zAttribValue("Name");

        //    SaveParameters(view, sPrmFile, sPrmName);
        //}

        //public static void SaveParameters(GridView view, string sFormName)
        //{
        //    SaveParameters(view, sFormName, null);
        //}

        //public static void SaveParameters(GridView view, string sFormName, string sGridName)
        //{
        //    if (sFormName == null) return;
        //    if (sGridName == null) sGridName = view.Name;
        //    if (sGridName == null) return;
        //    string sParameterName = sFormName + "_" + sGridName;

        //    XmlParameters_v1 xp = new XmlParameters_v1(sParameterName);
        //    SaveColumnWidth(xp, view, sGridName);
        //    foreach (BaseView v in view.GridControl.LevelDefaults.Values)
        //    {
        //        if (!(v is GridView)) continue;
        //        GridView gv = (GridView)v;
        //        SaveColumnWidth(xp, gv, gv.Name);
        //    }

        //    xp.Save();
        //}

        //public static void LoadColumnWidth(XmlParameters_v1 xp, GridView view, string sGrid)
        //{
        //    if (sGrid == null) return;
        //    sGrid += "_";
        //    foreach (GridColumn col in view.Columns)
        //    {
        //        object oWidth = xp.Get(sGrid + col.FieldName);
        //        if (oWidth != null && oWidth is int) XtraGridTools.SetGridColumnWidth(col, (int)oWidth);
        //    }
        //}

        //public static void SaveColumnWidth(XmlParameters_v1 xp, GridView view, string sGrid)
        //{
        //    if (sGrid == null) return;

        //    sGrid += "_";
        //    foreach (GridColumn col in view.Columns)
        //    {
        //        xp.Set(sGrid + col.FieldName, col.Width);
        //    }
        //}
        #endregion

        #region ClearGrid ...
        #region ClearGrid(GridControl grid)
        public static void ClearGrid(GridControl grid)
        {
            ClearGrid(grid, null);
        }
        #endregion

        #region ClearGrid(GridControl grid, SortedList<string, RepositoryItem> staticEditRepository)
        public static void ClearGrid(GridControl grid, SortedList<string, RepositoryItem> staticEditRepository)
        {
            if (grid == null) return;

            grid.DataSource = null;

            //if (gbTrace) cTrace.StartNestedLevel("Columns_Clear");
            GridView gridView = (GridView)grid.MainView;
            gridView.Columns.Clear();
            //if (gbTrace) cTrace.StopNestedLevel("Columns_Clear");

            //if (gbTrace) cTrace.StartNestedLevel("RepositoryItems_Clear");
            //************** ATTENTION *********************************************************************
            // pour pouvoir réutiliser des RepositoryItem il ne faut pas faire grid.RepositoryItems.Clear();
            // sinon plantage dans le code DevExpress
            //**********************************************************************************************
            if (staticEditRepository == null)
                grid.RepositoryItems.Clear();
            else
            {
                for (int i = grid.RepositoryItems.Count - 1; i >= 0; i--)
                {
                    RepositoryItem item = grid.RepositoryItems[i];
                    if (!staticEditRepository.ContainsKey(item.Name))
                        grid.RepositoryItems.RemoveAt(i);
                }
            }
            //if (gbTrace) cTrace.StopNestedLevel("RepositoryItems_Clear");

            //if (gbTrace) cTrace.StartNestedLevel("View_Dispose");
            for (int i = grid.Views.Count - 1; i >= 1; i--)
            {
                BaseView baseView = grid.Views[i];
                baseView.Dispose();
            }
            foreach (BaseView view in grid.LevelDefaults.Values) view.Dispose();
            //if (gbTrace) cTrace.StopNestedLevel("View_Dispose");

            //if (gbTrace) cTrace.StartNestedLevel("LevelDefaults_Clear");
            grid.LevelDefaults.Clear();
            //if (gbTrace) cTrace.StopNestedLevel("LevelDefaults_Clear");
        }
        #endregion
        #endregion

        #region NavigatorOption ...
        #region NavigatorInsertEnable
        public static void NavigatorInsertEnable(GridControl grid)
        {
            ((GridView)grid.MainView).OptionsView.ShowNewItemRow = true;
            grid.EmbeddedNavigator.Buttons.Append.Enabled = true;
        }
        #endregion

        #region NavigatorInsertDisable
        public static void NavigatorInsertDisable(GridControl grid)
        {
            XtraGridTools.SaveCurrentChangeToDataTable(grid);
            ((GridView)grid.MainView).OptionsView.ShowNewItemRow = false;
            grid.EmbeddedNavigator.Buttons.Append.Enabled = false;
        }
        #endregion

        #region SetNavigatorOption
        public static void SetNavigatorOption(GridControl grid, XtraGridOption option)
        {
            //if (gbTrace) cTrace.StartNestedLevel("GridSetNavigatorOption");

            grid.UseEmbeddedNavigator = !option.NoNavigator;

            grid.EmbeddedNavigator.Buttons.Append.Enabled = false;
            grid.EmbeddedNavigator.Buttons.Edit.Enabled = false;
            grid.EmbeddedNavigator.Buttons.EndEdit.Enabled = false;
            grid.EmbeddedNavigator.Buttons.CancelEdit.Enabled = false;
            grid.EmbeddedNavigator.Buttons.Remove.Enabled = false;

            if (!option.ReadOnly)
            {
                if (!option.NoUpdate)
                {
                    grid.EmbeddedNavigator.Buttons.Edit.Enabled = true;
                    grid.EmbeddedNavigator.Buttons.EndEdit.Enabled = true;
                    grid.EmbeddedNavigator.Buttons.CancelEdit.Enabled = true;
                }
                if (!option.NoInsert && !option.NoInsertButton)
                {
                    grid.EmbeddedNavigator.Buttons.Append.Enabled = true;
                }
                if (!option.NoDelete && !option.NoDeleteButton)
                {
                    grid.EmbeddedNavigator.Buttons.Remove.Enabled = true;
                    if (!option.NoDeleteSelected)
                    {
                        grid.EmbeddedNavigator.ButtonClick += new NavigatorButtonClickEventHandler(XtraGridTools.EmbeddedNavigator_DeleteSelection);
                    }
                }
            }

            grid.EmbeddedNavigator.NavigatorException += new NavigatorExceptionEventHandler(XtraGridTools.EmbeddedNavigator_NavigatorException);

            //if (gbTrace) cTrace.StopNestedLevel();
        }
        #endregion

        #region EmbeddedNavigator_DeleteSelection
        /// <summary>supprime tous les enregistrements sélectionnés</summary>
        public static void EmbeddedNavigator_DeleteSelection(object sender, NavigatorButtonClickEventArgs e)
        {
            GridControlNavigator nav = (GridControlNavigator)sender;
            GridControl grid = (GridControl)nav.Parent;
            if (e.Button.ButtonType == NavigatorButtonType.Remove && !e.Handled)
            {
                GridView gridView = (GridView)grid.MainView;
                int[] iSelect = gridView.GetSelectedRows();
                if (iSelect != null)
                {
                    for (int i = 0; i < iSelect.Length; i++)
                    {
                        DataRow row = gridView.GetDataRow(iSelect[i]);
                        if (row != null) DeleteRow(row);
                    }
                }
                else
                {
                    DataRow row = gridView.GetDataRow(gridView.FocusedRowHandle);
                    if (row != null) DeleteRow(row);
                }
                e.Handled = true;
            }
        }
        #endregion

        #region DeleteRow
        private static void DeleteRow(DataRow row)
        {
            try
            {
                row.Delete();
            }
            catch (Exception ex)
            {
                string s = Error.GetErrorMessage(ex, false, false);
                MessageBox.Show(s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region EmbeddedNavigator_NavigatorException
        public static void EmbeddedNavigator_NavigatorException(object sender, NavigatorExceptionEventArgs e)
        {
            //cTrace.Trace("EmbeddedNavigator_NavigatorException");
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            string s = Error.GetErrorMessage(e.Exception, false, false);
            MessageBox.Show(s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
        #endregion

        #region GridCreateDetailViews
        public static void GridCreateDetailViews(GridControl grid, DataTable dt)
        {
            foreach (DataRelation r in dt.ChildRelations)
            {
                string sRelation = r.RelationName;
                if (sRelation == null) break;
                if (grid.LevelDefaults.Contains(sRelation))
                {
                    GridView oldView = (GridView)grid.LevelDefaults[sRelation];
                    grid.LevelDefaults.Remove(sRelation);
                    oldView.Dispose();
                }
                GridView view = new GridView();
                grid.LevelDefaults.Add(sRelation, view);
            }
        }
        #endregion

        #region GridExpandAllMasterRows
        public static void GridExpandAllMasterRows(GridView view)
        {
            for (int i = 0; i < view.RowCount; i++)
                view.SetMasterRowExpanded(i, true);
        }
        #endregion

        #region SetGridSort
        public static void SetGridSort(GridView view, string sSort)
        {
            //if (gbTrace) cTrace.StartNestedLevel("SetGridSort");

            ColumnView colView = (ColumnView)view;
            view.ClearSorting();
            if (sSort == null) goto fin;
            colView.BeginSort();
            string[] sSortCol = zsplit.Split(sSort, ',', true);
            for (int i = 0; i < sSortCol.Length; i++)
            {
                ColumnSortOrder so = ColumnSortOrder.Ascending;
                string s = sSortCol[i].ToLower();
                if (s.EndsWith(" asc"))
                    s = zstr.left(s, s.Length - 4).TrimEnd();
                else if (s.EndsWith(" desc"))
                {
                    s = zstr.left(s, s.Length - 5).TrimEnd();
                    so = ColumnSortOrder.Descending;
                }
                GridColumn colGrid = view.Columns[s];
                if (colGrid != null) colGrid.SortOrder = so;
            }
            colView.EndSort();

        fin:;
            //if (gbTrace) cTrace.StopNestedLevel("SetGridSort");
        }
        #endregion

        #region SetGridFilter
        /// <summary>Defini le filtre des données</summary>
        /// <param name="sFilter">Filtre des données "column : filter, column : filter, ..."</param>
        public static void SetGridFilter(GridView view, string sFilter)
        {
            if (sFilter == null) return;

            //if (gbTrace) cTrace.StartNestedLevel("SetGridFilter");

            char[,] cCharZone = new char[,] { { '"', '"' }, { '\'', '\'' } };
            foreach (string sFilterCol in zsplit.Split(sFilter, ',', cCharZone, true))
            {
                string[] sValues = zsplit.Split(sFilterCol, ':', cCharZone, true);
                GridColumn col = view.Columns[sValues[0]];
                if (col == null) continue;
                col.FilterInfo = new ColumnFilterInfo(sValues[1], sValues[1]);
            }

            //if (gbTrace) cTrace.StopNestedLevel("SetGridFilter");
        }
        #endregion

        #region SetGridColumn
        /// <summary>Défini les colonnes visibles</summary>
        /// <param name="sColumn">liste des colonnes visibles</param>
        public static void SetGridColumn(GridView view, string sColumn)
        {
            int i;
            char[,] cCharZone = { { '"', '"' }, { '\'', '\'' } };
            string[] sColumns, sNames;
            GridColumn colGrid;

            if (sColumn == null) return;
            sColumns = zsplit.Split(sColumn, ',');
            foreach (GridColumn colGrid2 in view.Columns) colGrid2.VisibleIndex = -1;

            i = 0;
            foreach (string sCol in sColumns)
            {
                sNames = zsplit.Split(sCol.TrimStart(), ' ', cCharZone, true, true, false);
                if (sNames.Length == 0) continue;
                colGrid = view.Columns[sNames[0]];
                if (colGrid != null)
                {
                    colGrid.VisibleIndex = i++;
                    if (sNames.Length >= 2) colGrid.Caption = sNames[1];
                }
            }
        }
        #endregion

        #region GetGridColumnsOrder
        public static string GetGridColumnsOrder(GridView view)
        {
            string sCol;
            GridColumn col;

            sCol = "";
            for (int i = 0; i < view.VisibleColumns.Count; i++)
            {
                col = view.VisibleColumns[i];
                if (i != 0) sCol += ", ";
                sCol += col.FieldName;
            }
            return sCol;
        }
        #endregion

        #region SetGridColumnCaption
        /// <summary>Defini les titres des colones</summary>
        /// <param name="sColumnCaption">Liste des titres des colones "column caption, column caption, ..."</param>
        public static void SetGridColumnCaption(GridView view, string sColumnCaption)
        {
            char[,] cCharZone = { { '"', '"' }, { '\'', '\'' } };

            if (sColumnCaption == null) return;
            string[] sColumns = zsplit.Split(sColumnCaption, ',');

            foreach (string sCol in sColumns)
            {
                string[] sNames = zsplit.Split(sCol.TrimStart(), ' ', cCharZone, true, true, false);
                if (sNames.Length == 0) continue;
                GridColumn colGrid = view.Columns[sNames[0]];
                if (colGrid != null && sNames.Length >= 2)
                    colGrid.Caption = sNames[1];
            }
        }
        #endregion

        #region SetGridColumnUpdate
        /// <summary>Défini la liste des colonnes qui peuvent être mis à jour, toutes les autres colonnes sont en ReadOnly</summary>
        /// <param name="sColumn">Liste des colonnes qui peuvent être mis à jour "column, column, ..."</param>
        public static void SetGridColumnUpdate(GridView view, string sColumn)
        {
            if (sColumn == null) return;
            string[] sColumns = zsplit.Split(sColumn, ',', true);

            bool[] bUpdateCol = new bool[view.Columns.Count];
            for (int i = 0; i < view.Columns.Count; i++) bUpdateCol[i] = false;
            foreach (string sCol in sColumns)
            {
                GridColumn col = view.Columns[sCol];
                if (col != null) bUpdateCol[col.AbsoluteIndex] = true;
            }
            for (int i = 0; i < view.Columns.Count; i++)
            {
                if (!bUpdateCol[i]) view.Columns[i].Options |= ColumnOptions.ReadOnly;
            }
        }
        #endregion

        #region SetGridColumnReadOnly
        /// <summary>Défini la liste des colonnes qui sont en ReadOnly</summary>
        /// <param name="sColumn">Liste des colonnes qui sont en ReadOnly "column, column, ..."</param>
        public static void SetGridColumnReadOnly(GridView view, string sColumn)
        {
            if (sColumn == null) return;
            string[] sColumns = zsplit.Split(sColumn, ',', true);

            foreach (string sCol in sColumns)
            {
                GridColumn col = view.Columns[sCol];
                if (col != null) col.Options |= ColumnOptions.ReadOnly;
            }
        }
        #endregion

        #region SetGridColumnHide
        /// <summary>Défini la liste des colonnes cachées</summary>
        /// <param name="sColumn">Liste des colonnes cachées "column, column, ..."</param>
        public static void SetGridColumnHide(GridView view, string sColumn)
        {
            if (sColumn == null) return;
            string[] sColumns = zsplit.Split(sColumn, ',', true);

            foreach (string sCol in sColumns)
            {
                GridColumn col = view.Columns[sCol];
                if (col != null) col.VisibleIndex = -1;
            }
        }
        #endregion

        #region SetGridColumnOption
        /// <summary>Défini une colonne ReadOnly et/ou Hide</summary>
        /// <param name="sOption">"ReadOnly, Hide"</param>
        public static void SetGridColumnOption(GridColumn col, string sOption)
        {
            if (col == null || sOption == null || sOption == "") return;

            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnOption");

            string[] sOptions = zsplit.Split(sOption.ToLower(), ',', true);
            for (int i = 0; i < sOptions.Length; i++)
            {
                switch (sOptions[i])
                {
                    case "readonly":
                        col.Options |= ColumnOptions.ReadOnly;
                        break;
                    case "hide":
                        col.VisibleIndex = -1;
                        break;
                }
            }

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnOption");
        }
        #endregion

        #region SetRowHeight
        /// <summary>Défini la hauteur des lignes</summary>
        /// <param name="sRowHeight">hauteur des lignes</param>
        public static void SetRowHeight(GridView view, string sRowHeight)
        {
            int rowHeight;
            if (sRowHeight == null || sRowHeight == "") return;
            try
            {
                rowHeight = int.Parse(sRowHeight, NumberStyles.Any);
            }
            catch
            {
                throw new XtraGridException("Error in row height definition \"{0}\"", sRowHeight);
            }
            view.RowHeight = rowHeight;
        }
        #endregion

        #region GetFont
        /// <summary>Création d'une police de caractère</summary>
        /// <param name="sFont">Définition de la police de caractère "font name, size, Bold, Italic, Strikeout, Underline"</param>
        public static Font GetFont(string sFont)
        {
            float size;
            Font font;

            //"font name, size, Bold, Italic, Strikeout, Underline"
            string sErr = "";
            if (sFont == null || sFont == "") return null;
            string[] s = zsplit.Split(sFont.ToLower(), ',', true);
            try
            {
                size = float.Parse(s[1], NumberStyles.Any, NumberFormatInfo.InvariantInfo);
            }
            catch
            {
                sErr = "size is not correct";
                goto err;
            }
            FontStyle style = FontStyle.Regular;
            for (int i = 2; i < s.Length; i++)
            {
                switch (s[i].ToLower())
                {
                    case "bold":
                        style |= FontStyle.Bold;
                        break;
                    case "italic":
                        style |= FontStyle.Italic;
                        break;
                    case "strikeout":
                        style |= FontStyle.Strikeout;
                        break;
                    case "underline":
                        style |= FontStyle.Underline;
                        break;
                }
            }
            try
            {
                font = new Font(s[0], size, style);
            }
            catch (Exception ex)
            {
                sErr = ex.Message;
                goto err;
            }
            return font;

        err:
            throw new XtraGridException("Error in font definition \"{0}\" {1}", sFont, sErr);
        }
        #endregion

        #region SetGridColumnStyle
        /// <summary>Défini le style d'une colonne</summary>
        /// <param name="sStyle">style de la colonne "HAlignNear, HAlignCenter, HAlignFar, VAlignTop, VAlignCenter, VAlignBottom"</param>
        public static void SetGridColumnStyle(GridColumn col, string sStyle)
        {
            if (col == null || sStyle == null || sStyle == "") return;

            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnStyle");

            ViewStyle style = new ViewStyle();
            string[] sStyles = zsplit.Split(sStyle.ToLower(), ',', true);
            bool bStyle = false;
            for (int i = 0; i < sStyles.Length; i++)
            {
                switch (sStyles[i])
                {
                    case "halignnear":
                        bStyle = true;
                        style.HAlignment = HorzAlignment.Near;
                        break;
                    case "haligncenter":
                        bStyle = true;
                        style.HAlignment = HorzAlignment.Center;
                        break;
                    case "halignfar":
                        bStyle = true;
                        style.HAlignment = HorzAlignment.Far;
                        break;
                    case "valigntop":
                        bStyle = true;
                        style.VAlignment = VertAlignment.Top;
                        break;
                    case "valigncenter":
                        bStyle = true;
                        style.VAlignment = VertAlignment.Center;
                        break;
                    case "valignbottom":
                        bStyle = true;
                        style.VAlignment = VertAlignment.Bottom;
                        break;
                }
            }
            if (bStyle)
            {
                string sStyleName = col.Name;
                int i = 2;
                while (col.View.GridControl.Styles.Contains(sStyleName))
                {
                    sStyleName = col.Name + i.ToString();
                    i++;
                }
                style.StyleName = sStyleName;
                col.View.GridControl.Styles.Add(sStyleName, style);
                col.Style = style;
            }

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnStyle");
        }
        #endregion

        #region SetGridColumnFormatDate
        /// <summary>Définition du format de date de la colonne</summary>
        /// <param name="sFormat">Format de date (.net format)</param>
        public static void SetGridColumnFormatDate(GridColumn col, string sFormat)
        {
            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnFormatDate");

            if (col != null && sFormat != null)
            {
                col.DisplayFormat.FormatType = FormatType.DateTime;
                col.DisplayFormat.FormatString = sFormat;
                //col.DisplayFormat.Format = gCulture;
                col.DisplayFormat.Format = Thread.CurrentThread.CurrentCulture;
            }

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnFormatDate");
        }
        #endregion

        #region SetGridColumnFormatNum
        /// <summary>Défini le format numérique de la colonne</summary>
        /// <param name="sFormat">Format numérique (.net format)</param>
        public static void SetGridColumnFormatNum(GridColumn col, string sFormat)
        {
            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnFormatNum");

            if (col != null && sFormat != null)
            {
                col.DisplayFormat.FormatType = FormatType.Numeric;
                col.DisplayFormat.FormatString = sFormat;
                //col.DisplayFormat.Format = gCulture;
                col.DisplayFormat.Format = Thread.CurrentThread.CurrentCulture;
            }

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnFormatNum");
        }
        #endregion

        #region SetGridColumnsDefaultEdit
        public static void SetGridColumnsDefaultEdit(GridView view)
        {
            int i;
            GridColumn colGrid;

            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnsDefaultEdit");

            for (i = 0; i < view.Columns.Count; i++)
            {
                colGrid = view.Columns[i];
                if (colGrid.ColumnEdit == null)
                {
                    switch (Type.GetTypeCode(colGrid.ColumnType))
                    {
                        case TypeCode.String:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Byte:
                        case TypeCode.SByte:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            SetGridColumnsDefaultEdit_txt(view, colGrid);
                            break;
                        case TypeCode.Boolean:
                            SetGridColumnsDefaultEdit_check(view, colGrid);
                            break;
                    }
                }
            }

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnsDefaultEdit");
        }
        #endregion

        #region SetGridColumnsDefaultEdit_txt
        public static void SetGridColumnsDefaultEdit_txt(GridView view, GridColumn colGrid)
        {
            RepositoryItemTextEdit edit_txt;

            edit_txt = (RepositoryItemTextEdit)view.GridControl.RepositoryItems["PKT_Grid_DefaultEdit_txt"];
            if (edit_txt == null)
            {
                //if (gbTrace) cTrace.StartNestedLevel("Create_DefaultEdit_Txt");

                edit_txt = new RepositoryItemTextEdit();
                edit_txt.Name = "PKT_Grid_DefaultEdit_txt";
                ((System.ComponentModel.ISupportInitialize)edit_txt).BeginInit();
                view.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { edit_txt });
                //edit_txt.AutoHeight = false;
                edit_txt.ParseEditValue += new ConvertEditValueEventHandler(DefaultTextEdit_ParseEditValue);
                ((System.ComponentModel.ISupportInitialize)edit_txt).EndInit();

                //if (gbTrace) cTrace.StopNestedLevel("Create_DefaultEdit_Txt");
            }

            //if (gbTrace) cTrace.StartNestedLevel("Set_DefaultEdit_Txt");
            colGrid.ColumnEdit = edit_txt;
            //if (gbTrace) cTrace.StopNestedLevel("Set_DefaultEdit_Txt");
        }
        #endregion

        #region DefaultTextEdit_ParseEditValue
        private static void DefaultTextEdit_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            if (e.Value != null && e.Value.ToString() == string.Empty)
            {
                e.Value = DBNull.Value;
                e.Handled = true;
            }
        }
        #endregion

        #region SetGridColumnsDefaultEdit_check
        public static void SetGridColumnsDefaultEdit_check(GridView view, GridColumn colGrid)
        {
            //if (gbTrace) cTrace.StartNestedLevel("Set_DefaultEdit_Check");

            DataTable dt;
            DataColumn col;
            RepositoryItemCheckEdit edit_check;

            edit_check = new RepositoryItemCheckEdit();
            edit_check.Name = "PKT_Grid_Check" + colGrid.FieldName;
            ((System.ComponentModel.ISupportInitialize)edit_check).BeginInit();
            view.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { edit_check });
            edit_check.AllowGrayed = true;
            if (view.GridControl.DataSource is DataTable)
            {
                dt = (DataTable)view.GridControl.DataSource;
                if (dt.Columns.Contains(colGrid.FieldName))
                {
                    col = dt.Columns[colGrid.FieldName];
                    if (!col.AllowDBNull)
                    {
                        if (col.DefaultValue == null) col.DefaultValue = false;
                        edit_check.AllowGrayed = false;
                    }
                }
            }
            //edit_check.AutoHeight = false;
            ((System.ComponentModel.ISupportInitialize)edit_check).EndInit();
            colGrid.ColumnEdit = edit_check;

            //if (gbTrace) cTrace.StopNestedLevel("Set_DefaultEdit_Check");
        }
        #endregion

        #region SetGridColumnWidth
        public static void SetGridColumnWidth(GridColumn col, int iWidth)
        {
            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnWidth");

            if (col != null && iWidth != -1)
            {
                if (col.MinWidth > iWidth) col.MinWidth = iWidth;
                col.Width = iWidth;
            }

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnWidth");
        }
        #endregion

        #region GetStyleOptions
        /// <summary>Définition d'un StyleOptions</summary>
        /// <param name="sStyle">Définition du style "None, StyleEnabled, UseBackColor, UseDrawEndEllipsis, UseDrawFocusRect, UseFont, UseForeColor, UseHorzAlignment, UseImage, UseVertAlignment, UseWordWrap"</param>
        /// <returns>true si le style a été défini</returns>
        public static bool GetStyleOptions(string sStyle, out StyleOptions style)
        {
            bool bStyle = false;

            style = StyleOptions.None;

            if (sStyle == null) return false;
            string[] sStyles = zsplit.Split(sStyle, ',', true);
            foreach (string s in sStyles)
            {
                switch (s.ToLower())
                {
                    case "none":
                        bStyle = true;
                        break;
                    case "styleenabled":
                        bStyle = true;
                        style |= StyleOptions.StyleEnabled;
                        break;
                    case "usebackcolor":
                        bStyle = true;
                        style |= StyleOptions.UseBackColor;
                        break;
                    case "usedrawendellipsis":
                        bStyle = true;
                        style |= StyleOptions.UseDrawEndEllipsis;
                        break;
                    case "usedrawfocusrect":
                        bStyle = true;
                        style |= StyleOptions.UseDrawFocusRect;
                        break;
                    case "usefont":
                        bStyle = true;
                        style |= StyleOptions.UseFont;
                        break;
                    case "useforecolor":
                        bStyle = true;
                        style |= StyleOptions.UseForeColor;
                        break;
                    case "usehorzalignment":
                        bStyle = true;
                        style |= StyleOptions.UseHorzAlignment;
                        break;
                    case "useimage":
                        bStyle = true;
                        style |= StyleOptions.UseImage;
                        break;
                    case "usevertalignment":
                        bStyle = true;
                        style |= StyleOptions.UseVertAlignment;
                        break;
                    case "usewordwrap":
                        bStyle = true;
                        style |= StyleOptions.UseWordWrap;
                        break;
                }
            }
            return bStyle;
        }
        #endregion
        #endregion

        #region SetColumnEdit ...
        #region SetColumnEdit(GridColumn col, XElement colDefinition)
        public void SetColumnEdit(GridColumn col, XElement colDefinition)
        {
            if (col == null) return;
            //string sEdit = colDefinition.zAttribValue("Edit");
            //if (sEdit == null || sEdit == "") return;
            // modif le 27/03/2013 column edit Mask par defaut pour pouvoir afficher les valeurs nulles --null--
            string sEdit = colDefinition.zAttribValue("Edit", "Mask");
            switch (sEdit.ToLower())
            {
                case "lookup":
                    SetColumnEditLookUp(col, colDefinition);
                    break;
                case "combo":
                    SetGridColumnEditCombo(col, colDefinition);
                    break;
                case "mask":
                    SetGridColumnEditMask(col, colDefinition);
                    break;
                case "memo":
                    SetGridColumnEditMemo(col);
                    break;
                case "heuresec":
                case "second":
                    SetGridColumnEditSecond(col, colDefinition);
                    break;
            }
        }
        #endregion

        #region SetColumnEditLookUp ...
        #region SetColumnEditLookUp
        public void SetColumnEditLookUp(GridColumn col, XElement colDefinition)
        {
            //GridView view, IDbConnection con, DataList dataList, 
            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnEditLookUp");

            string sLookUpName = colDefinition.zAttribValue("LookUpName");
            if (sLookUpName != null && gStaticEditRepositoryList != null && gStaticEditRepositoryList.ContainsKey(sLookUpName))
            {
                //cTrace.Trace("Use LookUp Repository : {0}", sLookUpName);
                RepositoryItem item = gStaticEditRepositoryList[sLookUpName];
                col.ColumnEdit = item;
                goto fin;
            }

            bool bReadOnlyDataTable;
            DataTable dt = null;
            string sTableDef = colDefinition.zAttribValue("TableDef");
            if (sTableDef != null)
            {
                dt = gDataList[sTableDef].DataTable;
                bReadOnlyDataTable = true;
            }
            else
            {
                string sCmd = colDefinition.zAttribValue("Cmd");
                if (sCmd != null) // les valeurs du lookup viennent de la base
                {
                    //if (gbTrace) cTrace.StartNestedLevel("LoadData");
                    //dt = Ado.ExeCmd(gCon, sCmd);
                    dt = _ado.ExeCmd(gCon, sCmd);
                    //if (gbTrace) cTrace.StopNestedLevel("LoadData");
                }
                else // sinon les valeurs du lookup sont la table fille "value"
                {
                    dt = colDefinition.zXmlToDataTable("value");
                    //dt = zdt.ChangeColumnType(dt, col.FieldName, col.ColumnType);
                    dt = dt.zChangeColumnType(col.FieldName, col.ColumnType);
                }
                bReadOnlyDataTable = false;
            }

            RepositoryItemLookUpEdit look = CreateEditLookUp(col.FieldName, colDefinition, dt, bReadOnlyDataTable);
            //view.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { look });
            col.View.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { look });
            col.ColumnEdit = look;

            if (sLookUpName != null && gStaticEditRepositoryList != null)
            {
                //cTrace.Trace("Add LookUp Repository : {0}", sLookUpName);
                gStaticEditRepositoryList.Add(sLookUpName, col.ColumnEdit);
            }

        fin:;
            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnEditLookUp");
        }
        #endregion

        #region CreateEditLookUp
        public static RepositoryItemLookUpEdit CreateEditLookUp(string sFieldName, XElement colDefinition, DataTable dt, bool bReadOnlyDataTable)
        {
            //if (col == null) return;

            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnEditLookUp2");

            RepositoryItemLookUpEdit look = new RepositoryItemLookUpEdit();
            string sLookUpName = colDefinition.zAttribValue("LookUpName");
            //if (sLookUpName == null) sLookUpName = "PKT_Grid_LookUp_" + col.FieldName;
            if (sLookUpName == null) sLookUpName = "PKT_Grid_LookUp_" + sFieldName;
            look.Name = sLookUpName;
            ((System.ComponentModel.ISupportInitialize)look).BeginInit();
            //view.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { look });
            look.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            //col.ColumnEdit = look;
            look.AutoHeight = false;

            look.NullText = "";
            string sNullText = colDefinition.zAttribValue("NullText");
            if (sNullText != null) look.NullText = sNullText;

            //if (gbTrace) cTrace.StartNestedLevel("SetDataSource");
            look.DataSource = dt;
            look.PopulateColumns();
            if (look.Columns.Count == 0)
                throw new GridException("Error in lookup \"{0}\" no column in data source", sFieldName);
            //if (gbTrace) cTrace.StopNestedLevel("SetDataSource");

            //if (gbTrace) cTrace.StartNestedLevel("SetLookUpGridColumn");
            string sColumn = colDefinition.zAttribValue("Column");
            SetLookUpGridColumn(look, sColumn);
            //if (gbTrace) cTrace.StopNestedLevel("SetLookUpGridColumn");

            //if (gbTrace) cTrace.StartNestedLevel("BestFit");
            look.BestFit();
            //if (gbTrace) cTrace.StopNestedLevel("BestFit");

            //if (gbTrace) cTrace.StartNestedLevel("DisplayMember");
            // définition de look.DisplayMember
            string sDisplay = colDefinition.zAttribValue("Display");
            if (sDisplay != null)
            {
                if (look.Columns[sDisplay] != null)
                    look.DisplayMember = sDisplay;
                else
                    throw new GridException("Error in lookup \"{0}\" no display column, column {0} does'nt exist in data source", sFieldName, sDisplay);
            }
            else
            {
                look.DisplayMember = look.Columns[0].FieldName;
                for (int i = 0; i < look.Columns.Count; i++)
                    if (look.Columns[i].FieldName != sFieldName) { look.DisplayMember = look.Columns[i].FieldName; break; }
            }
            //if (gbTrace) cTrace.StopNestedLevel("DisplayMember");

            if (look.DisplayMember != sFieldName)
            {
                if (look.Columns[sFieldName] != null)
                    look.Columns[sFieldName].Visible = false;
            }

            //if (gbTrace) cTrace.StartNestedLevel("ValueMember");
            // définition de look.ValueMember
            string sValue = colDefinition.zAttribValue("Value");
            if (sValue != null)
            {
                if (look.Columns[sValue] != null)
                    look.ValueMember = sValue;
                else
                    throw new GridException("Error in lookup \"{0}\" no value column, column {0} does'nt exist in data source", sFieldName, sValue);
            }
            else
            {
                if (look.Columns[sFieldName] != null)
                    look.ValueMember = sFieldName;
                else
                    look.ValueMember = look.DisplayMember;
            }
            //if (gbTrace) cTrace.StopNestedLevel("ValueMember");

            //if (gbTrace) cTrace.StartNestedLevel("AddNullRow");
            string sAddNullRow = colDefinition.zAttribValue("AddNullRow");
            if (!bReadOnlyDataTable && sAddNullRow != null && sAddNullRow.ToLower() == "true")
            {
                DataRow dataRow = dt.NewRow();
                if (look.DisplayMember != look.ValueMember) dataRow[look.DisplayMember] = look.NullText;
                dt.Rows.InsertAt(dataRow, 0);
            }
            //if (gbTrace) cTrace.StopNestedLevel("AddNullRow");

            //look.TextEditStyle = TextEditStyles.Standard;
            look.SearchMode = SearchMode.OnlyInPopup;
            ((System.ComponentModel.ISupportInitialize)look).EndInit();

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnEditLookUp2");

            return look;
        }
        #endregion

        #region SetLookUpGridColumn
        public static void SetLookUpGridColumn(RepositoryItemLookUpEdit look, string sColumn)
        {
            char[,] cCharZone = { { '"', '"' }, { '\'', '\'' } };
            string[] sColumns, sNames;
            LookUpColumnInfo col;

            if (sColumn == null) return;
            sColumns = zsplit.Split(sColumn, ',');
            foreach (LookUpColumnInfo col2 in look.Columns) col2.Visible = false;

            foreach (string sCol in sColumns)
            {
                sNames = zsplit.Split(sCol.TrimStart(), ' ', cCharZone, true, true, false);
                if (sNames.Length == 0) continue;
                col = look.Columns[sNames[0]];
                if (col != null)
                {
                    col.Visible = true;
                    if (sNames.Length >= 2) col.Caption = sNames[1];
                }
            }
        }
        #endregion
        #endregion

        #region SetGridColumnEditCombo ...
        #region SetGridColumnEditCombo
        public void SetGridColumnEditCombo(GridColumn col, XElement colDefinition)
        {
            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnEditCombo");

            RepositoryItemComboBox combo = new RepositoryItemComboBox();
            combo.Name = "PKT_Grid_Combo";

            combo.AutoHeight = false;
            string sCmd = colDefinition.zAttribValue("Cmd");
            DataTable dt = null;
            if (sCmd != null) // la combo est rempli à partir d'une requête sql
                //dt = Ado.ExeCmd(gCon, sCmd);
                dt = _ado.ExeCmd(gCon, sCmd);
            else // sinon les valeurs de la combo sont la table fille "value"
                dt = colDefinition.zXmlToDataTable("value");
            for (int i = 0; i < dt.Rows.Count; i++) combo.Items.Add(dt.Rows[i][0]);
            combo.Name = "Combo_" + col.FieldName;
            combo.HotTrackDropDownItems = false;

            string sOption = colDefinition.zAttribValue("Option");
            bool bValueInListOnly;
            GetComboOptions(sOption, out bValueInListOnly);
            if (bValueInListOnly) combo.TextEditStyle = TextEditStyles.DisableTextEditor;

            combo.NullText = "";
            string sNullText = colDefinition.zAttribValue("NullText");
            if (sNullText != null) combo.NullText = sNullText;

            col.View.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { combo });
            col.ColumnEdit = combo;

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnEditCombo");
        }
        #endregion

        #region GetComboOptions
        public static void GetComboOptions(string sOption, out bool bValueInListOnly)
        {
            int i;
            string[] sOptions;

            bValueInListOnly = false;
            if (sOption != null)
            {
                sOptions = zsplit.Split(sOption, ',', true);
                for (i = 0; i < sOptions.Length; i++)
                {
                    switch (sOptions[i].ToLower())
                    {
                        case "valueinlistonly":
                            bValueInListOnly = true;
                            break;
                    }
                }
            }
        }
        #endregion
        #endregion

        #region SetGridColumnEditMask
        public static void SetGridColumnEditMask(GridColumn col, XElement colDefinition)
        {
            if (col == null) return;

            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnEditMask");

            RepositoryItemTextEdit edit = new RepositoryItemTextEdit();
            edit.Name = "PKT_Grid_Mask_Edit";
            ((System.ComponentModel.ISupportInitialize)edit).BeginInit();
            col.View.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { edit });

            string s = colDefinition.zAttribValue("Mask");
            if (s != null) edit.MaskData.EditMask = s;
            s = colDefinition.zAttribValue("MaskBlank");
            if (s != null) edit.MaskData.Blank = s;
            edit.MaskData.MaskType = MaskType.Simple;
            edit.MaskData.SaveLiteral = true;
            edit.MaskData.IgnoreMaskBlank = true;
            edit.MaskData.BeepOnError = false;
            // modif le 27/03/2013 column edit Mask par defaut pour pouvoir afficher les valeurs nulles --null--
            string sNullText = colDefinition.zAttribValue("NullText", "--null--");
            edit.NullText = sNullText;
            s = colDefinition.zAttribValue("MaskOption");
            SetTextEditOption(edit, s);
            col.ColumnEdit = edit;
            ((System.ComponentModel.ISupportInitialize)edit).EndInit();

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnEditMask");
        }
        #endregion

        #region SetGridColumnEditMemo
        public static void SetGridColumnEditMemo(GridColumn col)
        {
            if (col == null) return;

            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnEditMemo");

            RepositoryItemMemoEdit memo = new RepositoryItemMemoEdit();
            memo.Name = "PKT_Grid_Memo_Edit";
            ((System.ComponentModel.ISupportInitialize)memo).BeginInit();
            col.View.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { memo });
            col.ColumnEdit = memo;
            ((System.ComponentModel.ISupportInitialize)memo).EndInit();

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnEditMemo");
        }
        #endregion

        #region SetGridColumnEditSecond
        public static void SetGridColumnEditSecond(GridColumn col, XElement xe)
        {
            //if (gbTrace) cTrace.StartNestedLevel("SetGridColumnEditSecond");

            RepositoryItemTextEdit edit = new RepositoryItemTextEdit();
            edit.Name = "PKT_Grid_SecondEdit";
            ((System.ComponentModel.ISupportInitialize)edit).BeginInit();
            col.View.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { edit });
            //edit.AutoHeight = false;

            string sFormat = xe.zAttribValue("Format");
            if (sFormat == "" || sFormat == null) sFormat = "hh:mm:ss";
            SecondEdit secondEdit = new SecondEdit();
            secondEdit.Format = sFormat;

            edit.MaskData.EditMask = secondEdit.EditMask;
            edit.MaskData.MaskType = MaskType.Simple;
            edit.FormatEditValue += new ConvertEditValueEventHandler(secondEdit.FormatEditValue);
            edit.ParseEditValue += new ConvertEditValueEventHandler(secondEdit.ParseEditValue);
            col.ColumnEdit = edit;
            ((System.ComponentModel.ISupportInitialize)edit).EndInit();

            //if (gbTrace) cTrace.StopNestedLevel("SetGridColumnEditSecond");
        }
        #endregion

        #region SetTextEditOption
        public static void SetTextEditOption(RepositoryItemTextEdit edit, string sOption)
        {
            if (sOption == null || sOption == "") return;
            string[] sOptions = zsplit.Split(sOption, ',', true);
            foreach (string s in sOptions)
            {
                switch (s.ToLower())
                {
                    case "regular":
                        edit.MaskData.MaskType = MaskType.Regular;
                        break;
                    case "nosaveliteral":
                        edit.MaskData.SaveLiteral = true;
                        break;
                    case "noignoremaskblank":
                        edit.MaskData.IgnoreMaskBlank = true;
                        break;
                    case "beeponerror":
                        edit.MaskData.BeepOnError = true;
                        break;
                }
            }
        }
        #endregion
        #endregion
    }

    public class XtraGridOption
    {
        #region variable
        private bool gbReadOnly = false;
        private bool gbNoUpdate = false;
        private bool gbNoInsert = false;
        private bool gbNoInsertButton = false;
        private bool gbNoDelete = false;
        private bool gbNoDeleteSelected = false;
        private bool gbNoDeleteButton = false;
        private bool gbUpdateCommand = false;
        private bool gbInsertCommand = false;
        private bool gbDeleteCommand = false;
        private bool gbAutoRefresh = false;
        private bool gbDetailDynamic = false;
        private bool gbNoBestFitColumns = false;
        private bool gbNoNavigator = false;
        private bool gbColumnAutoWidth = false;
        private bool gbNoIndicator = false;
        private bool gbShowGroupPanel = false;
        private bool gbNoShowColumnHeaders = false;
        private bool gbNoShowFilterPanel = false;
        private bool gbNoMultiSelect = false;
        private bool gbNoAllowFilter = false;
        private bool gbNoAllowGroup = false;
        private bool gbNoAllowRowSizing = false;
        private bool gbNoAllowSort = false;

        //OptionsDetail
        private bool gbMasterViewMode = false;
        private bool gbAllowZoomDetail = false;
        private bool gbSmartDetailExpand = false;
        private bool gbAllowExpandEmptyDetails = false;
        private bool gbAutoZoomDetail = false;
        private bool gbEnableDetailToolTip = false;
        private bool gbShowDetailTabs = false;
        private bool gbSmartDetailHeight = false;
        private bool gbExpandAllMasterRows = false;
        #endregion

        #region property
        public bool ReadOnly { get { return gbReadOnly; } }
        public bool NoUpdate { get { return gbNoUpdate; } }
        public bool NoInsert { get { return gbNoInsert; } }
        public bool NoInsertButton { get { return gbNoInsertButton; } }
        public bool NoDelete { get { return gbNoDelete; } }
        public bool NoDeleteSelected { get { return gbNoDeleteSelected; } }
        public bool NoDeleteButton { get { return gbNoDeleteButton; } }
        public bool UpdateCommand { get { return gbUpdateCommand; } }
        public bool InsertCommand { get { return gbInsertCommand; } }
        public bool DeleteCommand { get { return gbDeleteCommand; } }
        public bool AutoRefresh { get { return gbAutoRefresh; } }
        public bool DetailDynamic { get { return gbDetailDynamic; } }
        public bool NoBestFitColumns { get { return gbNoBestFitColumns; } }
        public bool NoNavigator { get { return gbNoNavigator; } }
        public bool ColumnAutoWidth { get { return gbColumnAutoWidth; } }
        public bool NoIndicator { get { return gbNoIndicator; } }
        public bool ShowGroupPanel { get { return gbShowGroupPanel; } }
        public bool NoShowFilterPanel { get { return gbNoShowFilterPanel; } }
        public bool NoShowColumnHeaders { get { return gbNoShowColumnHeaders; } }
        public bool NoMultiSelect { get { return gbNoMultiSelect; } }
        public bool NoAllowFilter { get { return gbNoAllowFilter; } }
        public bool NoAllowGroup { get { return gbNoAllowGroup; } }
        public bool NoAllowRowSizing { get { return gbNoAllowRowSizing; } }
        public bool NoAllowSort { get { return gbNoAllowSort; } }

        //OptionsDetail
        public bool MasterViewMode { get { return gbMasterViewMode; } }
        public bool AllowZoomDetail { get { return gbAllowZoomDetail; } }
        public bool SmartDetailExpand { get { return gbSmartDetailExpand; } }
        public bool AllowExpandEmptyDetails { get { return gbAllowExpandEmptyDetails; } }
        public bool AutoZoomDetail { get { return gbAutoZoomDetail; } }
        public bool EnableDetailToolTip { get { return gbEnableDetailToolTip; } }
        public bool ShowDetailTabs { get { return gbShowDetailTabs; } }
        public bool SmartDetailHeight { get { return gbSmartDetailHeight; } }
        public bool ExpandAllMasterRows { get { return gbExpandAllMasterRows; } }
        #endregion

        #region constructor
        public XtraGridOption(string sOption)
        {
            SetOption(sOption);
        }
        #endregion

        #region SetOption
        private void SetOption(string sOption)
        {

            if (sOption == null) return;
            string[] sOptions = zsplit.Split(sOption, ',', true);
            foreach (string s in sOptions)
            {
                switch (s.ToLower())
                {
                    case "readonly":
                        gbReadOnly = true;
                        break;
                    case "noupdate":
                        gbNoUpdate = true;
                        break;
                    case "noinsert":
                        gbNoInsert = true;
                        break;
                    case "noinsertbutton":
                        gbNoInsertButton = true;
                        break;
                    case "nodelete":
                        gbNoDelete = true;
                        gbNoDeleteSelected = true;
                        break;
                    case "nodeleteselected":
                        gbNoDeleteSelected = true;
                        break;
                    case "nodeletebutton":
                        gbNoDeleteButton = true;
                        break;
                    case "allcommand":
                        gbUpdateCommand = true;
                        gbInsertCommand = true;
                        gbDeleteCommand = true;
                        break;
                    case "updatecommand":
                        gbUpdateCommand = true;
                        break;
                    case "insertcommand":
                        gbInsertCommand = true;
                        break;
                    case "deletecommand":
                        gbDeleteCommand = true;
                        break;
                    case "autorefresh":
                        gbAutoRefresh = true;
                        break;
                    case "nomultiselect":
                        gbNoMultiSelect = true;
                        gbNoDeleteSelected = true;
                        break;
                    case "detaildynamic":
                        gbDetailDynamic = true;
                        break;
                    case "nobestfitcolumns":
                        gbNoBestFitColumns = true;
                        break;
                    case "nonavigator":
                        gbNoNavigator = true;
                        break;
                    case "columnautowidth":
                        gbColumnAutoWidth = true;
                        break;
                    case "noindicator":
                        gbNoIndicator = true;
                        break;
                    case "showgrouppanel":
                        gbShowGroupPanel = true;
                        break;
                    case "noshowfilterpanel":
                        gbNoShowFilterPanel = true;
                        break;
                    case "noshowcolumnheaders":
                        gbNoShowColumnHeaders = true;
                        break;
                    case "noallowfilter":
                        gbNoAllowFilter = true;
                        break;
                    case "noallowgroup":
                        gbNoAllowGroup = true;
                        break;
                    case "noallowrowsizing":
                        gbNoAllowRowSizing = true;
                        break;
                    case "noallowsort":
                        gbNoAllowSort = true;
                        break;
                    //OptionsDetail
                    case "masterviewmode":
                        gbMasterViewMode = true;
                        break;
                    case "allowzoomdetail":
                        gbAllowZoomDetail = true;
                        break;
                    case "smartdetailexpand":
                        gbSmartDetailExpand = true;
                        break;
                    case "allowexpandemptydetails":
                        gbAllowExpandEmptyDetails = true;
                        break;
                    case "autozoomdetail":
                        gbAutoZoomDetail = true;
                        break;
                    case "enabledetailtooltip":
                        gbEnableDetailToolTip = true;
                        break;
                    case "showdetailtabs":
                        gbShowDetailTabs = true;
                        break;
                    case "smartdetailheight":
                        gbSmartDetailHeight = true;
                        break;
                    case "expandallmasterrows":
                        gbExpandAllMasterRows = true;
                        break;
                }
            }
        }
        #endregion
    }

    public class SecondEdit
    {
        #region variable
        private string gsFormat = null;         // hh:mm:ss, hh:mm, mm:ss
        private string gsToStringFormat = null; // "{0:00}:{1:00}:{2:00}" 0 = hh, 1 = mm, 2 = ss
        private string gsEditMask = null;       // "00:00:00"
        private int giHourPos = -1;
        private int giHourLength = -1;
        private int giMinutePos = -1;
        private int giMinuteLength = -1;
        private int giSecondPos = -1;
        private int giSecondLength = -1;
        #endregion

        #region property
        #region Format
        public string Format
        {
            get { return gsFormat; }
            set { SetFormat(value); }
        }
        #endregion

        public string ToStringFormat { get { return gsToStringFormat; } }
        public string EditMask { get { return gsEditMask; } }
        #endregion

        #region SetFormat
        private void SetFormat(string sFormat)
        {
            int i, iNb;
            string s;

            gsFormat = sFormat;

            gsEditMask = gsToStringFormat = sFormat.ToLower();

            CompteChar(gsEditMask, 'h', out giHourPos, out giHourLength);
            CompteChar(gsEditMask, 'm', out giMinutePos, out giMinuteLength);
            CompteChar(gsEditMask, 's', out giSecondPos, out giSecondLength);

            gsEditMask = gsEditMask.Replace('h', '0');
            gsEditMask = gsEditMask.Replace('m', '0');
            gsEditMask = gsEditMask.Replace('s', '0');

            CompteChar(gsToStringFormat, 'h', out i, out iNb);
            if (i != -1)
            {
                s = "{0:" + new string('0', iNb) + "}";
                gsToStringFormat = Replace(gsToStringFormat, i, iNb, s);
                if (gsToStringFormat.IndexOf('h') != -1) goto err;
            }

            CompteChar(gsToStringFormat, 'm', out i, out iNb);
            if (i != -1)
            {
                s = "{1:" + new string('0', iNb) + "}";
                gsToStringFormat = Replace(gsToStringFormat, i, iNb, s);
                if (gsToStringFormat.IndexOf('m') != -1) goto err;
            }

            CompteChar(gsToStringFormat, 's', out i, out iNb);
            if (i != -1)
            {
                s = "{2:" + new string('0', iNb) + "}";
                gsToStringFormat = Replace(gsToStringFormat, i, iNb, s);
                if (gsToStringFormat.IndexOf('s') != -1) goto err;
            }

            return;
        err:
            throw new GridException("hour format is not valid \"{0}\"", sFormat);
        }
        #endregion

        #region CompteChar
        private static void CompteChar(string s, char c, out int iPos, out int iCount)
        {
            int i;

            iCount = 0;
            iPos = s.IndexOf(c);
            if (iPos != -1)
            {
                for (i = iPos + 1; i < s.Length && s[i] == c; i++) ;
                iCount = i - iPos;
            }
        }
        #endregion

        #region Replace
        private static string Replace(string s, int iPos, int iCount, string s2)
        {
            int l;
            string s3;

            s3 = "";
            if (iPos > 0) s3 = s.Substring(0, iPos);
            s3 += s2;
            l = s.Length - (iPos + iCount);
            if (l > 0) s3 += s.Substring(iPos + iCount, l);
            return s3;
        }
        #endregion

        #region FormatEditValue
        public void FormatEditValue(object sender, ConvertEditValueEventArgs e)
        {
            // sender peut être soit RepositoryItemTextEdit soit TextEdit
            //edit = (RepositoryItemTextEdit)sender;
            //edit = (TextEdit)sender;
            if (e.Value != null && e.Value != DBNull.Value && Type.GetTypeCode(e.Value.GetType()) == TypeCode.Int32)
            {
                e.Value = SecondToString((int)e.Value);
                e.Handled = true;
            }
        }
        #endregion

        #region ParseEditValue
        public void ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            //edit = (TextEdit)sender;
            if (e.Value != null)
            {
                if (e.Value.ToString() == string.Empty)
                {
                    e.Value = DBNull.Value;
                    e.Handled = true;
                }
                else if (Type.GetTypeCode(e.Value.GetType()) == TypeCode.String)
                {
                    e.Value = StringToSecond((string)e.Value);
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region SecondToString
        private string SecondToString(int iSecond)
        {
            int hh, mm, ss;

            ss = iSecond % 60;
            iSecond /= 60;
            mm = iSecond % 60;
            hh = iSecond / 60;
            return string.Format(gsToStringFormat, hh, mm, ss);
        }
        #endregion

        private int StringToSecond(string sHeure)
        {
            int hh, mm, ss;

            hh = mm = ss = 0;
            //if (giHourPos != -1) hh = zconvert.Int(sHeure.Substring(giHourPos, giHourLength), -1);
            if (giHourPos != -1)
                hh = sHeure.Substring(giHourPos, giHourLength).zTryParseAs(-1);
            //if (giMinutePos != -1) mm = zconvert.Int(sHeure.Substring(giMinutePos, giMinuteLength), -1);
            if (giMinutePos != -1)
                mm = sHeure.Substring(giMinutePos, giMinuteLength).zTryParseAs(-1);
            //if (giSecondPos != -1) ss = zconvert.Int(sHeure.Substring(giSecondPos, giSecondLength), -1);
            if (giSecondPos != -1) ss = sHeure.Substring(giSecondPos, giSecondLength).zTryParseAs(-1);
            if (hh == -1 || mm == -1 || ss == -1)
                throw new GridException("hour not valid \"{0}\" format \"{1}\"", sHeure, gsFormat);
            return (hh * 60 + mm) * 60 + ss;
        }
    }

    public static class XmlForXtraGrid
    {
        public static DataTable zXmlToDataTable(this XElement xe, string sTable)
        {
            DataSet ds = xe.zXmlToDataSet();
            if (ds.Tables.Contains(sTable))
                return ds.Tables[sTable];
            return null;
        }

        public static DataSet zXmlToDataSet(this XElement xe)
        {
            DataSet ds = new DataSet();
            if (xe != null)
                ds.ReadXml(xe.CreateReader());
            return ds;
        }

        public static DataTable zChangeColumnType(this DataTable dt, string sColumn, Type type)
        {
            DataTable dt2 = dt.Clone();
            if (dt2.Columns.Contains(sColumn))
                dt2.Columns[sColumn].DataType = type;
            foreach (DataRow row in dt.Rows)
                dt2.Rows.Add(row.ItemArray);
            return dt2;
        }
    }
}
