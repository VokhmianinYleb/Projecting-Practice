// Guest.xaml.cs

using DataLib;
using System.Windows;
using System.Windows.Media;

namespace ClientWPF
{
    /// <summary>
    /// Клас екранної форми для роботи з формою гостя
    /// </summary>
    public partial class Guest : Window
    {
        // приватні поля для збереження коннектора та поточного гостя
        private HotelDC hoteldc;
        private DataLib.Guest guest;

        /// <summary>
        /// Конструктор, який приймає об'єкт конектору до бази даних та конкретного гостя готелю, який попередньо був обраний
        /// </summary>
        /// <param name="hoteldc">Об'єкт конектору до БД</param>
        /// <param name="guest">Об'єкт класу Гість</param>
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
                textBoxEmail.Text = this.guest.email;
                textBoxPhone.Text = this.guest.phone;
                textBoxPassport.Text = this.guest.passport;
            }

            // встановлення підказок у текстові поля
            new Placeholder(textBoxSurname, "Прізвище");
            new Placeholder(textBoxName, "Ім'я");
            new Placeholder(textBoxPatronymic, "По-батькові");
            new Placeholder(textBoxEmail, "Електронна пошта");
            new Placeholder(textBoxPassport, "Паспорт");
            new Placeholder(textBoxPhone, "Номер телефону");
        }

        /// <summary>
        /// Обробний подій при натисканні на кнопку очищення текстових полів
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void ClearAll(object sender, RoutedEventArgs e)
        {
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            textBoxPatronymic.Text = "";
            textBoxPassport.Text = "";
            textBoxEmail.Text = "";
            textBoxPhone.Text = "";
        }

        /// <summary>
        /// Обробний подій при натисканні на кнопку збереження
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            // Якщо не всі поля заповнені - повідомляємо користувача
            if (textBoxName.Foreground == Brushes.Gray || textBoxSurname.Foreground == Brushes.Gray || textBoxPatronymic.Foreground == Brushes.Gray || textBoxEmail.Foreground == Brushes.Gray || textBoxPhone.Foreground == Brushes.Gray || textBoxPassport.Foreground == Brushes.Gray)
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
                    email = textBoxEmail.Text,
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
                this.guest.email = textBoxEmail.Text;
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
