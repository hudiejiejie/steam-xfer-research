using System.Text;
using InventoryTransferConsole.Forms;
using InventoryTransferConsole.Models;
using InventoryTransferConsole.Services;

namespace InventoryTransferConsole;

public partial class TransferInventoryConsoleForm : Form, ILogSink
{
    private readonly Color Bg = ColorTranslator.FromHtml("#EEF2F6");
    private readonly Color PanelBg = ColorTranslator.FromHtml("#FBFCFE");
    private readonly Color PanelBg2 = ColorTranslator.FromHtml("#F1F4F8");
    private readonly Color TextMain = ColorTranslator.FromHtml("#243244");
    private readonly Color TextSub = ColorTranslator.FromHtml("#6B7B8F");
    private readonly Color Accent = ColorTranslator.FromHtml("#2F6FED");
    private readonly Color Success = ColorTranslator.FromHtml("#3FA46A");
    private readonly Color Warning = ColorTranslator.FromHtml("#D89A3D");
    private readonly Color Danger = ColorTranslator.FromHtml("#D96B6B");
    private readonly Color Border = ColorTranslator.FromHtml("#D8E0EA");

    private readonly ITransferController controller;
    private readonly ISettingsService settingsService;
    private readonly Button btnMasterRefreshStatus = new();
    private readonly Button btnMasterOpenLoad = new();
    private readonly Button btnMasterPasteLoad = new();
    private readonly Button btnWorkerOpenLoad = new();
    private readonly Button btnWorkerPasteLoad = new();
    private readonly FlowLayoutPanel pnlItemCategories = new();
    private readonly Dictionary<string, CheckBox> itemCategoryChecks = new(StringComparer.OrdinalIgnoreCase);
    private readonly string[] itemCategories = ["武器箱", "武器", "涂鸦"];
    private readonly HashSet<string> checkedMasters = [];
    private readonly HashSet<string> checkedWorkers = [];
    private bool masterChecksInitialized;
    private bool workerChecksInitialized;
    private DashboardSnapshot snapshot = new();
    private AppSettings appSettings = new();
    private bool suppressSettingsEvents;

    public TransferInventoryConsoleForm()
    {
        controller = new MockTransferController();
        settingsService = new JsonSettingsService();
        InitializeComponent();
        InitializeItemCategorySelector();
        InitializeSectionLoadButtons();
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

        radTransferSendAccept.Checked = appSettings.Runtime.TransferTypeIndex == 0;
        radTransferAllThenAccept.Checked = appSettings.Runtime.TransferTypeIndex == 1;
        radAcceptAuto.Checked = appSettings.Runtime.AcceptModeIndex == 0;
        radAcceptDelayed.Checked = appSettings.Runtime.AcceptModeIndex == 1;
        radAcceptManual.Checked = appSettings.Runtime.AcceptModeIndex == 2;

        numThreadCount.Value = Math.Max(numThreadCount.Minimum, Math.Min(numThreadCount.Maximum, appSettings.Runtime.ThreadCount));
        ApplySelectedItemCategories(appSettings.Runtime.SelectedItemCategories);

        lblThreadSummary.Text = $"线程：{numThreadCount.Value}";
        var modeText = radTransferSendAccept.Checked ? "边发边接" : "全发后统一接受";
        lblModeSummary.Text = $"模式：{modeText}";
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
        int transferTypeIndex = radTransferSendAccept.Checked ? 0 : radTransferAllThenAccept.Checked ? 1 : 0;
        int acceptModeIndex = radAcceptAuto.Checked ? 0 : radAcceptDelayed.Checked ? 1 : radAcceptManual.Checked ? 2 : 0;
        return new RuntimeSettings
        {
            ThreadCount = (int)numThreadCount.Value,
            TransferTypeIndex = transferTypeIndex,
            AcceptModeIndex = acceptModeIndex,
            SelectedItemCategories = GetSelectedItemCategories().ToList()
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
        btnExport.Enabled = !isRunning;
        btnViewInventory.Enabled = !isRunning;
        btnMasterRefreshStatus.Enabled = !isRunning;
        btnMasterOpenLoad.Enabled = !isRunning;
        btnMasterPasteLoad.Enabled = !isRunning;
        btnWorkerOpenLoad.Enabled = !isRunning;
        btnWorkerPasteLoad.Enabled = !isRunning;
    }

    private void ApplyTheme()
    {
        BackColor = Bg;
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        ForeColor = TextMain;

        foreach (var panel in new[] { pnlTopStatus, pnlBottomActions, pnlParams, pnlMaster, pnlBeTrade, pnlLogs, pnlDetails })
        {
            panel.BackColor = PanelBg;
            panel.BorderStyle = BorderStyle.FixedSingle;
        }

        foreach (var subPanel in new[] { pnlThread, pnlTransferType, pnlAcceptMode, pnlItemType })
        {
            subPanel.BackColor = PanelBg2;
            subPanel.BorderStyle = BorderStyle.FixedSingle;
        }

        pnlItemCategories.BackColor = PanelBg2;

        pnlMain.BackColor = Bg;
        pnlLeftMain.BackColor = Bg;
        pnlRightSide.BackColor = Bg;
        tableLayoutPanel1.BackColor = PanelBg;

        foreach (var title in new[] { lblParamsTitle, lblMasterTitle, lblBeTradeTitle, lblLogTitle, lblDetailsTitle })
        {
            title.ForeColor = TextMain;
            title.BackColor = PanelBg2;
            title.Font = new Font("Bahnschrift SemiBold", 10.25F, FontStyle.Bold, GraphicsUnit.Point);
            title.Padding = new Padding(10, 0, 0, 0);
        }

        lblAppTitle.ForeColor = TextMain;
        lblAppTitle.Font = new Font("Bahnschrift SemiBold", 16F, FontStyle.Bold, GraphicsUnit.Point);
        lblRunState.ForeColor = Accent;
        lblRunState.BackColor = PanelBg2;
        lblRunState.Padding = new Padding(10, 4, 10, 4);
        lblRunState.Font = new Font("Microsoft YaHei UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblRunState.BorderStyle = BorderStyle.FixedSingle;

        foreach (var s in new[] { lblThreadSummary, lblMasterSummary, lblBeTradeSummary, lblModeSummary })
        {
            s.ForeColor = TextSub;
            s.Font = new Font("Microsoft YaHei UI", 8.8F, FontStyle.Regular, GraphicsUnit.Point);
        }

        foreach (var label in new[] { lblThread, lblTransferType, lblAcceptMode, lblItemType, lblSelectedAccountTitle, lblSelectedSteamIdTitle, lblSelectedMaFileTitle, lblSelectedOfferTitle, lblSelectedErrorTitle, lblSelectedInventoryTitle })
            label.ForeColor = TextSub;

        foreach (var value in new[] { lblSelectedAccountValue, lblSelectedSteamIdValue, lblSelectedMaFileValue, lblSelectedOfferValue, lblSelectedErrorValue, lblSelectedInventoryValue })
        {
            value.ForeColor = TextMain;
            value.Font = new Font("Microsoft YaHei UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            value.BackColor = PanelBg2;
            value.Padding = new Padding(6, 3, 6, 3);
            value.Margin = new Padding(0, 1, 0, 1);
        }

        btnImportAccounts.Visible = false;
        btnImportMaFiles.Visible = false;
        btnViewInventory.Text = "刷新库存";
        StyleButton(btnViewInventory, PanelBg2, TextMain, true);
        StyleButton(btnExport, PanelBg2, TextMain);
        StyleButton(btnStop, PanelBg, Danger);
        StyleButton(btnStart, Accent, Color.White);
        StyleButton(btnClearLog, PanelBg, TextSub, true);
        StyleButton(btnMasterRefreshStatus, PanelBg2, TextMain, true);
        StyleButton(btnMasterOpenLoad, PanelBg2, TextMain, true);
        StyleButton(btnMasterPasteLoad, Accent, Color.White, true);
        StyleButton(btnWorkerOpenLoad, PanelBg2, TextMain, true);
        StyleButton(btnWorkerPasteLoad, Accent, Color.White, true);
        LayoutSectionLoadButtons();

        StyleRadio(radTransferSendAccept);
        StyleRadio(radTransferAllThenAccept);
        StyleRadio(radAcceptAuto);
        StyleRadio(radAcceptDelayed);
        StyleRadio(radAcceptManual);
        StyleNumeric(numThreadCount);
        foreach (var checkbox in itemCategoryChecks.Values)
            StyleCategoryCheck(checkbox);
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
        btn.FlatAppearance.BorderColor = Border;
        btn.FlatAppearance.BorderSize = 1;
        btn.Font = new Font("Microsoft YaHei UI", compact ? 8.5F : 9.25F, FontStyle.Regular, GraphicsUnit.Point);
        btn.Height = compact ? 28 : 42;
        btn.Cursor = Cursors.Hand;
    }

    private void InitializeSectionLoadButtons()
    {
        ConfigureSectionButton(btnMasterRefreshStatus, "刷新状态");
        ConfigureSectionButton(btnMasterOpenLoad, "打开载入");
        ConfigureSectionButton(btnMasterPasteLoad, "粘贴载入");
        ConfigureSectionButton(btnWorkerOpenLoad, "打开载入");
        ConfigureSectionButton(btnWorkerPasteLoad, "粘贴载入");
        ConfigureSectionButton(btnViewInventory, "刷新库存");

        pnlMaster.Controls.Add(btnMasterRefreshStatus);
        pnlMaster.Controls.Add(btnMasterOpenLoad);
        pnlMaster.Controls.Add(btnMasterPasteLoad);
        pnlBeTrade.Controls.Add(btnWorkerOpenLoad);
        pnlBeTrade.Controls.Add(btnWorkerPasteLoad);
        pnlBeTrade.Controls.Add(btnViewInventory);

        pnlMaster.Resize += (_, _) => LayoutSectionLoadButtons();
        pnlBeTrade.Resize += (_, _) => LayoutSectionLoadButtons();
        LayoutSectionLoadButtons();
    }

    private void ConfigureSectionButton(Button button, string text)
    {
        button.Text = text;
        button.Size = new Size(text.Length >= 4 ? 96 : 92, 28);
        button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button.Cursor = Cursors.Hand;
        button.BringToFront();
    }

    private void LayoutSectionLoadButtons()
    {
        LayoutPanelButtons(pnlMaster, btnMasterRefreshStatus, btnMasterOpenLoad, btnMasterPasteLoad);
        LayoutPanelButtons(pnlBeTrade, btnViewInventory, btnWorkerOpenLoad, btnWorkerPasteLoad);
    }

    private static void LayoutPanelButtons(Panel panel, params Button[] buttons)
    {
        const int top = 3;
        const int gap = 8;
        const int right = 14;
        var currentRight = panel.Width - right;

        for (var i = buttons.Length - 1; i >= 0; i--)
        {
            var button = buttons[i];
            button.Location = new Point(currentRight - button.Width, top);
            button.BringToFront();
            currentRight = button.Left - gap;
        }
    }

    private void StyleInput(TextBox tb)
    {
        tb.BackColor = Color.White;
        tb.ForeColor = TextMain;
        tb.BorderStyle = BorderStyle.FixedSingle;
    }

    private void StyleRadio(RadioButton rb)
    {
        rb.ForeColor = TextMain;
        rb.BackColor = PanelBg2;
        rb.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    }

    private void StyleCategoryCheck(CheckBox chk)
    {
        chk.ForeColor = TextMain;
        chk.BackColor = PanelBg2;
        chk.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        chk.AutoSize = true;
        chk.Margin = new Padding(0, 0, 16, 8);
    }

    private void StyleNumeric(NumericUpDown nud)
    {
        nud.BackColor = Color.White;
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
        rtb.BackColor = PanelBg2;
        rtb.ForeColor = TextMain;
        rtb.BorderStyle = BorderStyle.None;
        rtb.Font = new Font("Cascadia Mono", 9F, FontStyle.Regular, GraphicsUnit.Point);
        rtb.ReadOnly = true;
    }

    private void StyleGrid(DataGridView dgv)
    {
        dgv.BackgroundColor = Color.White;
        dgv.BorderStyle = BorderStyle.FixedSingle;
        dgv.GridColor = Border;
        dgv.EnableHeadersVisualStyles = false;
        dgv.RowHeadersVisible = false;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.AllowUserToResizeRows = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        dgv.EditMode = DataGridViewEditMode.EditOnEnter;
        dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dgv.ColumnHeadersHeight = 36;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E9EEF5");
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextMain;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dgv.DefaultCellStyle.BackColor = Color.White;
        dgv.DefaultCellStyle.ForeColor = TextMain;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F6F8FB");
        dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#DCE7F9");
        dgv.DefaultCellStyle.SelectionForeColor = TextMain;
        dgv.DefaultCellStyle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        dgv.RowTemplate.Height = 30;
    }

    private void SeedTables(DashboardSnapshot data)
    {
        EnsureCheckedSetsInitialized(data);
        dgvMaster.Columns.Clear();
        dgvMaster.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Selected", HeaderText = "选", Width = 42, FillWeight = 18 });
        dgvMaster.Columns.Add("Account", "账号");
        dgvMaster.Columns.Add("SteamId", "SteamID");
        dgvMaster.Columns.Add("LoginState", "登录状态");
        dgvMaster.Columns.Add("Pending", "待接受");
        dgvMaster.Columns.Add("Assigned", "已分配");
        dgvMaster.Columns.Add("Limit", "上限");
        dgvMaster.Columns.Add("MaFile", "maFile");
        foreach (var row in data.Masters)
            dgvMaster.Rows.Add(GetOrInitMasterChecked(row.Account), row.Account, row.SteamId, TranslateLoginState(row.LoginState), row.Pending, row.Assigned, row.Limit, TranslateMaFile(row.MaFile));
        ConfigureSelectionGrid(dgvMaster);

        dgvBeTrade.Columns.Clear();
        dgvBeTrade.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Selected", HeaderText = "选", Width = 42, FillWeight = 18 });
        dgvBeTrade.Columns.Add("Account", "账号");
        dgvBeTrade.Columns.Add("LoginState", "登录状态");
        dgvBeTrade.Columns.Add("Inventory", "库存数");
        dgvBeTrade.Columns.Add("Tradable", "可交易");
        dgvBeTrade.Columns.Add("Cooldown", "冷却");
        dgvBeTrade.Columns.Add("Sent", "已发报价");
        dgvBeTrade.Columns.Add("TaskState", "当前状态");
        dgvBeTrade.Columns.Add("MaFile", "maFile");
        foreach (var row in data.Workers)
            dgvBeTrade.Rows.Add(GetOrInitWorkerChecked(row.Account), row.Account, TranslateLoginState(row.LoginState), row.Inventory, row.Tradable, row.Cooldown, row.Sent, TranslateTaskState(row.TaskState), TranslateMaFile(row.MaFile));
        ConfigureSelectionGrid(dgvBeTrade);
    }

    private void EnsureCheckedSetsInitialized(DashboardSnapshot data)
    {
        if (!masterChecksInitialized)
        {
            foreach (var master in data.Masters)
                checkedMasters.Add(master.Account);
            masterChecksInitialized = true;
        }

        if (!workerChecksInitialized)
        {
            foreach (var worker in data.Workers)
                checkedWorkers.Add(worker.Account);
            workerChecksInitialized = true;
        }
    }

    private bool GetOrInitMasterChecked(string account) => checkedMasters.Contains(account);

    private bool GetOrInitWorkerChecked(string account) => checkedWorkers.Contains(account);

    private void ConfigureSelectionGrid(DataGridView dgv)
    {
        if (dgv.Columns.Count == 0) return;
        dgv.Columns[0].ReadOnly = false;
        dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        dgv.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        for (var i = 1; i < dgv.Columns.Count; i++)
            dgv.Columns[i].ReadOnly = true;
    }

    private void InitializeItemCategorySelector()
    {
        pnlParams.Height = 208;
        tblParams.RowStyles[0].Height = 42F;
        tblParams.RowStyles[1].Height = 46F;
        tblParams.SetColumnSpan(pnlItemType, 3);
        pnlFilterValue.Visible = false;
        lblItemType.Text = "转移物品分类";
        radFilterByType.Visible = false;
        radFilterByExact.Visible = false;
        pnlItemCategories.FlowDirection = FlowDirection.LeftToRight;
        pnlItemCategories.WrapContents = true;
        pnlItemCategories.AutoScroll = true;
        pnlItemCategories.Location = new Point(8, 26);
        pnlItemCategories.Size = new Size(Math.Max(100, pnlItemType.Width - 16), 40);
        pnlItemCategories.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        pnlItemType.Controls.Add(pnlItemCategories);
        pnlItemCategories.BringToFront();

        foreach (var category in itemCategories)
        {
            var checkbox = new CheckBox { Text = category, Checked = true };
            checkbox.CheckedChanged += (_, _) => SaveSettingsFromUi();
            itemCategoryChecks[category] = checkbox;
            pnlItemCategories.Controls.Add(checkbox);
        }

        pnlItemType.Resize += (_, _) =>
        {
            pnlItemCategories.Size = new Size(Math.Max(100, pnlItemType.Width - 16), Math.Max(36, pnlItemType.Height - 32));
        };
    }

    private IReadOnlyList<string> GetSelectedItemCategories()
        => itemCategoryChecks.Where(x => x.Value.Checked).Select(x => x.Key).ToList();

    private void ApplySelectedItemCategories(IReadOnlyCollection<string> selectedCategories)
    {
        var useDefaults = selectedCategories.Count == 0;
        foreach (var item in itemCategoryChecks)
            item.Value.Checked = useDefaults || selectedCategories.Contains(item.Key, StringComparer.OrdinalIgnoreCase);
    }

    private void BindEvents()
    {
        btnMasterOpenLoad.Click += (_, _) => LoadAccountsFromFile(isMaster: true);
        btnMasterPasteLoad.Click += (_, _) => LoadAccountsFromPaste(isMaster: true);
        btnWorkerOpenLoad.Click += (_, _) => LoadAccountsFromFile(isMaster: false);
        btnWorkerPasteLoad.Click += (_, _) => LoadAccountsFromPaste(isMaster: false);
        btnMasterRefreshStatus.Click += (_, _) => RefreshSelectedMastersStatus();
        btnViewInventory.Click += (_, _) =>
        {
            var selectedWorkers = GetCheckedWorkerAccounts();
            if (selectedWorkers.Count == 0)
            {
                Warn("未勾选待转号，无法刷新库存。");
                return;
            }

            controller.ViewInventory(this);
            foreach (var worker in snapshot.Workers.Where(x => selectedWorkers.Contains(x.Account)))
            {
                if (worker.LoginState == "Failed")
                    continue;

                worker.Inventory = Math.Max(worker.Inventory, 1) + 3;
                worker.Tradable = Math.Max(worker.Tradable, 0) + 2;
                worker.TaskState = "Inventory Ready";
                worker.RecentError = "None";
            }
            Info($"仅刷新已勾选待转号库存：{selectedWorkers.Count} 个。");
            RenderSnapshot();
        };
        btnExport.Click += (_, _) => controller.ExportResults(this);
        btnStart.Click += (_, _) =>
        {
            var selectedMasters = GetCheckedMasterAccounts();
            var selectedWorkers = GetCheckedWorkerAccounts();
            var selectedCategories = GetSelectedItemCategories();
            if (selectedMasters.Count == 0)
            {
                Warn("未勾选主库号，无法开始转移。");
                return;
            }
            if (selectedWorkers.Count == 0)
            {
                Warn("未勾选待转号，无法开始转移。");
                return;
            }
            if (selectedCategories.Count == 0)
            {
                Warn("未勾选任何物品分类，无法开始转移。");
                return;
            }

            SaveSettingsFromUi();
            var before = CloneSnapshot(snapshot);
            snapshot = controller.StartTransfer(ReadRuntimeSettings(), this);
            snapshot = KeepOnlyCheckedActivity(before, snapshot, selectedMasters, selectedWorkers);
            Info($"仅转移已勾选分类：{string.Join("、", selectedCategories)}。");
            Info($"仅对已勾选账号执行转移：主库号 {selectedMasters.Count} 个，待转号 {selectedWorkers.Count} 个。");
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
        dgvMaster.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (dgvMaster.IsCurrentCellDirty)
                dgvMaster.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };
        dgvBeTrade.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (dgvBeTrade.IsCurrentCellDirty)
                dgvBeTrade.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };
        dgvMaster.CellValueChanged += (_, e) => UpdateCheckedStateFromGrid(dgvMaster, e, true);
        dgvBeTrade.CellValueChanged += (_, e) => UpdateCheckedStateFromGrid(dgvBeTrade, e, false);
        radTransferSendAccept.CheckedChanged += OnTransferModeChanged;
        radTransferAllThenAccept.CheckedChanged += OnTransferModeChanged;
        radAcceptAuto.CheckedChanged += OnSettingOptionChanged;
        radAcceptDelayed.CheckedChanged += OnSettingOptionChanged;
        radAcceptManual.CheckedChanged += OnSettingOptionChanged;
        radFilterByType.CheckedChanged += OnSettingOptionChanged;
        radFilterByExact.CheckedChanged += OnSettingOptionChanged;
        txtItemFilter.TextChanged += (_, _) => SaveSettingsFromUi();
        numThreadCount.ValueChanged += (_, _) =>
        {
            lblThreadSummary.Text = $"线程：{numThreadCount.Value}";
            SaveSettingsFromUi();
        };
        FormClosing += (_, _) => SaveSettingsFromUi();
    }

    private void OnTransferModeChanged(object? sender, EventArgs e)
    {
        var mode = radTransferSendAccept.Checked ? "边发边接" : "全发后统一接受";
        lblModeSummary.Text = $"模式：{mode}";
        SaveSettingsFromUi();
    }

    private void OnSettingOptionChanged(object? sender, EventArgs e) => SaveSettingsFromUi();

    private void LoadAccountsFromFile(bool isMaster)
    {
        using var dialog = new OpenFileDialog
        {
            Title = isMaster ? "打开载入主库号" : "打开载入待转号",
            Filter = "文本文件|*.txt;*.csv|所有文件|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        var text = File.ReadAllText(dialog.FileName);
        var result = AccountImportService.ParseFile(dialog.FileName);
        ImportByRole(result, isMaster, $"文件：{Path.GetFileName(dialog.FileName)}");
    }

    private void LoadAccountsFromPaste(bool isMaster)
    {
        using var dialog = new ImportAccountsDialog(isMaster ? "主库号" : "待转号", isMaster ? "master" : "worker");
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        ImportByRole(dialog.Result, isMaster, "粘贴内容");
    }

    private void ImportByRole(ImportAccountsResult result, bool isMaster, string sourceLabel)
    {
        if (result.ParsedCount == 0)
        {
            Warn($"{sourceLabel} 没有可载入的有效账号。");
            return;
        }

        var beforeMasters = snapshot.Masters.Select(x => x.Account).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var beforeWorkers = snapshot.Workers.Select(x => x.Account).ToHashSet(StringComparer.OrdinalIgnoreCase);

        snapshot = isMaster
            ? controller.ImportMasters(result, this)
            : controller.ImportWorkers(result, this);

        if (isMaster)
        {
            foreach (var master in snapshot.Masters.Where(x => !beforeMasters.Contains(x.Account)))
                checkedMasters.Add(master.Account);
            masterChecksInitialized = true;
        }
        else
        {
            foreach (var worker in snapshot.Workers.Where(x => !beforeWorkers.Contains(x.Account)))
                checkedWorkers.Add(worker.Account);
            workerChecksInitialized = true;
        }

        Info($"{(isMaster ? "主库号" : "待转号")}载入完成：来源 {sourceLabel}；令牌目录为 maf/。");
        RenderSnapshot();
    }

    private void RefreshSelectedMastersStatus()
    {
        var selectedMasters = GetCheckedMasterAccounts();
        if (selectedMasters.Count == 0)
        {
            Warn("未勾选主库号，无法刷新状态。");
            return;
        }

        foreach (var master in snapshot.Masters.Where(x => selectedMasters.Contains(x.Account)))
        {
            var hasToken = HasMaFileToken(master.Account);
            master.MaFile = hasToken ? "Bound" : "Missing";
            master.LoginState = hasToken ? "Online" : "Offline";
        }

        Info($"仅刷新已勾选主库号状态：{selectedMasters.Count} 个。");
        RenderSnapshot();
    }

    private bool HasMaFileToken(string account)
    {
        return AccountImportService.HasMaFileToken(account);
    }

    private void UpdateCheckedStateFromGrid(DataGridView dgv, DataGridViewCellEventArgs e, bool isMaster)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != 0)
            return;

        var account = dgv.Rows[e.RowIndex].Cells[1].Value?.ToString();
        if (string.IsNullOrWhiteSpace(account))
            return;

        var isChecked = Convert.ToBoolean(dgv.Rows[e.RowIndex].Cells[0].Value ?? false);
        var target = isMaster ? checkedMasters : checkedWorkers;
        if (isMaster)
            masterChecksInitialized = true;
        else
            workerChecksInitialized = true;
        if (isChecked)
            target.Add(account);
        else
            target.Remove(account);
    }

    private HashSet<string> GetCheckedMasterAccounts() => [.. checkedMasters];
    private HashSet<string> GetCheckedWorkerAccounts() => [.. checkedWorkers];

    private DashboardSnapshot CloneSnapshot(DashboardSnapshot src)
    {
        return new DashboardSnapshot
        {
            ThreadCount = src.ThreadCount,
            ModeSummary = src.ModeSummary,
            RunState = src.RunState,
            Masters = src.Masters.Select(m => new MasterAccountRow
            {
                Account = m.Account,
                SteamId = m.SteamId,
                LoginState = m.LoginState,
                Pending = m.Pending,
                Assigned = m.Assigned,
                Limit = m.Limit,
                MaFile = m.MaFile
            }).ToList(),
            Workers = src.Workers.Select(w => new WorkerAccountRow
            {
                Account = w.Account,
                LoginState = w.LoginState,
                Inventory = w.Inventory,
                Tradable = w.Tradable,
                Cooldown = w.Cooldown,
                Sent = w.Sent,
                TaskState = w.TaskState,
                MaFile = w.MaFile,
                SteamId = w.SteamId,
                RecentOfferId = w.RecentOfferId,
                RecentError = w.RecentError
            }).ToList()
        };
    }

    private DashboardSnapshot KeepOnlyCheckedActivity(DashboardSnapshot before, DashboardSnapshot after, HashSet<string> selectedMasters, HashSet<string> selectedWorkers)
    {
        after.Masters = after.Masters.Select(m => selectedMasters.Contains(m.Account)
            ? m
            : before.Masters.FirstOrDefault(x => x.Account == m.Account) ?? m).ToList();

        after.Workers = after.Workers.Select(w => selectedWorkers.Contains(w.Account)
            ? w
            : before.Workers.FirstOrDefault(x => x.Account == w.Account) ?? w).ToList();

        return after;
    }

    private void SyncSelectionDetails()
    {
        if (dgvBeTrade.CurrentRow is not null)
        {
            var account = dgvBeTrade.CurrentRow.Cells[1].Value?.ToString();
            var worker = snapshot.Workers.FirstOrDefault(x => x.Account == account);
            if (worker is not null)
            {
                SyncWorkerDetails(worker);
                return;
            }
        }

        if (dgvMaster.CurrentRow is not null)
        {
            lblSelectedAccountValue.Text = dgvMaster.CurrentRow.Cells[1].Value?.ToString() ?? "-";
            lblSelectedSteamIdValue.Text = dgvMaster.CurrentRow.Cells[2].Value?.ToString() ?? "-";
            lblSelectedMaFileValue.Text = dgvMaster.CurrentRow.Cells[7].Value?.ToString() ?? "-";
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
            var login = row.Cells[3].Value?.ToString();
            var maFile = row.Cells[7].Value?.ToString();
            row.Cells[3].Style.ForeColor = login == "离线" ? Danger : TextSub;
            row.Cells[7].Style.ForeColor = maFile == "缺失" ? Warning : TextSub;
        }

        foreach (DataGridViewRow row in dgvBeTrade.Rows)
        {
            var login = row.Cells[2].Value?.ToString();
            var task = row.Cells[7].Value?.ToString();
            var maFile = row.Cells[8].Value?.ToString();

            row.Cells[2].Style.ForeColor = login == "失败" ? Danger : TextSub;
            row.Cells[8].Style.ForeColor = maFile == "缺失" ? Warning : TextSub;

            row.Cells[7].Style.ForeColor = task switch
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
