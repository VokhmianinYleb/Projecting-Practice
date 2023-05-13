// InvitationBooking.xaml.cs

using System;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Linq;
using DataLib;

namespace ClientWPF
{
    /// <summary>
    /// Логика взаимодействия для InvitationBooking.xaml
    /// </summary>
    public partial class InvitationBooking : Window
    {
        public InvitationBooking(HotelDC hoteldc, Booking booking, bool sending = false)
        {
            DataLib.Guest guest = hoteldc.Guest.Where(g => g.id == booking.id_guest).FirstOrDefault();
            Room room = hoteldc.Room.Where(r => r.id == booking.id_room).FirstOrDefault();
            
            double CountDays = (booking.departure_date.Subtract(booking.arrival_date)).TotalDays;

            this.DataContext = new
            {
                id = booking.id.ToString(),
                name = $"{guest.surname} {guest.name}",
                email = guest.email,
                phone = guest.phone,
                passport = guest.passport,
                created = booking.create_date,
                arrival = booking.arrival_date,
                departure = booking.departure_date,
                type = room.type,
                price = room.price,
                total = (room.price * CountDays).ToString()
            };

            InitializeComponent();

            // Якщо необхідно надіслати на пошту
            if (sending)
            {
                flowDocumentScrollViewer.Print();

                // Остаточне запитання стосовно намірів відправлення листа на пошту
                MessageBoxResult result = MessageBox.Show($"Надіслати запрошення на поштову скриньку {guest.surname} {guest.name}?", "Запрошення", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    this.SendMail();
            }
        }
        
        // Метод, який повертає шлях до останнього збереженого PDF файлу
        private string GetLatestFilePDFPath()
        {
            // Шлях до директорії документів
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // Список усіх файлів PDF у директорії
            string[] pdfFiles = Directory.GetFiles(directoryPath, "*.pdf");
            // Останній збережений PDF файл
            FileInfo latestFile = new DirectoryInfo(directoryPath).GetFiles("*.pdf").OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
            // Повернення шляху до останнього PDF файлу
            return latestFile.FullName;
        }

        // Метод, який відправляє лист на поштову скриньку гостя
        private void SendMail()
        {
            dynamic dataContext = this.DataContext;

            // Адреса відправника та отримувача
            string fromAddress = Properties.Settings.Default.address;
            string toAddress = $"{dataContext.email}";

            // Дані для входу на SMTP-сервер Google
            string password = Properties.Settings.Default.password;

            // Налаштування клієнта SMTP-серверу
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(fromAddress, password);

            // Створення повідомлення
            MailMessage message = new MailMessage(fromAddress, toAddress);
            message.Subject = "Hotel Grand KC 5* | Reservation confirmation";
            message.Body = $"Hello, {dataContext.name}!\n\nThank you very much for your reservation. Hotel Grand KC ***** in Dnipro is booked for you. The hotel is automatically being sent a reservation at this very moment!\n\nWe wish you a pleasant stay!";

            // Додавання файлу PDF до повідомлення
            Attachment attachment = new Attachment(this.GetLatestFilePDFPath());
            message.Attachments.Add(attachment);

            // Відправлення листа
            try
            {
                client.Send(message);
                MessageBox.Show("Успішно відправлено на поштову скриньку!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Виникла помилка відправки: {ex}");
            }
        }

        // Обробник подій при натисканні на кнопку меню
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            flowDocumentScrollViewer.Print();
        }
    }
}