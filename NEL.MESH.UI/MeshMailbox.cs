// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Force.DeepCloner;
using Microsoft.Extensions.Configuration;
using NEL.MESH.Clients;
using NEL.MESH.Models.Configurations;
using NEL.MESH.UI.Models;

namespace NEL.MESH.UI
{
    public partial class MeshMailbox : Form
    {
        private List<Mailbox> mailboxes;
        private List<string> applications;
        private List<string> environments;

        private List<string> filteredEnvironments;
        private List<Mailbox> filteredMailboxes;

        private List<MeshCertificates> meshCertificates;
        private MeshConfig meshConfig;
        private MeshClient meshClient;
        private string selectedApplication = "...";
        private string selectedEnvironment = "...";
        private Mailbox selectedMailbox;
        private bool isProcessing = false;

        public MeshMailbox(IConfiguration configuration)
        {
            InitializeComponent();

            mailboxes =
                configuration.GetSection("Mailboxes").Get<List<Mailbox>>();

            meshConfig =
                configuration.GetSection("MeshConfiguration").Get<MeshConfig>();

            meshCertificates =
                configuration.GetSection("MeshCertificates").Get<List<MeshCertificates>>();

            applications = ["ALL", .. mailboxes.Select(mailbox => mailbox.Application).Distinct().ToList()];
            environments = ["ALL", .. mailboxes.Select(mailbox => mailbox.Environment).Distinct().ToList()];
            filteredEnvironments = environments.DeepClone();

            FilterMailboxes();
        }

        private void FilterMailboxes(string application = "", string environment = "")
        {
            isProcessing = true;

            application = application ?? "ALL";
            environment = environment ?? "ALL";
            filteredEnvironments = environments.DeepClone();
            bool changes = false;

            if (application != selectedApplication)
            {
                changes = true;

                selectedApplication = application;
                cbApplications.DataSource = applications;
                cbApplications.SelectedItem = application;
            }

            if (environment != selectedEnvironment || changes)
            {
                changes = true;

                var mailboxList = mailboxes.DeepClone();
                if (selectedApplication != "ALL")
                {
                    mailboxList = mailboxList
                        .Where(mailbox => mailbox.Application == selectedApplication).ToList();
                }

                filteredEnvironments = ["ALL", .. mailboxList.Select(mailbox => mailbox.Environment).Distinct().ToList()];
                selectedEnvironment = environment;
                cbEnvironments.DataSource = filteredEnvironments;
                cbEnvironments.SelectedItem = environment;
            }

            if (changes)
            {
                filteredMailboxes = mailboxes.DeepClone();

                if (selectedApplication != "ALL")
                {
                    filteredMailboxes = filteredMailboxes.Where(mailbox => mailbox.Application == selectedApplication).ToList();
                }

                if (selectedEnvironment != "ALL")
                {
                    filteredMailboxes = filteredMailboxes.Where(mailbox => mailbox.Environment == selectedEnvironment).ToList();
                }

                var defaultMailbox = filteredMailboxes
                    .OrderByDescending(mailbox => mailbox.DefaultMailbox)
                    .ThenBy(mailbox => mailbox.Name).FirstOrDefault();

                if (filteredMailboxes.Any())
                {
                    cbMailboxes.DataSource = filteredMailboxes;
                    cbMailboxes.DisplayMember = "Name";
                    cbMailboxes.SelectedItem = defaultMailbox;
                    InitializeMailbox(defaultMailbox);
                }
                else
                {
                    cbMailboxes.DataSource = new List<Mailbox>();
                    cbMailboxes.DisplayMember = "Name";
                }
            }

            isProcessing = false;
        }

        private void InitializeMailbox(Mailbox mailbox)
        {
            if (mailbox != selectedMailbox)
            {
                selectedMailbox = mailbox;
                txtWorkflowId.Text = mailbox.WorkflowId;
                txtFrom.Text = mailbox.MailboxId;
                txtTo.Text = mailbox.DestinationMailbox;
                txtLocalId.Text = mailbox.LocalId;
                txtSubject.Text = mailbox.Subject;
                txtMessage.Text = mailbox.Message;
                txtChunkSize.Text = (meshConfig.ChunkSize <= 20 ? meshConfig.ChunkSize : 20).ToString();
                lbOutbox.Items.Clear();
                lbInbox.Items.Clear();
                txtHeaders.Text = string.Empty;
                txtContent.Text = string.Empty;

                var clientSigningCertificate =
                    meshCertificates.First(cert => cert.Environment == mailbox.Environment)
                        .ClientSigningCertificate;

                var tlsRootCertificates = meshCertificates.First(cert => cert.Environment == mailbox.Environment)
                    .TlsRootCertificates;

                var tlsIntermediateCertificates =
                    meshCertificates.First(cert => cert.Environment == mailbox.Environment)
                        .TlsIntermediateCertificates;

                var clientSigningCertificatePassword = string.Empty;

                var meshConfiguration = new MeshConfiguration
                {
                    Url = mailbox.Url,
                    MailboxId = mailbox.MailboxId,
                    Password = mailbox.Password,
                    TlsRootCertificates = GetCertificates(tlsRootCertificates.ToArray()),
                    TlsIntermediateCertificates = GetCertificates(tlsIntermediateCertificates.ToArray()),

                    ClientSigningCertificate =
                        GetPkcs12Certificate(clientSigningCertificate, clientSigningCertificatePassword),

                    SharedKey = mailbox.Key,
                    MaxChunkSizeInMegabytes = meshConfig.ChunkSize,
                    MexClientVersion = meshConfig.MexClientVersion,
                    MexOSName = meshConfig.MexOSName,
                    MexOSVersion = meshConfig.MexOSVersion
                };

                meshClient = new MeshClient(meshConfiguration);
            }
        }

        private static X509Certificate2Collection GetCertificates(params string[] certificates)
        {
            var certificateCollection = new X509Certificate2Collection();

            foreach (string item in certificates)
            {
                certificateCollection.Add(GetPemOrDerCertificate(item));
            }

            return certificateCollection;
        }

        private static X509Certificate2 GetPemOrDerCertificate(string value)
        {
            byte[] certBytes = Convert.FromBase64String(value);
            var certificate = X509CertificateLoader.LoadCertificate(certBytes);

            return certificate;
        }

        private static X509Certificate2 GetPkcs12Certificate(string value, string password = "")
        {
            byte[] certBytes = Convert.FromBase64String(value);
            var certificate = X509CertificateLoader.LoadPkcs12(certBytes, password);

            return certificate;
        }

        private void cbApplications_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                return;
            }

            FilterMailboxes();
        }

        private void cbEnvironments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                return;
            }

            FilterMailboxes();
        }

        private void FilterMailboxes()
        {
            FilterMailboxes(
                application: cbApplications.SelectedItem as string,
                environment: cbEnvironments.SelectedItem as string);
        }

        private void cbMailboxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                return;
            }

            var mailbox = cbMailboxes.SelectedItem as Mailbox;
            InitializeMailbox(mailbox);
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                var message = await meshClient.Mailbox.SendMessageAsync(
                    mexTo: txtTo.Text,
                    mexWorkflowId: txtWorkflowId.Text,
                    fileContent: Encoding.UTF8.GetBytes(txtMessage.Text),
                    mexSubject: txtSubject.Text,
                    mexLocalId: txtLocalId.Text,
                    mexFileName: "message.txt");

                lbOutbox.Items.Add(message.MessageId);
                lbOutbox.SelectedItem = message.MessageId;
            }
            catch (Exception ex)
            {
                txtHeaders.Text = string.Empty;
                txtContent.Text = "Failed to send message..."
                    + Environment.NewLine
                    + ex.Message
                    + Environment.NewLine
                    + ex?.InnerException?.Message;
            }
        }

        private void btnFindFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFileLocation.Text = openFileDialog.FileName;
                FileInfo file = new FileInfo(txtFileLocation.Text);
                txtFileName.Text = file.Name;
            }
        }

        private async void btnSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtFileLocation.Text))
                {
                    var fileBytes = File.ReadAllBytes(txtFileLocation.Text);

                    var message = await meshClient.Mailbox.SendMessageAsync(
                        mexTo: txtTo.Text,
                        mexWorkflowId: txtWorkflowId.Text,
                        fileContent: fileBytes,
                        mexSubject: txtSubject.Text,
                        mexLocalId: txtLocalId.Text,
                        mexFileName: txtFileName.Text);

                    lbOutbox.Items.Add(message.MessageId);
                    lbOutbox.SelectedItem = message.MessageId;
                }
            }
            catch (Exception ex)
            {
                txtHeaders.Text = string.Empty;
                txtContent.Text = "Failed to send file..."
                    + Environment.NewLine
                    + ex.Message
                    + Environment.NewLine
                    + ex?.InnerException?.Message;
            }

        }

        private void btnClearOutbox_Click(object sender, EventArgs e)
        {
            lbOutbox.Items.Clear();
            txtHeaders.Text = string.Empty;
            txtContent.Text = string.Empty;
        }

        private void btnViewMessage_Click(object sender, EventArgs e)
        {
            GetMessageDetail();
        }

        private void lbInbox_DoubleClick(object sender, EventArgs e)
        {
            GetMessageDetail();
        }

        private async void GetMessageDetail()
        {
            try
            {
                if (lbInbox.SelectedItem != null)
                {
                    string messageId = lbInbox.SelectedItem.ToString();
                    var message = await meshClient.Mailbox.RetrieveMessageAsync(messageId);
                    txtHeaders.Text = ConvertHeadersToString(message.Headers);

                    string filename = message.Headers["mex-filename"].FirstOrDefault() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(filename) || filename.EndsWith(".txt"))
                    {
                        string content = Encoding.UTF8.GetString(message.FileContent);

                        int lenghtLimit = 1000000;

                        if (content.Length > lenghtLimit)
                        {
                            content = content.Substring(0, lenghtLimit) + "..." + Environment.NewLine + Environment.NewLine +
                                "*** File Truncated - download the file to get all the content***";
                        }

                        txtContent.Text = content;
                    }
                    else
                    {
                        txtContent.Text = "File content not presentable as text.  File needs to be downloaded.";
                    }
                }
            }
            catch (Exception ex)
            {
                txtHeaders.Text = string.Empty;
                txtContent.Text = "Failed to get message..."
                    + Environment.NewLine
                    + ex.Message
                    + Environment.NewLine
                    + ex?.InnerException?.Message;
            }
        }

        private async void btnTrackMessage_Click(object sender, EventArgs e)
        {
            await TrackMessage();
        }

        private async void lbOutbox_DoubleClick(object sender, EventArgs e)
        {
            await TrackMessage();
        }

        private async Task TrackMessage()
        {
            try
            {
                if (lbOutbox.SelectedItem != null)
                {
                    string messageId = lbOutbox.SelectedItem.ToString();
                    var message = await meshClient.Mailbox.TrackMessageAsync(messageId);
                    txtHeaders.Text = GetTrackingInfoAsString(message.TrackingInfo);
                    txtContent.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                txtHeaders.Text = string.Empty;
                txtContent.Text = "Failed to track message..."
                    + Environment.NewLine
                    + ex.Message
                    + Environment.NewLine
                    + ex?.InnerException?.Message;
            }

        }

        private async void btnDownloadMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbInbox.SelectedItem != null)
                {
                    string messageId = lbInbox.SelectedItem.ToString();
                    var message = await meshClient.Mailbox.RetrieveMessageAsync(messageId);
                    string filename = message.Headers["mex-filename"].FirstOrDefault() ?? string.Empty;

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = filename;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            await fileStream.WriteAsync(message.FileContent, 0, message.FileContent.Length);
                            txtContent.Text = $"File saved to: {saveFileDialog.FileName}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtHeaders.Text = string.Empty;
                txtContent.Text = "Failed to download file..."
                    + Environment.NewLine
                    + ex.Message
                    + Environment.NewLine
                    + ex?.InnerException?.Message;
            }
        }

        private async void btnAckMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbInbox.SelectedItem != null)
                {
                    string messageId = lbInbox.SelectedItem.ToString();
                    await meshClient.Mailbox.AcknowledgeMessageAsync(messageId);
                    txtHeaders.Text = string.Empty;
                    txtContent.Text = string.Empty;
                    await GetMessages();
                }
            }
            catch (Exception ex)
            {
                txtHeaders.Text = string.Empty;
                txtContent.Text = "Failed to Ack message..."
                    + Environment.NewLine
                    + ex.Message
                    + Environment.NewLine
                    + ex?.InnerException?.Message;
            }

        }

        private async void btnAckAllMessages_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string messageId in this.lbInbox.Items)
                {
                    await meshClient.Mailbox.AcknowledgeMessageAsync(messageId);
                }

                txtHeaders.Text = string.Empty;
                txtContent.Text = string.Empty;
                await GetMessages();
            }
            catch (Exception ex)
            {
                txtHeaders.Text = string.Empty;
                txtContent.Text = "Failed to Ack all messages..."
                    + Environment.NewLine
                    + ex.Message
                    + Environment.NewLine
                    + ex?.InnerException?.Message;
            }
        }

        private async void btnGetMessages_Click(object sender, EventArgs e)
        {
            txtHeaders.Text = string.Empty;
            txtContent.Text = string.Empty;
            await GetMessages();
        }

        private async Task GetMessages()
        {
            lbInbox.Items.Clear();
            var messages = await meshClient.Mailbox.RetrieveMessagesAsync();

            foreach (var message in messages)
            {
                lbInbox.Items.Add(message);
            }
        }

        private async void btnValidateMailbox_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await meshClient.Mailbox.HandshakeAsync();

                txtContent.Text = result == true
                    ? "Mailbox validation successful"
                    : "Mailbox validation failed";

                txtHeaders.Text = string.Empty;

            }
            catch (Exception ex)
            {
                txtHeaders.Text = string.Empty;
                txtContent.Text = "Mailbox validation failed"
                    + Environment.NewLine
                    + ex.Message
                    + Environment.NewLine
                    + ex?.InnerException?.Message;
            }
        }

        static string ConvertHeadersToString(Dictionary<string, List<string>> headers)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var kvp in headers)
            {
                string header = kvp.Key;
                List<string> values = kvp.Value;
                string valuesString = string.Join(", ", values);
                string formattedString = $"{header}: {valuesString}";
                sb.AppendLine(formattedString);
            }

            return sb.ToString();
        }

        static string GetTrackingInfoAsString(object obj)
        {
            StringBuilder sb = new StringBuilder();

            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj);
                sb.AppendLine($"{property.Name}: {value}");
            }

            return sb.ToString();
        }


    }
}
