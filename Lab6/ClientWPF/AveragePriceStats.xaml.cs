// AveragePriceStats.xaml.cs

using DataLib;
using LiveCharts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PrintDialog = System.Windows.Controls.PrintDialog;

namespace ClientWPF
{
    /// <summary>
    /// Логика взаимодействия для AveragePriceStats.xaml
    /// </summary>
    public partial class AveragePriceStats : Window
    {
        // Поле для збереження результатів запитів
        private IEnumerable<RoomType> query;

        // Власний тип даних
        private class RoomType
        {
            public string Type { get; set; }
            public double AveragePrice { get; set; }
        }

        public AveragePriceStats(HotelDC hoteldc)
        {
            // Запит до БД який робить групування даних за типом номера готелю
            this.query = from r in hoteldc.Room
                        group r by r.type into typeGroup
                        select new RoomType { Type = typeGroup.Key, AveragePrice = typeGroup.Average(r => r.price) };

            // Встановлення контексту даних
            this.DataContext = new
            {
                standart = this.GetValues("Стандарт"),
                lux = this.GetValues("Люкс"),
                family = this.GetValues("Сімейний"),
                president = this.GetValues("Президентський")
            };

            InitializeComponent();
        }

        // Метод який повертає перелік значень
        private ChartValues<double> GetValues(string type)
        {
            ChartValues<double> values = new ChartValues<double>();
            var result = this.query.FirstOrDefault(x => x.Type == type);

            // Якщо номерів певного типу не знайдено, тоді значення приймаємо за 0
            if (result == null)
                values.Add(0);
            else
                values.Add(result.AveragePrice);

            return values;
        }

        // Обробник подій при натисканні на пункт меню друку
        private void MenuItem_Click_Print(object sender, RoutedEventArgs e)
        {
            // Створення діалогу друку
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                // Друк графіка
                printDialog.PrintVisual(pieChart, "Chart");
                // Закриття вікна
                printDialog.PrintQueue.Dispose();
            }
        }

        // Обробник подій при натисканні на пункт меню збереження графіка у форматі зображення .png
        private void MenuItem_Click_SaveToPng(object sender, RoutedEventArgs e)
        {
            // Растрове зображення
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)pieChart.ActualWidth, (int)pieChart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(pieChart); // зображення графіка

            // Створення декодеру
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            // Створення діалогового вікна обрання шляху
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = "chart"; // ім'я файлу за замовчуванням
            saveFileDialog.DefaultExt = ".png"; // розширення файлу за замовчуванням
            saveFileDialog.Filter = "PNG Image (.png)|*.png"; // фільтр для вибору типу файлів

            // Відображення діалогового вікна вибору файла
            bool? result = saveFileDialog.ShowDialog();

            // Якщо користувач натиснув на збереження
            if (result == true)
            {
                // Збереження файлу за вказаним шляхом
                FileStream stream = File.Create(saveFileDialog.FileName);
                png.Save(stream);
            }
        }
    }
}
