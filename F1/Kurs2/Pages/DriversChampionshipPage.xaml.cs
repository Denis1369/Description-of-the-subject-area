using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

namespace Kurs2.Pages
{
    public partial class DriversChampionshipPage : Page
    {
        public DriversChampionshipPage()
        {
            InitializeComponent();
            LoadResults();
        }

        private void LoadResults()
        {

            var tracks = App._context.Races
                .Select(t => t.Circuit.CircuitName)
                .Distinct()
                .ToList();

            var flatData = App._context.RaceResults
                .Include(r => r.Driver)
                .Include(r => r.Race)
                    .ThenInclude(race => race.Circuit)
                .Select(r => new
                {
                    r.Driver.Id,
                    r.Driver.Url,
                    FullName = $"{r.Driver.FirstName} {r.Driver.LastName}",
                    TrackTitle = r.Race.Circuit.CircuitName,
                    FinalPosition = r.FinishPosition,
                    Score = r.PointsAwarded ?? 0
                })
                .ToList();

            var table = new DataTable();
            table.Columns.Add("Позиция", typeof(int));
            table.Columns.Add("Гонщик", typeof(string));
            table.Columns.Add("WikiUrl", typeof(string));
            foreach (var track in tracks)
                table.Columns.Add(track, typeof(string));
            table.Columns.Add("Сумма очков", typeof(int));

            var racers = flatData
                .GroupBy(r => new { r.Id, r.FullName, r.Url })
                .Select(g => new
                {
                    g.Key.FullName,
                    g.Key.Url,
                    TotalScore = g.Sum(r => r.Score),
                    Results = g.ToDictionary(r => r.TrackTitle, r => r.FinalPosition)
                })
                .OrderByDescending(r => r.TotalScore)
                .ToList();

            int pos = 1;
            foreach (var rc in racers)
            {
                var row = table.NewRow();
                row["Позиция"] = pos++;
                row["Гонщик"] = rc.FullName;
                row["WikiUrl"] = rc.Url ?? "";
                row["Сумма очков"] = rc.TotalScore;

                foreach (var track in tracks)
                    row[track] = rc.Results.TryGetValue(track, out var p) ? p : "-";

                table.Rows.Add(row);
            }

            table.DefaultView.Sort = "[Сумма очков] DESC";

            ResultsGrid.ItemsSource = table.DefaultView;

            int insertAt = 2;
            foreach (var track in tracks)
            {
                ResultsGrid.Columns.Insert(insertAt++, new DataGridTextColumn
                {
                    Header = track,
                    Binding = new Binding($"[{track}]")
                });
            }
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
                string fileName = "Результаты чемпионата.pdf";
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
                        if (gridColumn.Header.ToString() == column.ColumnName)
                        {
                            isVisible = true;
                            break;
                        }
                    }
                    if (!isVisible) columnsToRemove.Add(column.ColumnName);
                }
                foreach (string columnName in columnsToRemove) dataTable.Columns.Remove(columnName);

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
                    Rectangle pageSize = new Rectangle(1200, 842);
                    Document doc = new Document(pageSize, 2, 2, 3, 3);
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);

                    doc.Open();

                    Font titleFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                    doc.Add(new Paragraph("Результаты чемпионата", titleFont)
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

                            if (i > 1 && i < dataTable.Columns.Count - 1)
                            {
                                text = text == "-" ? "-" : FormatPosition(value);
                            }

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

        private string FormatPosition(object position)
        {
            if (position == null) return "-";

            if (int.TryParse(position.ToString(), out int pos))
            {
                return pos switch
                {
                    1 => "1st",
                    2 => "2nd",
                    3 => "3rd",
                    4 => "4th",
                    5 => "5th",
                    6 => "6th",
                    7 => "7th",
                    8 => "8th",
                    9 => "9th",
                    10 => "10th",
                    _ => pos.ToString()
                };
            }
            return position.ToString();
        }
    }
}
