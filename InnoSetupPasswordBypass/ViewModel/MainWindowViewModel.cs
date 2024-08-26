using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using InnoSetupPasswordBypass.Models;
using InnoSetupPasswordBypass.Mvvm;
using InnoSetupPasswordBypass.InnoSetup;
using System.Windows;
using System.ComponentModel;

namespace InnoSetupPasswordBypass.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<ProcessInfo> Processes { get; }
            = new ObservableCollection<ProcessInfo>();

        private ProcessInfo _SelectedProcess;
        public ProcessInfo SelectedProcess 
        { 
            get
            {
                return _SelectedProcess;
            }
            set
            {
                _SelectedProcess = value;

                ProcessSelected();
            }
        }

        public RelayCommand RefreshProcessesRelayCommand { get; set; }
        public RelayCommand ApplyRelayCommand { get; set; }
        public RelayCommand UrlClickRelayCommand { get; set; }

        private bool _PatchButtonEnabled = false;
        public bool PatchButtonEnabled {
            get
            {
                return _PatchButtonEnabled;
            }
            set
            {
                _PatchButtonEnabled = value;

                OnPropertyChanged(nameof(PatchButtonEnabled));
            }
        }

        public MainWindowViewModel()
        {
            RefreshProcessesRelayCommand = new RelayCommand(RefreshProcessesCommand);
            ApplyRelayCommand = new RelayCommand(ApplyCommand);
            UrlClickRelayCommand = new RelayCommand(UrlClickCommand);

            RefreshProcesses();
        }

        private void RefreshProcesses()
        {
            var processes = Process.GetProcesses().Where(x => x.ProcessName.EndsWith(".tmp") && x.MainWindowHandle != IntPtr.Zero);

            Processes.Clear();

            foreach (var process in processes)
                Processes.Add(new ProcessInfo() { Id = process.Id, Name = process.ProcessName });
        }

        private void RefreshProcessesCommand(object parameter)
        {
            RefreshProcesses();
        }

        private void ApplyCommand(object parameter)
        {
            if (SelectedProcess == null)
                return;

            if (InnoSetupPatch.BypassPassword(SelectedProcess.Id))
            {
                MessageBox.Show("Patch has been applied", "Success");
            }
            else
            {
                MessageBox.Show("Patch is not applied", "Error");
            }
        }

        private void ProcessSelected()
        {
            PatchButtonEnabled = SelectedProcess != null;
        }

        private void UrlClickCommand(object parameter)
        {
            Process.Start("https://github.com/Hiteke");
        }
    }
}
