using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using SQLQueryTimer.Model;
using SQLQueryTimer.Utilities;

namespace SQLQueryTimer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private Settings _settings;
        private RegistryKey _runAtStartupRegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _settings = SettingsUtility.GetSettings();
            AddQueryCommand = new RelayCommand(() => OpenAddQueryWindow());
            ExitCommand = new RelayCommand(() => Exit());
            Messenger.Default.Register<KeyValuePair<string, Query>>(this, (query) =>
            {
                switch (query.Key)
                {
                    case "AddQuery":
                        OnQueryAdded(query.Value);
                        break;
                    case "DeleteQuery":
                        DeleteQuery(query.Value);
                        break;
                    case "RefreshQuery":
                        RefreshQuery(query.Value);
                        break;
                }
            });
        }

        public ICommand AddQueryCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public ObservableCollection<QueryViewModel> QueryViewModels
        {
            get { return new ObservableCollection<QueryViewModel>(_settings.Queries.Select(i => new QueryViewModel(i, this))); }
        }

        public bool AlwaysOnTop
        {
            get
            {
                return _settings.AlwaysOnTop;
            }
            set
            {
                _settings.AlwaysOnTop = value;
                RaisePropertyChanged(() => AlwaysOnTop);
                SettingsUtility.SetSettings(_settings);
            }
        }

        public bool RunAtStartup
        {
            get
            {
                return (_runAtStartupRegistryKey.GetValue("SQLQueryTimer") != null) ;
            }
            set
            {
                if (value)
                {
                    _runAtStartupRegistryKey.SetValue("SQLQueryTimer", System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    _runAtStartupRegistryKey.DeleteValue("SQLQueryTimer");
                }
                RaisePropertyChanged(() => RunAtStartup);
            }
        }

        private void OpenAddQueryWindow()
        {
            var window = new AddQueryWindow();
            window.Show();
        }

        private void OnQueryAdded(Query query)
        {
            _settings.Queries.Add(query);
            SettingsUtility.SetSettings(_settings);
            RaisePropertyChanged(() => QueryViewModels);
        }

        private void Exit()
        {
            SettingsUtility.SetSettings(_settings);
            App.Current.Shutdown();
        }

        private void DeleteQuery(Query query)
        {
            _settings.Queries.Remove(query);
            SettingsUtility.SetSettings(_settings);
            RaisePropertyChanged(() => QueryViewModels);
        }

        private void RefreshQuery(Query query)
        {
            QueryViewModels.First(i => i.Query.Equals(query)).UpdateResult();
        }
    }
}