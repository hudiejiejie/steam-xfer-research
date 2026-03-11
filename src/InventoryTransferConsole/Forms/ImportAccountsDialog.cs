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
        Text = "Import Accounts";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 520);
        Size = new Size(820, 560);
        BackColor = ColorTranslator.FromHtml("#0F1115");
        ForeColor = ColorTranslator.FromHtml("#E6EAF2");
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        var pnlTop = new Panel { Dock = DockStyle.Top, Height = 72, Padding = new Padding(16, 14, 16, 10), BackColor = ColorTranslator.FromHtml("#171A21") };
        var lblTitle = new Label
        {
            Text = "Paste accounts here",
            Dock = DockStyle.Top,
            Height = 26,
            Font = new Font("Bahnschrift SemiBold", 13F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = ColorTranslator.FromHtml("#E6EAF2")
        };
        lblHint = new Label
        {
            Text = "Supported: account----password  |  account:password  |  account,password",
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
            Width = 420,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = ColorTranslator.FromHtml("#9AA4B2"),
            Text = "No content yet."
        };

        btnLoadDemo = CreateButton("Load Demo", ColorTranslator.FromHtml("#1D2230"), ColorTranslator.FromHtml("#E6EAF2"));
        btnConfirm = CreateButton("Import", ColorTranslator.FromHtml("#10314D"), ColorTranslator.FromHtml("#4DA3FF"));
        btnCancel = CreateButton("Cancel", ColorTranslator.FromHtml("#2A2020"), ColorTranslator.FromHtml("#F5A524"));

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
            txtInput.Text = "worker_alpha----pass123\nworker_beta:pass456\nworker_alpha,pass999\ninvalid_line_only_account\nworker_gamma|pass789";
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
        lblSummary.Text = $"Lines: {result.RawLines.Count} | Parsed: {result.ParsedCount} | Invalid: {result.InvalidCount} | Duplicates: {result.DuplicateCount} | Accepted: {result.FinalAcceptedCount}";
    }

    private static ImportAccountsResult ParseText(string text)
    {
        var lines = text.Replace("\r", string.Empty)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var result = new ImportAccountsResult { RawLines = lines };
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in lines)
        {
            var parts = SplitLine(line);
            if (parts is null || string.IsNullOrWhiteSpace(parts.Value.account) || string.IsNullOrWhiteSpace(parts.Value.password))
            {
                result.InvalidCount++;
                continue;
            }

            result.ParsedCount++;
            if (!seen.Add(parts.Value.account))
            {
                result.DuplicateCount++;
                continue;
            }

            result.FinalAcceptedCount++;
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
