using InventoryTransferConsole.Forms;
using InventoryTransferConsole.Models;
using InventoryTransferConsole.Services;

namespace InventoryTransferConsole;

public partial class TransferInventoryConsoleForm : Form, ILogSink
{
    private readonly Color Bg = ColorTranslator.FromHtml("#111317");
    private readonly Color PanelBg = ColorTranslator.FromHtml("#171B22");
    private readonly Color PanelBg2 = ColorTranslator.FromHtml("#1E2430");
    private readonly Color TextMain = ColorTranslator.FromHtml("#ECEFF4");
    private readonly Color TextSub = ColorTranslator.FromHtml("#A7AFBD");
    private readonly Color Accent = ColorTranslator.FromHtml("#5F86B3");
    private readonly Color Success = ColorTranslator.FromHtml("#7EA37B");
    private readonly Color Warning = ColorTranslator.FromHtml("#B79B67");
    private readonly Color Danger = ColorTranslator.FromHtml("#B76D6D");

    private readonly ITransferController controller;
    private readonly ISettingsService settingsService;
    private DashboardSnapshot snapshot = new();
    private AppSettings appSettings = new();
    private bool suppressSettingsEvents;

    public TransferInventoryConsoleForm()
    {
        controller = new MockTransferController();
        settingsService = new JsonSettingsService();
        InitializeComponent();
        ApplyTheme();
        LoadDashboard();
        LoadSettingsIntoUi();
        BindEvents();
        UpdateActionState(false);
        Info($"控制台外壳已加载。配置文件：{settingsService.SettingsPath}");
    }

    private void LoadDashboard()
    {
        snapshot = controller.LoadSnapshot();
        RenderSnapshot();
    }

    private void RenderSnapshot()
    {
        SeedTables(snapshot);
        ApplyGridStateVisuals();
        lblMasterSummary.Text = $"主库号：{snapshot.Masters.Count}";
        lblBeTradeSummary.Text = $"待转号：{snapshot.Workers.Count}";
        lblRunState.Text = $"状态：{snapshot.RunState}";
        lblModeSummary.Text = $"模式：{snapshot.ModeSummary}";

        if (snapshot.Workers.Count > 0 && dgvBeTrade.Rows.Count > 0)
        {
            dgvBeTrade.Rows[0].Selected = true;
            SyncWorkerDetails(snapshot.Workers[0]);
        }
        else if (snapshot.Masters.Count > 0 && dgvMaster.Rows.Count > 0)
        {
            dgvMaster.Rows[0].Selected = true;
            lblSelectedAccountValue.Text = snapshot.Masters[0].Account;
            lblSelectedSteamIdValue.Text = snapshot.Masters[0].SteamId;
            lblSelectedMaFileValue.Text = TranslateMaFile(snapshot.Masters[0].MaFile);
            lblSelectedOfferValue.Text = "-";
            lblSelectedErrorValue.Text = "无";
            lblSelectedInventoryValue.Text = "无";
        }
    }

    private void LoadSettingsIntoUi()
    {
        suppressSettingsEvents = true;
        appSettings = settingsService.Load();

        cmbTransferType.Items.Clear();
        cmbTransferType.Items.AddRange(new object[] { "边发边接", "全发后统一接受" });
        cmbAcceptMode.Items.Clear();
        cmbAcceptMode.Items.AddRange(new object[] { "自动接受", "延后接受", "手动确认" });
        cmbItemType.Items.Clear();
        cmbItemType.Items.AddRange(new object[] { "按类型筛选", "按名称精确" });

        numThreadCount.Value = Math.Max(numThreadCount.Minimum, Math.Min(numThreadCount.Maximum, appSettings.Runtime.ThreadCount));
        cmbTransferType.SelectedIndex = ClampIndex(appSettings.Runtime.TransferTypeIndex, cmbTransferType.Items.Count);
        cmbAcceptMode.SelectedIndex = ClampIndex(appSettings.Runtime.AcceptModeIndex, cmbAcceptMode.Items.Count);
        cmbItemType.SelectedIndex = ClampIndex(appSettings.Runtime.ItemTypeIndex, cmbItemType.Items.Count);
        txtItemFilter.Text = appSettings.Runtime.ItemFilterValue;

        lblThreadSummary.Text = $"线程：{numThreadCount.Value}";
        if (cmbTransferType.SelectedIndex >= 0)
            lblModeSummary.Text = $"模式：{cmbTransferType.Text}";
        suppressSettingsEvents = false;
    }

    private static int ClampIndex(int index, int count)
    {
        if (count <= 0) return -1;
        if (index < 0 || index >= count) return 0;
        return index;
    }

    private RuntimeSettings ReadRuntimeSettings()
    {
        return new RuntimeSettings
        {
            ThreadCount = (int)numThreadCount.Value,
            TransferTypeIndex = cmbTransferType.SelectedIndex,
            AcceptModeIndex = cmbAcceptMode.SelectedIndex,
            ItemTypeIndex = cmbItemType.SelectedIndex,
            ItemFilterValue = txtItemFilter.Text.Trim()
        };
    }

    private void SaveSettingsFromUi()
    {
        if (suppressSettingsEvents) return;
        appSettings.Runtime = ReadRuntimeSettings();
        settingsService.Save(appSettings);
    }

    private void UpdateActionState(bool isRunning)
    {
        btnStart.Enabled = !isRunning;
        btnStop.Enabled = isRunning;
        btnImportAccounts.Enabled = !isRunning;
        btnImportMaFiles.Enabled = !isRunning;
        btnExport.Enabled = !isRunning;
        btnViewInventory.Enabled = !isRunning;
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
        dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#202734");
        dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#2A3443");
        dgv.DefaultCellStyle.SelectionForeColor = Color.White;
        dgv.DefaultCellStyle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        dgv.RowTemplate.Height = 30;
    }

    private void SeedTables(DashboardSnapshot data)
    {
        dgvMaster.Columns.Clear();
        dgvMaster.Columns.Add("Account", "账号");
        dgvMaster.Columns.Add("SteamId", "SteamID");
        dgvMaster.Columns.Add("LoginState", "登录状态");
        dgvMaster.Columns.Add("Pending", "待接受");
        dgvMaster.Columns.Add("Assigned", "已分配");
        dgvMaster.Columns.Add("Limit", "上限");
        dgvMaster.Columns.Add("MaFile", "maFile");
        foreach (var row in data.Masters)
            dgvMaster.Rows.Add(row.Account, row.SteamId, TranslateLoginState(row.LoginState), row.Pending, row.Assigned, row.Limit, TranslateMaFile(row.MaFile));

        dgvBeTrade.Columns.Clear();
        dgvBeTrade.Columns.Add("Account", "账号");
        dgvBeTrade.Columns.Add("LoginState", "登录状态");
        dgvBeTrade.Columns.Add("Inventory", "库存数");
        dgvBeTrade.Columns.Add("Tradable", "可交易");
        dgvBeTrade.Columns.Add("Cooldown", "冷却");
        dgvBeTrade.Columns.Add("Sent", "已发报价");
        dgvBeTrade.Columns.Add("TaskState", "当前状态");
        dgvBeTrade.Columns.Add("MaFile", "maFile");
        foreach (var row in data.Workers)
            dgvBeTrade.Rows.Add(row.Account, TranslateLoginState(row.LoginState), row.Inventory, row.Tradable, row.Cooldown, row.Sent, TranslateTaskState(row.TaskState), TranslateMaFile(row.MaFile));
    }

    private void BindEvents()
    {
        btnImportAccounts.Click += (_, _) =>
        {
            using var dialog = new ImportAccountsDialog();
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            snapshot = controller.ImportAccounts(dialog.Result, this);
            RenderSnapshot();
        };
        btnImportMaFiles.Click += (_, _) =>
        {
            snapshot = controller.ImportMaFiles(this);
            RenderSnapshot();
        };
        btnViewInventory.Click += (_, _) => controller.ViewInventory(this);
        btnExport.Click += (_, _) => controller.ExportResults(this);
        btnStart.Click += (_, _) =>
        {
            SaveSettingsFromUi();
            snapshot = controller.StartTransfer(ReadRuntimeSettings(), this);
            RenderSnapshot();
            lblRunState.ForeColor = Success;
            UpdateActionState(true);
        };
        btnStop.Click += (_, _) =>
        {
            snapshot = controller.StopTransfer(this);
            RenderSnapshot();
            lblRunState.ForeColor = Warning;
            UpdateActionState(false);
        };
        btnClearLog.Click += (_, _) => txtLog.Clear();
        chkAutoScroll.Checked = true;
        dgvBeTrade.SelectionChanged += (_, _) => SyncSelectionDetails();
        dgvMaster.SelectionChanged += (_, _) => SyncSelectionDetails();
        cmbTransferType.SelectedIndexChanged += (_, _) =>
        {
            lblModeSummary.Text = $"模式：{cmbTransferType.Text}";
            SaveSettingsFromUi();
        };
        cmbAcceptMode.SelectedIndexChanged += (_, _) => SaveSettingsFromUi();
        cmbItemType.SelectedIndexChanged += (_, _) => SaveSettingsFromUi();
        txtItemFilter.TextChanged += (_, _) => SaveSettingsFromUi();
        numThreadCount.ValueChanged += (_, _) =>
        {
            lblThreadSummary.Text = $"线程：{numThreadCount.Value}";
            SaveSettingsFromUi();
        };
        FormClosing += (_, _) => SaveSettingsFromUi();
    }

    private void SyncSelectionDetails()
    {
        if (dgvBeTrade.CurrentRow is not null)
        {
            var account = dgvBeTrade.CurrentRow.Cells[0].Value?.ToString();
            var worker = snapshot.Workers.FirstOrDefault(x => x.Account == account);
            if (worker is not null)
            {
                SyncWorkerDetails(worker);
                return;
            }
        }

        if (dgvMaster.CurrentRow is not null)
        {
            lblSelectedAccountValue.Text = dgvMaster.CurrentRow.Cells[0].Value?.ToString() ?? "-";
            lblSelectedSteamIdValue.Text = dgvMaster.CurrentRow.Cells[1].Value?.ToString() ?? "-";
            lblSelectedMaFileValue.Text = dgvMaster.CurrentRow.Cells[6].Value?.ToString() ?? "-";
            lblSelectedOfferValue.Text = "-";
            lblSelectedErrorValue.Text = "无";
            lblSelectedInventoryValue.Text = "无";
        }
    }

    private void SyncWorkerDetails(WorkerAccountRow worker)
    {
        lblSelectedAccountValue.Text = worker.Account;
        lblSelectedSteamIdValue.Text = worker.SteamId;
        lblSelectedMaFileValue.Text = TranslateMaFile(worker.MaFile);
        lblSelectedOfferValue.Text = worker.RecentOfferId;
        lblSelectedErrorValue.Text = worker.RecentError == "None" ? "无" : worker.RecentError;
        lblSelectedInventoryValue.Text = $"总 {worker.Inventory} / 可交易 {worker.Tradable} / 冷却 {worker.Cooldown}";
    }

    private string TranslateLoginState(string value) => value switch
    {
        "Online" => "在线",
        "Offline" => "离线",
        "Imported" => "已导入",
        "Failed" => "失败",
        _ => value
    };

    private string TranslateTaskState(string value) => value switch
    {
        "Offer Sent" => "已发报价",
        "Queued" => "排队中",
        "Inventory Ready" => "拉库存完成",
        "Login Failed" => "登录失败",
        "Awaiting Login" => "等待登录",
        _ => value
    };

    private string TranslateMaFile(string value) => value switch
    {
        "Bound" => "已绑定",
        "Missing" => "缺失",
        _ => value
    };

    private void ApplyGridStateVisuals()
    {
        foreach (DataGridViewRow row in dgvMaster.Rows)
        {
            var login = row.Cells[2].Value?.ToString();
            var maFile = row.Cells[6].Value?.ToString();
            row.Cells[2].Style.ForeColor = login == "离线" ? Danger : TextSub;
            row.Cells[6].Style.ForeColor = maFile == "缺失" ? Warning : TextSub;
        }

        foreach (DataGridViewRow row in dgvBeTrade.Rows)
        {
            var login = row.Cells[1].Value?.ToString();
            var task = row.Cells[6].Value?.ToString();
            var maFile = row.Cells[7].Value?.ToString();

            row.Cells[1].Style.ForeColor = login == "失败" ? Danger : TextSub;
            row.Cells[7].Style.ForeColor = maFile == "缺失" ? Warning : TextSub;

            row.Cells[6].Style.ForeColor = task switch
            {
                "已发报价" => Accent,
                "拉库存完成" => Success,
                "排队中" => Warning,
                "登录失败" => Danger,
                _ => TextSub
            };
        }
    }

    public void Info(string message) => AppendLog("INFO", message);
    public void Warn(string message) => AppendLog("WARN", message);
    public void Error(string message) => AppendLog("ERROR", message);

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
