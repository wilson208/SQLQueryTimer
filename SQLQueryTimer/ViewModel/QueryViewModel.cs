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
        private Query query;
        private String lastValue;
        private DateTime lastUpdated;
        private MainViewModel parentViewModel;
        private Timer timer;

        public QueryViewModel(Query query, MainViewModel parentViewModel) : base()
        {
            this.Query = query;
            this.LastValue = null;
            this.LastUpdated = default(DateTime);
            this.parentViewModel = parentViewModel;

            this.timer = new Timer();
            this.timer.Enabled = false;
            this.timer.Interval = query.IntervalMilliseconds;
            this.timer.Elapsed += timer_Elapsed;

            this.timer_Elapsed(null, null);
            TimerEnabled = true;
        }

        public Query Query
        {
            get
            {
                return query;
            }
            set
            {
                query = value;
                RaisePropertyChanged(() => LastUpdated);
            }
        }

        public String LastValue
        {
            get
            {
                if (lastValue == null)
                    return "";
                return lastValue;
            }
            set
            {
                lastValue = value;
                RaisePropertyChanged(() => LastValue);
                RaisePropertyChanged(() => Status);
            }
        }
        public DateTime LastUpdated
        {
            get
            {
                return lastUpdated;
            }
            set
            {
                lastUpdated = value;
                RaisePropertyChanged(() => LastUpdated);
                RaisePropertyChanged(() => Status);
            }
        }
        public bool TimerEnabled
        {
            get { return timer.Enabled; }
            set
            {
                timer.Enabled = value;
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

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
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
    }
}
