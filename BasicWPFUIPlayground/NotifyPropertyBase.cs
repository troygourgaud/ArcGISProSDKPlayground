using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace WoodsideTool.Transaction.Model
{
    public class NotifyPropertyBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        //// Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}
