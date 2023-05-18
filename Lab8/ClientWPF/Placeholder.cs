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

        /// <summary>
        /// Консткрутор, який приймає об'єкт текстового поля та назву підказки
        /// </summary>
        /// <param name="textbox">Об'єкт текстового поля</param>
        /// <param name="str">Назва підказки</param>
        public Placeholder(TextBox textbox, string str)
        {
            this.textbox = textbox;
            this.str = str;

            // Встановлення методів-обробників при фокусі на текстове поле
            textbox.GotFocus += this.RemoveText;
            textbox.LostFocus += this.AddText;
            this.checkNull();
        }

        /// <summary>
        /// Метод на перевірку чи порожнє поле
        /// </summary>
        public void checkNull()
        {
            if (string.IsNullOrWhiteSpace(this.textbox.Text))
            {
                this.textbox.Text = this.str;
                this.textbox.Foreground = Brushes.Gray;
            }
        }

        /// <summary>
        /// Обробник подій при фокусі на текстове поле
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        public void RemoveText(object sender, EventArgs e)
        {
            if (this.textbox.Text == this.str)
            {
                this.textbox.Text = "";
                this.textbox.Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// Обробник подій, коли фокусу на текстовому полі нема
        /// </summary>
        /// <param name="sender">Об'єкт відправнику</param>
        /// <param name="e">Об'єкт події</param>
        public void AddText(object sender, EventArgs e)
        {
            this.checkNull();
        }
    }
}
