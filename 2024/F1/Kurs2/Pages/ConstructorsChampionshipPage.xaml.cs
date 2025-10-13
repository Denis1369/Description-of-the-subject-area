using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

namespace Kurs2.Pages
{
    public partial class ConstructorsChampionshipPage : Page
    {
        public ConstructorsChampionshipPage()
        {
            InitializeComponent();
            LoadResults();
        }

        private void LoadResults()
        {
            var grandPrixList = App._context.Races
                .Select(r => r.Circuit.CircuitName)
                .Distinct()
                .ToList();

            var raw = App._context.RaceResults
                .Include(r => r.Constructor)
                .Include(r => r.Race).ThenInclude(r => r.Circuit)
                .Select(r => new
                {
                    ConstructorName = r.Constructor.ConstructorName,
                    ConstructorUrl = r.Constructor.Url,
                    GP = r.Race.Circuit.CircuitName,
                    FinishPosition = r.FinishPosition,
                    Points = r.PointsAwarded ?? 0,
                    DriverId = r.DriverId
                })
                .ToList();

            var teams = raw
                .GroupBy(x => new { x.ConstructorName, x.ConstructorUrl })
                .Select(g => new
                {
                    ConstructorName = g.Key.ConstructorName,
                    ConstructorUrl = g.Key.ConstructorUrl,
                    TotalPoints = g.Sum(x => (int)x.Points),
                    GpPositions = g.GroupBy(x => x.GP)
                                   .ToDictionary(
                                        gp => gp.Key,
                                        gp => gp
                                            .OrderBy(x => x.DriverId)
                                            .Select(x => x.FinishPosition?.ToString() ?? "-")
                                            .ToArray()
                                   )
                })
                .OrderByDescending(t => t.TotalPoints)
                .ToList();

            var table = new DataTable();
            table.Columns.Add("Позиция", typeof(int));
            table.Columns.Add("Конструктор", typeof(string));
            table.Columns.Add("ConstructorUrl", typeof(string));

            foreach (var gp in grandPrixList)
                table.Columns.Add(gp, typeof(string));

            table.Columns.Add("Очки", typeof(int));

            int pos = 1;
            foreach (var t in teams)
            {
                var row = table.NewRow();
                row["Позиция"] = pos++;
                row["Конструктор"] = t.ConstructorName;
                row["ConstructorUrl"] = t.ConstructorUrl ?? "";

                foreach (var gp in grandPrixList)
                {
                    if (t.GpPositions.TryGetValue(gp, out var positions))
                        row[gp] = string.Join("/", positions);
                    else
                        row[gp] = "-";
                }

                row["Очки"] = t.TotalPoints;
                table.Rows.Add(row);
            }

            table.DefaultView.Sort = "Позиция ASC";
            ResultsGrid.ItemsSource = table.DefaultView;

            while (ResultsGrid.Columns.Count > 3)
                ResultsGrid.Columns.RemoveAt(ResultsGrid.Columns.Count - 1);

            foreach (var gp in grandPrixList)
            {
                ResultsGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = gp,
                    Binding = new Binding($"[{gp}]")
                });
            }

            var pointsColumn = new DataGridTextColumn
            {
                Header = "Очки",
                Binding = new Binding("[Очки]"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right),
                        new Setter(TextBlock.PaddingProperty, new Thickness(0, 0, 5, 0))
                    }
                }
            };
            ResultsGrid.Columns.Add(pointsColumn);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = "Конструкторы чемпионата.pdf";
                string fullPath = Path.Combine(docsPath, fileName);

                if (File.Exists(fullPath))
                {
                    var result = MessageBox.Show(
                        "Файл уже существует. Перезаписать?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes) return;
                }

                DataView dataView = (DataView)ResultsGrid.ItemsSource;
                DataTable dataTable = dataView.ToTable();

                List<string> columnsToRemove = new List<string>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    bool isVisible = false;
                    foreach (DataGridColumn gridColumn in ResultsGrid.Columns)
                    {
                        if (gridColumn.Header != null && gridColumn.Header.ToString() == column.ColumnName)
                        {
                            isVisible = true;
                            break;
                        }
                    }
                    if (!isVisible) columnsToRemove.Add(column.ColumnName);
                }
                foreach (string columnName in columnsToRemove)
                    dataTable.Columns.Remove(columnName);

                Dictionary<string, string> trackShortNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"Melbourne Grand Prix Circuit", "AUS"},
            {"Shanghai International Circuit", "CHN"},
            {"Suzuka International Racing Course", "JPN"},
            {"Bahrain International Circuit", "BHR"},
            {"Jeddah Corniche Circuit", "SAU"},
            {"Miami International Autodrome", "MIA"},
            {"Autodromo Enzo e Dino Ferrari", "EMI"},
            {"Circuit de Monaco", "MON"},
            {"Circuit de Barcelona-Catalunya", "ESP"},
            {"Circuit Gilles Villeneuve", "CAN"},
            {"Red Bull Ring", "AUT"},
            {"Silverstone Circuit", "GBR"},
            {"Circuit de Spa-Francorchamps", "BEL"},
            {"Hungaroring", "HUN"},
            {"Circuit Zandvoort", "NED"},
            {"Autodromo Nazionale Monza", "ITA"},
            {"Baku City Circuit", "AZE"},
            {"Marina Bay Street Circuit", "SIN"},
            {"Circuit of the Americas", "USA"},
            {"Autodromo Hermanos Rodriguez", "MEX"},
            {"Autódromo José Carlos Pace", "BRA"},
            {"Las Vegas Street Circuit", "LVS"},
            {"Lusail International Circuit", "QAT"},
            {"Yas Marina Circuit", "ABU"}
        };

                foreach (DataColumn column in dataTable.Columns)
                {
                    if (trackShortNames.ContainsKey(column.ColumnName))
                    {
                        column.ColumnName = trackShortNames[column.ColumnName];
                    }
                }

                using (FileStream fs = new FileStream(fullPath, FileMode.Create))
                {
                    Document doc = new Document(PageSize.A4.Rotate(), 2, 2, 3, 3);
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);

                    doc.Open();

                    Font titleFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                    doc.Add(new Paragraph("Конструкторы чемпионата", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 3f
                    });

                    PdfPTable pdfTable = new PdfPTable(dataTable.Columns.Count);
                    pdfTable.WidthPercentage = 100;
                    pdfTable.DefaultCell.Padding = 1;
                    pdfTable.DefaultCell.BorderWidth = 0.3f;
                    pdfTable.DefaultCell.BorderColor = BaseColor.LIGHT_GRAY;

                    Font cellFont = new Font(Font.FontFamily.HELVETICA, 6);
                    Font headerFont = new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD);

                    float[] columnWidths = new float[dataTable.Columns.Count];
                    columnWidths[0] = 0.7f; 
                    columnWidths[1] = 2.5f;

                    for (int i = 2; i < columnWidths.Length - 1; i++)
                        columnWidths[i] = 0.7f;

                    columnWidths[columnWidths.Length - 1] = 1.2f; 
                    pdfTable.SetWidths(columnWidths);

                    foreach (DataColumn column in dataTable.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, headerFont));
                        cell.BackgroundColor = new BaseColor(240, 240, 240);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.MinimumHeight = 10;
                        pdfTable.AddCell(cell);
                    }

                    foreach (DataRow row in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            DataColumn column = dataTable.Columns[i];
                            object value = row[column];
                            string text = value.ToString();

                            PdfPCell cell = new PdfPCell(new Phrase(text, cellFont));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.MinimumHeight = 8;

                            if (row["Позиция"].ToString() == "1" && i == 0)
                            {
                                cell.BackgroundColor = new BaseColor(255, 255, 150);
                            }
                            else if (row["Позиция"].ToString() == "2" && i == 0)
                            {
                                cell.BackgroundColor = new BaseColor(230, 230, 230);
                            }
                            else if (row["Позиция"].ToString() == "3" && i == 0)
                            {
                                cell.BackgroundColor = new BaseColor(205, 127, 50);
                            }

                            pdfTable.AddCell(cell);
                        }
                    }

                    doc.Add(pdfTable);
                    doc.Close();
                }

                MessageBox.Show(
                    $"Данные успешно экспортированы в:\n{fullPath}",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при экспорте: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
