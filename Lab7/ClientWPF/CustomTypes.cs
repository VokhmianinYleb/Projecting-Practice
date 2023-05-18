// CustomTypes.cs

using System;

namespace ClientWPF
{
    /// <summary>
    /// Клас для визначення структури для типу номера та середньої вартості
    /// </summary>
    public class RoomType
    {
        public string Type { get; set; }
        public double AveragePrice { get; set; }
    }

    /// <summary>
    /// Клас для визначення структури для дати та кількості бронювань по вказаним датам
    /// </summary>
    public class BookingDateCount
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
