using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        Stack<Line> ArticleLines = new Stack<Line>();
        Path GridLine;
        Point GridSize = new Point(0, 0);

        public TimeLineWindow(ViewModel viewModel)
        {
            InitializeComponent();
            vm = viewModel;
            DataContext = vm;
            vm.TimelineWidth.Subscribe(_ => ResizeTimeLineGrid());
            vm.TimelineHeight.Subscribe(_ => ResizeTimeLineGrid());
            Loaded += (s, e) => { vm.TimelineHeight.Value = TimeLineRow.ActualHeight - ((ScrollBar)ScrollTimeLine.Template.FindName("PART_HorizontalScrollBar", ScrollTimeLine)).ActualHeight; };
            Loaded += (s, e) => { DrawTimeLineGrid(); };
            Loaded += (s, e) => { DrawRulerNumber(); };
        }

        private bool AddArticle(string name)
        {
            if (!vm.AddArticle(name))
                return false;

            //DrawArticleLines();
            return true;
        }

        private PathGeometry GetGridGeometry()
        {
            var points = new List<Point>();
            for (int i = 0; i < vm.TimelineWidth.Value; i++)
            {
                var x = i * Constants.FRAME_DISPLAY_INTERVAL + vm.TimeLineMargin.Left;
                points.Add(new Point(x, 0));
                points.Add(new Point(x, vm.TimelineHeight.Value));
            }

            for (int i = 1; i < vm.TimelineHeight.Value; i++)
            {
                var y = i * Constants.ARTICLE_ROW_HEIGHT;
                points.Add(new Point(-1 * vm.TimeLineMargin.Left, y));
                points.Add(new Point(vm.TimelineWidth.Value, y));
            }
            GridSize.X = vm.TimelineWidth.Value;
            GridSize.Y = vm.TimelineHeight.Value;

            var figures = new PathFigureCollection();
            figures.Add(new PathFigure(new Point(0, 0), new PathSegmentCollection(points.Select((p, i) => new LineSegment(p, i % 2 == 1))), false));
            return new PathGeometry(figures);
        }

        public void DrawTimeLineGrid()
        {
            var path = new Path();
            path.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            path.Stroke.Freeze();
            path.StrokeThickness = 1;

            path.Data = GetGridGeometry();
            path.Width = GridSize.X;
            path.Height = GridSize.Y;
            GridLine = path;
            CanvasTimeLine.Children.Add(path);
        }

        public void ResizeTimeLineGrid()
        {
             // 描画済みグリッドの大きさが現在の描画範囲より大きければ再描画しない
            if (GridSize.X > vm.TimelineWidth.Value && GridSize.Y > vm.TimelineHeight.Value)
                return;

            if (GridLine != null)
            {
                GridLine.Data = GetGridGeometry();
                GridLine.Width = GridSize.X;
                GridLine.Height = GridSize.Y;
            }
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
            //Canvas.SetLeft(rhombus, key.Frame.Value.FrameTime * Constants.FRAME_DISPLAY_INTERVAL - Constants.ARTICLE_ROW_HEIGHT / Math.Sqrt(2) / 2);
            //Canvas.SetTop(rhombus, key.Parent.Row.Value * Constants.ARTICLE_ROW_HEIGHT + 2);
            var x = key.Frame.Value.FrameTime * Constants.FRAME_DISPLAY_INTERVAL - Constants.ARTICLE_ROW_HEIGHT / Math.Sqrt(2) / 2 + vm.TimeLineMargin.Left;
            var y = key.Parent.Row.Value * Constants.ARTICLE_ROW_HEIGHT + 2;
            CanvasTimeLine.Children.Add(rhombus);
            FastCanvas.SetLocation(rhombus, new Point(x, y));
            key.Rhombus = rhombus;
        }

        private void DrawRulerNumber()
        {
            // 縦線が現在のタイムラインのキャンバスの横幅まで敷き詰められていなければ敷き詰める
            int leftPosition = 0;//描画位置
            int numberingInterval = 5;//表示する数値の間隔
            for (int i = 0; leftPosition < vm.TimelineWidth.Value; i++)
            {
                leftPosition = i * Constants.FRAME_DISPLAY_INTERVAL * numberingInterval + (int)vm.TimeLineMargin.Left;
                //5の倍数の時ルーラーキャンパスにフレーム数の数字を追加
                var num = new TextBlock();
                num.Text = (i * numberingInterval).ToString();
                // numの領域大を計算させる
                num.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                CanvasRuler.Children.Add(num);
                Canvas.SetBottom(num, 0);
                Canvas.SetLeft(num, leftPosition - num.DesiredSize.Width / 2);
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
            vm.TimelineWidth.Value = TimeLineColumn.ActualWidth - ((ScrollBar)ScrollTimeLine.Template.FindName("PART_VerticalScrollBar", ScrollTimeLine)).ActualWidth;
            vm.TimelineHeight.Value = TimeLineRow.ActualHeight - ((ScrollBar)ScrollTimeLine.Template.FindName("PART_HorizontalScrollBar", ScrollTimeLine)).ActualHeight;
            if (vm.TimelineLength.Value < vm.TimelineWidth.Value)
                vm.TimelineLength.Value = vm.TimelineWidth.Value;
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
