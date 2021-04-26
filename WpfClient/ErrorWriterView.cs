using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WpfClient
{
    public class ErrorWriterView : IWriter<string>
    {
        public void Write(string text)
        {
            MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
