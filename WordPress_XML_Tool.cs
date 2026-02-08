using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
// Eigene Module
using WordPress_XML_Tool;

namespace WordPress_XML_Tool
{
    public class MainForm : Form
    {
        private SplitContainer splitContainer;
        private ListBox listBox;
        private WebBrowser webBrowserHtml;
        private ComboBox filterYear, filterAuthor;
        private TextBox filterTitle;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ProgressBar progressBar;
        private MenuStrip menuStrip;
        private List<Article> articles = new List<Article>();
        private List<Article> filteredArticles = new List<Article>();

        public MainForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "WordPress XML Tool";
            this.Width = 1280;
            this.Height = 720;
            try { this.Icon = new System.Drawing.Icon("Icon.ico"); } catch { /* Icon optional */ }

            // Menüleiste
            menuStrip = new MenuStrip();
            var dateiMenu = new ToolStripMenuItem("Datei");
            var miOpen = new ToolStripMenuItem("XML öffnen", null, BtnLoadXml_Click);
            dateiMenu.DropDownItems.Add(miOpen);
            dateiMenu.DropDownItems.Add(new ToolStripSeparator());
            var miExit = new ToolStripMenuItem("Beenden", null, (s, e) => this.Close());
            dateiMenu.DropDownItems.Add(miExit);
            var editMenu = new ToolStripMenuItem("Edit");
            var miEditArticle = new ToolStripMenuItem("Artikel bearbeiten...", null, EditArticle_Click);
            editMenu.DropDownItems.Add(miEditArticle);

            var exportMenu = new ToolStripMenuItem("Export");
            var miExportMd = new ToolStripMenuItem("Alle als Markdown", null, (s, e) => ExportAll("md"));
            var miExportTxt = new ToolStripMenuItem("Alle als TXT", null, (s, e) => ExportAll("txt"));
            var miExportJson = new ToolStripMenuItem("Alle als JSON", null, (s, e) => ExportAll("json"));
            var miExportXml = new ToolStripMenuItem("Alle als XML", null, (s, e) => ExportAll("xml"));
            var miExportSingle = new ToolStripMenuItem("Ausgewählten Artikel speichern", null, (s, e) => ExportSingle());
            exportMenu.DropDownItems.AddRange(new ToolStripItem[] { miExportMd, miExportTxt, miExportJson, miExportXml, new ToolStripSeparator(), miExportSingle });
            var helpMenu = new ToolStripMenuItem("Hilfe");
            var miAbout = new ToolStripMenuItem("Über", null, (s, e) => ShowAboutDialog());

            helpMenu.DropDownItems.Add(miAbout);
            menuStrip.Items.AddRange(new ToolStripItem[] { dateiMenu, editMenu, exportMenu, helpMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };
            this.Controls.Add(splitContainer);
            this.PerformLayout();
            splitContainer.SplitterDistance = (int)(this.ClientSize.Width * 0.25); // 25% links, 75% rechts

            // Filterleiste oben
            var filterPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, FlowDirection = FlowDirection.LeftToRight };
            filterPanel.Controls.Add(new Label { Text = "Titel:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });
            filterTitle = new TextBox { Width = 150 };
            filterTitle.TextChanged += (s, e) => ApplyFilter();
            filterPanel.Controls.Add(filterTitle);
            filterPanel.Controls.Add(new Label { Text = "Jahr:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });
            filterYear = new ComboBox { Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };
            filterYear.SelectedIndexChanged += (s, e) => ApplyFilter();
            filterPanel.Controls.Add(filterYear);
            filterPanel.Controls.Add(new Label { Text = "Autor:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });
            filterAuthor = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            filterAuthor.SelectedIndexChanged += (s, e) => ApplyFilter();
            filterPanel.Controls.Add(filterAuthor);
            this.Controls.Add(filterPanel);
            this.Controls.SetChildIndex(filterPanel, 0); // Filterleiste ganz oben

            // Linke Seite: ListBox
            listBox = new ListBox { Dock = DockStyle.Fill, Margin = new Padding(0, 4, 0, 0) };
            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            splitContainer.Panel1.Controls.Add(listBox);

            // Rechte Seite: WebBrowser für HTML-Inhalt
            webBrowserHtml = new WebBrowser { Dock = DockStyle.Fill, AllowWebBrowserDrop = false, IsWebBrowserContextMenuEnabled = true, ScriptErrorsSuppressed = true };
            splitContainer.Panel2.Controls.Add(webBrowserHtml);

            // Buttonleiste entfällt, alles über Menü

            // Statusleiste und Fortschrittsbalken
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel { Text = "Bereit" };
            statusStrip.Items.Add(statusLabel);
            progressBar = new ProgressBar { Dock = DockStyle.Bottom, Height = 10, Visible = false };
            this.Controls.Add(progressBar);
            this.Controls.Add(statusStrip);
        }

        private void ShowAboutDialog()
        {
            var dlg = new Form { Text = "Über", Width = 500, Height = 420, FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent };
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 320));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 300));
            try
            {
                var pic = new PictureBox { Image = System.Drawing.Image.FromFile("WordPress-XML-Tool.png"), SizeMode = PictureBoxSizeMode.Zoom, Dock = DockStyle.Fill, Height = 120 };
                layout.Controls.Add(pic, 0, 0);
            }
            catch { /* PNG optional */ }
            var label = new Label { Text = "WordPress XML Tool\nDieses Tool ist dafür gemacht, die eigenen Artikel aus einer XML-Sicherung zu begutachten und gegebenenfalls wiederzuverwenden.\n(c) 2026 Manfred Zainhofer", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter };
            layout.Controls.Add(label, 0, 1);
            dlg.Controls.Add(layout);
            dlg.ShowDialog(this);
        }

        private void EditArticle_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex < 0 || filteredArticles.Count <= listBox.SelectedIndex)
            {
                MessageBox.Show("Kein Artikel ausgewählt.");
                return;
            }
            var article = filteredArticles[listBox.SelectedIndex];
            using (var dlg = new EditArticleDialog(article))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Änderungen übernehmen
                    article.Title = dlg.EditedTitle;
                    article.Author = dlg.EditedAuthor;
                    article.Date = dlg.EditedDate;
                    // HTML-Inhalt asynchron sicherstellen
                    if (string.IsNullOrEmpty(dlg.EditedHtml))
                    {
                        try
                        {
                            // Versuche, den HTML-Inhalt nachträglich zu holen
                            var htmlTask = dlg.GetType().GetMethod("GetHtmlFromWebViewAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                ?.Invoke(dlg, null) as System.Threading.Tasks.Task<string>;
                            if (htmlTask != null)
                            {
                                htmlTask.Wait();
                                article.HtmlContent = htmlTask.Result;
                            }
                            else
                            {
                                article.HtmlContent = dlg.EditedHtml;
                            }
                        }
                        catch
                        {
                            article.HtmlContent = dlg.EditedHtml;
                        }
                    }
                    else
                    {
                        article.HtmlContent = dlg.EditedHtml;
                    }
                    UpdateListBox();
                    ListBox_SelectedIndexChanged(null, null);
                    statusLabel.Text = "Artikel bearbeitet.";
                }
            }
        }

        private void BtnLoadXml_Click(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog { Filter = "XML Files|*.xml" };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                statusLabel.Text = "Lade XML...";
                progressBar.Visible = true;
                Application.DoEvents();
                var xmlDoc = XDocument.Load(openFile.FileName);
                articles = ArticleParser.ParseFromXml(xmlDoc);
                filteredArticles = new List<Article>(articles);
                FillFilterCombos();
                UpdateListBox();
                statusLabel.Text = $"{articles.Count} Artikel geladen.";
                progressBar.Visible = false;
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex >= 0 && filteredArticles.Count > listBox.SelectedIndex)
            {
                var article = filteredArticles[listBox.SelectedIndex];
                webBrowserHtml.DocumentText = article.HtmlContent;
            }
        }

        private void ApplyFilter()
        {
            var title = filterTitle.Text.Trim();
            var year = filterYear.SelectedItem as string;
            var author = filterAuthor.SelectedItem as string;
            filteredArticles = ArticleFilter.Filter(articles, title, year, author);
            UpdateListBox();
        }

        private void FillFilterCombos()
        {
            filterYear.Items.Clear();
            filterYear.Items.Add("");
            foreach (var y in articles.Select(a => a.Date?.Length >= 4 ? a.Date.Substring(0, 4) : "").Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s))
                filterYear.Items.Add(y);
            filterYear.SelectedIndex = 0;
            filterAuthor.Items.Clear();
            filterAuthor.Items.Add("");
            foreach (var a in articles.Select(a => a.Author).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s))
                filterAuthor.Items.Add(a);
            filterAuthor.SelectedIndex = 0;
        }

        private void UpdateListBox()
        {
            listBox.Items.Clear();
            foreach (var article in filteredArticles)
                listBox.Items.Add(article.Title ?? "(kein Titel)");
        }

        private void ExportAll(string format)
        {
            if (filteredArticles.Count == 0)
            {
                MessageBox.Show("Keine Artikel zum Exportieren.");
                return;
            }
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                progressBar.Visible = true;
                progressBar.Maximum = filteredArticles.Count;
                progressBar.Value = 0;
                foreach (var article in filteredArticles)
                {
                    var safeTitle = ArticleExporter.MakeSafeFileName(article.Title);
                    var filePath = Path.Combine(folderDialog.SelectedPath, safeTitle + "." + format);
                    switch (format)
                    {
                        case "md": ArticleExporter.ExportMarkdown(article, filePath); break;
                        case "txt": ArticleExporter.ExportPlain(article, filePath); break;
                        case "json": ArticleExporter.ExportJson(article, filePath); break;
                        case "xml": ArticleExporter.ExportXml(article, filePath); break;
                    }
                    progressBar.Value++;
                }
                progressBar.Visible = false;
                MessageBox.Show($"Alle Artikel als {format.ToUpper()} gespeichert.");
            }
        }

        private void ExportSingle()
        {
            if (listBox.SelectedIndex < 0 || filteredArticles.Count <= listBox.SelectedIndex)
            {
                MessageBox.Show("Kein Artikel ausgewählt.");
                return;
            }
            var article = filteredArticles[listBox.SelectedIndex];
            var saveFile = new SaveFileDialog { Filter = "Markdown|*.md|Text|*.txt|JSON|*.json|XML|*.xml", FileName = article.Title };
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(saveFile.FileName).ToLower();
                switch (ext)
                {
                    case ".md": ArticleExporter.ExportMarkdown(article, saveFile.FileName); break;
                    case ".txt": ArticleExporter.ExportPlain(article, saveFile.FileName); break;
                    case ".json": ArticleExporter.ExportJson(article, saveFile.FileName); break;
                    case ".xml": ArticleExporter.ExportXml(article, saveFile.FileName); break;
                    default: ArticleExporter.ExportPlain(article, saveFile.FileName); break;
                }
                MessageBox.Show("Artikel gespeichert.");
            }
        }

        private string ArticleToMarkdown(XElement post)
        {
            // Nicht mehr benötigt, alles in ArticleExporter
            return string.Empty;
        }

        private string ArticleToPlain(XElement post)
        {
            // Nicht mehr benötigt, alles in ArticleExporter
            return string.Empty;
        }

        private string ArticleToJson(XElement post)
        {
            // Nicht mehr benötigt, alles in ArticleExporter
            return string.Empty;
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
