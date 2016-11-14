using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCoding
{
    class Program
    {
        static void Main(string[] args)
        {

            string infile = "infile.dat";
            string outfile = "outfile.dat";

            string input = System.IO.File.ReadAllText(infile);

            HuffmanCode hc = new HuffmanCode(input);

            string output = hc.OutputString();

            System.IO.File.WriteAllText(outfile, output);
        }
    }

    class TreeNode
    {
        public int leftChild { get; set; }
        public int rightChild { get; set; }
        public int parent { get; set; }
        public string code { get; set; }

        public TreeNode()
        {
            leftChild = 0;
            rightChild = 0;
            parent = 0;
            code = "";
        }
    }

    class SymbolInfo
    {
        public char symbol { get; set; }
        public int frequency { get; set; }
        public int leaf { get; set; }

        public SymbolInfo(char c, int f, int l)
        {
            symbol = c;
            frequency = f;
            leaf = l;
        }
    }

    class HuffmanCode
    {
        private List<TreeNode> tree;
        private Dictionary<char, SymbolInfo> FrequencyTable;
        private SortedList<int, int> Forest;
    
        private int charCount;
        private int totalCharCount;

        public HuffmanCode(string input)
        {
            tree = new List<TreeNode>();
            FrequencyTable = new Dictionary<char, SymbolInfo>();
            Forest = new SortedList<int, int>(new DuplicateKeyComparer<int>());
            charCount = 0;
            totalCharCount = 0;

            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }

            string newInput = sb.ToString();

            GenerateFrequencyTable(newInput);
            BuildTree();
            encodeTree();
        }
        
        private void GenerateFrequencyTable(string input)
        {
            foreach (var c in input)
            {
                totalCharCount++;
                if (!FrequencyTable.ContainsKey(c))
                {
                    FrequencyTable.Add(c, new SymbolInfo(c, 1, charCount++));
                }
                else
                {
                    SymbolInfo info;
                    FrequencyTable.TryGetValue(c, out info);
                    info.frequency = info.frequency + 1;
                }
            }

            charCount = 0;
            foreach (var key in FrequencyTable.Keys)
            {
                tree.Add(new TreeNode());
                Forest.Add(FrequencyTable[key].frequency, charCount++);
            }
        }
        
        private void BuildTree()
        {
            while (Forest.Count > 1)
            {
                int minWeight = Forest.Keys.First();
                int minRoot = Forest.Values.First();
                Forest.RemoveAt(0);

                int nextWeight = Forest.Keys.First();
                int nextRoot = Forest.Values.First();
                Forest.RemoveAt(0);

                TreeNode node = new TreeNode();
                node.leftChild = minRoot;
                node.rightChild = nextRoot;
                tree.Add(node);
                tree[minRoot].parent = tree.IndexOf(node);
                tree[nextRoot].parent = tree.IndexOf(node);

                Forest.Add(minWeight + nextWeight, tree.IndexOf(node));
            }
        }    
        
        private void encode(int node)
        {
            if (tree[node].parent == 0 || !String.IsNullOrEmpty(tree[node].code))
            {
                return;
            }

            encode(tree[node].parent);

            if (tree[tree[node].parent].leftChild == node)
            {
                tree[node].code = tree[tree[node].parent].code + "0";
            }
            else
            {
                tree[node].code = tree[tree[node].parent].code + "1";
            }
        }
        
        private void encodeTree()
        {
            foreach (var c in FrequencyTable.Keys)
            {
                encode(FrequencyTable[c].leaf);
            }
        }
        
        public string OutputString()
        {
            StringBuilder sb = new StringBuilder("Symbol    Frequency" + System.Environment.NewLine);
            SortedDictionary<double, char> sortedFreq = new SortedDictionary<double, char>(new DuplicateKeyComparer<double>());
            foreach (var item in FrequencyTable)
            {
                sortedFreq.Add(Math.Round(item.Value.frequency * 100.0 / totalCharCount, 2), item.Key);
            }

            foreach (var item in sortedFreq.Reverse())
            {
                sb.Append(item.Value.ToString() + "         " + item.Key + "%" + System.Environment.NewLine);
            }

            sb.Append(System.Environment.NewLine + "Symbol    Code" + System.Environment.NewLine);

            foreach (var item in sortedFreq.Reverse())
            {
                char tableKey = item.Value;
                int node = FrequencyTable[tableKey].leaf;
                string code = tree[node].code;
                sb.Append(item.Value.ToString() + "         " + code + System.Environment.NewLine);
            }

            int bitCount = 0;

            foreach (var item in FrequencyTable)
            {
                bitCount += item.Value.frequency * tree[item.Value.leaf].code.Count();
            }


            sb.Append(System.Environment.NewLine + "Bit Count post encoding: " + bitCount);
            return sb.ToString();
        }
    }

    public class DuplicateKeyComparer<TKey>: IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }

        #endregion
    }


}
