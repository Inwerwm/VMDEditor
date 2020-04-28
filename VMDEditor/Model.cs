using MikuMikuMethods.Vmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VMDEditor
{
    class Model
    {
        public VocaloidMotionData Vmd { private get; set; }

        public Model()
        {
            Vmd = new VocaloidMotionData();
        }

        public void ReadVMD(string path)
        {
            using BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open));
            Vmd.Read(reader);
        }

        public List<Article> GetArticles()
        {
            var articles = new List<Article>();

            foreach (var g in Vmd.MorphFrames.GroupBy(f=>f.Name))
            {
                var a = new Article(ArticleType.Morph, g.Key);
                foreach (var f in g)
                {
                    a.Keys.Add(new Key(f));
                }
                articles.Add(a);
            }

            foreach (var g in Vmd.MotionFrames.GroupBy(f => f.Name))
            {
                var a = new Article(ArticleType.Morph, g.Key);
                foreach (var f in g)
                {
                    a.Keys.Add(new Key(f));
                }
                articles.Add(a);
            }

            return articles;
        }
    }
}
