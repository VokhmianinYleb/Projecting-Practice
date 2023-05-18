// Guests.xaml.cs

using DataLib;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClientWPF
{
    /// <summary>
    /// Клас екранної форми для роботи з переліком усіх гостей
    /// </summary>
    public partial class Guests : Window
    {
        // приватні поля для збереження коннектора та елементів списку
        private HotelDC hoteldc;
        private IBindingList items;

        /// <summary>
        /// Конструктор, який приймає об'єкт конектору до БД
        /// </summary>
        /// <param name="hoteldc">Об'єкт конектору до БД</param>
        public Guests(HotelDC hoteldc)
        {
            InitializeComponent();
            this.hoteldc = hoteldc;
            this.updateListBox();
        }

        /// <summary>
        /// Метод для оновлення списку
        /// </summary>
        private void updateListBox()
        {
            // отримання списку елементів з бази даних
            this.items = hoteldc.Guest.GetNewBindingList();
            listBoxGuests.ItemsSource = this.items;
        }

        /// <summary>
        /// Обробник подій при натисканні на кнопку меню додавання нового гостя
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void NewGuest(object sender, RoutedEventArgs e)
        {
            // створення вікна для налаштувань гостя
            Window window = new Guest(this.hoteldc, null);
            // демонстрація вікна у діалоговому форматі
            window.ShowDialog();
            // оновлення списку
            this.updateListBox();
        }

        /// <summary>
        /// Обробник подій при натисканні на пункт контекстного меню редагування гостя
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void EditGuest(object sender, RoutedEventArgs e)
        {
            // якщо елемент було обрано
            if (listBoxGuests.SelectedItem != null)
            {
                // отримання обраного гостя
                DataLib.Guest guest = ((DataLib.Guest)listBoxGuests.SelectedItem);
                // створення нового вікна
                Window window = new Guest(hoteldc, guest);
                // демонстрація вікна у діалоговому форматі
                window.ShowDialog();
                // оновлення списку елементів
                this.updateListBox();
            }
        }

        /// <summary>
        /// Обробник подій при натисканні на пункт контекстного меню видалення гостя
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void DeleteGuest(object sender, RoutedEventArgs e)
        {
            // якщо елемент було обрано
            if (listBoxGuests.SelectedItem != null)
            {
                // отримання обраного гостя
                DataLib.Guest guest = (DataLib.Guest)listBoxGuests.SelectedItem;

                // якщо гість пов'язан з іншими таблицями — повідомляємо користувача
                if (hoteldc.Room.Where(r => r.id_guest == guest.id).FirstOrDefault() != null ||
                    hoteldc.Booking.Where(b => b.id_guest == guest.id).FirstOrDefault() != null)
                {
                    MessageBox.Show("Ви не можете видалити гостя, якщо на нього зареєстрований або заброньований номер готелю!");
                    return;
                }

                // видаляємо гостя, зберігаємо дані, оновлюємо список елементів
                this.hoteldc.Guest.DeleteOnSubmit(guest);
                this.hoteldc.SubmitChanges();
                this.updateListBox();
            }
        }

        /// <summary>
        /// Обробник подій під час пошуку
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void textBoxSearchGuest_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Якщо поле пошуку порожнє, встановлюємо
            // компоненту textBoxSearchGuest усі номери готелю
            if (textBoxSearchGuest.Text == "")
            {
                listBoxGuests.ItemsSource = this.items;
            }
            else // Інакше - обираємо лише ті, які відповідають контенту пошуку
            {
                ItemCollection items = new ListBox().Items;

                foreach (DataLib.Guest item in this.items)
                {
                    string content = $"{item.surname} {item.name} {item.patronymic}";

                    if (content.ToLower().Contains(textBoxSearchGuest.Text.ToLower()))
                    {
                        items.Add(item);
                    }
                }

                listBoxGuests.ItemsSource = items;
            }
        }
    }
}