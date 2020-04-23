using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;
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
        public static int ArticleRowHeight { get; } = Constants.ARTICLE_ROW_HEIGHT;

        public Article(ArticleType type,string name = "")
        {
            Name = name;
            Type = type;
            Keys = new ReactiveCollection<Key>().AddTo(Disposable);
        }
        public void Dispose() => Disposable.Dispose();
    }

    public class Key : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public ReactiveProperty<IVmdFrameData> Frame { get; }
        public ReactiveProperty<bool> IsVirtual { get; }

        public Key(IVmdFrameData f)
        {
            Frame = new ReactiveProperty<IVmdFrameData>(f).AddTo(Disposable);
            IsVirtual = new ReactiveProperty<bool>().AddTo(Disposable);
        }

        public void Dispose() => Disposable.Dispose();
    }
}
