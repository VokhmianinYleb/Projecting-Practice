// BookingsStats.xaml.cs

using DataLib;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClientWPF
{
    /// <summary>
    /// Клас екранної форми для роботи з діаграмою статистики бронювань по датам
    /// </summary>
    public partial class BookingsStats : Window
    {
        private DataClient hoteldc;

        /// <summary>
        /// Конструктор, який приймає об'єкт конектору до бази даних
        /// </summary>
        /// <param name="hoteldc">Об'єкт конектору</param>
        public BookingsStats(DataClient hoteldc)
        {
            InitializeComponent();
            this.hoteldc = hoteldc;

            // Встановлення початкових значень для компонентів, які відповідають за дати
            datePickerStart.SelectedDate = DateTime.Now.AddDays(-7).Date;
            datePickerEnd.SelectedDate = DateTime.Now.AddDays(7).Date;
        }

        /// <summary>
        /// Метод оновлення контексту, який впливатиме на вигляд графіку
        /// </summary>
        private void UpdateDateContext()
        {
            if (hoteldc == null) return;

            IEnumerable<BookingDateCount> query = this.hoteldc.GetDataBookingsStats(comboBox.SelectedIndex, datePickerStart.SelectedDate.Value.Date, datePickerEnd.SelectedDate.Value.Date);

            // Оголошення та ініціалізація значень та заголовків для графіку
            ChartValues<string> dates = new ChartValues<string>();
            ChartValues<int> counts = new ChartValues<int>();

            // Додавання значень
            foreach (var result in query)
            {
                dates.Add(result.Date.ToString().Split(' ')[0]);
                counts.Add(result.Count);
            }

            // Встановлення нового контексту
            this.DataContext = new
            {
                DateArray = dates,
                CountArray = counts
            };
        }

        /// <summary>
        /// Метод перевірки порожності компонентів, які відповідають за дати
        /// </summary>
        /// <returns>Значення логічного типу, яке відповідає за те, чи порожні компоненти, які відповідають за дати</returns>
        private bool CheckDatePickers()
        {
            return datePickerStart.SelectedDate == null || datePickerEnd.SelectedDate == null;
        }

        /// <summary>
        /// Обробник подій при зміні значення стартової дати
        /// </summary>
        /// <param name="sender">Об'єкт відправника</param>
        /// <param name="e">Об'єкт події</param>
        private void datePickerStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CheckDatePickers()) return;

            // Перевірка якщо кінцева дата менша за початкову
            if (datePickerEnd.SelectedDate.Value < datePickerStart.SelectedDate.Value)
            {
                MessageBox.Show("Кінцева дата повинна бути більшою за початкову!");
                datePickerStart.SelectedDate = datePickerEnd.SelectedDate.Value.AddDays(-1);
                return;
            }

            // Виклик методу оновлення контексту
            this.UpdateDateContext();
        }

        /// <summary>
        /// Обробник подій при зміні значення кінцевої дати
        /// </summary>
        /// <param name="sender">Об'єкт відправника</param>
        /// <param name="e">Об'єкт події</param>
        private void datePickerEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CheckDatePickers()) return;

            // Перевірка якщо кінцева дата менша за початкову
            if (datePickerEnd.SelectedDate.Value < datePickerStart.SelectedDate.Value)
            {
                MessageBox.Show("Кінцева дата повинна бути більшою за початкову!");
                datePickerEnd.SelectedDate = datePickerStart.SelectedDate.Value.AddDays(1);
                return;
            }

            // Виклик методу оновлення контексту
            this.UpdateDateContext();
        }

        /// <summary>
        /// Обробник подій при завантаженні вікна
        /// </summary>
        /// <param name="sender">Об'єкт відправника</param>
        /// <param name="e">Об'єкт події</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Оновити контекст
            this.UpdateDateContext();
        }

        /// <summary>
        /// Обробник подій при зміні обраного типу, за яким будується графік
        /// </summary>
        /// <param name="sender">Об'єкт відправника</param>
        /// <param name="e">Об'єкт події</param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Оновити контекст
            this.UpdateDateContext();
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
                printDialog.PrintVisual(mainChart, "Chart");
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
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)mainChart.ActualWidth, (int)mainChart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(mainChart); // зображення графіка

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
