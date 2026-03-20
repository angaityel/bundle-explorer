using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using ZstdSharp;

namespace bundleexplorer
{
    public partial class Form1 : Form
    {
        string bundlePath = "";
        Dictionary<ulong, string> hashDict = new Dictionary<ulong, string>();
        Dictionary<ulong, string> hashDict32 = new Dictionary<ulong, string>();
        public Form1()
        {
            InitializeComponent();

            originalTreeView = new TreeView{ Dock = DockStyle.Fill, Visible = false };
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
                ulong fileNameHash64 = Murmur.ComputeHash64(Encoding.ASCII.GetBytes(fileName));
                hashDict[fileNameHash64] = fileName;
                ulong fileNameHash32 = fileNameHash64 >> 32;
                hashDict32[fileNameHash32] = fileName;
            }
        }
        public void Export(string[] bundleList, string saveFolder)
        {
            foreach (var bundleFile in bundleList)
            {
                if (!Path.HasExtension(bundleFile) || (bundleFile.Contains(".patch") && !bundleFile.EndsWith(".stream")))
                {
                    string bundleFileName = Path.GetFileName(bundleFile);
                    ulong fileNameHash = Convert.ToUInt64(Path.GetFileNameWithoutExtension(bundleFile), 16);
                    if (hashDict.TryGetValue(fileNameHash, out string fileNamePath))
                    { }
                    else
                    {
                        fileNamePath = "Unknown name";
                    }
                    List<string> list = new List<string>();

                    using (FileStream fileStream = new FileStream(bundleFile, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader brFileStream = new BinaryReader(fileStream))
                        {
                            uint version = brFileStream.ReadUInt32();
                            if (version is not (>= 4026531843 and <= 4026531847))
                                continue;
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
                                        uint compressionHeader = brFileStream.ReadUInt32();
                                        brFileStream.BaseStream.Position -= 0x04;
                                        byte[] compressedPart = brFileStream.ReadBytes(compressedSize);
                                        using (var stream = new MemoryStream(compressedPart, 0, compressedPart.Length))
                                        {
                                            if (compressionHeader == 4247762216)
                                            {
                                                var dict = File.ReadAllBytes(Path.GetDirectoryName(bundleFile) + "\\" + "compression.dictionary");
                                                using var decompressor = new Decompressor();
                                                decompressor.LoadDictionary(dict);
                                                using (var decompressionStream = new DecompressionStream(stream, decompressor))
                                                    decompressionStream.CopyTo(uncompressedStream);
                                            }
                                            else
                                            {
                                                using (var decompress = new ZLibStream(stream, CompressionMode.Decompress))
                                                    decompress.CopyTo(uncompressedStream);
                                            }
                                        }
                                    }
                                }

                                using (var bReader = new BinaryReader(uncompressedStream))
                                {
                                    bReader.BaseStream.Position = 0;
                                    int numFiles = bReader.ReadInt32();
                                    int numFiles2 = 0;
                                    bReader.BaseStream.Position += 0x100;

                                    if (bundleFile.Contains("immortal\\bundled") || bundleFile.Contains("Project Evil\\bundled"))
                                    {
                                        bReader.BaseStream.Position = 0x04;
                                        numFiles = bReader.ReadInt32();
                                        numFiles2 = bReader.ReadInt32();
                                        //bReader.BaseStream.Position += 0x04;
                                    }
                                    if (numFiles == 0 && !bundleFile.Contains("Warhammer End Times - VR\\bundle"))
                                    {
                                        continue;
                                    }
                                    if (bundleFile.Contains("Warhammer End Times - VR\\bundle"))
                                    {
                                        numFiles = bReader.ReadInt32();
                                        bReader.BaseStream.Position += 0x04;
                                    }

                                    for (int i = 0; i < numFiles; i++)
                                    {
                                        ulong hashExtension = bReader.ReadUInt64();
                                        if (hashDict.TryGetValue(hashExtension, out string fileExtension))
                                        {
                                            if (fileExtension == "texture")
                                            {
                                                fileExtension = "dds";
                                            }
                                        }
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

                                        if (version == 4026531845 && bundleFile.Contains("Warhammer End Times Vermintide\\bundle"))
                                            bReader.BaseStream.Position += 0x04;
                                        if (bundleFile.Contains("Warhammer Vermintide 2\\bundle"))
                                            bReader.BaseStream.Position += 0x08;
                                    }

                                    bReader.BaseStream.Position += numFiles2 * 0x20;

                                    foreach (var file in list)
                                    {
                                        bReader.BaseStream.Position += 0x10; //name and extension
                                        int numParts = bReader.ReadInt32(); //fonts, localization strings
                                        if (version > 4026531843)
                                        {
                                            bReader.BaseStream.Position += 0x04;
                                        }

                                        string partNumber = "";

                                        List<int> sizePartsList = new List<int>();
                                        for (int i = 0; i < numParts; i++)
                                        {
                                            if (bundleFile.Contains("immortal\\bundled") || bundleFile.Contains("Project Evil\\bundled"))
                                            {
                                                sizePartsList.Add(bReader.ReadInt32());
                                                bReader.BaseStream.Position += 0x04;
                                            }
                                            else
                                            {
                                                bReader.BaseStream.Position += 0x04;
                                                sizePartsList.Add(bReader.ReadInt32());
                                                if (version > 4026531843)
                                                {
                                                    bReader.BaseStream.Position += 0x04;
                                                }
                                            }
                                        }
                                        for (int i = 0; i < sizePartsList.Count; i++)
                                        {
                                            if (file.Contains(".lua"))
                                            {
                                                if (version == 4026531847 && bundleFile.Contains("Warhammer Vermintide 2\\bundle"))
                                                {
                                                    sizePartsList[i] = bReader.ReadInt32();
                                                    bReader.BaseStream.Position += 0x08;
                                                }
                                                else if(version > 4026531843)
                                                {
                                                    sizePartsList[i] = bReader.ReadInt32();
                                                    bReader.BaseStream.Position += 0x04;
                                                }
                                                else
                                                {
                                                    sizePartsList[i] = bReader.ReadInt32() - 4;
                                                }
                                            }
                                            byte[] buffer = new byte[sizePartsList[i]];
                                            buffer = bReader.ReadBytes(sizePartsList[i]);

                                            if (numParts != 1)
                                            {
                                                partNumber = "_" + i.ToString();
                                            }
                                            if (checkBoxExtact.Checked)
                                            {
                                                Directory.CreateDirectory(Path.GetDirectoryName(saveFolder + "\\bundles\\" + bundleFileName + " " + fileNamePath.Replace("/", "__") + "\\" + file.Replace(":", "__colon__")));
                                                File.WriteAllBytes(saveFolder + "\\bundles\\" + bundleFileName + " " + fileNamePath.Replace("/", "__") + "\\" + file.Replace(":", "__colon__") + partNumber, buffer);
                                            }
                                            else
                                            {
                                                Directory.CreateDirectory(Path.GetDirectoryName(saveFolder + "\\bundle\\" + file.Replace(":", "__colon__")));
                                                File.WriteAllBytes(saveFolder + "\\bundle\\" + file.Replace(":", "__colon__") + partNumber, buffer);
                                            }
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
                treeViewBundle.Nodes.Clear();
                originalTreeView.Nodes.Clear();
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
                if (!Path.HasExtension(bundleFile) || (bundleFile.Contains(".patch") && !bundleFile.EndsWith(".stream")))
                {
                    List<string> list = new List<string>();

                    ulong fileNameHash = Convert.ToUInt64(Path.GetFileNameWithoutExtension(bundleFile), 16);
                    if (hashDict.TryGetValue(fileNameHash, out string fileNamePath))
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
                            uint version = binaryReaderFileStream.ReadUInt32();
                            if (version is not (>= 4026531843 and <= 4026531847))
                                continue;
                            binaryReaderFileStream.BaseStream.Position += 0x08;
                            using (var uncompressedStream = new MemoryStream())
                            {
                                int chunksRead = 0;
                                while (binaryReaderFileStream.BaseStream.Position < binaryReaderFileStream.BaseStream.Length)
                                {
                                    if (chunksRead > 30)
                                    {
                                        break;
                                    }
                                    byte[] chunk = new byte[65536];
                                    int compressedSize = binaryReaderFileStream.ReadInt32();
                                    if (compressedSize == 65536)
                                    {
                                        chunk = binaryReaderFileStream.ReadBytes(compressedSize);
                                        uncompressedStream.Write(chunk);
                                    }
                                    else
                                    {
                                        uint compressionHeader = binaryReaderFileStream.ReadUInt32();
                                        binaryReaderFileStream.BaseStream.Position -= 0x04;
                                        byte[] compressedPart = binaryReaderFileStream.ReadBytes(compressedSize);
                                        using (var stream = new MemoryStream(compressedPart, 0, compressedPart.Length))
                                        {
                                            if (compressionHeader == 4247762216)
                                            {
                                                var dict = File.ReadAllBytes(Path.GetDirectoryName(bundleFile) + "\\" + "compression.dictionary");
                                                using var decompressor = new Decompressor();
                                                decompressor.LoadDictionary(dict);
                                                using (var decompressionStream = new DecompressionStream(stream, decompressor))
                                                decompressionStream.CopyTo(uncompressedStream);
                                            }
                                            else
                                            {
                                                using (var decompress = new ZLibStream(stream, CompressionMode.Decompress))
                                                    decompress.CopyTo(uncompressedStream);
                                            }
                                        }
                                    }
                                    chunksRead++;
                                }

                                using (var binaryReaderDecompress = new BinaryReader(uncompressedStream))
                                {
                                    binaryReaderDecompress.BaseStream.Position = 0;
                                    int numFiles = binaryReaderDecompress.ReadInt32();
                                    binaryReaderDecompress.BaseStream.Position += 0x100;
                                    if (bundleFile.Contains("immortal\\bundled") || bundleFile.Contains("Project Evil\\bundled"))
                                    {
                                        binaryReaderDecompress.BaseStream.Position = 0x04;
                                        numFiles = binaryReaderDecompress.ReadInt32();
                                        binaryReaderDecompress.BaseStream.Position += 0x04;
                                    }
                                    if (numFiles == 0 && !bundleFile.Contains("Warhammer End Times - VR\\bundle"))
                                    {
                                        continue;
                                    }
                                    if (bundleFile.Contains("Warhammer End Times - VR\\bundle"))
                                    {
                                        numFiles = binaryReaderDecompress.ReadInt32();
                                        binaryReaderDecompress.BaseStream.Position += 0x04;
                                    }
                                    for (int i = 0; i < numFiles; i++)
                                    {
                                        ulong hashExtension = binaryReaderDecompress.ReadUInt64();
                                        if (hashDict.TryGetValue(hashExtension, out string fileExtension))
                                        { }
                                        else
                                        {
                                            fileExtension = hashExtension.ToString("x").ToUpper();
                                        }

                                        ulong hashPath = binaryReaderDecompress.ReadUInt64();
                                        UInt128 combined = (UInt128)hashExtension << 64 | hashPath;
                                        if (hashDict.TryGetValue(hashPath, out string filePath))
                                        {
                                        }
                                        else
                                        {
                                            filePath = hashPath.ToString("x16").ToUpper();
                                        }
                                        list.Add(filePath + "." + fileExtension);

                                        if (version == 4026531845 && bundleFile.Contains("Warhammer End Times Vermintide\\bundle"))
                                            binaryReaderDecompress.BaseStream.Position += 0x04;
                                        if (bundleFile.Contains("Warhammer Vermintide 2\\bundle"))
                                            binaryReaderDecompress.BaseStream.Position += 0x08;

                                    }
                                }
                            }
                        }
                    }
                    treeViewBundle.Nodes.Add(LoadTree(list, Path.GetFileName(bundleFile) + " (" + fileNamePath + ")"));
                }
            }
            treeViewBundle.Sort();
            CloneTree(originalTreeView.Nodes, treeViewBundle.Nodes);
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
                        using (BinaryWriter writerMemory = new BinaryWriter(memory))
                        {
                            int filesCount = patch.Value.Count;
                            byte[] unk = new byte[256];
                            uint version;
                            using (FileStream fileStream = new FileStream(bundlePath + "\\" + patch.Key, FileMode.Open, FileAccess.Read))
                            {
                                using (BinaryReader binaryReaderfileStream = new BinaryReader(fileStream))
                                {
                                    version = binaryReaderfileStream.ReadUInt32();
                                    binaryReaderfileStream.BaseStream.Position += 0x08;
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
                                    bwCompressedMemory.Write(version);
                                    bwCompressedMemory.Write(memory.Length);

                                    byte[] buffer = new byte[65536];
                                    int bytesRead;
                                    int maxCompressedSize = 65536;

                                    while ((bytesRead = memory.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        using (var tempStream = new MemoryStream())
                                        {
                                            using (var compress = new ZLibStream(tempStream, CompressionMode.Compress, true))
                                            {
                                                compress.Write(buffer, 0, bytesRead);
                                            }
                                            byte[] compressedData = tempStream.ToArray();

                                            if (compressedData.Length >= bytesRead || compressedData.Length > maxCompressedSize)
                                            {
                                                bwCompressedMemory.Write(65536);
                                                bwCompressedMemory.Write(buffer, 0, bytesRead);
                                            }
                                            else
                                            {
                                                bwCompressedMemory.Write(compressedData.Length);
                                                bwCompressedMemory.Write(compressedData);
                                            }
                                        }
                                    }

                                    Directory.CreateDirectory(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + "\\patch\\"));
                                    using (FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\patch\\" + patch.Key + ".patch_0", FileMode.Create, FileAccess.Write))
                                    {
                                        compressedMemory.Position = 0;
                                        compressedMemory.CopyTo(file);
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

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewBundle.SelectedNode != null)
            {
                if (!treeViewBundle.SelectedNode.FullPath.Contains('.'))
                {
                    string bundle = treeViewBundle.SelectedNode.FullPath.Split(' ')[0];
                    string bundleFilePath = "";
                    if (treeViewBundle.SelectedNode.Parent != null)
                        bundleFilePath = treeViewBundle.SelectedNode.FullPath.Split(")\\")[1].Replace('\\', '/');

                    OpenFileDialog dialog = new()
                    {
                        Filter = "File to add | *.*"
                    };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        ListViewItem lvi = listViewBundle.Items.Add(bundle);
                        if (bundleFilePath != "")
                            lvi.SubItems.Add(bundleFilePath + "/" + Path.GetFileName(dialog.FileName).Replace("__colon__", ":").Replace(".dds", ".texture"));
                        else
                            lvi.SubItems.Add(Path.GetFileName(dialog.FileName).Replace("__colon__", ":").Replace(".dds", ".texture"));
                        lvi.SubItems.Add(dialog.FileName);
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ulong fileNameHash64 = Murmur.ComputeHash64(Encoding.ASCII.GetBytes(textBoxSearchString.Text));
            ulong fileNameHash32 = fileNameHash64 >> 32;

            var fileNameHash64string = BitConverter.ToString(BitConverter.GetBytes(fileNameHash64)).Replace("-", " ");
            var fileNameHash32string = BitConverter.ToString(BitConverter.GetBytes((uint)fileNameHash32)).Replace("-", " ");

            richTextBox64string.Text = fileNameHash64.ToString("X");
            richTextBox32string.Text = fileNameHash32.ToString("X");

            richTextBoxHEX64.Text = fileNameHash64string;
            richTextBoxHEX32.Text = fileNameHash32string;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSearchHEX.Text.All("0123456789abcdefABCDEF".Contains) && textBoxSearchHEX.Text.Length is > 0 and <= 16)
            {
                ulong hashPath = Convert.ToUInt64(textBoxSearchHEX.Text, 16);
                string filePath;
                if (hashDict.TryGetValue(hashPath, out filePath))
                {
                    textBoxSearchString.Text = filePath;
                }
                else
                {
                    textBoxSearchString.Text = "unkown name";
                }
            }
            else if (textBoxSearchHEX.Text.All("0123456789abcdefABCDEF ".Contains) && textBoxSearchHEX.Text.Length is > 0 and <= 11)
            {
                uint hashPath = Convert.ToUInt32(textBoxSearchHEX.Text.Replace(" ", ""), 16);
                uint reverseHashPath = BinaryPrimitives.ReverseEndianness(hashPath);
                string filePath;
                if (hashDict32.TryGetValue(reverseHashPath, out filePath))
                {
                    textBoxSearchString.Text = filePath;
                }
                else
                {
                    textBoxSearchString.Text = "unkown name";
                }
            }
            else if (textBoxSearchHEX.Text.All("0123456789abcdefABCDEF ".Contains) && textBoxSearchHEX.Text.Length is > 0 and <= 23)
            {
                ulong hashPath = Convert.ToUInt64(textBoxSearchHEX.Text.Replace(" ", ""), 16);
                var reverseHashPath = BinaryPrimitives.ReverseEndianness(hashPath);
                string filePath;
                if (hashDict.TryGetValue(reverseHashPath, out filePath))
                {
                    textBoxSearchString.Text = filePath;
                }
                else
                {
                    textBoxSearchString.Text = "unkown name";
                }
            }
        }
        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            
            string filterText = textBoxFilter.Text.Trim();

            if (string.IsNullOrEmpty(filterText))
            {
                treeViewBundle.Nodes.Clear();
                CloneTree(treeViewBundle.Nodes, originalTreeView.Nodes);
                //ExpandAllNodes(treeViewBundle);
            }
            else
            {
                FilterTreeView(filterText);
            }
        }
        private TreeView originalTreeView;
        private void FilterTreeView(string filterText)
        {
            treeViewBundle.Nodes.Clear();

            foreach (TreeNode originalRoot in originalTreeView.Nodes)
            {
                TreeNode filteredRoot = FilterNode(originalRoot, filterText);
                if (filteredRoot != null)
                {
                    treeViewBundle.Nodes.Add(filteredRoot);
                }
            }

            ExpandAllNodes(treeViewBundle);
        }
        private TreeNode FilterNode(TreeNode sourceNode, string filterText)
        {
            bool nodeMatches = sourceNode.Text.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;

            List<TreeNode> matchingChildren = new List<TreeNode>();
            foreach (TreeNode child in sourceNode.Nodes)
            {
                TreeNode filteredChild = FilterNode(child, filterText);
                if (filteredChild != null)
                {
                    matchingChildren.Add(filteredChild);
                }
            }

            if (nodeMatches || matchingChildren.Count > 0)
            {
                TreeNode result = (TreeNode)sourceNode.Clone();
                result.Nodes.Clear();

                foreach (TreeNode child in matchingChildren)
                {
                    result.Nodes.Add(child);
                }

                return result;
            }

            return null;
        }
        private void CloneTree(TreeNodeCollection destination, TreeNodeCollection source)
        {
            foreach (TreeNode node in source)
            {
                TreeNode clonedNode = (TreeNode)node.Clone();
                destination.Add(clonedNode);
            }
        }
        private void ExpandAllNodes(TreeView treeView)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                node.Expand();
            }
        }
    }
}
