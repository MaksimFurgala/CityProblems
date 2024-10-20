using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CityProblems.Models
{
    /// <summary>
    /// модель описывающая проблему и сущность из БД
    /// </summary>
    public class Problem : INotifyPropertyChanged
    {
        private string adress;
        private string description;
        private string annotation;
        private int status;
        private int priority;

        public int Id { get; set; }

        public string Adress
        {
            get { return adress; }
            set
            {
                adress = value;
                OnPropertyChanged("Adress");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public string Annotation
        {
            get { return annotation; }
            set
            {
                annotation = value;
                OnPropertyChanged("Annotation");
            }
        }

        public int Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public int Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                OnPropertyChanged("Priority");
            }
        }

        /// <summary>
        /// событие изменения свойств модели
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// свойство модели изменилось
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }


    }

    internal class CallerMemberNameAttribute : Attribute
    {
    }
}
