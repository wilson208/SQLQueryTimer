using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SQLQueryTimer.Model;
using SQLQueryTimer.Utilities;

namespace SQLQueryTimer.ViewModel
{
    public class AddQueryViewModel:ViewModelBase
    {
        private string _name;
        private string _connectionString;
        private string _query;
        private long _intervalMilliseconds;

        public AddQueryViewModel() : base()
        {
            AddButtonEnabled = false; 
            ValidateCommand = new RelayCommand(() => Validate());
            AddCommand = new RelayCommand(() => Add());
            CancelCommand = new RelayCommand(() => Cancel());
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
                RaisePropertyChanged(() => ConnectionString);
            }
        }

        public string Query
        {
            get
            {
                return _query;
            }
            set
            {
                _query = value;
                RaisePropertyChanged(() => Query);
            }
        }

        public int IntervalSeconds
        {
            get
            {
                return (int) (_intervalMilliseconds/1000);
            }
            set
            {
                _intervalMilliseconds = value*1000;
                RaisePropertyChanged(() => IntervalSeconds);
            }
        }

        public List<QueryType> QueryTypes
        {
            get { return Enum.GetNames(typeof (QueryType)).Select(i => (QueryType)Enum.Parse(typeof (QueryType), i)).ToList(); }
        }

        public QueryType QueryType { get; set; }

        public string Error { get; set; }

        public Visibility ShowErrorLabel { get { return String.IsNullOrEmpty(Error) ? Visibility.Collapsed : Visibility.Visible; } }

        public bool AddButtonEnabled { get; set; }
        
        public ICommand ValidateCommand { get; set; }
        
        public ICommand AddCommand { get; set; }
        
        public ICommand CancelCommand { get; set; }

        public void Validate()
        {
            if (String.IsNullOrEmpty(Name))
            {
                Error = "No Name Specified";
            }
            else if (String.IsNullOrEmpty(ConnectionString))
            {
                Error = "No Connection String Specified";
            }
            else if (String.IsNullOrEmpty(Query))
            {
                Error = "No Query Specified";
            }
            else if (IntervalSeconds < 5)
            {
                Error = "Interval must be atleast 5 seconds";
            }

            if (!String.IsNullOrEmpty(Error))
            {
                AddButtonEnabled = false;
                RaisePropertyChanged(() => Error);
                RaisePropertyChanged(() => ShowErrorLabel);
                RaisePropertyChanged(() => AddButtonEnabled);
                return;
            }

            try
            {
                string result = QueryUtility.GetQueryValue(ConnectionString, Query, QueryType);
                if(String.IsNullOrEmpty(result))
                    throw new Exception("Value returned is empty");

                MessageBox.Show("Validated successfully");
                AddButtonEnabled = true;
                Error = "";
                RaisePropertyChanged(() => Error);
                RaisePropertyChanged(() => ShowErrorLabel);
                RaisePropertyChanged(() => AddButtonEnabled);
            }
            catch (Exception e)
            {
                AddButtonEnabled = false;
                RaisePropertyChanged(() => Error);
                RaisePropertyChanged(() => ShowErrorLabel);
                RaisePropertyChanged(() => AddButtonEnabled);
            }
        }

        public void Add()
        {
            var query = new Query()
            {
                ConnectionString = ConnectionString,
                IntervalMilliseconds = _intervalMilliseconds,
                Name = Name,
                SqlQuery = Query,
                QueryType = QueryType
            };
            var message = new KeyValuePair<string, Query>("AddQuery", query);
            Messenger.Default.Send(message);
            Messenger.Default.Send("CloseAddQueryWindow");
        }

        public void Cancel()
        {
            Messenger.Default.Send("CloseAddQueryWindow");
        }
    }
}
