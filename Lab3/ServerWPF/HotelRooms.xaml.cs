// HotelRooms.xaml.cs

using System.Windows;
using System.Windows.Controls;

namespace ServerWPF
{
    /// <summary>
    /// Логика взаимодействия для HotelRooms.xaml
    /// </summary>
    public partial class HotelRooms : Window
    {
        // Поле для зберігання номерів готелю
        private ItemCollection items;

        public HotelRooms()
        {
            InitializeComponent();

            // Заповнюємо елементами номерів готелю
            this.items = new ListBox().Items;

            for (int i = 1; i <= 20; i++)
            {
                this.items.Add($"Номер готелю {i}");
            }

            // Встановлюємо заповнений список у компонент listBox
            listBox.ItemsSource = this.items;
        }

        // Обробник подій при натисканні на конкретний номер готелю
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Перевірка на те, аби обраний елемент не був порожнім
            if (listBox.SelectedItem != null)
            {
                // Отримання номера готелю
                string number = listBox.SelectedItem.ToString().Split(" ")[2];
                // Створення нового вікна
                Window window = new HotelRoom(number);
                // Виклик створеного вікна
                window.Show();
            }
        }

        // Обробний подій при внесенні змін до поля пошуку
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Якщо поле пошуку порожнє, встановлюємо
            // компоненту listBox усі номери готелю
            if (textBoxSearch.Text == "")
            {
                listBox.ItemsSource = this.items;
            }
            else // Інакше - обираємо лише ті, які відповідають контенту пошуку
            {
                ItemCollection items = new ListBox().Items;

                foreach (string item in this.items)
                {
                    if (item.Contains(textBoxSearch.Text))
                    {
                        items.Add(item);
                    }
                }

                listBox.ItemsSource = items;
            }
        }
    }
}
