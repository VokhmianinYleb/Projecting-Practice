// MainWindow.xaml.cs

using DataLib;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Поле для зберігання номерів готелю
        private IBindingList items;
        private DataClient hoteldc;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод для оновлення списку
        private void UpdateList()
        {
            // отримання списку з бази даних
            this.items = hoteldc.Room.GetNewBindingList();
            listBox.ItemsSource = this.items;
        }

        // Обробник подій при натисканні на пункт меню довідника гостей
        private void WindowGuests(object sender, RoutedEventArgs e)
        {
            Window window = new Guests(this.hoteldc);
            window.Show();
        }

        // Обробний подій під час запуску застосунку
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Створюємо нове підключення до бази даних
            hoteldc = new DataClient(Properties.Settings.Default.db_connection);

            // Оновлюємо список
            this.UpdateList();
        }

        // Обробник подій для пошуку
        private void textBoxSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Якщо поле пошуку порожнє, встановлюємо
            // компоненту listBox усі номери готелю
            if (textBoxSearch.Text == "")
            {
                listBox.ItemsSource = this.items;
            }
            else // Інакше - обираємо лише ті, які відповідають контенту пошуку
            {
                ItemCollection items = new ListBox().Items;

                foreach (Room item in this.items)
                {
                    string content = $"{item.id} {item.type} {item.status}";

                    if (content.ToLower().Contains(textBoxSearch.Text.ToLower()))
                    {
                        items.Add(item);
                    }
                }

                listBox.ItemsSource = items;
            }
        }

        // Обробний подій при натисканні на кнопку меню додавання нового номера готелю
        private void AddNewHotelRoom(object sender, RoutedEventArgs e)
        {
            // створення та демонстрація нового вікна з налаштуваннями номера готелю у форматі діалогового
            Window window = new HotelRoom(this.hoteldc, null);
            window.ShowDialog();
            // пілся того, як користувач закриє вікно, виконається оновлення списку
            this.UpdateList();
        }

        // Обробник подій при натисканні на кнопку контекстного меню редагування номера готелю
        private void EditHotelRoom(object sender, RoutedEventArgs e)
        {
            // якщо елемент обрано
            if (listBox.SelectedIndex >= 0)
            {
                // отримання обраного номера готелю
                Room room = ((Room)listBox.SelectedItem);
                // створенння вікна у форматі діалогового
                Window window = new HotelRoom(this.hoteldc, room);
                window.ShowDialog();
                // пілся того, як користувач закриє вікно, виконається оновлення списку
                this.UpdateList();
            }
        }

        // Обробник подій при натисканні на кнопку контекстного меню видалення номера готелю
        private void DeleteHotelRoom(object sender, RoutedEventArgs e)
        {
            // якщо елемент обрано
            if (listBox.SelectedIndex >= 0)
            {
                // отримання обраного номера готелю
                Room room = ((Room)listBox.SelectedItem);

                // перевірка на те, чи використовується цей номер готелю ще десь
                if (hoteldc.IsRoomBooked(room.id))
                {
                    MessageBox.Show("Ви не можете видалити цей номер готелю, оскільки в нього є бронювання!");
                    return;
                }

                // видалення номера, збереження змін, оновлення списку
                this.hoteldc.Room.DeleteOnSubmit(room);
                this.hoteldc.SubmitChanges();
                this.UpdateList();
            }
        }

        // Обробник подій при натисканні на пункт меню довідника бронювань
        private void WindowBookings(object sender, RoutedEventArgs e)
        {
            Window window = new HotelBookings(this.hoteldc);
            window.ShowDialog();
        }

        // Обробник подій при натисканні на пункт меню середньої вартості
        private void MenuItem_Click_AveragePrice(object sender, RoutedEventArgs e)
        {
            // Створити нове вікно
            Window window = new AveragePriceStats(this.hoteldc);
            window.ShowDialog(); // відобразити створене вікно
        }
    }
}
