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
    /// Логика взаимодействия для BookingsStats.xaml
    /// </summary>
    public partial class BookingsStats : Window
    {
        private DataClient hoteldc;

        public BookingsStats(DataClient hoteldc)
        {
            InitializeComponent();
            this.hoteldc = hoteldc;

            // Встановлення початкових значень для компонентів, які відповідають за дати
            datePickerStart.SelectedDate = DateTime.Now.AddDays(-7).Date;
            datePickerEnd.SelectedDate = DateTime.Now.AddDays(7).Date;
        }

        // Метод оновлення контексту, який впливатиме на вигляд графіку
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

        // Метод перевірки порожності компонентів, які відповідають за дати
        private bool CheckDatePickers()
        {
            return datePickerStart.SelectedDate == null || datePickerEnd.SelectedDate == null;
        }

        // Обробник подій при зміні значення стартової дати
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

        // Обробник подій при зміні значення кінцевої дати
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

        // Обробник подій при завантаженні вікна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Оновити контекст
            this.UpdateDateContext();
        }

        // Обробник подій при зміні обраного типу, за яким будується графік
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Оновити контекст
            this.UpdateDateContext();
        }

        // Обробник подій при натисканні на пункт меню друку
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

        // Обробник подій при натисканні на пункт меню збереження графіка у форматі зображення .png
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
