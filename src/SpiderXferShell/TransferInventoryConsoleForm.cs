namespace SpiderXferShell;

public partial class TransferInventoryConsoleForm : Form
{
    private readonly Color Bg = ColorTranslator.FromHtml("#0F1115");
    private readonly Color PanelBg = ColorTranslator.FromHtml("#171A21");
    private readonly Color PanelBg2 = ColorTranslator.FromHtml("#1D2230");
    private readonly Color TextMain = ColorTranslator.FromHtml("#E6EAF2");
    private readonly Color TextSub = ColorTranslator.FromHtml("#9AA4B2");
    private readonly Color Accent = ColorTranslator.FromHtml("#4DA3FF");
    private readonly Color Success = ColorTranslator.FromHtml("#2ECC71");
    private readonly Color Warning = ColorTranslator.FromHtml("#F5A524");
    private readonly Color Danger = ColorTranslator.FromHtml("#FF5D5D");

    public TransferInventoryConsoleForm()
    {
        InitializeComponent();
        ApplyTheme();
        SeedTables();
        BindEvents();
        AppendLog("INFO", "Shell started.");
    }

    private void ApplyTheme()
    {
        BackColor = Bg;
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        ForeColor = TextMain;

        foreach (var panel in new[] { pnlTopStatus, pnlBottomActions, pnlParams, pnlMaster, pnlBeTrade, pnlLogs, pnlDetails })
            panel.BackColor = PanelBg;

        pnlMain.BackColor = Bg;
        pnlLeftMain.BackColor = Bg;
        pnlRightSide.BackColor = Bg;

        foreach (var title in new[] { lblParamsTitle, lblMasterTitle, lblBeTradeTitle, lblLogTitle, lblDetailsTitle, lblAppTitle })
            title.ForeColor = TextMain;

        lblAppTitle.Font = new Font("Bahnschrift SemiBold", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
        lblRunState.ForeColor = Success;
        foreach (var s in new[] { lblThreadSummary, lblMasterSummary, lblBeTradeSummary, lblModeSummary })
            s.ForeColor = TextSub;

        foreach (var label in new[] { lblThread, lblTransferType, lblAcceptMode, lblItemType, lblItemFilter, lblSelectedAccountTitle, lblSelectedSteamIdTitle, lblSelectedMaFileTitle, lblSelectedOfferTitle, lblSelectedErrorTitle, lblSelectedInventoryTitle })
            label.ForeColor = TextSub;
        foreach (var value in new[] { lblSelectedAccountValue, lblSelectedSteamIdValue, lblSelectedMaFileValue, lblSelectedOfferValue, lblSelectedErrorValue, lblSelectedInventoryValue })
            value.ForeColor = TextMain;

        StyleButton(btnImportAccounts, PanelBg2, TextMain);
        StyleButton(btnImportMaFiles, PanelBg2, TextMain);
        StyleButton(btnViewInventory, PanelBg2, TextMain);
        StyleButton(btnExport, PanelBg2, TextMain);
        StyleButton(btnStop, ColorTranslator.FromHtml("#3A2722"), Warning);
        StyleButton(btnStart, ColorTranslator.FromHtml("#10314D"), Accent);
        StyleButton(btnClearLog, PanelBg2, TextSub, true);

        StyleInput(txtItemFilter);
        StyleCombo(cmbTransferType);
        StyleCombo(cmbAcceptMode);
        StyleCombo(cmbItemType);
        StyleNumeric(numThreadCount);
        StyleCheckbox(chkAutoScroll);
        StyleLog(txtLog);
        StyleGrid(dgvMaster);
        StyleGrid(dgvBeTrade);
    }

    private void StyleButton(Button btn, Color back, Color fore, bool compact = false)
    {
        btn.BackColor = back;
        btn.ForeColor = fore;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderColor = Accent;
        btn.FlatAppearance.BorderSize = 1;
        btn.Font = new Font("Microsoft YaHei UI", compact ? 8.5F : 9F, FontStyle.Regular, GraphicsUnit.Point);
        btn.Height = compact ? 30 : 38;
        btn.Cursor = Cursors.Hand;
    }

    private void StyleInput(TextBox tb)
    {
        tb.BackColor = PanelBg2;
        tb.ForeColor = TextMain;
        tb.BorderStyle = BorderStyle.FixedSingle;
    }

    private void StyleCombo(ComboBox cb)
    {
        cb.BackColor = PanelBg2;
        cb.ForeColor = TextMain;
        cb.FlatStyle = FlatStyle.Flat;
        cb.DropDownStyle = ComboBoxStyle.DropDownList;
    }

    private void StyleNumeric(NumericUpDown nud)
    {
        nud.BackColor = PanelBg2;
        nud.ForeColor = TextMain;
        nud.BorderStyle = BorderStyle.FixedSingle;
    }

    private void StyleCheckbox(CheckBox chk)
    {
        chk.ForeColor = TextSub;
        chk.BackColor = PanelBg;
    }

    private void StyleLog(RichTextBox rtb)
    {
        rtb.BackColor = ColorTranslator.FromHtml("#121722");
        rtb.ForeColor = TextMain;
        rtb.BorderStyle = BorderStyle.None;
        rtb.Font = new Font("Cascadia Mono", 9F, FontStyle.Regular, GraphicsUnit.Point);
        rtb.ReadOnly = true;
    }

    private void StyleGrid(DataGridView dgv)
    {
        dgv.BackgroundColor = PanelBg2;
        dgv.BorderStyle = BorderStyle.None;
        dgv.GridColor = ColorTranslator.FromHtml("#273043");
        dgv.EnableHeadersVisualStyles = false;
        dgv.RowHeadersVisible = false;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.AllowUserToResizeRows = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#151B26");
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextMain;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        dgv.DefaultCellStyle.BackColor = PanelBg2;
        dgv.DefaultCellStyle.ForeColor = TextMain;
        dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#1E3554");
        dgv.DefaultCellStyle.SelectionForeColor = Color.White;
        dgv.DefaultCellStyle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        dgv.RowTemplate.Height = 30;
    }

    private void SeedTables()
    {
        dgvMaster.Columns.Clear();
        dgvMaster.Columns.Add("Account", "Account");
        dgvMaster.Columns.Add("SteamId", "SteamID");
        dgvMaster.Columns.Add("LoginState", "Login");
        dgvMaster.Columns.Add("Pending", "Pending");
        dgvMaster.Columns.Add("Assigned", "Assigned");
        dgvMaster.Columns.Add("Limit", "Limit");
        dgvMaster.Columns.Add("MaFile", "maFile");
        dgvMaster.Rows.Add("master_01", "7656119...001", "Online", "3", "87", "200", "Bound");
        dgvMaster.Rows.Add("master_02", "7656119...002", "Offline", "0", "0", "200", "Missing");

        dgvBeTrade.Columns.Clear();
        dgvBeTrade.Columns.Add("Account", "Account");
        dgvBeTrade.Columns.Add("LoginState", "Login");
        dgvBeTrade.Columns.Add("Inventory", "Inventory");
        dgvBeTrade.Columns.Add("Tradable", "Tradable");
        dgvBeTrade.Columns.Add("Cooldown", "Cooldown");
        dgvBeTrade.Columns.Add("Sent", "Sent");
        dgvBeTrade.Columns.Add("TaskState", "Task State");
        dgvBeTrade.Columns.Add("MaFile", "maFile");
        dgvBeTrade.Rows.Add("worker_01", "Online", "154", "147", "7", "2", "Offer Sent", "Bound");
        dgvBeTrade.Rows.Add("worker_02", "Online", "91", "88", "3", "0", "Inventory Ready", "Bound");
        dgvBeTrade.Rows.Add("worker_03", "Failed", "0", "0", "0", "0", "Login Failed", "Missing");

        cmbTransferType.Items.AddRange(new object[] { "Send+Accept", "Send All Then Accept" });
        cmbAcceptMode.Items.AddRange(new object[] { "Auto Accept", "Delayed Accept", "Manual Confirm" });
        cmbItemType.Items.AddRange(new object[] { "Filter by Type", "Filter by Exact Name" });
        cmbTransferType.SelectedIndex = 0;
        cmbAcceptMode.SelectedIndex = 0;
        cmbItemType.SelectedIndex = 0;
        numThreadCount.Value = 8;

        lblThreadSummary.Text = "Threads: 8";
        lblMasterSummary.Text = "Masters: 2";
        lblBeTradeSummary.Text = "Workers: 3";
        lblModeSummary.Text = "Mode: Send+Accept";
        lblSelectedAccountValue.Text = "worker_01";
        lblSelectedSteamIdValue.Text = "7656119...003";
        lblSelectedMaFileValue.Text = "Bound";
        lblSelectedOfferValue.Text = "#1039284";
        lblSelectedErrorValue.Text = "None";
        lblSelectedInventoryValue.Text = "154 total / 147 tradable / 7 cooldown";
    }

    private void BindEvents()
    {
        btnImportAccounts.Click += (_, _) => AppendLog("INFO", "Import accounts clicked.");
        btnImportMaFiles.Click += (_, _) => AppendLog("INFO", "Import maFiles clicked.");
        btnViewInventory.Click += (_, _) => AppendLog("INFO", "View inventory clicked.");
        btnExport.Click += (_, _) => AppendLog("INFO", "Export clicked.");
        btnStart.Click += (_, _) =>
        {
            lblRunState.Text = "State: Running";
            lblRunState.ForeColor = Success;
            AppendLog("INFO", "Start transfer clicked.");
        };
        btnStop.Click += (_, _) =>
        {
            lblRunState.Text = "State: Stopping";
            lblRunState.ForeColor = Warning;
            AppendLog("WARN", "Stop transfer clicked.");
        };
        btnClearLog.Click += (_, _) => txtLog.Clear();
        chkAutoScroll.Checked = true;
        dgvBeTrade.SelectionChanged += (_, _) => SyncSelectionDetails();
        dgvMaster.SelectionChanged += (_, _) => SyncSelectionDetails();
        cmbTransferType.SelectedIndexChanged += (_, _) => lblModeSummary.Text = $"Mode: {cmbTransferType.Text}";
        numThreadCount.ValueChanged += (_, _) => lblThreadSummary.Text = $"Threads: {numThreadCount.Value}";
    }

    private void SyncSelectionDetails()
    {
        var row = dgvBeTrade.CurrentRow ?? dgvMaster.CurrentRow;
        if (row == null) return;

        lblSelectedAccountValue.Text = row.Cells[0].Value?.ToString() ?? "-";
        lblSelectedSteamIdValue.Text = dgvBeTrade.CurrentRow != null ? "7656119...003" : (row.Cells.Count > 1 ? row.Cells[1].Value?.ToString() ?? "-" : "-");
        lblSelectedMaFileValue.Text = row.Cells[row.Cells.Count - 1].Value?.ToString() ?? "-";
        lblSelectedOfferValue.Text = dgvBeTrade.CurrentRow != null ? "#1039284" : "-";
        lblSelectedErrorValue.Text = dgvBeTrade.CurrentRow != null && (row.Cells[1].Value?.ToString() == "Failed") ? "Login Failed" : "None";
        lblSelectedInventoryValue.Text = dgvBeTrade.CurrentRow != null
            ? $"{row.Cells[2].Value} total / {row.Cells[3].Value} tradable / {row.Cells[4].Value} cooldown"
            : "N/A";
    }

    private void AppendLog(string level, string message)
    {
        var color = level switch
        {
            "ERROR" => Danger,
            "WARN" => Warning,
            _ => TextMain
        };

        txtLog.SelectionStart = txtLog.TextLength;
        txtLog.SelectionLength = 0;
        txtLog.SelectionColor = TextSub;
        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ");
        txtLog.SelectionColor = color;
        txtLog.AppendText($"[{level}] ");
        txtLog.SelectionColor = TextMain;
        txtLog.AppendText(message + Environment.NewLine);
        txtLog.SelectionColor = TextMain;

        if (chkAutoScroll.Checked)
        {
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }
    }
}
