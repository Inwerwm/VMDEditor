using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace VMDEditor
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public ReactiveProperty<ReactiveProperty<Article>> SelectedArticle { get; }
        public ReactiveProperty<int> SelectedArticleIndex { get; }
        public ReactiveProperty<int> TimelineLength { get; }
        public ReactiveProperty<int> TimelineHeight { get; }
        public ReactiveProperty<bool> IsTimeLineWindowVisible { get; }

        public ReactiveCollection<ReactiveProperty<Article>> Articles { get; }

        // Binding用定数
        public static int ArticleRowHeight { get; } = Constants.ARTICLE_ROW_HEIGHT;
        public static int RulerRowHeight { get; } = 50;
        public static Thickness TimeLineMargin { get; } = new Thickness(10, 0, 0, 0);

        public ViewModel()
        {
            SelectedArticle = new ReactiveProperty<ReactiveProperty<Article>>().AddTo(Disposable);
            SelectedArticleIndex = new ReactiveProperty<int>().AddTo(Disposable);
            TimelineLength = new ReactiveProperty<int>(1000).AddTo(Disposable);
            TimelineHeight = new ReactiveProperty<int>(0).AddTo(Disposable);
            IsTimeLineWindowVisible = new ReactiveProperty<bool>(true).AddTo(Disposable);
            
            Articles = new ReactiveCollection<ReactiveProperty<Article>>().AddTo(Disposable);
        }

        /// <summary>
        /// 項目を追加する
        /// </summary>
        /// <param name="name">追加する項目名</param>
        /// <returns>項目の追加に成功したかを返す</returns>
        public bool AddArticle(string name)
        {
            if (Articles.Any(a => a.Value.Name == name))
                return false;
            
            Articles.Add(new ReactiveProperty<Article>(new Article(ArticleType.Bone, name)).AddTo(Disposable));
            return true;
        }

        public void Dispose() => Disposable.Dispose();
    }

    public class MinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return -1 * (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return -1 * (double)value;
        }
    }
}
