using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KeywordCompare
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private string FileName1 = "keyword1.txt";
        private string FileName2 = "keyword2.txt";

        private List<string> keywordList1 = new List<string>();
        private List<string> keywordList2 = new List<string>();

        private bool CompareFileValidate()
        {
            bool IsKeyword1Exist = File.Exists(FileName1);
            if (IsKeyword1Exist == true)
            {
                Keyword1TextBlock.Text = FileName1 + " 파일 준비 완료";
                Keyword1TextBlock.Foreground = Brushes.DeepSkyBlue;

                StreamReader sr = new StreamReader(FileName1);
                string keyword = "";
                while ((keyword = sr.ReadLine()) != null)
                {
                    keywordList1.Add(keyword);
                }
            }
            else
            {
                Keyword1TextBlock.Text = FileName1 + " 파일을 준비해야 합니다.";
                Keyword1TextBlock.Foreground = Brushes.Red;
            }

            bool IsKeyword2Exist = File.Exists(FileName2);
            if (IsKeyword2Exist == true)
            {
                Keyword2TextBlock.Text = FileName2 + " 파일 준비 완료";
                Keyword2TextBlock.Foreground = Brushes.DeepSkyBlue;

                StreamReader sr = new StreamReader(FileName2);
                string keyword = "";
                while ((keyword = sr.ReadLine()) != null)
                {
                    keywordList2.Add(keyword);
                }
            }
            else
            {
                Keyword2TextBlock.Text = FileName2 + " 파일을 준비해야 합니다.";
                Keyword2TextBlock.Foreground = Brushes.Red;
            }

            return IsKeyword1Exist && IsKeyword2Exist;
        }

        private void StartButtonEnable()
        {
            bool IsValidate = CompareFileValidate();
            if (IsValidate == true)
            {
                StartButton.IsEnabled = true;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StartButtonEnable();
        }        

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            StartButtonEnable();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            CompareProgressBar.Maximum = keywordList1.Count;
            CompareProgressTextBlock.Text = string.Format("(0/{0})", keywordList1.Count);

            RefreshButton.IsEnabled = false;
            StartButton.IsEnabled = false;
            
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Task.Factory.StartNew(() =>
            {
                StreamWriter streamWriter = new StreamWriter("keyword_result.txt");

                foreach (var item in keywordList1)
                {
                    var keywordCount = keywordList2.Where(keyword => keyword == item);
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        CompareProgressBar.Value++;
                        CompareProgressTextBlock.Text = string.Format("({0}/{1})", CompareProgressBar.Value, keywordList1.Count);
                        CompareProgressTimeTextBlock.Text = sw.Elapsed.ToString();
                    }));

                    streamWriter.WriteLine(item + "|" + keywordCount.Count());                    
                    // Debug.WriteLine(item + ", " + keywordCount.Count());
                }

                streamWriter.Close();

                MessageBoxResult mbr = MessageBox.Show("Keyword comapre is completed!", "KeywordCompare", MessageBoxButton.OK, MessageBoxImage.Information);
                if (mbr == MessageBoxResult.OK)
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        this.Close();
                    }));
                }
            });
        }
    }
}
