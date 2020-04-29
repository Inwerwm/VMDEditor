using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using MikuMikuMethods.Vmd;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace VMDEditor
{
    public enum ArticleType
    {
        Bone,
        Morph
    }

    public class Article : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public string Name { get; set; }
        public ArticleType Type { get; private set; }
        public ReactiveCollection<Key> Keys { get; }

        // Binding用
        public ReactiveProperty<int> Row { get; }
        public static int ArticleRowHeight { get; } = Constants.ARTICLE_ROW_HEIGHT;

        public Article(ArticleType type,string name = "")
        {
            Name = name;
            Type = type;
            Keys = new ReactiveCollection<Key>().AddTo(Disposable);

            Row = new ReactiveProperty<int>(-1).AddTo(Disposable);
        }
        public void Dispose() => Disposable.Dispose();
    }

    public class Key : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CompositeDisposable Disposable { get; } = new CompositeDisposable();
        public Article Parent { get; private set; }
        public Rectangle Rhombus { get; set; }

        public ReactiveProperty<IVmdFrameData> Frame { get; }
        public ReactiveProperty<bool> IsVirtual { get; }

        public Key(Article parent, IVmdFrameData f)
        {
            Parent = parent;
            Frame = new ReactiveProperty<IVmdFrameData>(f).AddTo(Disposable);
            IsVirtual = new ReactiveProperty<bool>().AddTo(Disposable);
        }

        public void Dispose() => Disposable.Dispose();
    }
}
