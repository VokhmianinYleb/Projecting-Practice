// HotelRoom.xaml.cs

using System.Windows;

namespace ServerWPF
{
    /// <summary>
    /// Логика взаимодействия для HotelRoom.xaml
    /// </summary>
    public partial class HotelRoom : Window
    {
        // Конструктор, який приймає конкретний номер готелю
        public HotelRoom(string number)
        {
            InitializeComponent();
            // Встановлюємо нові назву вікна,
            // яка містить номер готелю
            this.Title += " " + number;
            // Також встановлюємо цей номер у поле для введення тексту
            textBoxNumber.Text = number;
        }
    }
}
