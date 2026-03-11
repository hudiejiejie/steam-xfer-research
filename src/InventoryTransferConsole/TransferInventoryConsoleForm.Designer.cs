namespace InventoryTransferConsole;

partial class TransferInventoryConsoleForm
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
        // pnlTopStatus
        pnlTopStatus.Controls.Add(lblModeSummary);
        pnlTopStatus.Controls.Add(lblBeTradeSummary);
        pnlTopStatus.Controls.Add(lblMasterSummary);
        pnlTopStatus.Controls.Add(lblThreadSummary);
        pnlTopStatus.Controls.Add(lblRunState);
        pnlTopStatus.Controls.Add(lblAppTitle);
        pnlTopStatus.Dock = DockStyle.Top;
        pnlTopStatus.Padding = new Padding(16, 10, 16, 10);
        pnlTopStatus.Size = new Size(1600, 56);
        pnlTopStatus.Name = "pnlTopStatus";
        // lblModeSummary
        lblModeSummary.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblModeSummary.AutoSize = true;
        lblModeSummary.Location = new Point(1440, 20);
        lblModeSummary.Name = "lblModeSummary";
        lblModeSummary.Text = "Mode: -";
        // lblBeTradeSummary
        lblBeTradeSummary.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblBeTradeSummary.AutoSize = true;
        lblBeTradeSummary.Location = new Point(1330, 20);
        lblBeTradeSummary.Name = "lblBeTradeSummary";
        lblBeTradeSummary.Text = "Workers: 0";
        // lblMasterSummary
        lblMasterSummary.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblMasterSummary.AutoSize = true;
        lblMasterSummary.Location = new Point(1230, 20);
        lblMasterSummary.Name = "lblMasterSummary";
        lblMasterSummary.Text = "Masters: 0";
        // lblThreadSummary
        lblThreadSummary.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblThreadSummary.AutoSize = true;
        lblThreadSummary.Location = new Point(1145, 20);
        lblThreadSummary.Name = "lblThreadSummary";
        lblThreadSummary.Text = "Threads: 0";
        // lblRunState
        lblRunState.AutoSize = true;
        lblRunState.Location = new Point(280, 20);
        lblRunState.Name = "lblRunState";
        lblRunState.Text = "State: Idle";
        // lblAppTitle
        lblAppTitle.AutoSize = true;
        lblAppTitle.Location = new Point(16, 16);
        lblAppTitle.Name = "lblAppTitle";
        lblAppTitle.Text = "Steam Asset Transfer Console";
        // pnlBottomActions
        pnlBottomActions.Controls.Add(btnStart);
        pnlBottomActions.Controls.Add(btnStop);
        pnlBottomActions.Controls.Add(btnExport);
        pnlBottomActions.Controls.Add(btnViewInventory);
        pnlBottomActions.Controls.Add(btnImportMaFiles);
        pnlBottomActions.Controls.Add(btnImportAccounts);
        pnlBottomActions.Dock = DockStyle.Bottom;
        pnlBottomActions.Padding = new Padding(16, 12, 16, 12);
        pnlBottomActions.Size = new Size(1600, 72);
        pnlBottomActions.Name = "pnlBottomActions";
        // btnStart
        btnStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnStart.Location = new Point(1424, 17);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(152, 38);
        btnStart.Text = "Start Transfer";
        // btnStop
        btnStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnStop.Location = new Point(1292, 17);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(116, 38);
        btnStop.Text = "Stop";
        // btnExport
        btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnExport.Location = new Point(1160, 17);
        btnExport.Name = "btnExport";
        btnExport.Size = new Size(116, 38);
        btnExport.Text = "Export";
        // btnViewInventory
        btnViewInventory.Location = new Point(280, 17);
        btnViewInventory.Name = "btnViewInventory";
        btnViewInventory.Size = new Size(116, 38);
        btnViewInventory.Text = "Inventory";
        // btnImportMaFiles
        btnImportMaFiles.Location = new Point(148, 17);
        btnImportMaFiles.Name = "btnImportMaFiles";
        btnImportMaFiles.Size = new Size(116, 38);
        btnImportMaFiles.Text = "Import maFile";
        // btnImportAccounts
        btnImportAccounts.Location = new Point(16, 17);
        btnImportAccounts.Name = "btnImportAccounts";
        btnImportAccounts.Size = new Size(116, 38);
        btnImportAccounts.Text = "Import Accounts";
        // pnlMain
        pnlMain.Controls.Add(pnlLeftMain);
        pnlMain.Controls.Add(pnlRightSide);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Name = "pnlMain";
        // pnlLeftMain
        pnlLeftMain.Controls.Add(pnlBeTrade);
        pnlLeftMain.Controls.Add(pnlMaster);
        pnlLeftMain.Controls.Add(pnlParams);
        pnlLeftMain.Dock = DockStyle.Fill;
        pnlLeftMain.Name = "pnlLeftMain";
        // pnlBeTrade
        pnlBeTrade.Controls.Add(dgvBeTrade);
        pnlBeTrade.Controls.Add(lblBeTradeTitle);
        pnlBeTrade.Dock = DockStyle.Fill;
        pnlBeTrade.Padding = new Padding(12, 0, 12, 12);
        pnlBeTrade.Name = "pnlBeTrade";
        // dgvBeTrade
        dgvBeTrade.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvBeTrade.Dock = DockStyle.Fill;
        dgvBeTrade.Name = "dgvBeTrade";
        // lblBeTradeTitle
        lblBeTradeTitle.Dock = DockStyle.Top;
        lblBeTradeTitle.Height = 28;
        lblBeTradeTitle.Name = "lblBeTradeTitle";
        lblBeTradeTitle.Text = "Workers / Senders";
        lblBeTradeTitle.TextAlign = ContentAlignment.MiddleLeft;
        // pnlMaster
        pnlMaster.Controls.Add(dgvMaster);
        pnlMaster.Controls.Add(lblMasterTitle);
        pnlMaster.Dock = DockStyle.Top;
        pnlMaster.Height = 250;
        pnlMaster.Padding = new Padding(12, 0, 12, 8);
        pnlMaster.Name = "pnlMaster";
        // dgvMaster
        dgvMaster.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvMaster.Dock = DockStyle.Fill;
        dgvMaster.Name = "dgvMaster";
        // lblMasterTitle
        lblMasterTitle.Dock = DockStyle.Top;
        lblMasterTitle.Height = 28;
        lblMasterTitle.Name = "lblMasterTitle";
        lblMasterTitle.Text = "Masters / Receivers";
        lblMasterTitle.TextAlign = ContentAlignment.MiddleLeft;
        // pnlParams
        pnlParams.Controls.Add(tblParams);
        pnlParams.Controls.Add(lblParamsTitle);
        pnlParams.Dock = DockStyle.Top;
        pnlParams.Height = 150;
        pnlParams.Padding = new Padding(12);
        pnlParams.Name = "pnlParams";
        // lblParamsTitle
        lblParamsTitle.Dock = DockStyle.Top;
        lblParamsTitle.Height = 28;
        lblParamsTitle.Name = "lblParamsTitle";
        lblParamsTitle.Text = "Runtime Parameters";
        lblParamsTitle.TextAlign = ContentAlignment.MiddleLeft;
        // tblParams
        tblParams.ColumnCount = 3;
        tblParams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
        tblParams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
        tblParams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
        tblParams.RowCount = 2;
        tblParams.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tblParams.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tblParams.Dock = DockStyle.Fill;
        tblParams.Location = new Point(12, 40);
        tblParams.Name = "tblParams";
        tblParams.Controls.Add(pnlThread, 0, 0);
        tblParams.Controls.Add(pnlTransferType, 1, 0);
        tblParams.Controls.Add(pnlAcceptMode, 2, 0);
        tblParams.Controls.Add(pnlItemType, 0, 1);
        tblParams.Controls.Add(pnlFilterValue, 1, 1);
        // pnlThread
        pnlThread.Controls.Add(numThreadCount);
        pnlThread.Controls.Add(lblThread);
        pnlThread.Dock = DockStyle.Fill;
        pnlThread.Padding = new Padding(4);
        // lblThread
        lblThread.AutoSize = true;
        lblThread.Location = new Point(4, 0);
        lblThread.Name = "lblThread";
        lblThread.Text = "Threads";
        // numThreadCount
        numThreadCount.Dock = DockStyle.Bottom;
        numThreadCount.Minimum = 1;
        numThreadCount.Maximum = 64;
        numThreadCount.Name = "numThreadCount";
        // pnlTransferType
        pnlTransferType.Controls.Add(cmbTransferType);
        pnlTransferType.Controls.Add(lblTransferType);
        pnlTransferType.Dock = DockStyle.Fill;
        pnlTransferType.Padding = new Padding(4);
        // lblTransferType
        lblTransferType.AutoSize = true;
        lblTransferType.Location = new Point(4, 0);
        lblTransferType.Name = "lblTransferType";
        lblTransferType.Text = "Transfer Mode";
        // cmbTransferType
        cmbTransferType.Dock = DockStyle.Bottom;
        cmbTransferType.Name = "cmbTransferType";
        // pnlAcceptMode
        pnlAcceptMode.Controls.Add(cmbAcceptMode);
        pnlAcceptMode.Controls.Add(lblAcceptMode);
        pnlAcceptMode.Dock = DockStyle.Fill;
        pnlAcceptMode.Padding = new Padding(4);
        // lblAcceptMode
        lblAcceptMode.AutoSize = true;
        lblAcceptMode.Location = new Point(4, 0);
        lblAcceptMode.Name = "lblAcceptMode";
        lblAcceptMode.Text = "Accept Mode";
        // cmbAcceptMode
        cmbAcceptMode.Dock = DockStyle.Bottom;
        cmbAcceptMode.Name = "cmbAcceptMode";
        // pnlItemType
        pnlItemType.Controls.Add(cmbItemType);
        pnlItemType.Controls.Add(lblItemType);
        pnlItemType.Dock = DockStyle.Fill;
        pnlItemType.Padding = new Padding(4);
        // lblItemType
        lblItemType.AutoSize = true;
        lblItemType.Location = new Point(4, 0);
        lblItemType.Name = "lblItemType";
        lblItemType.Text = "Filter Type";
        // cmbItemType
        cmbItemType.Dock = DockStyle.Bottom;
        cmbItemType.Name = "cmbItemType";
        // pnlFilterValue
        pnlFilterValue.Controls.Add(txtItemFilter);
        pnlFilterValue.Controls.Add(lblItemFilter);
        pnlFilterValue.Dock = DockStyle.Fill;
        pnlFilterValue.Padding = new Padding(4);
        // lblItemFilter
        lblItemFilter.AutoSize = true;
        lblItemFilter.Location = new Point(4, 0);
        lblItemFilter.Name = "lblItemFilter";
        lblItemFilter.Text = "Filter Value";
        // txtItemFilter
        txtItemFilter.Dock = DockStyle.Bottom;
        txtItemFilter.Name = "txtItemFilter";
        txtItemFilter.PlaceholderText = "keyword...";
        // pnlRightSide
        pnlRightSide.Controls.Add(pnlLogs);
        pnlRightSide.Controls.Add(pnlDetails);
        pnlRightSide.Dock = DockStyle.Right;
        pnlRightSide.Width = 420;
        pnlRightSide.Name = "pnlRightSide";
        // pnlLogs
        pnlLogs.Controls.Add(txtLog);
        pnlLogs.Controls.Add(chkAutoScroll);
        pnlLogs.Controls.Add(btnClearLog);
        pnlLogs.Controls.Add(lblLogTitle);
        pnlLogs.Dock = DockStyle.Fill;
        pnlLogs.Padding = new Padding(8, 8, 12, 8);
        pnlLogs.Name = "pnlLogs";
        // lblLogTitle
        lblLogTitle.Dock = DockStyle.Top;
        lblLogTitle.Height = 28;
        lblLogTitle.Name = "lblLogTitle";
        lblLogTitle.Text = "Live Logs";
        lblLogTitle.TextAlign = ContentAlignment.MiddleLeft;
        // btnClearLog
        btnClearLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClearLog.Location = new Point(232, 5);
        btnClearLog.Name = "btnClearLog";
        btnClearLog.Size = new Size(74, 28);
        btnClearLog.Text = "Clear";
        // chkAutoScroll
        chkAutoScroll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        chkAutoScroll.AutoSize = true;
        chkAutoScroll.Location = new Point(314, 10);
        chkAutoScroll.Name = "chkAutoScroll";
        chkAutoScroll.Text = "Auto Scroll";
        // txtLog
        txtLog.Dock = DockStyle.Fill;
        txtLog.Location = new Point(8, 36);
        txtLog.Name = "txtLog";
        txtLog.Text = "";
        // pnlDetails
        pnlDetails.Controls.Add(tableLayoutPanel1);
        pnlDetails.Controls.Add(lblDetailsTitle);
        pnlDetails.Dock = DockStyle.Bottom;
        pnlDetails.Height = 220;
        pnlDetails.Padding = new Padding(8, 0, 12, 12);
        pnlDetails.Name = "pnlDetails";
        // lblDetailsTitle
        lblDetailsTitle.Dock = DockStyle.Top;
        lblDetailsTitle.Height = 28;
        lblDetailsTitle.Name = "lblDetailsTitle";
        lblDetailsTitle.Text = "Current Details";
        lblDetailsTitle.TextAlign = ContentAlignment.MiddleLeft;
        // tableLayoutPanel1
        tableLayoutPanel1.ColumnCount = 2;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.RowCount = 6;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(8, 28);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.Controls.Add(lblSelectedAccountTitle, 0, 0);
        tableLayoutPanel1.Controls.Add(lblSelectedAccountValue, 1, 0);
        tableLayoutPanel1.Controls.Add(lblSelectedSteamIdTitle, 0, 1);
        tableLayoutPanel1.Controls.Add(lblSelectedSteamIdValue, 1, 1);
        tableLayoutPanel1.Controls.Add(lblSelectedMaFileTitle, 0, 2);
        tableLayoutPanel1.Controls.Add(lblSelectedMaFileValue, 1, 2);
        tableLayoutPanel1.Controls.Add(lblSelectedOfferTitle, 0, 3);
        tableLayoutPanel1.Controls.Add(lblSelectedOfferValue, 1, 3);
        tableLayoutPanel1.Controls.Add(lblSelectedErrorTitle, 0, 4);
        tableLayoutPanel1.Controls.Add(lblSelectedErrorValue, 1, 4);
        tableLayoutPanel1.Controls.Add(lblSelectedInventoryTitle, 0, 5);
        tableLayoutPanel1.Controls.Add(lblSelectedInventoryValue, 1, 5);
        // detail labels
        lblSelectedAccountTitle.AutoSize = true; lblSelectedAccountTitle.Text = "Account:";
        lblSelectedSteamIdTitle.AutoSize = true; lblSelectedSteamIdTitle.Text = "SteamID:";
        lblSelectedMaFileTitle.AutoSize = true; lblSelectedMaFileTitle.Text = "maFile:";
        lblSelectedOfferTitle.AutoSize = true; lblSelectedOfferTitle.Text = "Offer:";
        lblSelectedErrorTitle.AutoSize = true; lblSelectedErrorTitle.Text = "Error:";
        lblSelectedInventoryTitle.AutoSize = true; lblSelectedInventoryTitle.Text = "Inventory:";
        lblSelectedAccountValue.AutoSize = true; lblSelectedAccountValue.Text = "-";
        lblSelectedSteamIdValue.AutoSize = true; lblSelectedSteamIdValue.Text = "-";
        lblSelectedMaFileValue.AutoSize = true; lblSelectedMaFileValue.Text = "-";
        lblSelectedOfferValue.AutoSize = true; lblSelectedOfferValue.Text = "-";
        lblSelectedErrorValue.AutoSize = true; lblSelectedErrorValue.Text = "-";
        lblSelectedInventoryValue.AutoSize = true; lblSelectedInventoryValue.Text = "-";
        // Form
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1600, 980);
        Controls.Add(pnlMain);
        Controls.Add(pnlBottomActions);
        Controls.Add(pnlTopStatus);
        MinimumSize = new Size(1360, 820);
        Name = "TransferInventoryConsoleForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Steam Asset Transfer Console";
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

