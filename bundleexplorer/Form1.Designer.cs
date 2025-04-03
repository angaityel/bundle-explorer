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
            listViewBundle = new ListView();
            columnBundle = new ColumnHeader();
            columnBundleFile = new ColumnHeader();
            columnReplace = new ColumnHeader();
            buttonCreatePatch = new Button();
            buttonExtract = new Button();
            checkBoxExtact = new CheckBox();
            contextMenuStripReplaceList = new ContextMenuStrip(components);
            deleteToolStripMenuItem = new ToolStripMenuItem();
            addNewToolStripMenuItem = new ToolStripMenuItem();
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
            treeViewBundle.Location = new Point(12, 12);
            treeViewBundle.Name = "treeViewBundle";
            treeViewBundle.Size = new Size(476, 426);
            treeViewBundle.TabIndex = 1;
            treeViewBundle.NodeMouseClick += treeViewBundle_NodeMouseClick;
            treeViewBundle.MouseDown += treeViewBundle_MouseDown;
            // 
            // contextMenuStripBundleTree
            // 
            contextMenuStripBundleTree.Items.AddRange(new ToolStripItem[] { replaceToolStripMenuItem, addNewToolStripMenuItem });
            contextMenuStripBundleTree.Name = "contextMenuStrip1";
            contextMenuStripBundleTree.Size = new Size(181, 70);
            // 
            // replaceToolStripMenuItem
            // 
            replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            replaceToolStripMenuItem.Size = new Size(180, 22);
            replaceToolStripMenuItem.Text = "Replace";
            replaceToolStripMenuItem.Click += replaceToolStripMenuItem_Click;
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
            // addNewToolStripMenuItem
            // 
            addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
            addNewToolStripMenuItem.Size = new Size(180, 22);
            addNewToolStripMenuItem.Text = "Add new";
            addNewToolStripMenuItem.Click += addNewToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1198, 450);
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
    }
}
