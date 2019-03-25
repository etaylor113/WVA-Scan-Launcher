﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WVA_Scan_Launcher
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            SetUp();
        }

        private void SetUp()
        {         
            // Make image transparent
            var pos1 = label1.Location;
            pos1 = backdrop.PointToClient(pos1);
            label1.Parent = backdrop;
            label1.Location = pos1;
            label1.BackColor = Color.Transparent;

            // Make topmost to hide any installer windows
            TopMost = true;

            // Set 
            try
            {
                Text = $"WVA Scan v{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}";
            }
            catch (Exception ex)
            {
                Text = $"WVA Scan";
                Error.Report(ex);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {  
            try
            {
                backgroundWorker1.ReportProgress(0);
                if (App.DoesExist())
                {
                    if (API.NeedsUpdate())
                    {
                        backgroundWorker1.ReportProgress(15);
                        API.GetUpdate();
                        backgroundWorker1.ReportProgress(35);
                        App.Uninstall();
                        backgroundWorker1.ReportProgress(65);
                        App.Install();
                        backgroundWorker1.ReportProgress(100);
                        App.LaunchWVA_Scan();
                        App.Close();
                    }
                    else
                    {
                        backgroundWorker1.ReportProgress(100);
                        Thread.Sleep(500);
                        App.LaunchWVA_Scan();
                        App.Close();
                    }              
                }
                else
                {
                    backgroundWorker1.ReportProgress(15);
                    API.GetUpdate();
                    backgroundWorker1.ReportProgress(65);
                    App.Install();
                    backgroundWorker1.ReportProgress(100);
                    Thread.Sleep(1000);
                    App.LaunchWVA_Scan();
                    App.Close();
                }
            }
            catch (Exception ex)
            {
                Error.Report(ex);
                App.Close();
            }         
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar1.Value = e.ProgressPercentage;

            switch (e.ProgressPercentage)
            {
                case 0:
                    textBox1.Text = "Checking for updates...";
                    break;
                case 15:
                    textBox1.Text = "Checking for updates...";
                    break;
                case 35:
                    textBox1.Text = "Loading files...";
                    break;
                case 65:
                    textBox1.Text = "Installing Update...";
                    break;
                case 100:
                    textBox1.Text = "All done!";
                    break;
            }
        } 
    }
}
