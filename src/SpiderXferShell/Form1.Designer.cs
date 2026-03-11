namespace SpiderXferShell;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        pnlTopStatus = new Panel();
        lblModeSummary = new Label();
        lblBeTradeSummary = new Label();
        lblMasterSummary = new Label();
        lblThreadSummary = new Label();
        lblRunState = new Label();
        lblAppTitle = new Label();
        pnlBottomActions = new Panel();
        btnStart = new Button();
        btnStop = new Button();
        btnExport = new Button();
        btnViewInventory = new Button();
        btnImportMaFiles = new Button();
        btnImportAccounts = new Button();
        pnlMain = new Panel();
        pnlLeftMain = new Panel();
        pnlBeTrade = new Panel();
        dgvBeTrade = new DataGridView();
        lblBeTradeTitle = new Label();
        pnlMaster = new Panel();
        dgvMaster = new DataGridView();
        lblMasterTitle = new Label();
        pnlParams = new Panel();
        tblParams = new TableLayoutPanel();
        pnlFilterValue = new Panel();
        txtItemFilter = new TextBox();
        lblItemFilter = new Label();
        pnlItemType = new Panel();
        cmbItemType = new ComboBox();
        lblItemType = new Label();
        pnlAcceptMode = new Panel();
        cmbAcceptMode = new ComboBox();
        lblAcceptMode = new Label();
        pnlTransferType = new Panel();
        cmbTransferType = new ComboBox();
        lblTransferType = new Label();
        pnlThread = new Panel();
        numThreadCount = new NumericUpDown();
        lblThread = new Label();
        lblParamsTitle = new Label();
        pnlRightSide = new Panel();
        pnlLogs = new Panel();
        txtLog = new RichTextBox();
        chkAutoScroll = new CheckBox();
        btnClearLog = new Button();
        lblLogTitle = new Label();
        pnlDetails = new Panel();
        tableLayoutPanel1 = new TableLayoutPanel();
        lblSelectedInventoryValue = new Label();
        lblSelectedInventoryTitle = new Label();
        lblSelectedErrorValue = new Label();
        lblSelectedErrorTitle = new Label();
        lblSelectedOfferValue = new Label();
        lblSelectedOfferTitle = new Label();
        lblSelectedMaFileValue = new Label();
        lblSelectedMaFileTitle = new Label();
        lblSelectedSteamIdValue = new Label();
        lblSelectedSteamIdTitle = new Label();
        lblSelectedAccountValue = new Label();
        lblSelectedAccountTitle = new Label();
        lblDetailsTitle = new Label();
        pnlTopStatus.SuspendLayout();
        pnlBottomActions.SuspendLayout();
        pnlMain.SuspendLayout();
        pnlLeftMain.SuspendLayout();
        pnlBeTrade.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvBeTrade).BeginInit();
        pnlMaster.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvMaster).BeginInit();
        pnlParams.SuspendLayout();
        tblParams.SuspendLayout();
        pnlFilterValue.SuspendLayout();
        pnlItemType.SuspendLayout();
        pnlAcceptMode.SuspendLayout();
        pnlTransferType.SuspendLayout();
        pnlThread.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numThreadCount).BeginInit();
        pnlRightSide.SuspendLayout();
        pnlLogs.SuspendLayout();
        pnlDetails.SuspendLayout();
        tableLayoutPanel1.SuspendLayout();
        SuspendLayout();
        // 
        // pnlTopStatus
        // 
        pnlTopStatus.Controls.Add(lblModeSummary);
        pnlTopStatus.Controls.Add(lblBeTradeSummary);
        pnlTopStatus.Controls.Add(lblMasterSummary);
        pnlTopStatus.Controls.Add(lblThreadSummary);
        pnlTopStatus.Controls.Add(lblRunState);
        pnlTopStatus.Controls.Add(lblAppTitle);
        pnlTopStatus.Dock = DockStyle.Top;
        pnlTopStatus.Location = new Point(0, 0);
        pnlTopStatus.Name = "pnlTopStatus";
        pnlTopStatus.Padding = new Padding(16, 10, 16, 10);
        pnlTopStatus.Size = new Size(1600, 56);
        pnlTopStatus.TabIndex = 0;
        // 
        // lblModeSummary
        // 
        lblModeSummary.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblModeSummary.AutoSize = true;
        lblModeSummary.Location = new Point(1440, 20);
        lblModeSummary.Name = "lblModeSummary";
        lblModeSummary.Size = new Size(62, 17);
        lblModeSummary.TabIndex = 5;
        lblModeSummary.Text = "模式：-";
        // 
        // lblBeTradeSummary
        // 
        lblBeTradeSummary.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblBeTradeSummary.AutoSize = true;
        lblBeTradeSummary.Location = new Point(1336, 20);
        lblBeTradeSummary.Name = "lblBeTradeSummary";
        lblBeTradeSummary.Size = new Size(74, 17);
        lblBeTradeSummary.TabIndex = 4;
        lblBeTradeSummary.Text = "待转号：0";
        // 
        // lblMasterSummary
        // 
        lblMasterSummary.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblMasterSummary.AutoSize = true;
        lblMasterSummary.Location = new Point(1238, 20);
        lblMasterSummary.Name = "lblMasterSummary";
        lblMasterSummary.Size = new Size(74, 17);
        lblMasterSummary.TabIndex = 3;
        lblMasterSummary.Text = "主库号：0";
        // 
        // lblThreadSummary
        // 
        lblThreadSummary.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblThreadSummary.AutoSize = true;
        lblThreadSummary.Location = new Point(1159, 20);
        lblThreadSummary.Name = "lblThreadSummary";
        lblThreadSummary.Size = new Size(62, 17);
        lblThreadSummary.TabIndex = 2;
        lblThreadSummary.Text = "线程：0";
        // 
        // lblRunState
        // 
        lblRunState.AutoSize = true;
        lblRunState.Location = new Point(260, 20);
        lblRunState.Name = "lblRunState";
        lblRunState.Size = new Size(74, 17);
        lblRunState.TabIndex = 1;
        lblRunState.Text = "状态：空闲";
        // 
        // lblAppTitle
        // 
        lblAppTitle.AutoSize = true;
        lblAppTitle.Font = new Font("Bahnschrift SemiBold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        lblAppTitle.Location = new Point(16, 16);
        lblAppTitle.Name = "lblAppTitle";
        lblAppTitle.Size = new Size(208, 23);
        lblAppTitle.TabIndex = 0;
        lblAppTitle.Text = "Steam 资产转移控制台";
        // 
        // pnlBottomActions
        // 
        pnlBottomActions.Controls.Add(btnStart);
        pnlBottomActions.Controls.Add(btnStop);
        pnlBottomActions.Controls.Add(btnExport);
        pnlBottomActions.Controls.Add(btnViewInventory);
        pnlBottomActions.Controls.Add(btnImportMaFiles);
        pnlBottomActions.Controls.Add(btnImportAccounts);
        pnlBottomActions.Dock = DockStyle.Bottom;
        pnlBottomActions.Location = new Point(0, 908);
        pnlBottomActions.Name = "pnlBottomActions";
        pnlBottomActions.Padding = new Padding(16, 12, 16, 12);
        pnlBottomActions.Size = new Size(1600, 72);
        pnlBottomActions.TabIndex = 1;
        // 
        // btnStart
        // 
        btnStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnStart.Location = new Point(1424, 17);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(152, 38);
        btnStart.TabIndex = 5;
        btnStart.Text = "开始转移";
        btnStart.UseVisualStyleBackColor = true;
        // 
        // btnStop
        // 
        btnStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnStop.Location = new Point(1288, 17);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(120, 38);
        btnStop.TabIndex = 4;
        btnStop.Text = "停止任务";
        btnStop.UseVisualStyleBackColor = true;
        // 
        // btnExport
        // 
        btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnExport.Location = new Point(1156, 17);
        btnExport.Name = "btnExport";
        btnExport.Size = new Size(116, 38);
        btnExport.TabIndex = 3;
        btnExport.Text = "导出结果";
        btnExport.UseVisualStyleBackColor = true;
        // 
        // btnViewInventory
        // 
        btnViewInventory.Location = new Point(280, 17);
        btnViewInventory.Name = "btnViewInventory";
        btnViewInventory.Size = new Size(116, 38);
        btnViewInventory.TabIndex = 2;
        btnViewInventory.Text = "查看库存";
        btnViewInventory.UseVisualStyleBackColor = true;
        // 
        // btnImportMaFiles
        // 
        btnImportMaFiles.Location = new Point(148, 17);
        btnImportMaFiles.Name = "btnImportMaFiles";
        btnImportMaFiles.Size = new Size(116, 38);
        btnImportMaFiles.TabIndex = 1;
        btnImportMaFiles.Text = "导入 maFile";
        btnImportMaFiles.UseVisualStyleBackColor = true;
        // 
        // btnImportAccounts
        // 
        btnImportAccounts.Location = new Point(16, 17);
        btnImportAccounts.Name = "btnImportAccounts";
        btnImportAccounts.Size = new Size(116, 38);
        btnImportAccounts.TabIndex = 0;
        btnImportAccounts.Text = "导入账号";
        btnImportAccounts.UseVisualStyleBackColor = true;
        // 
        // pnlMain
        // 
        pnlMain.Controls.Add(pnlLeftMain);
        pnlMain.Controls.Add(pnlRightSide);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Location = new Point(0, 56);
        pnlMain.Name = "pnlMain";
        pnlMain.Size = new Size(1600, 852);
        pnlMain.TabIndex = 2;
        // 
        // pnlLeftMain
        // 
        pnlLeftMain.Controls.Add(pnlBeTrade);
        pnlLeftMain.Controls.Add(pnlMaster);
        pnlLeftMain.Controls.Add(pnlParams);
        pnlLeftMain.Dock = DockStyle.Fill;
        pnlLeftMain.Location = new Point(0, 0);
        pnlLeftMain.Name = "pnlLeftMain";
        pnlLeftMain.Size = new Size(1180, 852);
        pnlLeftMain.TabIndex = 0;
        // 
        // pnlBeTrade
        // 
        pnlBeTrade.Controls.Add(dgvBeTrade);
        pnlBeTrade.Controls.Add(lblBeTradeTitle);
        pnlBeTrade.Dock = DockStyle.Fill;
        pnlBeTrade.Location = new Point(0, 400);
        pnlBeTrade.Name = "pnlBeTrade";
        pnlBeTrade.Padding = new Padding(12, 0, 12, 12);
        pnlBeTrade.Size = new Size(1180, 452);
        pnlBeTrade.TabIndex = 2;
        // 
        // dgvBeTrade
        // 
        dgvBeTrade.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvBeTrade.Dock = DockStyle.Fill;
        dgvBeTrade.Location = new Point(12, 28);
        dgvBeTrade.Name = "dgvBeTrade";
        dgvBeTrade.Size = new Size(1156, 412);
        dgvBeTrade.TabIndex = 1;
        // 
        // lblBeTradeTitle
        // 
        lblBeTradeTitle.Dock = DockStyle.Top;
        lblBeTradeTitle.Location = new Point(12, 0);
        lblBeTradeTitle.Name = "lblBeTradeTitle";
        lblBeTradeTitle.Size = new Size(1156, 28);
        lblBeTradeTitle.TabIndex = 0;
        lblBeTradeTitle.Text = "待转号 / 发货端";
        lblBeTradeTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlMaster
        // 
        pnlMaster.Controls.Add(dgvMaster);
        pnlMaster.Controls.Add(lblMasterTitle);
        pnlMaster.Dock = DockStyle.Top;
        pnlMaster.Location = new Point(0, 150);
        pnlMaster.Name = "pnlMaster";
        pnlMaster.Padding = new Padding(12, 0, 12, 8);
        pnlMaster.Size = new Size(1180, 250);
        pnlMaster.TabIndex = 1;
        // 
        // dgvMaster
        // 
        dgvMaster.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvMaster.Dock = DockStyle.Fill;
        dgvMaster.Location = new Point(12, 28);
        dgvMaster.Name = "dgvMaster";
        dgvMaster.Size = new Size(1156, 214);
        dgvMaster.TabIndex = 1;
        // 
        // lblMasterTitle
        // 
        lblMasterTitle.Dock = DockStyle.Top;
        lblMasterTitle.Location = new Point(12, 0);
        lblMasterTitle.Name = "lblMasterTitle";
        lblMasterTitle.Size = new Size(1156, 28);
        lblMasterTitle.TabIndex = 0;
        lblMasterTitle.Text = "主库号 / 接收端";
        lblMasterTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlParams
        // 
        pnlParams.Controls.Add(tblParams);
        pnlParams.Controls.Add(lblParamsTitle);
        pnlParams.Dock = DockStyle.Top;
        pnlParams.Location = new Point(0, 0);
        pnlParams.Name = "pnlParams";
        pnlParams.Padding = new Padding(12);
        pnlParams.Size = new Size(1180, 150);
        pnlParams.TabIndex = 0;
        // 
        // tblParams
        // 
        tblParams.ColumnCount = 3;
        tblParams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        tblParams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        tblParams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        tblParams.Controls.Add(pnlFilterValue, 1, 1);
        tblParams.Controls.Add(pnlItemType, 0, 1);
        tblParams.Controls.Add(pnlAcceptMode, 2, 0);
        tblParams.Controls.Add(pnlTransferType, 1, 0);
        tblParams.Controls.Add(pnlThread, 0, 0);
        tblParams.Dock = DockStyle.Fill;
        tblParams.Location = new Point(12, 40);
        tblParams.Name = "tblParams";
        tblParams.RowCount = 2;
        tblParams.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tblParams.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tblParams.Size = new Size(1156, 98);
        tblParams.TabIndex = 1;
        // 
        // pnlFilterValue
        // 
        pnlFilterValue.Controls.Add(txtItemFilter);
        pnlFilterValue.Controls.Add(lblItemFilter);
        pnlFilterValue.Dock = DockStyle.Fill;
        pnlFilterValue.Location = new Point(388, 52);
        pnlFilterValue.Name = "pnlFilterValue";
        pnlFilterValue.Padding = new Padding(4);
        pnlFilterValue.Size = new Size(379, 43);
        pnlFilterValue.TabIndex = 4;
        // 
        // txtItemFilter
        // 
        txtItemFilter.Dock = DockStyle.Bottom;
        txtItemFilter.Location = new Point(4, 16);
        txtItemFilter.Name = "txtItemFilter";
        txtItemFilter.PlaceholderText = "输入筛选关键字...";
        txtItemFilter.Size = new Size(371, 23);
        txtItemFilter.TabIndex = 1;
        // 
        // lblItemFilter
        // 
        lblItemFilter.AutoSize = true;
        lblItemFilter.Location = new Point(4, 0);
        lblItemFilter.Name = "lblItemFilter";
        lblItemFilter.Size = new Size(56, 17);
        lblItemFilter.TabIndex = 0;
        lblItemFilter.Text = "筛选值";
        // 
        // pnlItemType
        // 
        pnlItemType.Controls.Add(cmbItemType);
        pnlItemType.Controls.Add(lblItemType);
        pnlItemType.Dock = DockStyle.Fill;
        pnlItemType.Location = new Point(3, 52);
        pnlItemType.Name = "pnlItemType";
        pnlItemType.Padding = new Padding(4);
        pnlItemType.Size = new Size(379, 43);
        pnlItemType.TabIndex = 3;
        // 
        // cmbItemType
        // 
        cmbItemType.Dock = DockStyle.Bottom;
        cmbItemType.FormattingEnabled = true;
        cmbItemType.Location = new Point(4, 16);
        cmbItemType.Name = "cmbItemType";
        cmbItemType.Size = new Size(371, 25);
        cmbItemType.TabIndex = 1;
        // 
        // lblItemType
        // 
        lblItemType.AutoSize = true;
        lblItemType.Location = new Point(4, 0);
        lblItemType.Name = "lblItemType";
        lblItemType.Size = new Size(68, 17);
        lblItemType.TabIndex = 0;
        lblItemType.Text = "筛选方式";
        // 
        // pnlAcceptMode
        // 
        pnlAcceptMode.Controls.Add(cmbAcceptMode);
        pnlAcceptMode.Controls.Add(lblAcceptMode);
        pnlAcceptMode.Dock = DockStyle.Fill;
        pnlAcceptMode.Location = new Point(773, 3);
        pnlAcceptMode.Name = "pnlAcceptMode";
        pnlAcceptMode.Padding = new Padding(4);
        pnlAcceptMode.Size = new Size(380, 43);
        pnlAcceptMode.TabIndex = 2;
        // 
        // cmbAcceptMode
        // 
        cmbAcceptMode.Dock = DockStyle.Bottom;
        cmbAcceptMode.FormattingEnabled = true;
        cmbAcceptMode.Location = new Point(4, 14);
        cmbAcceptMode.Name = "cmbAcceptMode";
        cmbAcceptMode.Size = new Size(372, 25);
        cmbAcceptMode.TabIndex = 1;
        // 
        // lblAcceptMode
        // 
        lblAcceptMode.AutoSize = true;
        lblAcceptMode.Location = new Point(4, 0);
        lblAcceptMode.Name = "lblAcceptMode";
        lblAcceptMode.Size = new Size(68, 17);
        lblAcceptMode.TabIndex = 0;
        lblAcceptMode.Text = "接受方式";
        // 
        // pnlTransferType
        // 
        pnlTransferType.Controls.Add(cmbTransferType);
        pnlTransferType.Controls.Add(lblTransferType);
        pnlTransferType.Dock = DockStyle.Fill;
        pnlTransferType.Location = new Point(388, 3);
        pnlTransferType.Name = "pnlTransferType";
        pnlTransferType.Padding = new Padding(4);
        pnlTransferType.Size = new Size(379, 43);
        pnlTransferType.TabIndex = 1;
        // 
        // cmbTransferType
        // 
        cmbTransferType.Dock = DockStyle.Bottom;
        cmbTransferType.FormattingEnabled = true;
        cmbTransferType.Location = new Point(4, 14);
        cmbTransferType.Name = "cmbTransferType";
        cmbTransferType.Size = new Size(371, 25);
        cmbTransferType.TabIndex = 1;
        // 
        // lblTransferType
        // 
        lblTransferType.AutoSize = true;
        lblTransferType.Location = new Point(4, 0);
        lblTransferType.Name = "lblTransferType";
        lblTransferType.Size = new Size(68, 17);
        lblTransferType.TabIndex = 0;
        lblTransferType.Text = "转移模式";
        // 
        // pnlThread
        // 
        pnlThread.Controls.Add(numThreadCount);
        pnlThread.Controls.Add(lblThread);
        pnlThread.Dock = DockStyle.Fill;
        pnlThread.Location = new Point(3, 3);
        pnlThread.Name = "pnlThread";
        pnlThread.Padding = new Padding(4);
        pnlThread.Size = new Size(379, 43);
        pnlThread.TabIndex = 0;
        // 
        // numThreadCount
        // 
        numThreadCount.Dock = DockStyle.Bottom;
        numThreadCount.Location = new Point(4, 16);
        numThreadCount.Maximum = new decimal(new int[] {64, 0, 0, 0});
        numThreadCount.Minimum = new decimal(new int[] {1, 0, 0, 0});
        numThreadCount.Name = "numThreadCount";
        numThreadCount.Size = new Size(371, 23);
        numThreadCount.TabIndex = 1;
        numThreadCount.Value = new decimal(new int[] {1, 0, 0, 0});
        // 
        // lblThread
        // 
        lblThread.AutoSize = true;
        lblThread.Location = new Point(4, 0);
        lblThread.Name = "lblThread";
        lblThread.Size = new Size(56, 17);
        lblThread.TabIndex = 0;
        lblThread.Text = "线程数";
        // 
        // lblParamsTitle
        // 
        lblParamsTitle.Dock = DockStyle.Top;
        lblParamsTitle.Location = new Point(12, 12);
        lblParamsTitle.Name = "lblParamsTitle";
        lblParamsTitle.Size = new Size(1156, 28);
        lblParamsTitle.TabIndex = 0;
        lblParamsTitle.Text = "运行参数";
        lblParamsTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlRightSide
        // 
        pnlRightSide.Controls.Add(pnlLogs);
        pnlRightSide.Controls.Add(pnlDetails);
        pnlRightSide.Dock = DockStyle.Right;
        pnlRightSide.Location = new Point(1180, 0);
        pnlRightSide.Name = "pnlRightSide";
        pnlRightSide.Size = new Size(420, 852);
        pnlRightSide.TabIndex = 1;
        // 
        // pnlLogs
        // 
        pnlLogs.Controls.Add(txtLog);
        pnlLogs.Controls.Add(chkAutoScroll);
        pnlLogs.Controls.Add(btnClearLog);
        pnlLogs.Controls.Add(lblLogTitle);
        pnlLogs.Dock = DockStyle.Fill;
        pnlLogs.Location = new Point(0, 0);
        pnlLogs.Name = "pnlLogs";
        pnlLogs.Padding = new Padding(8, 8, 12, 8);
        pnlLogs.Size = new Size(420, 632);
        pnlLogs.TabIndex = 0;
        // 
        // txtLog
        // 
        txtLog.Dock = DockStyle.Fill;
        txtLog.Location = new Point(8, 36);
        txtLog.Name = "txtLog";
        txtLog.Size = new Size(400, 588);
        txtLog.TabIndex = 3;
        txtLog.Text = "";
        // 
        // chkAutoScroll
        // 
        chkAutoScroll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        chkAutoScroll.AutoSize = true;
        chkAutoScroll.Location = new Point(314, 10);
        chkAutoScroll.Name = "chkAutoScroll";
        chkAutoScroll.Size = new Size(75, 21);
        chkAutoScroll.TabIndex = 2;
        chkAutoScroll.Text = "自动滚动";
        chkAutoScroll.UseVisualStyleBackColor = true;
        // 
        // btnClearLog
        // 
        btnClearLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClearLog.Location = new Point(232, 5);
        btnClearLog.Name = "btnClearLog";
        btnClearLog.Size = new Size(74, 28);
        btnClearLog.TabIndex = 1;
        btnClearLog.Text = "清空";
        btnClearLog.UseVisualStyleBackColor = true;
        // 
        // lblLogTitle
        // 
        lblLogTitle.Dock = DockStyle.Top;
        lblLogTitle.Location = new Point(8, 8);
        lblLogTitle.Name = "lblLogTitle";
        lblLogTitle.Size = new Size(400, 28);
        lblLogTitle.TabIndex = 0;
        lblLogTitle.Text = "实时日志";
        lblLogTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlDetails
        // 
        pnlDetails.Controls.Add(tableLayoutPanel1);
        pnlDetails.Controls.Add(lblDetailsTitle);
        pnlDetails.Dock = DockStyle.Bottom;
        pnlDetails.Location = new Point(0, 632);
        pnlDetails.Name = "pnlDetails";
        pnlDetails.Padding = new Padding(8, 0, 12, 12);
        pnlDetails.Size = new Size(420, 220);
        pnlDetails.TabIndex = 1;
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 2;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.Controls.Add(lblSelectedInventoryValue, 1, 5);
        tableLayoutPanel1.Controls.Add(lblSelectedInventoryTitle, 0, 5);
        tableLayoutPanel1.Controls.Add(lblSelectedErrorValue, 1, 4);
        tableLayoutPanel1.Controls.Add(lblSelectedErrorTitle, 0, 4);
        tableLayoutPanel1.Controls.Add(lblSelectedOfferValue, 1, 3);
        tableLayoutPanel1.Controls.Add(lblSelectedOfferTitle, 0, 3);
        tableLayoutPanel1.Controls.Add(lblSelectedMaFileValue, 1, 2);
        tableLayoutPanel1.Controls.Add(lblSelectedMaFileTitle, 0, 2);
        tableLayoutPanel1.Controls.Add(lblSelectedSteamIdValue, 1, 1);
        tableLayoutPanel1.Controls.Add(lblSelectedSteamIdTitle, 0, 1);
        tableLayoutPanel1.Controls.Add(lblSelectedAccountValue, 1, 0);
        tableLayoutPanel1.Controls.Add(lblSelectedAccountTitle, 0, 0);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(8, 28);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 6;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.Size = new Size(400, 180);
        tableLayoutPanel1.TabIndex = 1;
        // 
        // lblSelectedInventoryValue
        // 
        lblSelectedInventoryValue.AutoSize = true;
        lblSelectedInventoryValue.Location = new Point(93, 130);
        lblSelectedInventoryValue.Name = "lblSelectedInventoryValue";
        lblSelectedInventoryValue.Size = new Size(12, 17);
        lblSelectedInventoryValue.TabIndex = 11;
        lblSelectedInventoryValue.Text = "-";
        // 
        // lblSelectedInventoryTitle
        // 
        lblSelectedInventoryTitle.AutoSize = true;
        lblSelectedInventoryTitle.Location = new Point(3, 130);
        lblSelectedInventoryTitle.Name = "lblSelectedInventoryTitle";
        lblSelectedInventoryTitle.Size = new Size(68, 17);
        lblSelectedInventoryTitle.TabIndex = 10;
        lblSelectedInventoryTitle.Text = "库存摘要：";
        // 
        // lblSelectedErrorValue
        // 
        lblSelectedErrorValue.AutoSize = true;
        lblSelectedErrorValue.Location = new Point(93, 104);
        lblSelectedErrorValue.Name = "lblSelectedErrorValue";
        lblSelectedErrorValue.Size = new Size(12, 17);
        lblSelectedErrorValue.TabIndex = 9;
        lblSelectedErrorValue.Text = "-";
        // 
        // lblSelectedErrorTitle
        // 
        lblSelectedErrorTitle.AutoSize = true;
        lblSelectedErrorTitle.Location = new Point(3, 104);
        lblSelectedErrorTitle.Name = "lblSelectedErrorTitle";
        lblSelectedErrorTitle.Size = new Size(68, 17);
        lblSelectedErrorTitle.TabIndex = 8;
        lblSelectedErrorTitle.Text = "最近错误：";
        // 
        // lblSelectedOfferValue
        // 
        lblSelectedOfferValue.AutoSize = true;
        lblSelectedOfferValue.Location = new Point(93, 78);
        lblSelectedOfferValue.Name = "lblSelectedOfferValue";
        lblSelectedOfferValue.Size = new Size(12, 17);
        lblSelectedOfferValue.TabIndex = 7;
        lblSelectedOfferValue.Text = "-";
        // 
        // lblSelectedOfferTitle
        // 
        lblSelectedOfferTitle.AutoSize = true;
        lblSelectedOfferTitle.Location = new Point(3, 78);
        lblSelectedOfferTitle.Name = "lblSelectedOfferTitle";
        lblSelectedOfferTitle.Size = new Size(68, 17);
        lblSelectedOfferTitle.TabIndex = 6;
        lblSelectedOfferTitle.Text = "最近报价：";
        // 
        // lblSelectedMaFileValue
        // 
        lblSelectedMaFileValue.AutoSize = true;
        lblSelectedMaFileValue.Location = new Point(93, 52);
        lblSelectedMaFileValue.Name = "lblSelectedMaFileValue";
        lblSelectedMaFileValue.Size = new Size(12, 17);
        lblSelectedMaFileValue.TabIndex = 5;
        lblSelectedMaFileValue.Text = "-";
        // 
        // lblSelectedMaFileTitle
        // 
        lblSelectedMaFileTitle.AutoSize = true;
        lblSelectedMaFileTitle.Location = new Point(3, 52);
        lblSelectedMaFileTitle.Name = "lblSelectedMaFileTitle";
        lblSelectedMaFileTitle.Size = new Size(53, 17);
        lblSelectedMaFileTitle.TabIndex = 4;
        lblSelectedMaFileTitle.Text = "maFile：";
        // 
        // lblSelectedSteamIdValue
        // 
        lblSelectedSteamIdValue.AutoSize = true;
        lblSelectedSteamIdValue.Location = new Point(93, 26);
        lblSelectedSteamIdValue.Name = "lblSelectedSteamIdValue";
        lblSelectedSteamIdValue.Size = new Size(12, 17);
        lblSelectedSteamIdValue.TabIndex = 3;
        lblSelectedSteamIdValue.Text = "-";
        // 
        // lblSelectedSteamIdTitle
        // 
        lblSelectedSteamIdTitle.AutoSize = true;
        lblSelectedSteamIdTitle.Location = new Point(3, 26);
        lblSelectedSteamIdTitle.Name = "lblSelectedSteamIdTitle";
        lblSelectedSteamIdTitle.Size = new Size(57, 17);
        lblSelectedSteamIdTitle.TabIndex = 2;
        lblSelectedSteamIdTitle.Text = "SteamID：";
        // 
        // lblSelectedAccountValue
        // 
        lblSelectedAccountValue.AutoSize = true;
        lblSelectedAccountValue.Location = new Point(93, 0);
        lblSelectedAccountValue.Name = "lblSelectedAccountValue";
        lblSelectedAccountValue.Size = new Size(12, 17);
        lblSelectedAccountValue.TabIndex = 1;
        lblSelectedAccountValue.Text = "-";
        // 
        // lblSelectedAccountTitle
        // 
        lblSelectedAccountTitle.AutoSize = true;
        lblSelectedAccountTitle.Location = new Point(3, 0);
        lblSelectedAccountTitle.Name = "lblSelectedAccountTitle";
        lblSelectedAccountTitle.Size = new Size(44, 17);
        lblSelectedAccountTitle.TabIndex = 0;
        lblSelectedAccountTitle.Text = "账号：";
        // 
        // lblDetailsTitle
        // 
        lblDetailsTitle.Dock = DockStyle.Top;
        lblDetailsTitle.Location = new Point(8, 0);
        lblDetailsTitle.Name = "lblDetailsTitle";
        lblDetailsTitle.Size = new Size(400, 28);
        lblDetailsTitle.TabIndex = 0;
        lblDetailsTitle.Text = "当前详情";
        lblDetailsTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1600, 980);
        Controls.Add(pnlMain);
        Controls.Add(pnlBottomActions);
        Controls.Add(pnlTopStatus);
        MinimumSize = new Size(1360, 820);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "SpiderXferShell";
        pnlTopStatus.ResumeLayout(false);
        pnlTopStatus.PerformLayout();
        pnlBottomActions.ResumeLayout(false);
        pnlMain.ResumeLayout(false);
        pnlLeftMain.ResumeLayout(false);
        pnlBeTrade.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvBeTrade).EndInit();
        pnlMaster.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvMaster).EndInit();
        pnlParams.ResumeLayout(false);
        tblParams.ResumeLayout(false);
        pnlFilterValue.ResumeLayout(false);
        pnlFilterValue.PerformLayout();
        pnlItemType.ResumeLayout(false);
        pnlItemType.PerformLayout();
        pnlAcceptMode.ResumeLayout(false);
        pnlAcceptMode.PerformLayout();
        pnlTransferType.ResumeLayout(false);
        pnlTransferType.PerformLayout();
        pnlThread.ResumeLayout(false);
        pnlThread.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numThreadCount).EndInit();
        pnlRightSide.ResumeLayout(false);
        pnlLogs.ResumeLayout(false);
        pnlLogs.PerformLayout();
        pnlDetails.ResumeLayout(false);
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTopStatus;
    private Label lblAppTitle;
    private Label lblRunState;
    private Label lblThreadSummary;
    private Label lblMasterSummary;
    private Label lblBeTradeSummary;
    private Label lblModeSummary;
    private Panel pnlBottomActions;
    private Button btnImportAccounts;
    private Button btnImportMaFiles;
    private Button btnViewInventory;
    private Button btnExport;
    private Button btnStop;
    private Button btnStart;
    private Panel pnlMain;
    private Panel pnlLeftMain;
    private Panel pnlParams;
    private Label lblParamsTitle;
    private TableLayoutPanel tblParams;
    private Panel pnlThread;
    private Label lblThread;
    private NumericUpDown numThreadCount;
    private Panel pnlTransferType;
    private ComboBox cmbTransferType;
    private Label lblTransferType;
    private Panel pnlAcceptMode;
    private ComboBox cmbAcceptMode;
    private Label lblAcceptMode;
    private Panel pnlItemType;
    private ComboBox cmbItemType;
    private Label lblItemType;
    private Panel pnlFilterValue;
    private TextBox txtItemFilter;
    private Label lblItemFilter;
    private Panel pnlMaster;
    private Label lblMasterTitle;
    private DataGridView dgvMaster;
    private Panel pnlBeTrade;
    private Label lblBeTradeTitle;
    private DataGridView dgvBeTrade;
    private Panel pnlRightSide;
    private Panel pnlLogs;
    private Label lblLogTitle;
    private Button btnClearLog;
    private CheckBox chkAutoScroll;
    private RichTextBox txtLog;
    private Panel pnlDetails;
    private Label lblDetailsTitle;
    private TableLayoutPanel tableLayoutPanel1;
    private Label lblSelectedAccountTitle;
    private Label lblSelectedAccountValue;
    private Label lblSelectedSteamIdTitle;
    private Label lblSelectedSteamIdValue;
    private Label lblSelectedMaFileTitle;
    private Label lblSelectedMaFileValue;
    private Label lblSelectedOfferTitle;
    private Label lblSelectedOfferValue;
    private Label lblSelectedErrorTitle;
    private Label lblSelectedErrorValue;
    private Label lblSelectedInventoryTitle;
    private Label lblSelectedInventoryValue;
}
