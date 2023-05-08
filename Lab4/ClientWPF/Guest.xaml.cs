// Guest.xaml.cs

using DataLib;
using System.Windows;
using System.Windows.Media;

namespace ClientWPF
{
    /// <summary>
    /// Логика взаимодействия для Guest.xaml
    /// </summary>
    public partial class Guest : Window
    {
        // приватні поля для збереження коннектора та поточного гостя
        private HotelDC hoteldc;
        private DataLib.Guest guest;

        // конструктор
        public Guest(HotelDC hoteldc, DataLib.Guest guest)
        {
            InitializeComponent();
            this.hoteldc = hoteldc;
            this.guest = guest;

            // якщо редагування налаштувань - встановлюємо існуючі налаштування
            if (guest != null)
            {
                textBoxSurname.Text = this.guest.surname;
                textBoxName.Text = this.guest.name;
                textBoxPatronymic.Text = this.guest.patronymic;
                textBoxPhone.Text = this.guest.phone;
                textBoxPassport.Text = this.guest.passport;
            }

            // встановлення підказок у текстові поля
            new Placeholder(textBoxSurname, "Прізвище");
            new Placeholder(textBoxName, "Ім'я");
            new Placeholder(textBoxPatronymic, "По-батькові");
            new Placeholder(textBoxPassport, "Паспорт");
            new Placeholder(textBoxPhone, "Номер телефону");
        }

        // Обробний подій при натисканні на кнопку очищення текстових полів
        private void ClearAll(object sender, RoutedEventArgs e)
        {
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            textBoxPatronymic.Text = "";
            textBoxPassport.Text = "";
            textBoxPhone.Text = "";
        }

        // Обробний подій при натисканні на кнопку збереження
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            // Якщо не всі поля заповнені - повідомляємо користувача
            if (textBoxName.Foreground == Brushes.Gray || textBoxSurname.Foreground == Brushes.Gray || textBoxPatronymic.Foreground == Brushes.Gray || textBoxPhone.Foreground == Brushes.Gray || textBoxPassport.Foreground == Brushes.Gray)
            {
                MessageBox.Show("Необхідно заповнити усі поля!");
                return;
            }

            // якщо додавання нового гостя
            if (this.guest == null)
            {
                DataLib.Guest NewGuest = new DataLib.Guest
                {
                    name = textBoxName.Text,
                    surname = textBoxSurname.Text,
                    patronymic = textBoxPatronymic.Text,
                    phone = textBoxPhone.Text,
                    passport = textBoxPassport.Text
                };

                hoteldc.Guest.InsertOnSubmit(NewGuest);
            }
            else // інакше - редагуємо поточного гостя
            {
                this.guest.name = textBoxName.Text;
                this.guest.surname = textBoxSurname.Text;
                this.guest.patronymic = textBoxPatronymic.Text;
                this.guest.phone = textBoxPhone.Text;
                this.guest.passport = textBoxPassport.Text;
            }

            // збереження змін, повідомлення користувача про успіх
            hoteldc.SubmitChanges();
            MessageBox.Show("Успішно збережено!");
            // закриття поточного вікна
            this.Close();
        }
    }
}
