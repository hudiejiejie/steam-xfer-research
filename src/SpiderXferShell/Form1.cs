using System.Drawing.Drawing2D;

namespace SpiderXferShell;

public partial class Form1 : Form
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

    public Form1()
    {
        InitializeComponent();
        ApplyTheme();
        SeedTables();
        BindEvents();
        AppendLog("INFO", "SpiderXferShell 已启动（UI shell prototype）");
    }

    private void ApplyTheme()
    {
        BackColor = Bg;
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        ForeColor = TextMain;

        lblAppTitle.ForeColor = TextMain;
        lblRunState.ForeColor = Success;
        lblThreadSummary.ForeColor = TextSub;
        lblMasterSummary.ForeColor = TextSub;
        lblBeTradeSummary.ForeColor = TextSub;
        lblModeSummary.ForeColor = TextSub;

        foreach (var panel in new[] { pnlTopStatus, pnlBottomActions, pnlParams, pnlMaster, pnlBeTrade, pnlLogs, pnlDetails, pnlRightSide, pnlLeftMain })
        {
            panel.BackColor = panel == pnlRightSide || panel == pnlLeftMain ? Bg : PanelBg;
        }

        foreach (var title in new[] { lblParamsTitle, lblMasterTitle, lblBeTradeTitle, lblLogTitle, lblDetailsTitle })
        {
            title.ForeColor = TextMain;
            title.Font = new Font("Bahnschrift SemiBold", 10.5F, FontStyle.Bold, GraphicsUnit.Point);
        }

        foreach (var label in new[] { lblThread, lblTransferType, lblAcceptMode, lblItemType, lblItemFilter, lblSelectedAccountTitle, lblSelectedSteamIdTitle, lblSelectedMaFileTitle, lblSelectedOfferTitle, lblSelectedErrorTitle, lblSelectedInventoryTitle })
        {
            label.ForeColor = TextSub;
        }

        foreach (var value in new[] { lblSelectedAccountValue, lblSelectedSteamIdValue, lblSelectedMaFileValue, lblSelectedOfferValue, lblSelectedErrorValue, lblSelectedInventoryValue })
        {
            value.ForeColor = TextMain;
        }

        StyleButton(btnImportAccounts, PanelBg2, TextMain);
        StyleButton(btnImportMaFiles, PanelBg2, TextMain);
        StyleButton(btnViewInventory, PanelBg2, TextMain);
        StyleButton(btnExport, PanelBg2, TextMain);
        StyleButton(btnStop, ColorTranslator.FromHtml("#3A2722"), Warning);
        StyleButton(btnStart, ColorTranslator.FromHtml("#10314D"), Accent);
        StyleButton(btnClearLog, PanelBg2, TextSub, compact: true);

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
        dgvMaster.Columns.Add("Account", "账号");
        dgvMaster.Columns.Add("SteamId", "SteamID");
        dgvMaster.Columns.Add("LoginState", "登录状态");
        dgvMaster.Columns.Add("Pending", "待接受");
        dgvMaster.Columns.Add("Assigned", "已分配");
        dgvMaster.Columns.Add("Limit", "上限");
        dgvMaster.Columns.Add("MaFile", "maFile");
        dgvMaster.Rows.Add("master_01", "7656119...001", "在线", "3", "87", "200", "已绑定");
        dgvMaster.Rows.Add("master_02", "7656119...002", "离线", "0", "0", "200", "缺失");

        dgvBeTrade.Columns.Clear();
        dgvBeTrade.Columns.Add("Account", "账号");
        dgvBeTrade.Columns.Add("LoginState", "登录状态");
        dgvBeTrade.Columns.Add("Inventory", "库存数");
        dgvBeTrade.Columns.Add("Tradable", "可交易");
        dgvBeTrade.Columns.Add("Cooldown", "冷却");
        dgvBeTrade.Columns.Add("Sent", "已发报价");
        dgvBeTrade.Columns.Add("TaskState", "当前状态");
        dgvBeTrade.Columns.Add("MaFile", "maFile");
        dgvBeTrade.Rows.Add("worker_01", "在线", "154", "147", "7", "2", "已发报价", "已绑定");
        dgvBeTrade.Rows.Add("worker_02", "在线", "91", "88", "3", "0", "拉库存完成", "已绑定");
        dgvBeTrade.Rows.Add("worker_03", "失败", "0", "0", "0", "0", "登录失败", "缺失");

        cmbTransferType.Items.AddRange(new object[] { "边发边接", "全发后统一接受" });
        cmbAcceptMode.Items.AddRange(new object[] { "自动接受", "延后接受", "手动确认" });
        cmbItemType.Items.AddRange(new object[] { "按类型筛选", "按名称精确" });
        cmbTransferType.SelectedIndex = 0;
        cmbAcceptMode.SelectedIndex = 0;
        cmbItemType.SelectedIndex = 0;
        numThreadCount.Value = 8;

        lblThreadSummary.Text = "线程：8";
        lblMasterSummary.Text = "主库号：2";
        lblBeTradeSummary.Text = "待转号：3";
        lblModeSummary.Text = "模式：边发边接";
        lblSelectedAccountValue.Text = "worker_01";
        lblSelectedSteamIdValue.Text = "7656119...003";
        lblSelectedMaFileValue.Text = "已绑定";
        lblSelectedOfferValue.Text = "#1039284";
        lblSelectedErrorValue.Text = "无";
        lblSelectedInventoryValue.Text = "总 154 / 可交易 147 / 冷却 7";
    }

    private void BindEvents()
    {
        btnImportAccounts.Click += (_, _) => AppendLog("INFO", "点击：导入账号（占位）");
        btnImportMaFiles.Click += (_, _) => AppendLog("INFO", "点击：导入 maFile（占位）");
        btnViewInventory.Click += (_, _) => AppendLog("INFO", "点击：查看库存（占位）");
        btnExport.Click += (_, _) => AppendLog("INFO", "点击：导出结果（占位）");
        btnStart.Click += (_, _) =>
        {
            lblRunState.Text = "状态：运行中";
            lblRunState.ForeColor = Success;
            AppendLog("INFO", "开始转移（UI shell 占位）");
        };
        btnStop.Click += (_, _) =>
        {
            lblRunState.Text = "状态：停止中";
            lblRunState.ForeColor = Warning;
            AppendLog("WARN", "停止任务（UI shell 占位）");
        };
        btnClearLog.Click += (_, _) => txtLog.Clear();
        chkAutoScroll.Checked = true;
        dgvBeTrade.SelectionChanged += (_, _) => SyncSelectionDetails();
        dgvMaster.SelectionChanged += (_, _) => SyncSelectionDetails();
        cmbTransferType.SelectedIndexChanged += (_, _) => lblModeSummary.Text = $"模式：{cmbTransferType.Text}";
        numThreadCount.ValueChanged += (_, _) => lblThreadSummary.Text = $"线程：{numThreadCount.Value}";
    }

    private void SyncSelectionDetails()
    {
        var row = dgvBeTrade.CurrentRow ?? dgvMaster.CurrentRow;
        if (row == null) return;
        lblSelectedAccountValue.Text = row.Cells[0].Value?.ToString() ?? "-";
        lblSelectedSteamIdValue.Text = dgvBeTrade.CurrentRow != null ? "7656119...003" : row.Cells.Count > 1 ? row.Cells[1].Value?.ToString() ?? "-" : "-";
        lblSelectedMaFileValue.Text = row.Cells[row.Cells.Count - 1].Value?.ToString() ?? "-";
        lblSelectedOfferValue.Text = dgvBeTrade.CurrentRow != null ? "#1039284" : "-";
        lblSelectedErrorValue.Text = dgvBeTrade.CurrentRow != null && (row.Cells[1].Value?.ToString() == "失败") ? "登录失败" : "无";
        lblSelectedInventoryValue.Text = dgvBeTrade.CurrentRow != null ? $"总 {row.Cells[2].Value} / 可交易 {row.Cells[3].Value} / 冷却 {row.Cells[4].Value}" : "无";
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
