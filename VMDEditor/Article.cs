using System;
using System.Collections.Generic;
using System.Text;
using MikuMikuMethods.Vmd;

namespace VMDEditor
{
    public enum ArticleType
    {
        Bone,
        Morph
    }

    public class Article
    {
        public string Name { get; set; }
        public ArticleType Type { get; private set; }
        public List<IVmdModelFrameData> Keys { get; } = new List<IVmdModelFrameData>();
        
        public Article(ArticleType type,string name = "")
        {
            Name = name;
            Type = type;
        }
    }
}
