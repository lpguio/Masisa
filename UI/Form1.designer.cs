namespace EnroladorStandAlone
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            this.repositoryItemProgressBar3 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            this.repositoryItemProgressBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.recargar = new DevExpress.XtraBars.BarButtonItem();
            this.enrolar = new DevExpress.XtraBars.BarButtonItem();
            this.Version = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.statusText = new DevExpress.XtraBars.BarStaticItem();
            this.errorText = new DevExpress.XtraBars.BarStaticItem();
            this.barHeaderItem1 = new DevExpress.XtraBars.BarHeaderItem();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.gcHistoria = new DevExpress.XtraGrid.GridControl();
            this.gvHistoria = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.RUT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Nombre = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Apellido = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Identificacion = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TipoAcciones = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colContratosActivos = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.panelGeneral = new System.Windows.Forms.Panel();
            this.axZKFPEngX1 = new AxZKFPEngXControl.AxZKFPEngX();
            this.barEditItem1 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.cmsMenuContextual = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.casinosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcHistoria)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistoria)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            this.panelGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axZKFPEngX1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit1)).BeginInit();
            this.cmsMenuContextual.SuspendLayout();
            this.SuspendLayout();
            // 
            // repositoryItemProgressBar3
            // 
            this.repositoryItemProgressBar3.LookAndFeel.SkinName = "Visual Studio 2013 Dark";
            this.repositoryItemProgressBar3.Name = "repositoryItemProgressBar3";
            this.repositoryItemProgressBar3.ShowTitle = true;
            this.repositoryItemProgressBar3.TextOrientation = DevExpress.Utils.Drawing.TextOrientation.Horizontal;
            // 
            // repositoryItemProgressBar1
            // 
            this.repositoryItemProgressBar1.Name = "repositoryItemProgressBar1";
            this.repositoryItemProgressBar1.ShowTitle = true;
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.recargar,
            this.statusText,
            this.errorText,
            this.enrolar,
            this.barHeaderItem1,
            this.Version});
            this.barManager.MainMenu = this.bar2;
            this.barManager.MaxItemId = 47;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.recargar),
            new DevExpress.XtraBars.LinkPersistInfo(this.enrolar),
            new DevExpress.XtraBars.LinkPersistInfo(this.Version)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DisableClose = true;
            this.bar2.OptionsBar.DisableCustomization = true;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // recargar
            // 
            this.recargar.Caption = "Sincronizar";
            this.recargar.Enabled = false;
            this.recargar.Id = 12;
            this.recargar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("recargar.ImageOptions.Image")));
            this.recargar.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("recargar.ImageOptions.LargeImage")));
            this.recargar.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 15F);
            this.recargar.ItemAppearance.Normal.Options.UseFont = true;
            this.recargar.Name = "recargar";
            this.recargar.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.recargar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.recargar_ItemClick);
            // 
            // enrolar
            // 
            this.enrolar.Caption = "Enrolar";
            this.enrolar.Enabled = false;
            this.enrolar.Id = 16;
            this.enrolar.ItemAppearance.Hovered.Font = new System.Drawing.Font("Verdana", 12F);
            this.enrolar.ItemAppearance.Hovered.Options.UseFont = true;
            this.enrolar.ItemAppearance.Normal.Font = new System.Drawing.Font("Verdana", 12F);
            this.enrolar.ItemAppearance.Normal.Options.UseFont = true;
            this.enrolar.ItemAppearance.Pressed.Font = new System.Drawing.Font("Verdana", 12F);
            this.enrolar.ItemAppearance.Pressed.Options.UseFont = true;
            this.enrolar.Name = "enrolar";
            this.enrolar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.enrolar_ItemClick);
            // 
            // Version
            // 
            this.Version.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.Version.Caption = "Versión 1.0";
            this.Version.Enabled = false;
            this.Version.Id = 43;
            this.Version.ItemAppearance.Disabled.Font = new System.Drawing.Font("Verdana", 12F);
            this.Version.ItemAppearance.Disabled.Options.UseFont = true;
            this.Version.ItemAppearance.Hovered.Font = new System.Drawing.Font("Verdana", 12F);
            this.Version.ItemAppearance.Hovered.Options.UseFont = true;
            this.Version.ItemAppearance.Normal.Font = new System.Drawing.Font("Verdana", 12F);
            this.Version.ItemAppearance.Normal.Options.UseFont = true;
            this.Version.ItemAppearance.Pressed.Font = new System.Drawing.Font("Verdana", 12F);
            this.Version.ItemAppearance.Pressed.Options.UseFont = true;
            this.Version.LeftIndent = 20;
            this.Version.Name = "Version";
            this.Version.RightIndent = 20;
            superToolTip1.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            toolTipTitleItem1.Text = "Versión 1.0";
            toolTipItem1.LeftIndent = 6;
            toolTipItem1.Text = "Versión inicial\r\n\r\n<b>Versión 0.1</b>\r\nVersión de desarrollo";
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.Version.SuperTip = superToolTip1;
            this.Version.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(1008, 31);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 712);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(1008, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 31);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 681);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1008, 31);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 681);
            // 
            // statusText
            // 
            this.statusText.Id = 13;
            this.statusText.Name = "statusText";
            this.statusText.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // errorText
            // 
            this.errorText.Id = 14;
            this.errorText.ItemAppearance.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.errorText.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Red;
            this.errorText.ItemAppearance.Normal.Options.UseFont = true;
            this.errorText.ItemAppearance.Normal.Options.UseForeColor = true;
            this.errorText.Name = "errorText";
            this.errorText.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barHeaderItem1
            // 
            this.barHeaderItem1.Caption = "barHeaderItem1";
            this.barHeaderItem1.Id = 21;
            this.barHeaderItem1.Name = "barHeaderItem1";
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            // 
            // gcHistoria
            // 
            this.gcHistoria.ContextMenuStrip = this.cmsMenuContextual;
            this.gcHistoria.Cursor = System.Windows.Forms.Cursors.Default;
            this.gcHistoria.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcHistoria.Location = new System.Drawing.Point(0, 0);
            this.gcHistoria.MainView = this.gvHistoria;
            this.gcHistoria.Name = "gcHistoria";
            this.gcHistoria.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEdit2,
            this.repositoryItemTextEdit1});
            this.gcHistoria.Size = new System.Drawing.Size(1008, 681);
            this.gcHistoria.TabIndex = 1;
            this.gcHistoria.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvHistoria});
            // 
            // gvHistoria
            // 
            this.gvHistoria.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 15F);
            this.gvHistoria.Appearance.Row.Options.UseFont = true;
            this.gvHistoria.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.gvHistoria.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.RUT,
            this.Nombre,
            this.Apellido,
            this.Identificacion,
            this.TipoAcciones,
            this.colContratosActivos});
            this.gvHistoria.GridControl = this.gcHistoria;
            this.gvHistoria.Name = "gvHistoria";
            this.gvHistoria.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvHistoria.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvHistoria.OptionsBehavior.AutoSelectAllInEditor = false;
            this.gvHistoria.OptionsBehavior.AutoUpdateTotalSummary = false;
            this.gvHistoria.OptionsBehavior.Editable = false;
            this.gvHistoria.OptionsBehavior.ReadOnly = true;
            this.gvHistoria.OptionsCustomization.AllowColumnMoving = false;
            this.gvHistoria.OptionsCustomization.AllowGroup = false;
            this.gvHistoria.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvHistoria.OptionsDetail.AllowZoomDetail = false;
            this.gvHistoria.OptionsDetail.EnableMasterViewMode = false;
            this.gvHistoria.OptionsDetail.ShowDetailTabs = false;
            this.gvHistoria.OptionsDetail.SmartDetailExpand = false;
            this.gvHistoria.OptionsLayout.Columns.AddNewColumns = false;
            this.gvHistoria.OptionsLayout.Columns.RemoveOldColumns = false;
            this.gvHistoria.OptionsLayout.Columns.StoreLayout = false;
            this.gvHistoria.OptionsMenu.EnableColumnMenu = false;
            this.gvHistoria.OptionsMenu.EnableFooterMenu = false;
            this.gvHistoria.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvHistoria.OptionsMenu.ShowDateTimeGroupIntervalItems = false;
            this.gvHistoria.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.gvHistoria.OptionsMenu.ShowSplitItem = false;
            this.gvHistoria.OptionsNavigation.AutoFocusNewRow = true;
            this.gvHistoria.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvHistoria.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gvHistoria.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gvHistoria.OptionsSelection.UseIndicatorForSelection = false;
            this.gvHistoria.OptionsView.RowAutoHeight = true;
            this.gvHistoria.OptionsView.ShowAutoFilterRow = true;
            this.gvHistoria.OptionsView.ShowGroupPanel = false;
            this.gvHistoria.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvHistoria_RowCellStyle);
            this.gvHistoria.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvHistoria_RowStyle);
            this.gvHistoria.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvHistoria_CustomColumnDisplayText);
            this.gvHistoria.DoubleClick += new System.EventHandler(this.gvHistoria_DoubleClick);
            // 
            // RUT
            // 
            this.RUT.Caption = "RUT";
            this.RUT.FieldName = "Item1";
            this.RUT.MinWidth = 10;
            this.RUT.Name = "RUT";
            this.RUT.OptionsColumn.AllowEdit = false;
            this.RUT.OptionsColumn.AllowFocus = false;
            this.RUT.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.RUT.OptionsColumn.AllowIncrementalSearch = false;
            this.RUT.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.RUT.OptionsColumn.AllowMove = false;
            this.RUT.OptionsColumn.AllowShowHide = false;
            this.RUT.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.RUT.OptionsColumn.Printable = DevExpress.Utils.DefaultBoolean.True;
            this.RUT.OptionsColumn.ReadOnly = true;
            this.RUT.OptionsFilter.AllowFilter = false;
            this.RUT.Visible = true;
            this.RUT.VisibleIndex = 0;
            this.RUT.Width = 100;
            // 
            // Nombre
            // 
            this.Nombre.Caption = "Nombre";
            this.Nombre.FieldName = "Item2";
            this.Nombre.MinWidth = 10;
            this.Nombre.Name = "Nombre";
            this.Nombre.OptionsColumn.AllowEdit = false;
            this.Nombre.OptionsColumn.AllowFocus = false;
            this.Nombre.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.Nombre.OptionsColumn.AllowIncrementalSearch = false;
            this.Nombre.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.Nombre.OptionsColumn.AllowMove = false;
            this.Nombre.OptionsColumn.AllowShowHide = false;
            this.Nombre.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.Nombre.OptionsColumn.Printable = DevExpress.Utils.DefaultBoolean.True;
            this.Nombre.OptionsColumn.ReadOnly = true;
            this.Nombre.OptionsFilter.AllowFilter = false;
            this.Nombre.Visible = true;
            this.Nombre.VisibleIndex = 1;
            this.Nombre.Width = 100;
            // 
            // Apellido
            // 
            this.Apellido.Caption = "Apellidos";
            this.Apellido.FieldName = "Item3";
            this.Apellido.MinWidth = 10;
            this.Apellido.Name = "Apellido";
            this.Apellido.OptionsColumn.AllowEdit = false;
            this.Apellido.OptionsColumn.AllowFocus = false;
            this.Apellido.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.Apellido.OptionsColumn.AllowIncrementalSearch = false;
            this.Apellido.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.Apellido.OptionsColumn.AllowMove = false;
            this.Apellido.OptionsColumn.AllowShowHide = false;
            this.Apellido.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.Apellido.OptionsColumn.Printable = DevExpress.Utils.DefaultBoolean.True;
            this.Apellido.OptionsColumn.ReadOnly = true;
            this.Apellido.OptionsFilter.AllowFilter = false;
            this.Apellido.Visible = true;
            this.Apellido.VisibleIndex = 2;
            this.Apellido.Width = 100;
            // 
            // Identificacion
            // 
            this.Identificacion.Caption = "Identificación";
            this.Identificacion.FieldName = "Item4";
            this.Identificacion.Name = "Identificacion";
            this.Identificacion.Visible = true;
            this.Identificacion.VisibleIndex = 3;
            // 
            // TipoAcciones
            // 
            this.TipoAcciones.Caption = "TipoAcciones";
            this.TipoAcciones.FieldName = "Item5";
            this.TipoAcciones.Name = "TipoAcciones";
            this.TipoAcciones.OptionsColumn.AllowEdit = false;
            this.TipoAcciones.OptionsColumn.AllowFocus = false;
            this.TipoAcciones.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.TipoAcciones.OptionsColumn.AllowIncrementalSearch = false;
            this.TipoAcciones.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.TipoAcciones.OptionsColumn.AllowMove = false;
            this.TipoAcciones.OptionsColumn.AllowShowHide = false;
            this.TipoAcciones.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.TipoAcciones.OptionsColumn.Printable = DevExpress.Utils.DefaultBoolean.True;
            this.TipoAcciones.OptionsColumn.ReadOnly = true;
            this.TipoAcciones.OptionsFilter.AllowFilter = false;
            // 
            // colContratosActivos
            // 
            this.colContratosActivos.AppearanceCell.Options.UseTextOptions = true;
            this.colContratosActivos.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colContratosActivos.AppearanceHeader.Options.UseTextOptions = true;
            this.colContratosActivos.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colContratosActivos.Caption = "Contrato Vigente";
            this.colContratosActivos.ColumnEdit = this.repositoryItemMemoEdit2;
            this.colContratosActivos.Name = "colContratosActivos";
            this.colContratosActivos.OptionsColumn.AllowEdit = false;
            this.colContratosActivos.OptionsColumn.AllowFocus = false;
            this.colContratosActivos.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.colContratosActivos.OptionsColumn.AllowIncrementalSearch = false;
            this.colContratosActivos.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colContratosActivos.OptionsColumn.AllowMove = false;
            this.colContratosActivos.OptionsColumn.AllowShowHide = false;
            this.colContratosActivos.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colContratosActivos.OptionsColumn.Printable = DevExpress.Utils.DefaultBoolean.True;
            this.colContratosActivos.OptionsColumn.ReadOnly = true;
            this.colContratosActivos.Visible = true;
            this.colContratosActivos.VisibleIndex = 4;
            // 
            // repositoryItemMemoEdit2
            // 
            this.repositoryItemMemoEdit2.Name = "repositoryItemMemoEdit2";
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // panelGeneral
            // 
            this.panelGeneral.Controls.Add(this.axZKFPEngX1);
            this.panelGeneral.Controls.Add(this.gcHistoria);
            this.panelGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGeneral.Location = new System.Drawing.Point(0, 31);
            this.panelGeneral.Name = "panelGeneral";
            this.panelGeneral.Size = new System.Drawing.Size(1008, 681);
            this.panelGeneral.TabIndex = 21;
            // 
            // axZKFPEngX1
            // 
            this.axZKFPEngX1.Enabled = true;
            this.axZKFPEngX1.Location = new System.Drawing.Point(437, 155);
            this.axZKFPEngX1.Name = "axZKFPEngX1";
            this.axZKFPEngX1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axZKFPEngX1.OcxState")));
            this.axZKFPEngX1.Size = new System.Drawing.Size(24, 24);
            this.axZKFPEngX1.TabIndex = 2;
            // 
            // barEditItem1
            // 
            this.barEditItem1.Edit = null;
            this.barEditItem1.Id = -1;
            this.barEditItem1.Name = "barEditItem1";
            // 
            // repositoryItemLookUpEdit1
            // 
            this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
            // 
            // cmsMenuContextual
            // 
            this.cmsMenuContextual.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.casinosToolStripMenuItem});
            this.cmsMenuContextual.Name = "cmsMenuContextual";
            this.cmsMenuContextual.Size = new System.Drawing.Size(116, 26);
            // 
            // casinosToolStripMenuItem
            // 
            this.casinosToolStripMenuItem.Name = "casinosToolStripMenuItem";
            this.casinosToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.casinosToolStripMenuItem.Text = "Casinos";
            this.casinosToolStripMenuItem.Click += new System.EventHandler(this.casinosToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 712);
            this.Controls.Add(this.panelGeneral);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(600, 480);
            this.Name = "Form1";
            this.Text = "Enrolador Biometria Aplicada";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcHistoria)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistoria)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            this.panelGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axZKFPEngX1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit1)).EndInit();
            this.cmsMenuContextual.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.GridControl gcHistoria;
        private DevExpress.XtraGrid.Views.Grid.GridView gvHistoria;
        private DevExpress.XtraBars.BarButtonItem recargar;
        private DevExpress.XtraBars.BarStaticItem statusText;
        private DevExpress.XtraGrid.Columns.GridColumn RUT;
        private DevExpress.XtraGrid.Columns.GridColumn Nombre;
        private DevExpress.XtraGrid.Columns.GridColumn Apellido;
        private System.Windows.Forms.Panel panelGeneral;
        private DevExpress.XtraBars.BarStaticItem errorText;
        private DevExpress.XtraBars.BarButtonItem enrolar;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1;
        private DevExpress.XtraBars.BarHeaderItem barHeaderItem1;
        private DevExpress.XtraBars.BarEditItem barEditItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemLookUpEdit1;
        private DevExpress.XtraBars.BarStaticItem Version;
        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar repositoryItemProgressBar3;
        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar repositoryItemProgressBar1;
        private DevExpress.XtraGrid.Columns.GridColumn TipoAcciones;
        private DevExpress.XtraGrid.Columns.GridColumn Identificacion;
        private AxZKFPEngXControl.AxZKFPEngX axZKFPEngX1;
        private DevExpress.XtraGrid.Columns.GridColumn colContratosActivos;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private System.Windows.Forms.ContextMenuStrip cmsMenuContextual;
        private System.Windows.Forms.ToolStripMenuItem casinosToolStripMenuItem;
    }
}
