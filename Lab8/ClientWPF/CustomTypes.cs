// CustomTypes.cs

using System;

namespace ClientWPF
{
    /// <summary>
    /// Клас для визначення структури для типу номера та середньої вартості
    /// </summary>
    public class RoomType
    {
        /// <summary>
        /// Тип номера готелю
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Середня вартість по типу номера готелю
        /// </summary>
        public double AveragePrice { get; set; }
    }

    /// <summary>
    /// Клас для визначення структури для дати та кількості бронювань по вказаним датам
    /// </summary>
    public class BookingDateCount
    {
        /// <summary>
        /// Поле для збереження дати
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Поле для збереження кількості бронювань для конкретної дати
        /// </summary>
        public int Count { get; set; }
    }
}
