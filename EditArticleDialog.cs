using System;
using System.Windows.Forms;

namespace WordPress_XML_Tool
{
    public class EditArticleDialog : Form
    {
        private TextBox tbTitle, tbAuthor, tbDate;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewHtml;
        private Button btnOk, btnCancel;
        public string EditedTitle => tbTitle.Text;
        public string EditedAuthor => tbAuthor.Text;
        public string EditedDate => tbDate.Text;
        public string EditedHtml { get; private set; } = string.Empty;

        public EditArticleDialog(Article article)
        {
            this.Text = "Artikel bearbeiten";
            this.Width = 1280;
            this.Height = 720;
            try { this.Icon = new System.Drawing.Icon("Icon.ico"); } catch { /* Icon optional */ }
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 5, ColumnCount = 2 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.Controls.Add(new Label { Text = "Titel:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 0);
            tbTitle = new TextBox { Text = article.Title, Dock = DockStyle.Fill };
            layout.Controls.Add(tbTitle, 1, 0);
            layout.Controls.Add(new Label { Text = "Autor:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 1);
            tbAuthor = new TextBox { Text = article.Author, Dock = DockStyle.Fill };
            layout.Controls.Add(tbAuthor, 1, 1);
            layout.Controls.Add(new Label { Text = "Datum:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 2);
            tbDate = new TextBox { Text = article.Date, Dock = DockStyle.Fill };
            layout.Controls.Add(tbDate, 1, 2);
            layout.Controls.Add(new Label { Text = "HTML-Inhalt:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 3);
            webViewHtml = new Microsoft.Web.WebView2.WinForms.WebView2 { Dock = DockStyle.Fill, Height = 250 };
            layout.Controls.Add(webViewHtml, 1, 3);
            string htmlBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(article.HtmlContent ?? ""));
            webViewHtml.Source = new System.Uri($"file:///{System.IO.Path.GetFullPath("editor.html")}?content_b64={htmlBase64}");
            webViewHtml.EnsureCoreWebView2Async();

            btnOk = new Button { Text = "OK" };
            btnCancel = new Button { Text = "Abbrechen", DialogResult = DialogResult.Cancel };
            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnPanel.Controls.Add(btnOk);
            btnPanel.Controls.Add(btnCancel);
            layout.Controls.Add(btnPanel, 1, 4);
            this.Controls.Add(layout);
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            // Event für OK-Button asynchron behandeln
            btnOk.Click += async (s, e) =>
            {
                btnOk.Enabled = false;
                try
                {
                    EditedHtml = await GetHtmlFromWebViewAsync();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Auslesen des HTML-Inhalts: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    EditedHtml = string.Empty;
                }
                btnOk.Enabled = true;
            };
        }

        private async System.Threading.Tasks.Task<string> GetHtmlFromWebViewAsync()
        {
            // Robust: Holt den Wert aus dem Editor über JavaScript (falls möglich)
            if (webViewHtml?.CoreWebView2 == null)
            {
                await webViewHtml.EnsureCoreWebView2Async();
            }
            try
            {
                var result = await webViewHtml.ExecuteScriptAsync("getContent()");
                // Monaco Editor gibt den Wert als JSON-String zurück
                if (!string.IsNullOrEmpty(result) && result.StartsWith("\"") && result.EndsWith("\""))
                    return System.Text.Json.JsonSerializer.Deserialize<string>(result);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fehler beim Auslesen des HTML-Inhalts: {ex}");
                return string.Empty;
            }
        }
    }
}