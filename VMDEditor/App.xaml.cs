using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VMDEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }

    /// <summary>
    /// 定数の静的クラス
    /// </summary>
    public static class Constants
    {
        public static readonly int ARTICLE_ROW_HEIGHT = 20;

        public static readonly int FRAME_DISPLAY_INTERVAL = 20;
    }
}
