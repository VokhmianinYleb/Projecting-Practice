// HotelBookings.xaml.cs

using DataLib;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClientWPF
{
    /// <summary>
    /// Клас екранної форми для роботи з переліком усіх бронювань
    /// </summary>
    public partial class HotelBookings : Window
    {
        // приватні поля класу для коннектора бази даних та елементів списку
        private DataClient hoteldc;
        private IBindingList items;

        /// <summary>
        /// Конструктор, який приймає об'єкт конектору до БД
        /// </summary>
        /// <param name="hoteldc">Об'єкт конектору до БД</param>
        public HotelBookings(DataClient hoteldc)
        {
            InitializeComponent();
            this.hoteldc = hoteldc;
            this.UpdateList();
        }

        /// <summary>
        /// Метод для оновлення списку
        /// </summary>
        private void UpdateList()
        {
            // Отримання списку з бази даних
            this.items = hoteldc.Booking.GetNewBindingList();
            listBoxBooking.ItemsSource = this.items;
        }

        /// <summary>
        /// Обробник подій при натисканні на пункт меню створення нового бронювання
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void CreateBooking(object sender, RoutedEventArgs e)
        {
            // створення нового вікна налаштувань бронювання
            Window window = new HotelBooking(this.hoteldc, null);
            // відображення у форматі діалогового вікна
            window.ShowDialog();
            // після того, як користувач закриє вікно, оновлюємо список
            this.UpdateList();
        }

        /// <summary>
        /// Обробник подій при натисканні на пункт контекстного меню редагування бронювання
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void EditBooking(object sender, RoutedEventArgs e)
        {
            // Якщо елемент у списку обраний
            if (listBoxBooking.SelectedIndex >= 0)
            {
                // отримання обраного бронювання
                Booking booking = (Booking)listBoxBooking.SelectedItem;
                // створення нового вікна для налаштувань бронювання
                Window window = new HotelBooking(this.hoteldc, booking);
                // демонстрація вікна у діалоговому форматі
                window.ShowDialog();
                // оновлення списку після того, як користувач закриє вікно налаштувань
                this.UpdateList();
            }
        }

        /// <summary>
        /// Обробний подій при пошуку
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Якщо поле пошуку порожнє, встановлюємо
            // компоненту listBox усі номери готелю
            if (textBoxSearch.Text == "")
            {
                listBoxBooking.ItemsSource = this.items;
            }
            else // Інакше - обираємо лише ті, які відповідають контенту пошуку
            {
                ItemCollection items = new ListBox().Items;

                foreach (Booking item in this.items)
                {
                    string content = $"{item.id_room} {item.arrival_date} {item.departure_date}";

                    if (content.ToLower().Contains(textBoxSearch.Text.ToLower()))
                    {
                        items.Add(item);
                    }
                }

                listBoxBooking.ItemsSource = items;
            }
        }

        /// <summary>
        /// Обробник подій при натисканні на пункт контекстного меню видалення бронювання
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            // якщо елемент було обрано
            if (listBoxBooking.SelectedIndex >= 0)
            {
                // отримання обраного бронювання
                Booking booking = (Booking)listBoxBooking.SelectedItem;
                // видалення з бази даних, збереження змін та оновлення списку
                this.hoteldc.Booking.DeleteOnSubmit(booking);
                this.hoteldc.SubmitChanges();
                this.UpdateList();
            }
        }

        /// <summary>
        /// Обробник подій при натисканні на пункт меню редагування бронювання
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxBooking.SelectedIndex >= 0)
            {
                Booking booking = (Booking)listBoxBooking.SelectedItem;
                Window window = new InvitationBooking(this.hoteldc, booking);
                window.ShowDialog();
            }
        }

        /// <summary>
        /// Обробник подій при натисканні на пункт меню статистики
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void BookingStats(object sender, RoutedEventArgs e)
        {
            // Створити нове вікно
            Window window = new BookingsStats(this.hoteldc);
            window.ShowDialog(); // відобразити створене вікно
        }
    }
}
