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
    /// Клас екранної форми для роботи з діаграмою середньої вартості номерів готелю по їх типах
    /// </summary>
    public partial class AveragePriceStats : Window
    {
        // Поле для збереження результатів запитів
        private IEnumerable<RoomType> query;

        /// <summary>
        /// Конструктор, в який передається об'єкт конектору до бази даних
        /// </summary>
        /// <param name="hoteldc">Конектор до бази даних</param>
        public AveragePriceStats(DataClient hoteldc)
        {
            this.query = hoteldc.GetDataAveragePriceStats();

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

        /// <summary>
        /// Метод який повертає перелік значень
        /// </summary>
        /// <param name="type">Тип певного номера готелю</param>
        /// <returns>Перелік значень для діаграми</returns>
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

        /// <summary>
        /// Обробник подій при натисканні на пункт меню друку
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
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

        /// <summary>
        /// Обробник подій при натисканні на пункт меню збереження графіка у форматі зображення .png
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
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
