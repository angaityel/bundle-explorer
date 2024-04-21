using System.IO.Compression;
using System.Text;

namespace bundleexplorer
{
    public partial class Form1 : Form
    {
        string bundlePath = "";
        Dictionary<ulong, string> hashDict = new Dictionary<ulong, string>();
        public Form1()
        {
            InitializeComponent();

            if (File.Exists("replacedfiles.txt"))
            {
                var lines = File.ReadAllLines("replacedfiles.txt");
                foreach (var line in lines)
                {
                    if (line != "")
                    {
                        var items = line.Split(';');
                        ListViewItem lvi = listViewBundle.Items.Add(items[0]);
                        lvi.SubItems.Add(items[1]);
                        lvi.SubItems.Add(items[2]);
                    }
                }
            }

            var fileNames = File.ReadAllLines("filenames.txt");
            foreach (string fileName in fileNames)
            {
                ulong fileNameHash = Murmur.ComputeHash64(Encoding.ASCII.GetBytes(fileName));
                hashDict[fileNameHash] = fileName;
            }
        }
        public void Export(string[] bundleList, string saveFolder)
        {
            foreach (var bundleFile in bundleList)
            {
                if (!bundleFile.Contains('.'))
                {
                    string bundleFileName = Path.GetFileName(bundleFile);
                    List<string> list = new List<string>();

                    using (FileStream fileStream = new FileStream(bundleFile, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader brFileStream = new BinaryReader(fileStream))
                        {
                            brFileStream.ReadInt32();
                            int uncompressedSize = brFileStream.ReadInt32();
                            byte[] uncompressedfile = new byte[uncompressedSize];

                            brFileStream.ReadInt32();
                            using (var uncompressedStream = new MemoryStream())
                            {
                                while (brFileStream.BaseStream.Position < brFileStream.BaseStream.Length)
                                {
                                    byte[] chunk = new byte[65536];
                                    int compressedSize = brFileStream.ReadInt32();
                                    if (compressedSize == 65536)
                                    {
                                        chunk = brFileStream.ReadBytes(compressedSize);
                                        uncompressedStream.Write(chunk);
                                    }
                                    else
                                    {
                                        byte[] compressedPart = brFileStream.ReadBytes(compressedSize);
                                        using (var stream = new MemoryStream(compressedPart, 0, compressedPart.Length))
                                        using (var decompress = new ZLibStream(stream, CompressionMode.Decompress))
                                            decompress.CopyTo(uncompressedStream);
                                    }
                                }

                                using (var bReader = new BinaryReader(uncompressedStream))
                                {
                                    bReader.BaseStream.Position = 0;
                                    int numFiles = bReader.ReadInt32();
                                    bReader.ReadBytes(0x100);
                                    for (int i = 0; i < numFiles; i++)
                                    {
                                        ulong hashExtension = bReader.ReadUInt64();
                                        string fileExtension;
                                        if (hashDict.TryGetValue(hashExtension, out fileExtension))
                                        { }
                                        else
                                            fileExtension = hashExtension.ToString("x").ToUpper();

                                        ulong hashPath = bReader.ReadUInt64();
                                        string filePath;
                                        if (hashDict.TryGetValue(hashPath, out filePath))
                                        { }
                                        else
                                        {
                                            filePath = hashPath.ToString("x").ToUpper();
                                        }
                                        list.Add(filePath + "." + fileExtension);

                                    }

                                    foreach (var file in list)
                                    {
                                        bReader.ReadBytes(0x10);
                                        int unk = bReader.ReadByte();
                                        bReader.ReadBytes(0x0B);
                                        int fSize = bReader.ReadInt32();
                                        if (unk == 2)
                                        {
                                            bReader.ReadBytes(0x08);
                                            fSize += bReader.ReadInt32();
                                        }
                                        else if (unk == 3)
                                        {
                                            bReader.ReadBytes(0x08);
                                            fSize += bReader.ReadInt32();
                                            bReader.ReadBytes(0x08);
                                            fSize += bReader.ReadInt32();
                                        }
                                        bReader.ReadInt32();


                                        if (file.Contains(".lua"))
                                        {

                                            fSize = bReader.ReadInt32();
                                            bReader.ReadInt32();
                                        }
                                        byte[] buffer = new byte[fSize];
                                        buffer = bReader.ReadBytes(fSize);

                                        if (checkBoxExtact.Checked)
                                        {
                                            Directory.CreateDirectory(Path.GetDirectoryName(saveFolder + "\\bundles\\bundleFileName\\" + file));
                                            File.WriteAllBytes(saveFolder + "\\bundles\\bundleFileName\\" + file, buffer);
                                        }
                                        else
                                        {
                                            Directory.CreateDirectory(Path.GetDirectoryName(saveFolder + "\\bundle\\" + file));
                                            File.WriteAllBytes(saveFolder + "\\bundle\\" + file, buffer);
                                        }

                                    }

                                }

                            }

                        }
                    }

                }
            }



        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new()
            {
                Filter = "Any file from bundle/contents/data folder | *.*"
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                bundlePath = Path.GetDirectoryName(openFile.FileName);
                string[] bundleList = Directory.GetFiles(bundlePath);
                LoadTree(bundleList);
                buttonExtract.Enabled = true;
                buttonCreatePatch.Enabled = true;
                checkBoxExtact.Enabled = true;
            }
        }

        public void LoadTree(string[] bundleList)
        {
            foreach (var bundleFile in bundleList)
            {

                if (!bundleFile.Contains('.'))
                {
                    List<string> list = new List<string>();


                    ulong fileNameHash = Convert.ToUInt64(Path.GetFileName(bundleFile), 16);
                    string fileNamePath;
                    if (hashDict.TryGetValue(fileNameHash, out fileNamePath))
                    {
                    }
                    else
                    {
                        fileNamePath = "Unknown name";
                    }

                    using (FileStream fileStream = new FileStream(bundleFile, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader binaryReaderFileStream = new BinaryReader(fileStream))
                        {
                            binaryReaderFileStream.ReadBytes(0x0C);
                            int compressedSize = binaryReaderFileStream.ReadInt32();
                            byte[] compressedPart = binaryReaderFileStream.ReadBytes(compressedSize);

                            using (var streamCompressedPart = new MemoryStream(compressedPart, 0, compressedPart.Length))
                            using (var decompress = new ZLibStream(streamCompressedPart, CompressionMode.Decompress))
                            using (var binaryReaderDecompress = new BinaryReader(decompress))
                            {
                                int numfiles = binaryReaderDecompress.ReadInt32();
                                binaryReaderDecompress.ReadBytes(0x100);
                                for (int i = 0; i < numfiles; i++)
                                {
                                    ulong hashExtension = binaryReaderDecompress.ReadUInt64();
                                    string fileExtension;
                                    if (hashDict.TryGetValue(hashExtension, out fileExtension))
                                    {
                                    }
                                    else
                                    {
                                        fileExtension = hashExtension.ToString("x").ToUpper();
                                    }

                                    ulong hashPath = binaryReaderDecompress.ReadUInt64();
                                    string filePath;
                                    if (hashDict.TryGetValue(hashPath, out filePath))
                                    {
                                    }
                                    else
                                    {
                                        filePath = hashPath.ToString("x").ToUpper();
                                    }
                                    list.Add(filePath + "." + fileExtension);
                                }

                            }
                        }
                    }
                    treeViewBundle.Nodes.Add(LoadTree(list, Path.GetFileName(bundleFile) + " (" + fileNamePath + ")"));

                }
            }
            treeViewBundle.Sort();
        }
        public static TreeNode LoadTree(List<string> paths, string rootNodeName)
        {
            var rootNode = new TreeNode(rootNodeName);
            foreach (var path in paths.Where(x => !string.IsNullOrEmpty(x.Trim())))
            {
                var currentNode = rootNode;

                var pathItems = path.Split('/');
                foreach (var item in pathItems)
                {
                    var tmp = currentNode.Nodes.Cast<TreeNode>().Where(x => x.Text.Equals(item));
                    currentNode = tmp.Count() > 0 ? tmp.Single() : currentNode.Nodes.Add(item);
                }
            }
            return rootNode;
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewBundle.SelectedNode != null)
            {
                if (treeViewBundle.SelectedNode.FullPath.Contains('.'))
                {
                    string bundle = treeViewBundle.SelectedNode.FullPath.Split(' ')[0];
                    string bundleFilePath = treeViewBundle.SelectedNode.FullPath.Split(")\\")[1].Replace('\\', '/');

                    OpenFileDialog dialog = new()
                    {
                        Filter = "File for replace | *.*"
                    };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        ListViewItem lvi = listViewBundle.Items.Add(bundle);
                        lvi.SubItems.Add(bundleFilePath);
                        lvi.SubItems.Add(dialog.FileName);
                    }

                }

            }
        }

        private void treeViewBundle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (treeViewBundle.HitTest(e.Location).Node == null)
                {
                    contextMenuStripBundleTree.Enabled = false;
                }

            }
        }

        private void treeViewBundle_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeViewBundle.SelectedNode = e.Node;
                var focusedItem = treeViewBundle.SelectedNode;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStripBundleTree.Enabled = true;
                    contextMenuStripBundleTree.Show(Cursor.Position);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listViewBundle.Items.Count > 0)
            {
                Dictionary<string, List<List<string>>> patchDict = new Dictionary<string, List<List<string>>>();

                listViewBundle.Sort();

                using (var replacedFiles = new StreamWriter("replacedfiles.txt"))
                {
                    foreach (ListViewItem item in listViewBundle.Items)
                    {
                        replacedFiles.WriteLine(item.Text + ";" + item.SubItems[1].Text + ";" + item.SubItems[2].Text);
                    }
                }

                foreach (ListViewItem item in listViewBundle.Items)
                {
                    string path = bundlePath + "\\" + item.Text;

                    if (patchDict.ContainsKey(item.Text))
                    {
                        List<string> tempList = new List<string> { item.SubItems[1].Text, item.SubItems[2].Text };
                        patchDict[item.Text].Add(tempList);
                    }
                    else
                    {
                        List<string> tempList = new List<string> { item.SubItems[1].Text, item.SubItems[2].Text };
                        List<List<string>> listList = new List<List<string>>();
                        listList.Add(tempList);
                        patchDict[item.Text] = listList;
                    }
                }
                foreach (var patch in patchDict)
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        // Use the memory stream in a binary reader.
                        using (BinaryWriter writerMemory = new BinaryWriter(memory))
                        {
                            int filesCount = patch.Value.Count;
                            byte[] unk = new byte[256];
                            using (FileStream fileStream = new FileStream(bundlePath + "\\" + patch.Key, FileMode.Open, FileAccess.Read))
                            {
                                using (BinaryReader binaryReaderfileStream = new BinaryReader(fileStream))
                                {
                                    binaryReaderfileStream.ReadBytes(0x0C);
                                    int compressedSize = binaryReaderfileStream.ReadInt32();
                                    byte[] compressedPart = binaryReaderfileStream.ReadBytes(compressedSize);

                                    using (var streamCompressedPart = new MemoryStream(compressedPart, 0, compressedPart.Length))
                                    using (var decompress = new ZLibStream(streamCompressedPart, CompressionMode.Decompress))
                                    using (var bReader = new BinaryReader(decompress))
                                    {
                                        bReader.ReadInt32();
                                        unk = bReader.ReadBytes(0x100);
                                    }
                                }
                            }
                            writerMemory.Write(filesCount);
                            writerMemory.Write(unk);
                            foreach (var items in patch.Value)
                            {
                                var path = items[0].Split('.');
                                ulong hashExtension = Murmur.ComputeHash64(Encoding.ASCII.GetBytes(path[1]));
                                writerMemory.Write(hashExtension);
                                if (path[0].ToUpper() == path[0])
                                {
                                    ulong hashName = Convert.ToUInt64(Path.GetFileName(path[0]), 16);
                                    writerMemory.Write(hashName);
                                }
                                else
                                {
                                    ulong hashName = Murmur.ComputeHash64(Encoding.ASCII.GetBytes(path[0]));
                                    writerMemory.Write(hashName);
                                }

                            }

                            foreach (var items in patch.Value)
                            {
                                var path = items[0].Split('.');
                                ulong hashExtension = Murmur.ComputeHash64(Encoding.ASCII.GetBytes(path[1]));
                                writerMemory.Write(hashExtension);
                                if (path[0].ToUpper() == path[0])
                                {
                                    ulong hashName = Convert.ToUInt64(Path.GetFileName(path[0]), 16);
                                    writerMemory.Write(hashName);
                                }
                                else
                                {
                                    ulong hashName = Murmur.ComputeHash64(Encoding.ASCII.GetBytes(path[0]));
                                    writerMemory.Write(hashName);
                                }

                                using (FileStream file = new FileStream(items[1], FileMode.Open, FileAccess.Read))
                                {
                                    writerMemory.Write(1);
                                    writerMemory.Write((long)0);

                                    if (path[1] == "lua")
                                    {
                                        writerMemory.Write((uint)file.Length + 8);
                                        writerMemory.Write(0);
                                        writerMemory.Write((uint)file.Length);
                                        writerMemory.Write(2);
                                    }
                                    else
                                    {
                                        writerMemory.Write((uint)file.Length);
                                        writerMemory.Write(0);
                                    }

                                    file.CopyTo(memory);
                                }
                            }

                            memory.Position = 0;

                            using (MemoryStream compressedMemory = new MemoryStream())
                            {
                                using (BinaryWriter bwCompressedMemory = new BinaryWriter(compressedMemory))
                                {
                                    bwCompressedMemory.Write(4026531844);
                                    bwCompressedMemory.Write(memory.Length);

                                    byte[] buffer = new byte[65536];
                                    while (memory.Read(buffer, 0, buffer.Length) > 0)
                                    {
                                        using (var tempStream = new MemoryStream())
                                        {
                                            using (var compress = new ZLibStream(tempStream, CompressionMode.Compress, true))
                                            {
                                                compress.Write(buffer, 0, buffer.Length);
                                            }
                                            bwCompressedMemory.Write((uint)tempStream.Length);



                                            tempStream.WriteTo(compressedMemory);
                                        }
                                    }
                                    Directory.CreateDirectory(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + "\\patch\\"));
                                    using (FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\patch\\" + patch.Key + ".patch_0", FileMode.Create, FileAccess.Write))
                                    {
                                        compressedMemory.WriteTo(file);
                                    }
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("Done");
            }
            else
                MessageBox.Show("Patch list empty");
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog selectSaveFolder = new FolderBrowserDialog();
            if (selectSaveFolder.ShowDialog() == DialogResult.OK)
            {
                string folderName = selectSaveFolder.SelectedPath;
                string[] bundleList = Directory.GetFiles(bundlePath);

                buttonCreatePatch.Enabled = false;
                buttonExtract.Enabled = false;
                buttonOpen.Enabled = false;
                checkBoxExtact.Enabled = false;

                await Task.Run(() => Export(bundleList, folderName));
                MessageBox.Show("Done");

                buttonCreatePatch.Enabled = true;
                buttonExtract.Enabled = true;
                buttonOpen.Enabled = true;
                checkBoxExtact.Enabled = true;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listViewBundle.SelectedItems)
            {
                listViewBundle.Items.Remove(lvi);
            }
        }

        private void listViewBundle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listViewBundle.HitTest(e.Location).Item == null)
                {
                    contextMenuStripReplaceList.Enabled = false;
                }
            }
        }

        private void listViewBundle_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = listViewBundle.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStripReplaceList.Enabled = true;
                    contextMenuStripReplaceList.Show(Cursor.Position);
                }
            }
        }
    }
}
