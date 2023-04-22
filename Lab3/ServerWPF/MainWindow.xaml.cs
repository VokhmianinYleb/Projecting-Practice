// MainWindow.xaml.cs

using System.Windows;

namespace ServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Обробник подій при натисканні на пункт меню
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Створення нового вікна з усіма номерами готелю
            Window window = new HotelRooms();
            // Демонстрація вікна
            window.Show();
        }
    }
}
