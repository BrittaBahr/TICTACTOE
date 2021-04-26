using System;
using System.Collections.Generic;
using System.Text;

namespace WpfClient
{
    public interface IWriter<T>
    {
        void Write(T text);
    }
}
