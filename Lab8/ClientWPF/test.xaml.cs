using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientWPF
{
    /// <summary>
    /// Логика взаимодействия для test.xaml
    /// </summary>
    public partial class test : Window
    {
        public test()
        {
            ChartValues<int> one = new ChartValues<int> { 2500 };
            ChartValues<int> two = new ChartValues<int> { 1700 };
            ChartValues<int> three = new ChartValues<int> { 3200 };
            ChartValues<int> four = new ChartValues<int> { 2890 };

            this.DataContext = new
            {
                one = one,
                two = two,
                three = three,
                four = four
            };

            InitializeComponent();
        }
    }
}
