// DataClient.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataLib;

namespace ClientWPF
{
    /// <summary>
    /// Клас, який наслідує HotelDC та реалізує функціональні методи для співпраці з базою даних,
    /// незалежно від окремих форм
    /// </summary>
    public class DataClient : HotelDC
    {
        /// <summary>
        /// Конструктор, який ініціалізує з'єднання з базою даних
        /// </summary>
        /// <param name="db_connection">Посилання для підключення до бази даних</param>
        public DataClient(string db_connection) : base(db_connection)
        {
            if (!this.DatabaseExists())
                this.CreateDatabase();
        }

        /// <summary>
        /// Метод, який перевіряє наявність бронювань для вказаного номера готелю
        /// </summary>
        /// <param name="id_room">Номер готелю</param>
        /// <returns>Значення логічного типу, яке визначає, чи є бронювання для вказаного номера готелю</returns>
        public bool IsRoomBooked(int id_room)
        {
            return this.Booking.Where(x => x.id_room == id_room).FirstOrDefault() != null;
        }

        /// <summary>
        /// Метод, який перевіряє наявність вказаного номера готелю у базі даних
        /// </summary>
        /// <param name="id_room">Номер готелю</param>
        /// <returns>Значення логічного типу, яке визначає, чи є вказаний номер готелю у базі даних</returns>
        public bool RoomExists(int id_room)
        {
            return this.Room.Where(x => x.id == id_room).FirstOrDefault() != null;
        }

        /// <summary>
        /// Метод, який призначений для групування номерів готелю за їх типом
        /// </summary>
        /// <returns>Перелік номерів готелю, згрупованих по типу</returns>
        public IEnumerable<RoomType> GetDataAveragePriceStats()
        {
            return from r in this.Room
                         group r by r.type into typeGroup
                         select new RoomType { Type = typeGroup.Key, AveragePrice = typeGroup.Average(r => r.price) };
        }

        /// <summary>
        /// Метод, який в залежності від обраного типу дати бронювання групує дані
        /// </summary>
        /// <param name="index">Індекс обраного типу дати</param>
        /// <param name="start">Початкова дата пошуку</param>
        /// <param name="end">Кінцева дата пошуку</param>
        /// <returns>Перелік згрупованих даних по даті</returns>
        public IEnumerable<BookingDateCount> GetDataBookingsStats(int index, DateTime start, DateTime end)
        {
            if (index == 0)
                return from b in this.Booking
                       where b.create_date.Date >= start && b.create_date.Date <= end
                       group b by b.create_date.Date into dateGroup
                       select new BookingDateCount { Date = dateGroup.Key, Count = dateGroup.Count() };

            if (index == 1)
                return from b in this.Booking
                       where b.arrival_date.Date >= start && b.arrival_date.Date <= end
                       group b by b.arrival_date.Date into dateGroup
                       select new BookingDateCount { Date = dateGroup.Key, Count = dateGroup.Count() };

            return from b in this.Booking
                   where b.departure_date.Date >= start && b.departure_date.Date <= end
                   group b by b.departure_date.Date into dateGroup
                   select new BookingDateCount { Date = dateGroup.Key, Count = dateGroup.Count() };
        }

        /// <summary>
        /// Метод, що видаляє усі дані з усіх таблиць БД
        /// </summary>
        public void DeleteData()
        {
            var table = this.GetTable<DataLib.Guest>();
            table.DeleteAllOnSubmit(table);

            var table2 = this.GetTable<Room>();
            table2.DeleteAllOnSubmit(table2);

            var table3 = this.GetTable<Booking>();
            table3.DeleteAllOnSubmit(table3);

            this.SubmitChanges();
        }

        /// <summary>
        /// Метод, який встановлює тестові дані до бази даних
        /// </summary>
        public void SetTestData()
        {
            this.DeleteData();

            DataLib.Guest guest = new DataLib.Guest
            {
                name = "Ivan",
                surname = "Ivanov",
                patronymic = "Ivanovich",
                phone = "+380000000000",
                passport = "СА00000000",
                email = "ivanov.ivan@gmail.com"
            };

            this.Guest.InsertOnSubmit(guest);
            this.SubmitChanges();

            Room room = new Room
            {
                id = 1,
                type = "Стандарт",
                status = "Вільний",
                price = 2500,
                id_guest = guest.id
            };

            this.Room.InsertOnSubmit(room);
            this.SubmitChanges();

            Booking booking = new Booking
            {
                id = 1,
                id_guest = guest.id,
                id_room = room.id,
                arrival_date = DateTime.Now,
                departure_date = DateTime.Now,
                create_date = DateTime.Now
            };

            this.Booking.InsertOnSubmit(booking);
            this.SubmitChanges();
        }
    }
}
