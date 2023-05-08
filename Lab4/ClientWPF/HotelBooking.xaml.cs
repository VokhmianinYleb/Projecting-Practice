﻿// HotelBooking.xaml.cs

using DataLib;
using System.Linq;
using System.Windows;

namespace ClientWPF
{
    /// <summary>
    /// Логика взаимодействия для HotelBooking.xaml
    /// </summary>
    public partial class HotelBooking : Window
    {
        // приватні поля для збереження коннектора та поточного бронювання
        private HotelDC hoteldc;
        private Booking booking;

        // Конструктор
        public HotelBooking(HotelDC hoteldc, Booking booking)
        {
            InitializeComponent();
            this.hoteldc = hoteldc;
            this.booking = booking;

            // отримання списків гостей та номерів готелю
            comboBoxRooms.ItemsSource = hoteldc.Room.GetNewBindingList();
            comboBoxGuests.ItemsSource = hoteldc.Guest.GetNewBindingList();

            // якщо редагування обраного бронювання
            if (booking != null)
            {
                comboBoxRooms.SelectedItem = (Room)hoteldc.Room.Where(r => r.id == booking.id_room).FirstOrDefault();
                comboBoxGuests.SelectedItem = (DataLib.Guest)hoteldc.Guest.Where(g => g.id == booking.id_guest).FirstOrDefault();
                datePickerArrival.SelectedDate = booking.arrival_date;
                datePickerDeparture.SelectedDate = booking.departure_date;
            }
        }

        // Обробник подій при натисканні на кнопку збереження змін
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            // Перевірка на те, чи вказані дати
            if (datePickerArrival.SelectedDate == null || datePickerDeparture.SelectedDate == null)
            {
                MessageBox.Show("Дати повинні бути заповнені!");
                return;
            }

            // Дата заїзду повинна бути раніша за дату виїзду
            if (datePickerArrival.SelectedDate >= datePickerDeparture.SelectedDate)
            {
                MessageBox.Show("Дата виїзду повинна бути пізніше за дату заїзду!");
                return;
            }

            // якщо додавання нового бронювання
            if (this.booking == null)
            {
                Booking b = new Booking
                {
                    id_room = ((Room)comboBoxRooms.SelectedItem).id,
                    id_guest = ((DataLib.Guest)comboBoxGuests.SelectedItem).id,
                    arrival_date = datePickerArrival.SelectedDate.Value,
                    departure_date = datePickerDeparture.SelectedDate.Value
                };

                hoteldc.Booking.InsertOnSubmit(b);
            }
            else // інакше - редагування обраного бронювання
            {
                this.booking.id_room = ((Room)comboBoxRooms.SelectedItem).id;
                this.booking.id_guest = ((DataLib.Guest)comboBoxGuests.SelectedItem).id;
                this.booking.arrival_date = datePickerArrival.SelectedDate.Value;
                this.booking.departure_date = datePickerDeparture.SelectedDate.Value;
            }

            // збереження змін, повідомлення користувача про успіх та закриття вікна
            hoteldc.SubmitChanges();
            MessageBox.Show("Успішно збережено!");
            this.Close();
        }
    }
}
