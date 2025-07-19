namespace bundleexplorer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            buttonOpen = new Button();
            treeViewBundle = new TreeView();
            contextMenuStripBundleTree = new ContextMenuStrip(components);
            replaceToolStripMenuItem = new ToolStripMenuItem();
            addNewToolStripMenuItem = new ToolStripMenuItem();
            listViewBundle = new ListView();
            columnBundle = new ColumnHeader();
            columnBundleFile = new ColumnHeader();
            columnReplace = new ColumnHeader();
            buttonCreatePatch = new Button();
            buttonExtract = new Button();
            checkBoxExtact = new CheckBox();
            contextMenuStripReplaceList = new ContextMenuStrip(components);
            deleteToolStripMenuItem = new ToolStripMenuItem();
            textBoxSearchString = new TextBox();
            richTextBox64string = new RichTextBox();
            richTextBox32string = new RichTextBox();
            richTextBoxHEX64 = new RichTextBox();
            richTextBoxHEX32 = new RichTextBox();
            label64string = new Label();
            label32string = new Label();
            labelHEX64 = new Label();
            labelHEX32 = new Label();
            labelSearchHEX = new Label();
            textBoxSearchHEX = new TextBox();
            labelSearchString = new Label();
            contextMenuStripBundleTree.SuspendLayout();
            contextMenuStripReplaceList.SuspendLayout();
            SuspendLayout();
            // 
            // buttonOpen
            // 
            buttonOpen.Location = new Point(494, 12);
            buttonOpen.Name = "buttonOpen";
            buttonOpen.Size = new Size(75, 23);
            buttonOpen.TabIndex = 0;
            buttonOpen.Text = "Open";
            buttonOpen.UseVisualStyleBackColor = true;
            buttonOpen.Click += buttonOpen_Click;
            // 
            // treeViewBundle
            // 
            treeViewBundle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            treeViewBundle.ContextMenuStrip = contextMenuStripBundleTree;
            treeViewBundle.Location = new Point(12, 131);
            treeViewBundle.Name = "treeViewBundle";
            treeViewBundle.Size = new Size(476, 307);
            treeViewBundle.TabIndex = 1;
            treeViewBundle.NodeMouseClick += treeViewBundle_NodeMouseClick;
            treeViewBundle.MouseDown += treeViewBundle_MouseDown;
            // 
            // contextMenuStripBundleTree
            // 
            contextMenuStripBundleTree.Items.AddRange(new ToolStripItem[] { replaceToolStripMenuItem, addNewToolStripMenuItem });
            contextMenuStripBundleTree.Name = "contextMenuStrip1";
            contextMenuStripBundleTree.Size = new Size(122, 48);
            // 
            // replaceToolStripMenuItem
            // 
            replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            replaceToolStripMenuItem.Size = new Size(121, 22);
            replaceToolStripMenuItem.Text = "Replace";
            replaceToolStripMenuItem.Click += replaceToolStripMenuItem_Click;
            // 
            // addNewToolStripMenuItem
            // 
            addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
            addNewToolStripMenuItem.Size = new Size(121, 22);
            addNewToolStripMenuItem.Text = "Add new";
            addNewToolStripMenuItem.Click += addNewToolStripMenuItem_Click;
            // 
            // listViewBundle
            // 
            listViewBundle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listViewBundle.Columns.AddRange(new ColumnHeader[] { columnBundle, columnBundleFile, columnReplace });
            listViewBundle.FullRowSelect = true;
            listViewBundle.Location = new Point(494, 41);
            listViewBundle.Name = "listViewBundle";
            listViewBundle.Size = new Size(692, 397);
            listViewBundle.TabIndex = 7;
            listViewBundle.UseCompatibleStateImageBehavior = false;
            listViewBundle.View = View.Details;
            listViewBundle.MouseClick += listViewBundle_MouseClick;
            listViewBundle.MouseDown += listViewBundle_MouseDown;
            // 
            // columnBundle
            // 
            columnBundle.Text = "Bundle";
            columnBundle.Width = 150;
            // 
            // columnBundleFile
            // 
            columnBundleFile.Text = "File";
            columnBundleFile.Width = 150;
            // 
            // columnReplace
            // 
            columnReplace.Text = "Replace with";
            columnReplace.Width = 500;
            // 
            // buttonCreatePatch
            // 
            buttonCreatePatch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonCreatePatch.Enabled = false;
            buttonCreatePatch.Location = new Point(1076, 12);
            buttonCreatePatch.Name = "buttonCreatePatch";
            buttonCreatePatch.Size = new Size(110, 23);
            buttonCreatePatch.TabIndex = 8;
            buttonCreatePatch.Text = "Create patch";
            buttonCreatePatch.UseVisualStyleBackColor = true;
            buttonCreatePatch.Click += button2_Click;
            // 
            // buttonExtract
            // 
            buttonExtract.Enabled = false;
            buttonExtract.Location = new Point(635, 12);
            buttonExtract.Name = "buttonExtract";
            buttonExtract.Size = new Size(82, 23);
            buttonExtract.TabIndex = 9;
            buttonExtract.Text = "Extract all";
            buttonExtract.UseVisualStyleBackColor = true;
            buttonExtract.Click += button1_Click;
            // 
            // checkBoxExtact
            // 
            checkBoxExtact.AutoSize = true;
            checkBoxExtact.Enabled = false;
            checkBoxExtact.Location = new Point(732, 15);
            checkBoxExtact.Name = "checkBoxExtact";
            checkBoxExtact.Size = new Size(244, 19);
            checkBoxExtact.TabIndex = 10;
            checkBoxExtact.Text = "Extract each bundle into a separate folder";
            checkBoxExtact.UseVisualStyleBackColor = true;
            // 
            // contextMenuStripReplaceList
            // 
            contextMenuStripReplaceList.Items.AddRange(new ToolStripItem[] { deleteToolStripMenuItem });
            contextMenuStripReplaceList.Name = "contextMenuStripReplaceList";
            contextMenuStripReplaceList.Size = new Size(108, 26);
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(107, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // textBoxSearchString
            // 
            textBoxSearchString.Location = new Point(83, 12);
            textBoxSearchString.Name = "textBoxSearchString";
            textBoxSearchString.Size = new Size(405, 23);
            textBoxSearchString.TabIndex = 11;
            textBoxSearchString.TextChanged += textBox1_TextChanged;
            // 
            // richTextBox64string
            // 
            richTextBox64string.Location = new Point(83, 41);
            richTextBox64string.Name = "richTextBox64string";
            richTextBox64string.ReadOnly = true;
            richTextBox64string.Size = new Size(160, 23);
            richTextBox64string.TabIndex = 12;
            richTextBox64string.Text = "";
            // 
            // richTextBox32string
            // 
            richTextBox32string.Location = new Point(311, 41);
            richTextBox32string.Name = "richTextBox32string";
            richTextBox32string.ReadOnly = true;
            richTextBox32string.Size = new Size(129, 23);
            richTextBox32string.TabIndex = 13;
            richTextBox32string.Text = "";
            // 
            // richTextBoxHEX64
            // 
            richTextBoxHEX64.Location = new Point(83, 70);
            richTextBoxHEX64.Name = "richTextBoxHEX64";
            richTextBoxHEX64.ReadOnly = true;
            richTextBoxHEX64.Size = new Size(160, 23);
            richTextBoxHEX64.TabIndex = 14;
            richTextBoxHEX64.Text = "";
            // 
            // richTextBoxHEX32
            // 
            richTextBoxHEX32.Location = new Point(311, 70);
            richTextBoxHEX32.Name = "richTextBoxHEX32";
            richTextBoxHEX32.ReadOnly = true;
            richTextBoxHEX32.Size = new Size(129, 23);
            richTextBoxHEX32.TabIndex = 15;
            richTextBoxHEX32.Text = "";
            // 
            // label64string
            // 
            label64string.AutoSize = true;
            label64string.Location = new Point(26, 44);
            label64string.Name = "label64string";
            label64string.Size = new Size(53, 15);
            label64string.TabIndex = 16;
            label64string.Text = "String 64";
            // 
            // label32string
            // 
            label32string.AutoSize = true;
            label32string.Location = new Point(252, 44);
            label32string.Name = "label32string";
            label32string.Size = new Size(53, 15);
            label32string.TabIndex = 17;
            label32string.Text = "String 32";
            // 
            // labelHEX64
            // 
            labelHEX64.AutoSize = true;
            labelHEX64.Location = new Point(35, 73);
            labelHEX64.Name = "labelHEX64";
            labelHEX64.Size = new Size(44, 15);
            labelHEX64.TabIndex = 18;
            labelHEX64.Text = "HEX 64";
            // 
            // labelHEX32
            // 
            labelHEX32.AutoSize = true;
            labelHEX32.Location = new Point(261, 73);
            labelHEX32.Name = "labelHEX32";
            labelHEX32.Size = new Size(44, 15);
            labelHEX32.TabIndex = 19;
            labelHEX32.Text = "HEX 32";
            // 
            // labelSearchHEX
            // 
            labelSearchHEX.AutoSize = true;
            labelSearchHEX.Location = new Point(12, 105);
            labelSearchHEX.Name = "labelSearchHEX";
            labelSearchHEX.Size = new Size(67, 15);
            labelSearchHEX.TabIndex = 21;
            labelSearchHEX.Text = "Search HEX";
            // 
            // textBoxSearchHEX
            // 
            textBoxSearchHEX.Location = new Point(83, 102);
            textBoxSearchHEX.Name = "textBoxSearchHEX";
            textBoxSearchHEX.Size = new Size(405, 23);
            textBoxSearchHEX.TabIndex = 22;
            textBoxSearchHEX.TextChanged += textBox2_TextChanged;
            // 
            // labelSearchString
            // 
            labelSearchString.AutoSize = true;
            labelSearchString.Location = new Point(4, 15);
            labelSearchString.Name = "labelSearchString";
            labelSearchString.Size = new Size(75, 15);
            labelSearchString.TabIndex = 23;
            labelSearchString.Text = "Search string";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1198, 450);
            Controls.Add(labelSearchString);
            Controls.Add(textBoxSearchHEX);
            Controls.Add(labelSearchHEX);
            Controls.Add(labelHEX32);
            Controls.Add(labelHEX64);
            Controls.Add(label32string);
            Controls.Add(label64string);
            Controls.Add(richTextBoxHEX32);
            Controls.Add(richTextBoxHEX64);
            Controls.Add(richTextBox32string);
            Controls.Add(richTextBox64string);
            Controls.Add(textBoxSearchString);
            Controls.Add(checkBoxExtact);
            Controls.Add(buttonExtract);
            Controls.Add(buttonCreatePatch);
            Controls.Add(listViewBundle);
            Controls.Add(treeViewBundle);
            Controls.Add(buttonOpen);
            Name = "Form1";
            Text = "Bundle Explorer";
            contextMenuStripBundleTree.ResumeLayout(false);
            contextMenuStripReplaceList.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonOpen;
        private TreeView treeViewBundle;
        private ListView listViewBundle;
        private ColumnHeader columnBundle;
        private ColumnHeader columnBundleFile;
        private ColumnHeader columnReplace;
        private ContextMenuStrip contextMenuStripBundleTree;
        private ToolStripMenuItem replaceToolStripMenuItem;
        private Button buttonCreatePatch;
        private Button buttonExtract;
        private CheckBox checkBoxExtact;
        private ContextMenuStrip contextMenuStripReplaceList;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem addNewToolStripMenuItem;
        private TextBox textBoxSearchString;
        private RichTextBox richTextBox64string;
        private RichTextBox richTextBox32string;
        private RichTextBox richTextBoxHEX64;
        private RichTextBox richTextBoxHEX32;
        private Label label64string;
        private Label label32string;
        private Label labelHEX64;
        private Label labelHEX32;
        private Label labelSearchHEX;
        private TextBox textBoxSearchHEX;
        private Label labelSearchString;
    }
}
