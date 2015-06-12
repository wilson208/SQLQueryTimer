using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight;
using SQLQueryTimer.Model;
using SQLQueryTimer.Utilities;

namespace SQLQueryTimer.ViewModel
{
    public class QueryViewModel:ViewModelBase
    {
        private Query _query;
        private String _lastValue;
        private DateTime _lastUpdated;
        private MainViewModel _parentViewModel;
        private Timer _timer;

        public QueryViewModel(Query query, MainViewModel parentViewModel) : base()
        {
            this.Query = query;
            this.LastValue = null;
            this.LastUpdated = default(DateTime);
            this._parentViewModel = parentViewModel;

            this._timer = new Timer();
            this._timer.Enabled = false;
            this._timer.Interval = query.IntervalMilliseconds;
            this._timer.Elapsed += timer_Elapsed;

            this.timer_Elapsed(null, null);
            TimerEnabled = true;
        }

        public Query Query
        {
            get
            {
                return _query;
            }
            set
            {
                _query = value;
                RaisePropertyChanged(() => LastUpdated);
            }
        }

        public String LastValue
        {
            get
            {
                if (_lastValue == null)
                    return "";
                return _lastValue;
            }
            set
            {
                _lastValue = value;
                RaisePropertyChanged(() => LastValue);
                RaisePropertyChanged(() => Status);
            }
        }
        public DateTime LastUpdated
        {
            get
            {
                return _lastUpdated;
            }
            set
            {
                _lastUpdated = value;
                RaisePropertyChanged(() => LastUpdated);
                RaisePropertyChanged(() => Status);
            }
        }
        public bool TimerEnabled
        {
            get { return _timer.Enabled; }
            set
            {
                _timer.Enabled = value;
                RaisePropertyChanged(() => TimerEnabled);
                RaisePropertyChanged(() => Status);
            }
        }
        public string Status
        {
            get
            {
                if (LastUpdated == default(DateTime) && LastValue == null)
                {
                    return "Initialising";
                }
                else if (!TimerEnabled)
                {
                    return "Not Running";
                }
                else
                {
                    return "Running";
                }
            }
        }

        public void UpdateResult()
        {
            try
            {
                LastValue = QueryUtility.GetQueryValue(Query.ConnectionString, Query.SqlQuery);
                LastUpdated = DateTime.Now;
            }
            catch (QueryException ex)
            {
                TimerEnabled = false;
                //Log Exception
            }
            catch (Exception ex)
            {
                TimerEnabled = false;
                MessageBox.Show("Unhandled error ocurred", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                //Log Exception
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateResult();
        }
    }
}
