using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reactive.Disposables;
using System.Text;
using System.Windows.Data;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace VMDEditor
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public ReactiveProperty<bool> IsTimeLineWindowVisible { get; }

        public ViewModel()
        {
            IsTimeLineWindowVisible = new ReactiveProperty<bool>(true).AddTo(Disposable);
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }

    }

    class WithoutBarScrollPositionConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
