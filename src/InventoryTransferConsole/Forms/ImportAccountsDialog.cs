using InventoryTransferConsole.Models;
using InventoryTransferConsole.Services;

namespace InventoryTransferConsole.Forms;

public sealed class ImportAccountsDialog : Form
{
    private readonly RichTextBox txtInput;
    private readonly Label lblHint;
    private readonly Label lblSummary;
    private readonly Button btnLoadDemo;
    private readonly Button btnConfirm;
    private readonly Button btnCancel;
    private readonly string demoPrefix;

    public ImportAccountsResult Result { get; private set; } = new();

    public ImportAccountsDialog(string scopeTitle, string demoPrefix)
    {
        this.demoPrefix = demoPrefix;
        Text = $"粘贴载入{scopeTitle}";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 520);
        Size = new Size(820, 560);
        BackColor = ColorTranslator.FromHtml("#EEF2F6");
        ForeColor = ColorTranslator.FromHtml("#243244");
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        var pnlTop = new Panel { Dock = DockStyle.Top, Height = 72, Padding = new Padding(16, 14, 16, 10), BackColor = ColorTranslator.FromHtml("#FBFCFE") };
        var lblTitle = new Label
        {
            Text = $"粘贴{scopeTitle}账号账密",
            Dock = DockStyle.Top,
            Height = 26,
            Font = new Font("Bahnschrift SemiBold", 13F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = ColorTranslator.FromHtml("#243244")
        };
        lblHint = new Label
        {
            Text = "格式：账号----密码（兼容 —— / : / , / |）",
            Dock = DockStyle.Bottom,
            Height = 20,
            ForeColor = ColorTranslator.FromHtml("#6B7B8F")
        };
        pnlTop.Controls.Add(lblHint);
        pnlTop.Controls.Add(lblTitle);

        txtInput = new RichTextBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            BackColor = ColorTranslator.FromHtml("#FFFFFF"),
            ForeColor = ColorTranslator.FromHtml("#243244"),
            Font = new Font("Cascadia Mono", 10F, FontStyle.Regular, GraphicsUnit.Point)
        };

        var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 68, Padding = new Padding(16, 12, 16, 12), BackColor = ColorTranslator.FromHtml("#FBFCFE") };
        lblSummary = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Left,
            Width = 360,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = ColorTranslator.FromHtml("#6B7B8F"),
            Text = "尚未输入内容。"
        };

        btnLoadDemo = CreateButton("加载示例", ColorTranslator.FromHtml("#F1F4F8"), ColorTranslator.FromHtml("#243244"));
        btnConfirm = CreateButton("载入", ColorTranslator.FromHtml("#2F6FED"), Color.White);
        btnCancel = CreateButton("取消", ColorTranslator.FromHtml("#F1F4F8"), ColorTranslator.FromHtml("#6B7B8F"));

        btnLoadDemo.Location = new Point(0, 0);
        btnConfirm.Location = new Point(0, 0);
        btnCancel.Location = new Point(0, 0);

        btnLoadDemo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnConfirm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        pnlBottom.Controls.Add(lblSummary);
        pnlBottom.Controls.Add(btnCancel);
        pnlBottom.Controls.Add(btnConfirm);
        pnlBottom.Controls.Add(btnLoadDemo);

        Controls.Add(txtInput);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTop);

        Load += (_, _) => LayoutButtons();
        Resize += (_, _) => LayoutButtons();
        txtInput.TextChanged += (_, _) => RecomputeSummary();
        btnLoadDemo.Click += (_, _) =>
        {
            txtInput.Text = $"{demoPrefix}_alpha----pass123\n{demoPrefix}_beta----pass456\n{demoPrefix}_gamma----pass789";
        };
        btnCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
        btnConfirm.Click += (_, _) =>
        {
            Result = ParseText(txtInput.Text);
            DialogResult = DialogResult.OK;
            Close();
        };
    }

    private Button CreateButton(string text, Color back, Color fore)
    {
        return new Button
        {
            Text = text,
            Size = new Size(116, 36),
            FlatStyle = FlatStyle.Flat,
            BackColor = back,
            ForeColor = fore,
            Cursor = Cursors.Hand
        };
    }

    private void LayoutButtons()
    {
        btnCancel.Location = new Point(ClientSize.Width - 16 - btnCancel.Width, 16);
        btnConfirm.Location = new Point(btnCancel.Left - 12 - btnConfirm.Width, 16);
        btnLoadDemo.Location = new Point(btnConfirm.Left - 12 - btnLoadDemo.Width, 16);
    }

    private void RecomputeSummary()
    {
        var result = ParseText(txtInput.Text);
        lblSummary.Text = $"总行数：{result.RawLines.Count}  |  解析成功：{result.ParsedCount}  |  非法：{result.InvalidCount}";
    }

    public static ImportAccountsResult ParseText(string text)
        => AccountImportService.ParseText(text);
}
