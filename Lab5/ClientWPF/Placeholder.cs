// Placeholder.cs

using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientWPF
{
    /// <summary>
    /// Клас для встановлення підказок у текстові поля
    /// </summary>
    internal class Placeholder
    {
        // приватні поля для збереження текстового поля та тексту-підказки
        private TextBox textbox;
        private string str;

        // конструктор
        public Placeholder(TextBox textbox, string str)
        {
            this.textbox = textbox;
            this.str = str;

            // Встановлення методів-обробників при фокусі на текстове поле
            textbox.GotFocus += this.RemoveText;
            textbox.LostFocus += this.AddText;
            this.checkNull();
        }

        // метод на перевірку чи порожнє поле
        public void checkNull()
        {
            if (string.IsNullOrWhiteSpace(this.textbox.Text))
            {
                this.textbox.Text = this.str;
                this.textbox.Foreground = Brushes.Gray;
            }
        }

        // Обробник подій при фокусі на текстове поле
        public void RemoveText(object sender, EventArgs e)
        {
            if (this.textbox.Text == this.str)
            {
                this.textbox.Text = "";
                this.textbox.Foreground = Brushes.Black;
            }
        }

        // Обробний подій, коли фокусу на текстовому полі нема
        public void AddText(object sender, EventArgs e)
        {
            this.checkNull();
        }
    }
}
