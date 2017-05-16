using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;

namespace CustomerReviewRating
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadCellPhonesJson();
            DataAnalysis();
            WriteCategoryOuput("");
        }

        List<ReviewDataItem> cellPhoneItems = new List<ReviewDataItem>();
        List<string> AnalysisReport = new List<string>();

        public void LoadCellPhonesJson()
        {

            using (StreamReader r = new StreamReader(@"F:\Data Mining\Project\Cell Phones and Accessories\reviews_Cell_Phones_and_Accessories.json"))
            using (JsonTextReader reader = new JsonTextReader(r))
            {
                reader.SupportMultipleContent = true;

                var serializer = new JsonSerializer();
                while (reader.Read())
                {
                    try
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            ReviewDataItem c = serializer.Deserialize<ReviewDataItem>(reader);
                            if (cellPhoneItems.Count < 100000)
                                cellPhoneItems.Add(c);
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        //dispLst.ItemsSource = cellPhoneItems;
                        return;
                    }
                }
            }
            //dispLst.ItemsSource = cellPhoneItems;
        }

        public void DataAnalysis()
        {
            int worst = 0;
            int bad = 0;
            int average = 0;
            int good = 0;
            int excellent = 0;
            List<ReviewDataItem> cellPhoneItemsAnalyzed = new List<ReviewDataItem>();
            List<ReviewDataItem> cellPhoneSameItems = new List<ReviewDataItem>();
            List<string> analysisReport = new List<string>();
            foreach (ReviewDataItem obj in cellPhoneItems)
            {
                ReviewDataItem reviewObj = cellPhoneItemsAnalyzed.FirstOrDefault<ReviewDataItem>(cell => cell.asin == obj.asin);
                if(reviewObj != null)
                    continue;
                else
                {
                    cellPhoneItemsAnalyzed.Add(obj);
                    cellPhoneSameItems = cellPhoneItems.Where(cell => cell.asin == obj.asin).ToList<ReviewDataItem>();
                    if (cellPhoneSameItems != null)
                    {
                        foreach (ReviewDataItem item in cellPhoneSameItems)
                        {
                            float rating = item.overall;
                            if (rating == 0.0 || rating <= 1.0)
                                worst++;
                            else if (rating > 1.0 && rating <= 2.0)
                                bad++;
                            else if (rating > 2.0 && rating <= 3.0)
                                average++;
                            else if (rating > 3.0 && rating <= 4.0)
                                good++;
                            else if (rating > 4.0)
                                excellent++;
                        }
                    }
                    List<int> overallCount = new List<int>();
                    overallCount.Add(worst);
                    overallCount.Add(bad);
                    overallCount.Add(average);
                    overallCount.Add(good);
                    overallCount.Add(excellent);
                    int overall = overallCount.Max();

                    string overallRating = string.Empty;
                    if (excellent > worst && excellent > bad && excellent > average && excellent > good)
                        overallRating = "excellent";
                    else if (good > worst && good > bad && good > average && good >= excellent)
                        overallRating = "good";
                    else if (average > worst && average > bad && average >= good && average > excellent)
                        overallRating = "average";
                    else if (bad > worst && bad >= good && bad >= average && bad >= excellent)
                        overallRating = "bad";
                    else if (worst >= good && worst >= bad && worst >= average && worst >= excellent)
                        overallRating = "worst";

                    worst = 0;
                    bad = 0;
                    average = 0;
                    good = 0;
                    excellent = 0;
                    analysisReport.Add(obj.asin + "           " + overallRating);
                    //dispLst.ItemsSource = analysisReport;
                }
            }
            dispLst.ItemsSource = analysisReport;
            AnalysisReport = new List<string>(analysisReport);
        }

        public void WriteCategoryOuput(string category)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(Directory.GetCurrentDirectory() + "\\Classified_Output_"+category+".txt"))
            {
                file.WriteLine("                      Product Category - "+category);
                file.WriteLine("  ProductID      Class/Category");
                foreach (string item in AnalysisReport)
                {
                    file.WriteLine(item);
                }
            }
        }
    }
}
