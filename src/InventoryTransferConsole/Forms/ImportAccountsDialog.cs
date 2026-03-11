using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Forms;

public sealed class ImportAccountsDialog : Form
{
    private readonly RichTextBox txtInput;
    private readonly Label lblHint;
    private readonly Label lblSummary;
    private readonly Button btnLoadDemo;
    private readonly Button btnConfirm;
    private readonly Button btnCancel;

    public ImportAccountsResult Result { get; private set; } = new();

    public ImportAccountsDialog()
    {
        Text = "导入代转号（账密）";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 520);
        Size = new Size(820, 560);
        BackColor = ColorTranslator.FromHtml("#111317");
        ForeColor = ColorTranslator.FromHtml("#ECEFF4");
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        var pnlTop = new Panel { Dock = DockStyle.Top, Height = 72, Padding = new Padding(16, 14, 16, 10), BackColor = ColorTranslator.FromHtml("#171B22") };
        var lblTitle = new Label
        {
            Text = "粘贴代转号账密（账号----密码等）",
            Dock = DockStyle.Top,
            Height = 26,
            Font = new Font("Bahnschrift SemiBold", 13F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = ColorTranslator.FromHtml("#E6EAF2")
        };
        lblHint = new Label
        {
            Text = "格式：账号----密码  |  账号:密码  |  账号,密码",
            Dock = DockStyle.Bottom,
            Height = 20,
            ForeColor = ColorTranslator.FromHtml("#9AA4B2")
        };
        pnlTop.Controls.Add(lblHint);
        pnlTop.Controls.Add(lblTitle);

        txtInput = new RichTextBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            BackColor = ColorTranslator.FromHtml("#121722"),
            ForeColor = ColorTranslator.FromHtml("#E6EAF2"),
            Font = new Font("Cascadia Mono", 10F, FontStyle.Regular, GraphicsUnit.Point)
        };

        var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 68, Padding = new Padding(16, 12, 16, 12), BackColor = ColorTranslator.FromHtml("#171A21") };
        lblSummary = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Left,
            Width = 360,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = ColorTranslator.FromHtml("#9AA4B2"),
            Text = "尚未输入内容。"
        };

        btnLoadDemo = CreateButton("加载代转号示例", ColorTranslator.FromHtml("#1D2230"), ColorTranslator.FromHtml("#E6EAF2"));
        btnConfirm = CreateButton("导入代转号", ColorTranslator.FromHtml("#2C3442"), ColorTranslator.FromHtml("#E6EAF2"));
        btnCancel = CreateButton("取消", ColorTranslator.FromHtml("#23272F"), ColorTranslator.FromHtml("#A7AFBD"));

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
            txtInput.Text = "worker_alpha----pass123\nworker_beta:pass456\nworker_gamma,pass789";
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

    private static ImportAccountsResult ParseText(string text)
    {
        var lines = text.Replace("\r", string.Empty)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var result = new ImportAccountsResult { RawLines = lines };
        foreach (var line in lines)
        {
            var parts = SplitLine(line);
            if (parts is null || string.IsNullOrWhiteSpace(parts.Value.account) || string.IsNullOrWhiteSpace(parts.Value.password))
                result.InvalidCount++;
            else
                result.ParsedCount++;
        }
        return result;
    }

    private static (string account, string password)? SplitLine(string line)
    {
        foreach (var sep in new[] { "----", ":", ",", "|" })
        {
            var parts = line.Split(sep, StringSplitOptions.TrimEntries);
            if (parts.Length >= 2)
                return (parts[0], parts[1]);
        }
        return null;
    }
}
