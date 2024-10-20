using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using CityProblems.Models;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;

namespace CityProblems
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        /// <summary>
        /// Добавление
        /// </summary>
        private string CurrentTabHeader = "";//текущая вкладка (header)
        private string CurrentTab;//текущая вкладка (name)

        /// <summary>
        /// Редактирование
        /// </summary>
        private string CurrentTabHeaderUpd = "";//текущая вкладка (header)
        private string CurrentTabUpd = "";//текущая вкладка (name)

        private Problem CurrentProblemUpd { get; set; }

        /// <summary>
        /// контекст данных
        /// </summary>
        ApplicationContext db;

        public MainWindow()
        {
            InitializeComponent();//инициализируем элементы в XAML

            //устанавливаем контекст
            db = new ApplicationContext();
            db.Problems.Load();
            //парсим данные из БД в XAML разметку
            this.DataContext = db.Problems.Local.ToBindingList();
            GridProblems.Columns["Priority"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
        }

        /// <summary>
        /// открытие окна для добавления новой проблемы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProblemPopup_Click(object sender, EventArgs e)
        {
            AddedProblemPopup.IsOpen = true;
        }

        /// <summary>
        /// добавление новой проблемы из формы в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProblem_Click(object sender, RoutedEventArgs e)
        {
            Problem problem = new Problem();
            List<string> strList = GetAllDescriptionForProblem(CurrentTab, out int priority);
            string description = GetTextDescription(strList);

            problem.Description = description;
            if(AdressText.Text != null && AdressText.Text != "")
            {
                problem.Adress = AdressText.Text;
                problem.Annotation = AnnotationText.Text;
                problem.Priority = priority;

                db.Problems.Add(problem);
                db.SaveChanges();

                //очищаем поля и сообщения
                ErrorAdress.Visibility = Visibility.Collapsed;

                AdressText.Text = "";
                AnnotationText.Text = "";

                foreach (CheckEdit tb in FindVisualChildren<CheckEdit>(DescriptionTab))
                {
                    tb.IsChecked = false;
                }

                AddedProblemPopup.IsOpen = false;
            }
            else
            {
                ErrorAdress.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// получаем данные о проблемах и их приоритет
        /// </summary>
        /// <param name="nameTab">имя активной вкладки</param>
        /// <param name="priority">приоритет</param>
        /// <returns></returns>
        private List<string> GetAllDescriptionForProblem(string nameTab, out int priority)
        {
            
            List<string> result = new List<string>();
            var tabGeneral = DescriptionTab.FindName(nameTab);
            var listItems = ((System.Windows.Controls.Panel)((System.Windows.Controls.ContentControl)tabGeneral).Content).Children.OfType<CheckEdit>().ToList().Where(f => f.IsChecked == true && f.Content.ToString() != "Другая проблема");

            //проверяем какая вкладка активна
            switch(CurrentTabHeader)
            {
                case "Городское хозяйство" : priority = 52;
                    break;
                case "Безопасность" : priority = 53;
                    break;
                default: priority = 55;
                    break;
            }

            for (var i = 0; i < 56; i++)
            {
                var tmpItem = listItems.FirstOrDefault(x => x.TabIndex == i);//ищем первое совпадение для получения самомого высокого приоритета
                if(tmpItem != null)
                {
                    priority = tmpItem.TabIndex;//получааем общий индекс (приоритет) проблемы и останавливаем цикл
                    break;
                }
            }

            result = listItems.Select(x => x.Content.ToString()).ToList();
            
            
            return result;
        }

        /// <summary>
        /// получение финальной строки (список-описание проблемы)
        /// </summary>
        /// <param name="descriptionList"></param>
        /// <returns></returns>
        private string GetTextDescription(List<string> descriptionList)
        {
            var result = String.Join(",", descriptionList);
            return result;
        }

        /// <summary>
        /// переключение вкладок проблемы (событие)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescriptionTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentTabHeader = ((System.Windows.Controls.HeaderedContentControl)((object[])e.AddedItems)[0]).Header.ToString();
            CurrentTab = ((System.Windows.FrameworkElement)((object[])e.AddedItems)[0]).Name;

            foreach(CheckEdit tb in FindVisualChildren<CheckEdit>(DescriptionTab))
            {
                tb.IsChecked = false;
            }
        }

        /// <summary>
        /// нахождение вложенных элементов в XAML разметке
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdressText_Validate(object sender, ValidationEventArgs e)
        {
            //AddProblem.IsEnabled = false;
            e.IsValid = true;
            if (e.Value == null) return;
            
            AddProblem.IsEnabled = true;
            ErrorAdress.Visibility = Visibility.Collapsed;
        }

        private void ExceptionCheck_1_Checked(object sender, RoutedEventArgs e)
        {
            //AnnotationPanel.Visibility = Visibility.Visible;
            foreach (CheckEdit tb in FindVisualChildren<CheckEdit>(DescriptionTab))
            {
                if (tb.Content.ToString() != "Другая проблема")
                {
                    tb.IsChecked = false;
                }
            }
        }

        private void ExceptionCheck_2_Checked(object sender, RoutedEventArgs e)
        {
            //AnnotationPanel.Visibility = Visibility.Visible;
            foreach (CheckEdit tb in FindVisualChildren<CheckEdit>(DescriptionTab))
            {
                if (tb.Content.ToString() != "Другая проблема")
                {
                    tb.IsChecked = false;
                }
            }
        }

        private void ExceptionCheck_3_Checked(object sender, RoutedEventArgs e)
        {
            //AnnotationPanel.Visibility = Visibility.Visible;
            foreach (CheckEdit tb in FindVisualChildren<CheckEdit>(DescriptionTab))
            {
                if (tb.Content.ToString() != "Другая проблема")
                {
                    tb.IsChecked = false;
                }
            }
        }

        private void ExceptionCheck_4_Checked(object sender, RoutedEventArgs e)
        {
            //AnnotationPanel.Visibility = Visibility.Visible;
            foreach (CheckEdit tb in FindVisualChildren<CheckEdit>(DescriptionTab))
            {
                if (tb.Content.ToString() != "Другая проблема")
                {
                    tb.IsChecked = false;
                }
            }
        }

        private void ExceptionCheck_1_Unchecked(object sender, RoutedEventArgs e)
        {
            //AnnotationPanel.Visibility = Visibility.Collapsed;
        }

        private void ExceptionCheck_2_Unchecked(object sender, RoutedEventArgs e)
        {
            //AnnotationPanel.Visibility = Visibility.Collapsed;
        }

        private void ExceptionCheck_3_Unchecked(object sender, RoutedEventArgs e)
        {
            //AnnotationPanel.Visibility = Visibility.Collapsed;
        }

        private void ExceptionCheck_4_Unchecked(object sender, RoutedEventArgs e)
        {
            //AnnotationPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// удаление проблемы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteProblem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var selectedItem = (Problem)GridProblems.SelectedItem;
            if(selectedItem != null)
            {
                db.Problems.Remove(selectedItem);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// обновление проблемы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateProblem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            UpdateProblemPopup.IsOpen = true;

            var selectedItem = (Problem)GridProblems.SelectedItem;
            
            if (selectedItem != null)
            {
                CurrentProblemUpd = selectedItem;

                AdressTextUpd.Text = selectedItem.Adress;
                AnnotationTextUpd.Text = selectedItem.Annotation;

                string description = selectedItem.Description;
                List<string> listDescription = new List<string>();
                if(description != null)
                {
                    listDescription = description.Split(new char[] { ',' }).ToList();

                    var a = DescriptionTabUpd;
                    foreach (var item in listDescription)
                    {
                        foreach (CheckEdit tb in FindVisualChildren<CheckEdit>(DescriptionTabUpd))
                        {
                            if (tb.Content.ToString() == item.ToString())
                            {
                                tb.IsChecked = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// обновляем в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateProblemButton_Click(object sender, RoutedEventArgs e)
        {
            var updProblem = db.Problems.Find(((Problem)GridProblems.SelectedItem).Id);//проблема для обновления

            int priority;
            List<string> strList = GetAllDescriptionForProblem(CurrentTabUpd, out priority);
            string description = GetTextDescription(strList);

            updProblem.Description = description;
            if (AdressTextUpd.Text != null && AdressTextUpd.Text != "")
            {
                updProblem.Adress = AdressTextUpd.Text;
                updProblem.Annotation = AnnotationTextUpd.Text;
                updProblem.Priority = priority;

                db.Entry(updProblem).State = EntityState.Modified;
                db.SaveChanges();

                //очищаем поля и сообщения
                ErrorAdressUpd.Visibility = Visibility.Collapsed;

                AdressTextUpd.Text = "";
                AnnotationTextUpd.Text = "";

                foreach (CheckEdit tb in FindVisualChildren<CheckEdit>(DescriptionTabUpd))
                {
                    tb.IsChecked = false;
                }

                UpdateProblemPopup.IsOpen = false;
            }
            else
            {
                ErrorAdressUpd.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// событие изменения описания (вкладки)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescriptionTabUpd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentTabHeaderUpd = ((System.Windows.Controls.HeaderedContentControl)((object[])e.AddedItems)[0]).Header.ToString();
            CurrentTabUpd = ((System.Windows.FrameworkElement)((object[])e.AddedItems)[0]).Name;
        }

        /// <summary>
        /// валидация адреса (панель редактирования проблемы)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdressTextUpd_Validate(object sender, ValidationEventArgs e)
        {
            e.IsValid = true;
            if (e.Value == null) return;

            UpdateProblemButton.IsEnabled = true;
            ErrorAdressUpd.Visibility = Visibility.Collapsed;
        }

        private void UpdateStatusDone_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var updProblem = db.Problems.Find(((Problem)GridProblems.SelectedItem).Id);
            updProblem.Status = 2;
            db.Entry(updProblem).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void UpdateStatusInProcess_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var updProblem = db.Problems.Find(((Problem)GridProblems.SelectedItem).Id);
            updProblem.Status = 1;
            db.Entry(updProblem).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}