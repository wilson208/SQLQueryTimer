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
        private Settings settings;
        private bool topmostWindow = false;
        private RegistryKey runAtStartupRegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            settings = SettingsUtility.GetSettings();
            AddQueryCommand = new RelayCommand(() => OpenAddQueryWindow());
            ExitCommand = new RelayCommand(() => Exit());
            Messenger.Default.Register<Query>(this, (query) => OnQueryAdded(query));
        }

        public ICommand AddQueryCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public ObservableCollection<QueryViewModel> QueryViewModels
        {
            get { return new ObservableCollection<QueryViewModel>(settings.Queries.Select(i => new QueryViewModel(i, this))); }
        }

        public bool AlwaysOnTop
        {
            get
            {
                return settings.AlwaysOnTop;
            }
            set
            {
                settings.AlwaysOnTop = value;
                RaisePropertyChanged(() => AlwaysOnTop);
                SettingsUtility.SetSettings(settings);
            }
        }

        public bool RunAtStartup
        {
            get
            {
                return (runAtStartupRegistryKey.GetValue("SQLQueryTimer") != null) ;
            }
            set
            {
                if (value)
                {
                    runAtStartupRegistryKey.SetValue("SQLQueryTimer", System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    runAtStartupRegistryKey.DeleteValue("SQLQueryTimer");
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
            settings.Queries.Add(query);
            SettingsUtility.SetSettings(settings);
            RaisePropertyChanged(() => QueryViewModels);
        }

        private void Exit()
        {
            SettingsUtility.SetSettings(settings);
            App.Current.Shutdown();
        }
    }
}