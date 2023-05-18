//UnitTest.cs

using DataLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ClientWPF;
using System.Linq;
using System.Collections.Generic;

namespace UnitTestProject
{
    /// <summary>
    /// Клас для тестування застосунку
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        // Статичне поле для збереження посилання на підключення до тестової бази даних
        private static string db_connection = "Data Source=(LocalDb)\\MSSQLLocalDB;AttachDBFilename=E:\\studies\\University\\github\\3course\\Project-Practice\\Lab7\\my_test_database.mdf";

        /// <summary>
        /// Тестовий метод для підключення до бази даних
        /// </summary>
        [TestMethod]
        public void TestDBConnection()
        {
            DataClient dataClient = new DataClient(db_connection);
            Assert.IsNotNull(dataClient);
        }

        /// <summary>
        /// Тестовий метод для перевірки встановлених тестових даних
        /// </summary>
        [TestMethod]
        public void TestSetTestData()
        {
            DataClient dataClient = new DataClient(db_connection);
            dataClient.SetTestData();
            Assert.AreEqual(dataClient.Guest.Count(), 1);
            Assert.AreEqual(dataClient.Room.Count(), 1);
            Assert.AreEqual(dataClient.Booking.Count(), 1);
        }

        /// <summary>
        /// Тестовий метод для перевірки видалення усіх даних з усіх таблиць
        /// </summary>
        [TestMethod]
        public void TestDeleteData()
        {
            DataClient dataClient = new DataClient(db_connection);
            dataClient.DeleteData();
            Assert.AreEqual(dataClient.Guest.Count(), 0);
            Assert.AreEqual(dataClient.Room.Count(), 0);
            Assert.AreEqual(dataClient.Booking.Count(), 0);
        }

        /// <summary>
        /// Тестовий метод для перевірки на наявність бронювання за вказаним номером готелю
        /// </summary>
        [TestMethod]
        public void TestIsRoomBooked()
        {
            DataClient dataClient = new DataClient(db_connection);
            dataClient.SetTestData();
            Assert.IsTrue(dataClient.IsRoomBooked(1));
        }

        /// <summary>
        /// Тестовий метод для перевірки на наявність у базі даних вказаного номера готелю
        /// </summary>
        [TestMethod]
        public void TestRoomExists()
        {
            DataClient dataClient = new DataClient(db_connection);
            dataClient.SetTestData();
            Assert.IsTrue(dataClient.RoomExists(1));
        }

        /// <summary>
        /// Тестовий метод для перевірки групування даних за типом номера готелю
        /// </summary>
        [TestMethod]
        public void TestGetDataAveragePriceStats()
        {
            DataClient dataClient = new DataClient(db_connection);
            dataClient.SetTestData();
            int id_guest = dataClient.Guest.FirstOrDefault().id;

            dataClient.Room.InsertOnSubmit(
                new Room
                {
                    id = 2,
                    type = "Стандарт",
                    status = "Вільний",
                    price = 3500,
                    id_guest = id_guest
                }
            );
            dataClient.Room.InsertOnSubmit(
                new Room
                {
                    id = 3,
                    type = "Люкс",
                    status = "Вільний",
                    price = 5600,
                    id_guest = id_guest
                }
            );
            dataClient.SubmitChanges();

            IEnumerable<RoomType> query = dataClient.GetDataAveragePriceStats();
            Assert.AreEqual(query.FirstOrDefault().Type, "Люкс");
            Assert.AreEqual(query.FirstOrDefault().AveragePrice, 5600f);
            Assert.AreEqual(query.LastOrDefault().Type, "Стандарт");
            Assert.AreEqual(query.LastOrDefault().AveragePrice, 3000f);
        }
        
        /// <summary>
        /// Тестовий метод для перевірки групування даних по датам бронювань
        /// </summary>
        [TestMethod]
        public void TestGetDataBookingsStats()
        {
            DataClient dataClient = new DataClient(db_connection);
            dataClient.SetTestData();
            int id_guest = dataClient.Guest.FirstOrDefault().id;

            dataClient.Booking.InsertOnSubmit(
                new Booking
                {
                    id_guest = id_guest,
                    id_room = 1,
                    arrival_date = DateTime.Now,
                    departure_date = DateTime.Now.AddDays(-3),
                    create_date = DateTime.Now.AddDays(-5)
                }
            );
            dataClient.Booking.InsertOnSubmit(
                new Booking
                {
                    id_guest = id_guest,
                    id_room = 1,
                    arrival_date = DateTime.Now,
                    departure_date = DateTime.Now.AddDays(-1),
                    create_date = DateTime.Now.AddDays(-10)
                }
            );
            dataClient.SubmitChanges();

            DateTime start = DateTime.Now.Date.AddDays(-7), end = DateTime.Now.Date.AddDays(2);

            IEnumerable<BookingDateCount> query = dataClient.GetDataBookingsStats(0, start, end);
            Assert.AreEqual(query.Count(), 2);

            query = dataClient.GetDataBookingsStats(1, start, end);
            Assert.AreEqual(query.Count(), 1);

            query = dataClient.GetDataBookingsStats(2, start, end);
            Assert.AreEqual(query.Count(), 3);
        }
    }
}
