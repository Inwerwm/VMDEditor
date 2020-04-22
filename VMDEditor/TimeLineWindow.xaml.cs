using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VMDEditor
{
    /// <summary>
    /// TimeLineWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeLineWindow : Window
    {
        ViewModel vm;

        public TimeLineWindow(ViewModel viewModel)
        {
            InitializeComponent();
            vm = viewModel;
            DataContext = vm;
        }

        private void WindowTimeLine_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vm.IsTimeLineWindowVisible.Value = false;
            e.Cancel = true;
        }

        /// <summary>
        /// 新しい項目を追加
        /// </summary>
        private void AddArticleButton_Click(object sender, RoutedEventArgs e)
        {
            // 追加する項目名の末尾に連番を付与する
            // 空いている最も小さい番号を連番として付与する
            for (int i = 1; i <= int.MaxValue; i++)
            {
                var newName = $"新規項目{i}";
                // 名前被りがなければ追加してループから抜ける
                if (AddArticle(newName))
                    break;
            }
        }

        private bool AddArticle(string name)
        {
            if (!vm.AddArticle(name))
                return false;

            Binding binding = new Binding("TimelineLength.Value");
            binding.Source = vm;
            
            var line = new Line();
            line.X1 = 0;
            line.SetBinding(Line.X2Property, binding);
            line.Y1 = vm.Articles.Count * Constants.ARTICLE_ROW_HEIGHT;
            line.Y2 = vm.Articles.Count * Constants.ARTICLE_ROW_HEIGHT;
            line.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            line.StrokeThickness = 1;

            CanvasTimeLine.Height = vm.Articles.Count * Constants.ARTICLE_ROW_HEIGHT;
            CanvasTimeLine.Children.Add(line);

            return true;
        }

        private void WindowTimeLine_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var spaceWidth = (int)Math.Ceiling(TimeLineColumn.ActualWidth);
            if (vm.TimelineLength.Value < spaceWidth)
                vm.TimelineLength.Value = spaceWidth;
        }
    }
}
