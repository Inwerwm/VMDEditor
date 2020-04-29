using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace VMDEditor
{
    /// <summary>
    /// TimeLineWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeLineWindow : Window
    {
        ViewModel vm;
        Stack<Line> ArticleLines = new Stack<Line>();
        int NumberOfFrameLinesDrawn = 0;

        public TimeLineWindow(ViewModel viewModel)
        {
            InitializeComponent();
            vm = viewModel;
            DataContext = vm;
            vm.TimelineLength.Subscribe(_ => DrawFrameLine());
            vm.TimelineHeight.Value = (int)Math.Ceiling(TimeLineRow.ActualHeight);
            Loaded += (s, e) => { DrawFrameLine(); };
        }

        private bool AddArticle(string name)
        {
            if (!vm.AddArticle(name))
                return false;

            DrawArticleLines();
            return true;
        }

        /// <summary>
        /// 現在の項目数に合わせて横線を描画する
        /// </summary>
        public void DrawArticleLines()
        {
            while (vm.Articles.Count > ArticleLines.Count)
            {
                Binding binding = new Binding("TimelineLength.Value");
                binding.Source = vm;
                binding.Mode = BindingMode.OneWay;

                var line = new Line();
                line.X1 = -1 * ViewModel.TimeLineMargin.Left;
                line.SetBinding(Line.X2Property, binding);
                line.Y1 = (ArticleLines.Count + 1) * Constants.ARTICLE_ROW_HEIGHT;
                line.Y2 = (ArticleLines.Count + 1) * Constants.ARTICLE_ROW_HEIGHT;
                line.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                line.Stroke.Freeze();
                line.StrokeThickness = 1;
                //Panel.SetZIndex(line, 0);

                vm.TimelineHeight.Value = (ArticleLines.Count + 1) * Constants.ARTICLE_ROW_HEIGHT;
                CanvasTimeLine.Children.Add(line);
                ArticleLines.Push(line);
            }
        }

        /// <summary>
        /// タイムラインの横線を1つ消去する
        /// </summary>
        public void DeleteArticleLine()
        {
            CanvasTimeLine.Children.Remove(ArticleLines.Pop());
            vm.TimelineHeight.Value = ArticleLines.Count * Constants.ARTICLE_ROW_HEIGHT;
        }

        public void DrawKey(Key key)
        {
            var length = Constants.ARTICLE_ROW_HEIGHT / Math.Sqrt(2);
            var rhombus = new Rectangle();
            rhombus.Width = length;
            rhombus.Height = length;
            rhombus.Stroke = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            rhombus.Fill = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00));
            rhombus.RenderTransformOrigin = new Point(0.5, 0.5);
            rhombus.RenderTransform = new RotateTransform(45);
            //Panel.SetZIndex(rhombus, 3);
            CanvasTimeLine.Children.Add(rhombus);
            Canvas.SetLeft(rhombus, key.Frame.Value.FrameTime * Constants.FRAME_DISPLAY_INTERVAL - Constants.ARTICLE_ROW_HEIGHT / Math.Sqrt(2) / 2);
            Canvas.SetTop(rhombus, key.Parent.Row.Value * Constants.ARTICLE_ROW_HEIGHT + 2);
            key.Rhombus = rhombus;
        }

        private void DrawFrameLine()
        {
            Binding binding = new Binding("TimelineHeight.Value");
            binding.Source = vm;
            binding.Mode = BindingMode.OneWay;

            // 縦線が現在のタイムラインのキャンバスの横幅まで敷き詰められていなければ敷き詰める
            for (; NumberOfFrameLinesDrawn * Constants.FRAME_DISPLAY_INTERVAL < vm.TimelineLength.Value * Constants.FRAME_DISPLAY_INTERVAL; NumberOfFrameLinesDrawn++)
            {
                //タイムラインのキャンバスにフレーム数の縦線を追加
                var line = new Line();
                line.X1 = NumberOfFrameLinesDrawn * Constants.FRAME_DISPLAY_INTERVAL;
                line.X2 = NumberOfFrameLinesDrawn * Constants.FRAME_DISPLAY_INTERVAL;
                line.Y1 = -1 * ViewModel.TimeLineMargin.Left;
                line.SetBinding(Line.Y2Property, binding);
                line.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                line.Stroke.Freeze();
                line.StrokeThickness = 1;
                //Panel.SetZIndex(line, 1);

                CanvasTimeLine.Children.Add(line);

                //5の倍数の時ルーラーキャンパスにフレーム数の数字を追加
                if(NumberOfFrameLinesDrawn % 5 == 0)
                {
                    var num = new TextBlock();
                    num.Text = NumberOfFrameLinesDrawn.ToString();
                    // numの領域大を計算させる
                    num.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                    CanvasRuler.Children.Add(num);
                    Canvas.SetBottom(num, 0);
                    Canvas.SetLeft(num, NumberOfFrameLinesDrawn * Constants.FRAME_DISPLAY_INTERVAL - num.DesiredSize.Width / 2);
                }
            }
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

        private void WindowTimeLine_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var spaceWidth = (int)Math.Ceiling(TimeLineColumn.ActualWidth);
            if (vm.TimelineLength.Value < spaceWidth)
            {
                vm.TimelineLength.Value = spaceWidth;
            }
        }

        private void WindowTimeLine_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
        }

        private void WindowTimeLine_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Count() > 1)
            {
                MessageBox.Show("投入するファイルは1つだけにしてください。");
                return;
            }
            vm.LoadVMD(files[0]);
        }
    }
}
