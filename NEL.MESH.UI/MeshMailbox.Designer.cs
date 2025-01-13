// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Drawing;
using System.Windows.Forms;

namespace NEL.MESH.UI
{
    partial class MeshMailbox
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
            groupBox1 = new GroupBox();
            label14 = new Label();
            label13 = new Label();
            cbEnvironments = new ComboBox();
            label12 = new Label();
            cbApplications = new ComboBox();
            cbMailboxes = new ComboBox();
            groupBox2 = new GroupBox();
            txtSubject = new TextBox();
            label5 = new Label();
            txtLocalId = new TextBox();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            btnSendMessage = new Button();
            txtMessage = new TextBox();
            label6 = new Label();
            tabPage2 = new TabPage();
            btnSendFile = new Button();
            txtFileName = new TextBox();
            label9 = new Label();
            btnFindFile = new Button();
            txtFileLocation = new TextBox();
            label8 = new Label();
            txtChunkSize = new TextBox();
            label7 = new Label();
            label4 = new Label();
            txtWorkflowId = new TextBox();
            label3 = new Label();
            txtTo = new TextBox();
            label2 = new Label();
            txtFrom = new TextBox();
            label1 = new Label();
            groupBox3 = new GroupBox();
            btnTrackMessage = new Button();
            btnClearOutbox = new Button();
            lbOutbox = new ListBox();
            groupBox4 = new GroupBox();
            btnGetMessages = new Button();
            btnValidateMailbox = new Button();
            btnAckAllMessages = new Button();
            btnAckMessage = new Button();
            btnDownloadMessage = new Button();
            btnViewMessage = new Button();
            lbInbox = new ListBox();
            groupBox5 = new GroupBox();
            label11 = new Label();
            txtContent = new TextBox();
            txtHeaders = new TextBox();
            label10 = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label14);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(cbEnvironments);
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(cbApplications);
            groupBox1.Controls.Add(cbMailboxes);
            groupBox1.Location = new Point(7, 8);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(280, 156);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Mailbox";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(9, 107);
            label14.Name = "label14";
            label14.Size = new Size(42, 15);
            label14.TabIndex = 5;
            label14.Text = "Name:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(9, 63);
            label13.Name = "label13";
            label13.Size = new Size(78, 15);
            label13.TabIndex = 4;
            label13.Text = "Environment:";
            // 
            // cbEnvironments
            // 
            cbEnvironments.FormattingEnabled = true;
            cbEnvironments.Location = new Point(9, 81);
            cbEnvironments.Name = "cbEnvironments";
            cbEnvironments.Size = new Size(265, 23);
            cbEnvironments.TabIndex = 3;
            cbEnvironments.SelectedIndexChanged += cbEnvironments_SelectedIndexChanged;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(8, 19);
            label12.Name = "label12";
            label12.Size = new Size(71, 15);
            label12.TabIndex = 2;
            label12.Text = "Application:";
            // 
            // cbApplications
            // 
            cbApplications.FormattingEnabled = true;
            cbApplications.Location = new Point(8, 37);
            cbApplications.Name = "cbApplications";
            cbApplications.Size = new Size(265, 23);
            cbApplications.TabIndex = 1;
            cbApplications.SelectedIndexChanged += cbApplications_SelectedIndexChanged;
            // 
            // cbMailboxes
            // 
            cbMailboxes.FormattingEnabled = true;
            cbMailboxes.Location = new Point(9, 125);
            cbMailboxes.Name = "cbMailboxes";
            cbMailboxes.Size = new Size(265, 23);
            cbMailboxes.TabIndex = 0;
            cbMailboxes.SelectedIndexChanged += cbMailboxes_SelectedIndexChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtSubject);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(txtLocalId);
            groupBox2.Controls.Add(tabControl1);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(txtWorkflowId);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(txtTo);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(txtFrom);
            groupBox2.Controls.Add(label1);
            groupBox2.Location = new Point(7, 170);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(280, 735);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Message:";
            // 
            // txtSubject
            // 
            txtSubject.Location = new Point(6, 223);
            txtSubject.Name = "txtSubject";
            txtSubject.Size = new Size(267, 23);
            txtSubject.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 203);
            label5.Name = "label5";
            label5.Size = new Size(49, 15);
            label5.TabIndex = 8;
            label5.Text = "Subject:";
            // 
            // txtLocalId
            // 
            txtLocalId.Location = new Point(6, 177);
            txtLocalId.Name = "txtLocalId";
            txtLocalId.Size = new Size(267, 23);
            txtLocalId.TabIndex = 7;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(8, 262);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(265, 467);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnSendMessage);
            tabPage1.Controls.Add(txtMessage);
            tabPage1.Controls.Add(label6);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(257, 439);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Send Message";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(6, 398);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(245, 35);
            btnSendMessage.TabIndex = 12;
            btnSendMessage.Text = "Send Message";
            btnSendMessage.UseVisualStyleBackColor = true;
            btnSendMessage.Click += btnSendMessage_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(6, 21);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(245, 371);
            txtMessage.TabIndex = 11;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 3);
            label6.Name = "label6";
            label6.Size = new Size(56, 15);
            label6.TabIndex = 10;
            label6.Text = "Message:";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(btnSendFile);
            tabPage2.Controls.Add(txtFileName);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(btnFindFile);
            tabPage2.Controls.Add(txtFileLocation);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(txtChunkSize);
            tabPage2.Controls.Add(label7);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(257, 439);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Send File";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnSendFile
            // 
            btnSendFile.Location = new Point(6, 174);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(245, 30);
            btnSendFile.TabIndex = 18;
            btnSendFile.Text = "Send File";
            btnSendFile.UseVisualStyleBackColor = true;
            btnSendFile.Click += btnSendFile_Click;
            // 
            // txtFileName
            // 
            txtFileName.Location = new Point(6, 145);
            txtFileName.Name = "txtFileName";
            txtFileName.Size = new Size(245, 23);
            txtFileName.TabIndex = 17;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(6, 127);
            label9.Name = "label9";
            label9.Size = new Size(63, 15);
            label9.TabIndex = 16;
            label9.Text = "File Name:";
            // 
            // btnFindFile
            // 
            btnFindFile.Location = new Point(6, 94);
            btnFindFile.Name = "btnFindFile";
            btnFindFile.Size = new Size(245, 30);
            btnFindFile.TabIndex = 15;
            btnFindFile.Text = "Find File";
            btnFindFile.UseVisualStyleBackColor = true;
            btnFindFile.Click += btnFindFile_Click;
            // 
            // txtFileLocation
            // 
            txtFileLocation.Location = new Point(6, 65);
            txtFileLocation.Name = "txtFileLocation";
            txtFileLocation.ReadOnly = true;
            txtFileLocation.Size = new Size(245, 23);
            txtFileLocation.TabIndex = 14;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 47);
            label8.Name = "label8";
            label8.Size = new Size(46, 15);
            label8.TabIndex = 13;
            label8.Text = "Source:";
            // 
            // txtChunkSize
            // 
            txtChunkSize.Location = new Point(6, 21);
            txtChunkSize.Name = "txtChunkSize";
            txtChunkSize.ReadOnly = true;
            txtChunkSize.Size = new Size(245, 23);
            txtChunkSize.TabIndex = 12;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 3);
            label7.Name = "label7";
            label7.Size = new Size(97, 15);
            label7.TabIndex = 10;
            label7.Text = "Chunk Size (Mb):";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 157);
            label4.Name = "label4";
            label4.Size = new Size(51, 15);
            label4.TabIndex = 6;
            label4.Text = "Local Id:";
            // 
            // txtWorkflowId
            // 
            txtWorkflowId.Location = new Point(6, 131);
            txtWorkflowId.Name = "txtWorkflowId";
            txtWorkflowId.Size = new Size(267, 23);
            txtWorkflowId.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 111);
            label3.Name = "label3";
            label3.Size = new Size(74, 15);
            label3.TabIndex = 4;
            label3.Text = "Workflow Id:";
            // 
            // txtTo
            // 
            txtTo.Location = new Point(6, 85);
            txtTo.Name = "txtTo";
            txtTo.Size = new Size(267, 23);
            txtTo.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 65);
            label2.Name = "label2";
            label2.Size = new Size(22, 15);
            label2.TabIndex = 2;
            label2.Text = "To:";
            // 
            // txtFrom
            // 
            txtFrom.Location = new Point(6, 39);
            txtFrom.Name = "txtFrom";
            txtFrom.ReadOnly = true;
            txtFrom.Size = new Size(267, 23);
            txtFrom.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 0;
            label1.Text = "From:";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnTrackMessage);
            groupBox3.Controls.Add(btnClearOutbox);
            groupBox3.Controls.Add(lbOutbox);
            groupBox3.Location = new Point(293, 8);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(412, 341);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Mesh Outbox";
            // 
            // btnTrackMessage
            // 
            btnTrackMessage.Location = new Point(6, 269);
            btnTrackMessage.Name = "btnTrackMessage";
            btnTrackMessage.Size = new Size(400, 30);
            btnTrackMessage.TabIndex = 21;
            btnTrackMessage.Text = "Track Message";
            btnTrackMessage.UseVisualStyleBackColor = true;
            btnTrackMessage.Click += btnTrackMessage_Click;
            // 
            // btnClearOutbox
            // 
            btnClearOutbox.Location = new Point(6, 302);
            btnClearOutbox.Name = "btnClearOutbox";
            btnClearOutbox.Size = new Size(400, 30);
            btnClearOutbox.TabIndex = 19;
            btnClearOutbox.Text = "Clear Outbox";
            btnClearOutbox.UseVisualStyleBackColor = true;
            btnClearOutbox.Click += btnClearOutbox_Click;
            // 
            // lbOutbox
            // 
            lbOutbox.FormattingEnabled = true;
            lbOutbox.ItemHeight = 15;
            lbOutbox.Location = new Point(6, 19);
            lbOutbox.Name = "lbOutbox";
            lbOutbox.Size = new Size(400, 244);
            lbOutbox.TabIndex = 0;
            lbOutbox.DoubleClick += lbOutbox_DoubleClick;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btnGetMessages);
            groupBox4.Controls.Add(btnValidateMailbox);
            groupBox4.Controls.Add(btnAckAllMessages);
            groupBox4.Controls.Add(btnAckMessage);
            groupBox4.Controls.Add(btnDownloadMessage);
            groupBox4.Controls.Add(btnViewMessage);
            groupBox4.Controls.Add(lbInbox);
            groupBox4.Location = new Point(293, 363);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(412, 542);
            groupBox4.TabIndex = 3;
            groupBox4.TabStop = false;
            groupBox4.Text = "Mesh Inbox";
            // 
            // btnGetMessages
            // 
            btnGetMessages.Location = new Point(211, 22);
            btnGetMessages.Name = "btnGetMessages";
            btnGetMessages.Size = new Size(195, 30);
            btnGetMessages.TabIndex = 24;
            btnGetMessages.Text = "Get Messages From Inbox";
            btnGetMessages.UseVisualStyleBackColor = true;
            btnGetMessages.Click += btnGetMessages_Click;
            // 
            // btnValidateMailbox
            // 
            btnValidateMailbox.Location = new Point(6, 22);
            btnValidateMailbox.Name = "btnValidateMailbox";
            btnValidateMailbox.Size = new Size(195, 30);
            btnValidateMailbox.TabIndex = 23;
            btnValidateMailbox.Text = "Validate Mailbox / Hand Shake";
            btnValidateMailbox.UseVisualStyleBackColor = true;
            btnValidateMailbox.Click += btnValidateMailbox_Click;
            // 
            // btnAckAllMessages
            // 
            btnAckAllMessages.Location = new Point(6, 506);
            btnAckAllMessages.Name = "btnAckAllMessages";
            btnAckAllMessages.Size = new Size(400, 30);
            btnAckAllMessages.TabIndex = 22;
            btnAckAllMessages.Text = "Ack All Messages";
            btnAckAllMessages.UseVisualStyleBackColor = true;
            btnAckAllMessages.Click += btnAckAllMessages_Click;
            // 
            // btnAckMessage
            // 
            btnAckMessage.Location = new Point(6, 470);
            btnAckMessage.Name = "btnAckMessage";
            btnAckMessage.Size = new Size(400, 30);
            btnAckMessage.TabIndex = 21;
            btnAckMessage.Text = "Ack Message";
            btnAckMessage.UseVisualStyleBackColor = true;
            btnAckMessage.Click += btnAckMessage_Click;
            // 
            // btnDownloadMessage
            // 
            btnDownloadMessage.Location = new Point(6, 434);
            btnDownloadMessage.Name = "btnDownloadMessage";
            btnDownloadMessage.Size = new Size(400, 30);
            btnDownloadMessage.TabIndex = 20;
            btnDownloadMessage.Text = "Download Message";
            btnDownloadMessage.UseVisualStyleBackColor = true;
            btnDownloadMessage.Click += btnDownloadMessage_Click;
            // 
            // btnViewMessage
            // 
            btnViewMessage.Location = new Point(6, 398);
            btnViewMessage.Name = "btnViewMessage";
            btnViewMessage.Size = new Size(400, 30);
            btnViewMessage.TabIndex = 19;
            btnViewMessage.Text = "View Message";
            btnViewMessage.UseVisualStyleBackColor = true;
            btnViewMessage.Click += btnViewMessage_Click;
            // 
            // lbInbox
            // 
            lbInbox.FormattingEnabled = true;
            lbInbox.ItemHeight = 15;
            lbInbox.Location = new Point(6, 67);
            lbInbox.Name = "lbInbox";
            lbInbox.Size = new Size(400, 319);
            lbInbox.TabIndex = 0;
            lbInbox.DoubleClick += lbInbox_DoubleClick;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(label11);
            groupBox5.Controls.Add(txtContent);
            groupBox5.Controls.Add(txtHeaders);
            groupBox5.Controls.Add(label10);
            groupBox5.Location = new Point(711, 8);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(808, 897);
            groupBox5.TabIndex = 4;
            groupBox5.TabStop = false;
            groupBox5.Text = "Message Info";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(6, 463);
            label11.Name = "label11";
            label11.Size = new Size(53, 15);
            label11.TabIndex = 14;
            label11.Text = "Content:";
            // 
            // txtContent
            // 
            txtContent.Location = new Point(6, 481);
            txtContent.Multiline = true;
            txtContent.Name = "txtContent";
            txtContent.ScrollBars = ScrollBars.Both;
            txtContent.Size = new Size(796, 410);
            txtContent.TabIndex = 13;
            // 
            // txtHeaders
            // 
            txtHeaders.Location = new Point(6, 37);
            txtHeaders.Multiline = true;
            txtHeaders.Name = "txtHeaders";
            txtHeaders.ScrollBars = ScrollBars.Both;
            txtHeaders.Size = new Size(796, 410);
            txtHeaders.TabIndex = 12;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(6, 19);
            label10.Name = "label10";
            label10.Size = new Size(53, 15);
            label10.TabIndex = 1;
            label10.Text = "Headers:";
            // 
            // MeshMailbox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1526, 911);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "MeshMailbox";
            Text = "MESH Mailbox";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ComboBox cbMailboxes;
        private GroupBox groupBox2;
        private TextBox txtTo;
        private Label label2;
        private TextBox txtFrom;
        private Label label1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox txtWorkflowId;
        private Label label3;
        private TextBox txtSubject;
        private Label label5;
        private TextBox txtLocalId;
        private TextBox txtMessage;
        private Label label6;
        private Label label4;
        private Button btnSendMessage;
        private Label label7;
        private TextBox txtChunkSize;
        private TextBox txtFileLocation;
        private Label label8;
        private Button btnFindFile;
        private Button btnSendFile;
        private TextBox txtFileName;
        private Label label9;
        private GroupBox groupBox3;
        private Button btnClearOutbox;
        private ListBox lbOutbox;
        private GroupBox groupBox4;
        private Button btnViewMessage;
        private ListBox lbInbox;
        private Button btnAckAllMessages;
        private Button btnAckMessage;
        private Button btnDownloadMessage;
        private GroupBox groupBox5;
        private Label label11;
        private TextBox txtContent;
        private TextBox txtHeaders;
        private Label label10;
        private Label label14;
        private Label label13;
        private ComboBox cbEnvironments;
        private Label label12;
        private ComboBox cbApplications;
        private Button btnGetMessages;
        private Button btnValidateMailbox;
        private Button btnTrackMessage;
    }
}
