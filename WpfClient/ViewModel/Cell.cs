using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace WpfClient.ViewModel
{
    public class Cell : INotifyPropertyChanged
    {
        private bool active;

        private bool isEnabled;

        private AlphaRedGreenBlue argb;

        public Cell(ICommand cellCommand, AlphaRedGreenBlue argb, bool isEnabled)
        {
            this.CellCommand = cellCommand;
            this.ARGB = argb;
            this.active = false;
            this.IsEnabled = isEnabled;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand CellCommand
        {
            get;
            private set;
        }

        public bool Active
        {
            get
            {
                return this.active;
            }

            set
            {
                if (this.active != value)
                {
                    this.active = value;
                    this.OnPropertyChanged(nameof(this.Active));
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                    this.OnPropertyChanged(nameof(this.IsEnabled));
                }
            }
        }

        public AlphaRedGreenBlue ARGB
        {
            get
            {
                return this.argb;
            }

            set
            {
                if (!this.argb.Equals(value))
                {
                    this.argb = value;
                    this.OnPropertyChanged(nameof(this.ARGB));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
