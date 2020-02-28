using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ufal.UDPipe;

namespace UdpipeLibrary
{
    public static class UdpipeWrapper
    {
        public static List<Sentence> GetSentenceFromTexts(List<string> texts)
        {
            string modelFile = @"danish-ddt-ud-2.4-190531.udpipe";
            var appDomain = AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;

            Model udpipeModel = Model.load(System.IO.Path.Combine(basePath, modelFile));
            if (udpipeModel == null)
            {
                throw new Exception("Failed: Cannot load model from file '" + modelFile + "'");
            }

            Pipeline pipeline = new Pipeline(udpipeModel, "tokenizer=presegmented", "tag ", "parse", OutputFormat.epe.ToString());
            var processedTexts = texts.Select((x) =>
            {
                using (ProcessingError error = new ProcessingError())
                {
                    var processedText = pipeline.process(x, error);
                    if (error.occurred())
                    {
                        throw new Exception($"Error occured {error.message}");
                    }
                    return processedText;
                }
            }).ToList();
            var sentences = new List<Sentence>();
            foreach (var epeResult in processedTexts)
            {
                Sentence sentence = JsonConvert.DeserializeObject<Sentence>(epeResult);
                sentences.Add(sentence);
            }
            return sentences;
        }

        public partial class Sentence
        {
            public long Id { get; set; }
            public List<Node> Nodes { get; set; }
        }

        public partial class Node
        {
            public long Id { get; set; }
            public string Form { get; set; }
            public Properties Properties { get; set; }
            public bool? Top { get; set; }
        }

        public partial class Properties
        {
            public Abbr? Abbr { get; set; }
            public Typo? Typo { get; set; }
            public AdpType? AdpType { get; set; }
            public Case? Case { get; set; }
            public Definite? Definite { get; set; }
            public Degree? Degree { get; set; }
            public Foreign? Foreign { get; set; }
            public Gender? Gender { get; set; }
            public string Lemma { get; set; }
            public Mood? Mood { get; set; }
            public Number? Number { get; set; }
            public Number? NumberPsor { get; set; }
            public NumType? NumType { get; set; }
            public PartType? PartType { get; set; }
            public Person? Person { get; set; }
            public Poss? Poss { get; set; }
            public PronType? PronType { get; set; }
            public Reflex? Reflex { get; set; }
            public Style Style { get; set; }
            public Tense? Tense { get; set; }
            public Upos Upos { get; set; }
            public VerbForm? VerbForm { get; set; }
            public Voice? Voice { get; set; }
            public string Xpos { get; set; }

        }
        public enum Upos { Adj, Adp, Adv, Aux, Cconj, Det, Intj, Noun, Num, Part, Pron, Propn, Punct, Sconj, Sym, Verb, X };

        #region Lexical features
        public enum PronType { Art, Dem, Emp, Exc, Ind, Int, IntRel, Neg, Prs, Rcp, Rel, Tot };
        public enum NumType { Card, Dist, Frac, Mult, Ord, Range, Sets };
        public enum Poss { Yes };
        public enum Reflex { Yes };
        public enum Foreign { Yes };
        public enum Abbr { Yes };
        public enum Typo { Yes };
        #endregion


        #region Inflectional features

        #region Nominal
        public enum Gender { Masc, Fem, Com, Neut };
        public enum Animacy { Anim, Hum, Inan, Nhum };
        public enum NounClass { Bantu1, Bantu2, Bantu3, Bantu4, Bantu5, Bantu6, Bantu7, Bantu8, Bantu9, Bantu10, Bantu11, Bantu12, Bantu13, Bantu14, Bantu15, Bantu16, Bantu17, Bantu18, Bantu19, Bantu20 };
        public enum Number { Coll, Count, Dual, Grpa, Grpl, Inv, Pauc, Plur, Ptan, Sing, Tri };
        public enum Case { Abs, Acc, Erg, Nom, Abe, Ben, Cau, Cmp, Cns, Com, Dat, Dis, Equ, Gen, Ins, Par, Tem, Tra, Voc, Abl, Add, Ade, All, Del, Ela, Ess, Ill, Ine, Lat, Loc, Per, Sub, Sup, Ter };
        public enum Definite { Com, Cons, Def, Ind, Spec };
        public enum Degree { Abs, Cmp, Equ, Pos, Sup };
        #endregion


        #region Verbal
        public enum VerbForm { Conv, Fin, Gdv, Ger, Inf, Part, Sup, Vnoun };
        public enum Mood { Adm, Cnd, Des, Imp, Ind, Jus, Nec, Opt, Pot, Prp, Qot, Sub };
        public enum Tense { Past, Pres, Fut, Imp, Pqp };
        public enum Aspect { Hab, Imp, Iter, Perf, Prog, Prosp };
        public enum Voice { Act, Antip, Cau, Dir, Inv, Mid, Pass, Rcp };
        public enum Evident { Fh, Nfh };
        public enum Polarity { Neg, Pos };
        public enum Person { ZeroPerson = 0, FirstPerson = 1, SecondPerson = 2, ThirdPerson = 3, FourthPerson = 4 };
        public enum Polite { Elev, Form, Humb, Infm };
        public enum Clusivity { Ex, In };
        #endregion

        #endregion

        #region Other
        public enum AdpType { Prep, Post, Circ, Voc };
        public enum PartType { Mod, Emp, Res, Inf, Vbp };
        public enum Style { Arch, Rare, Form, Poet, Norm, Coll, Vrnc, Slng, Expr, Derg, Vulg };
        public enum OutputFormat { conllu, epe, matxin, horizontal, plaintext, vertical };

        #endregion
    }
}
