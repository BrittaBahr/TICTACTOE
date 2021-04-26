using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WpfClient
{
    public class InfoWriterView : IWriter<string>
    {
        public void Write(string text)
        {
            MessageBox.Show(text, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
