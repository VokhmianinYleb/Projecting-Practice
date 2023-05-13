// HotelRoom.xaml.cs

using DataLib;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientWPF
{
    /// <summary>
    /// Логика взаимодействия для HotelRoom.xaml
    /// </summary>
    public partial class HotelRoom : Window
    {
        private HotelDC hoteldc;
        private Room room;

        // Конструктор, який приймає конкретний номер готелю
        public HotelRoom(HotelDC hoteldc, Room room)
        {
            InitializeComponent();
            this.hoteldc = hoteldc;
            this.room = room;

            comboBoxGuests.ItemsSource = hoteldc.Guest.GetNewBindingList();

            // якщо номер готелю було передано, встановлюємо існуючі налаштування
            if (this.room != null)
            {
                textBoxNumber.IsReadOnly = true;
                textBoxNumber.Text = this.room.id.ToString();
                textBoxPrice.Text = this.room.price.ToString();
                comboBoxType.Text = this.room.type;
                comboBoxStatus.Text = this.room.status;

                // якщо номер зайнятий, шукаємо гостя та встановлюємо його
                if (this.room.id_guest != null)
                {
                    DataLib.Guest guest = hoteldc.Guest.Where(g => g.id == this.room.id_guest).First();
                    comboBoxGuests.SelectedItem = guest;
                }
            }

            // перевіряємо статус номера готелю
            this.CheckedCurrentStatus();

            // додаємо підказки для текстових полів
            new Placeholder(textBoxNumber, "Номер кімнати");
            new Placeholder(textBoxPrice, "Вартість за добу");
        }

        // Метод перевірки статусу номера готелю. Якщо номер зайнятий - необхідно відобразити
        // поле для обрання гостя, на якого номер заброньований
        // інакше - необхідно сховати це поле
        private void CheckedCurrentStatus()
        {
            if (comboBoxStatus.SelectedIndex == 1)
            {
                comboBoxGuests.Visibility = Visibility.Visible;
            }
            else
            {
                comboBoxGuests.Visibility = Visibility.Hidden;
            }
        }

        // Обробний подій при натисканні на кнопку збереження
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            // оголошення додаткових змінних
            int id;
            float price;
            int? id_guest = null;

            // перевірка на незаповнені поля
            if (textBoxNumber.Foreground == Brushes.Gray || textBoxPrice.Foreground == Brushes.Gray)
            {
                MessageBox.Show("Всі поля повинні бути заповненими!");
                return;
            }

            // спроба отримати чисельні значення
            try
            {
                id = int.Parse(textBoxNumber.Text);
                price = float.Parse(textBoxPrice.Text);
            }
            catch // якщо помилка - повідомляємо користувача
            {
                MessageBox.Show("Ідентифікатор та ціна має бути числом.");
                return;
            }

            // перевірка на унікальність ідентифікатора, якщо користувач додає новий номер
            // у випадку редагування, поле для зміни номера кімнати недоступне
            if (this.room == null && hoteldc.Room.Where(r => r.id == id).FirstOrDefault() != null)
            {
                MessageBox.Show("Ідентифікатор має бути унікальним, без повторень!");
                return;
            }

            // отримання гостя, якщо статус номера готелю - зайнятий
            // інакше гість залишиться як null
            if (comboBoxGuests.Visibility == Visibility.Visible)
                id_guest = ((DataLib.Guest)comboBoxGuests.SelectedItem).id;

            // якщо відбувається додавання нового номера
            if (this.room == null)
            {
                Room room = new Room
                {
                    id = id,
                    type = comboBoxType.Text,
                    status = comboBoxStatus.Text,
                    price = price,
                    id_guest = id_guest
                };

                hoteldc.Room.InsertOnSubmit(room);
            }
            else // якщо відбувається редагування існуючого номера готелю
            {
                this.room.id = id;
                this.room.type = comboBoxType.Text;
                this.room.status = comboBoxStatus.Text;
                this.room.price = price;
                this.room.id_guest = id_guest;
            }

            // збереження змін
            hoteldc.SubmitChanges();
            // оновлення з'єдняння для того, аби у інших довідниках оновились зміни
            this.hoteldc = new HotelDC(Properties.Settings.Default.db_connection);
            MessageBox.Show("Успішно збережено!"); // повідомляємо про успіх
            this.Close(); // закриваємо вікно
        }

        // Обробник подій при виборі статуса номера готелю
        // якщо "зайнятий" - необхідно відобразити поле для обрання гостя
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxGuests == null)
                return;

            this.CheckedCurrentStatus();
        }
    }
}
