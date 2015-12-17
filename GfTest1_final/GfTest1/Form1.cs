using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.IO;

namespace GfTest1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Retreiving data from App.config file
            string cxn = ConfigurationManager.ConnectionStrings["AzureStorageConn"].ConnectionString; // cxn is the connection string
            string localFolder = ConfigurationManager.AppSettings["sourceFolder"]; // localFolder is the connection string for the folder having files for upload

            // Creating the blob storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cxn);

            // Creating the blob storage client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            // Creating the container
            CloudBlobContainer container = client.GetContainerReference("NAME OF CONTAINER");
            container.CreateIfNotExists();

            //After creating the container we can upload
            string[] entries = Directory.GetFiles(localFolder); // we want all files in the source folder

            foreach (string file in entries)
            {
                string key = Path.GetFileName(file); // get file name with extension
                CloudBlockBlob myblob = container.GetBlockBlobReference(key);

                using (var f = System.IO.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None)) //open file stream with read capability
                {
                    myblob.UploadFromStream(f);
                }
            }

            label2.Text = "Upload Completed!";

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Retreiving data from App.config file
            string cxn = ConfigurationManager.ConnectionStrings["AzureStorageConn"].ConnectionString; // cxn is the connection string
            string localFolder = ConfigurationManager.AppSettings["destFolder"]; // localFolder is the connection string for the folder that will receive the downloads

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cxn);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("NAME OF CONTAINER");

            label4.Text = "Working on it...";

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
                    listBox1.Items.Add ("Block blob of length {0}: {1}" + blob.Properties.Length + blob.Uri);

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);
                    listBox1.Items.Add ( "Page blob of length {0}: {1}"+ pageBlob.Properties.Length+ pageBlob.Uri);
                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                    Console.WriteLine("Directory: {0}", directory.Uri);
                    listBox1.Items.Add ( "Directory: {0}" + directory.Uri);
                }
            }

            label4.Text = "done";

           
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           


        }

        private void button3_Click(object sender, EventArgs e)
        {

            try
            {
                // Retreiving data from App.config file
                string cxn = ConfigurationManager.ConnectionStrings["AzureStorageConn"].ConnectionString; // cxn is the connection string
                string localFolder = ConfigurationManager.AppSettings["destFolder"]; // localFolder is the connection string for the folder that will receive the downloads

                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cxn);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference("NAME OF CONTAINER");

                // Retrieve reference to a blob named as selected
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(textBox1.Text);
                var extension = textBox1.Text.Split('.')[1];
                label5.Text = "Working on it...";

                // Save blob contents to a file.
                // chnage below directory to that of your destination folder
                using (var fileStream = System.IO.File.OpenWrite(@"C:\Users\Administrator\Desktop\destination\file." + extension))

                {
                    blockBlob.DownloadToStream(fileStream);


                }

                label5.Text = "Download Done; Please check the destination folder!";
            }
            catch (Exception)
            {

                label5.Text = "please insert valid input";
            }
        }
    }
}
