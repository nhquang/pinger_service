using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pinger
{
    partial class Pinger : ServiceBase
    {
        Task pinger_;
        HttpClient httpClient_;
        CancellationTokenSource cts_;
        public Pinger()
        {
            InitializeComponent();
            cts_ = new CancellationTokenSource();
            pinger_ = new Task(() => pinger(cts_.Token), cts_.Token, TaskCreationOptions.LongRunning);
            httpClient_ = new HttpClient();
            httpClient_.BaseAddress = new Uri(ConfigurationManager.AppSettings["URL"].Trim());
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            try
            {
                pinger_?.Start();
            }
            catch(Exception ex)
            {

            }
        }

        void pinger(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var rs = httpClient_.GetStringAsync("index.html").Result;
                    Thread.Sleep(900000);
                    //Thread.Sleep(120000);
                }
            }
            catch(Exception ex)
            {

            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            try
            {
                //stop the sleeping thread gracefully by waiting till the task status is RanToCompletion
                cts_.Cancel();
                while (pinger_.Status != TaskStatus.RanToCompletion) { }

                pinger_.Dispose();
                httpClient_.Dispose();
                cts_.Dispose();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
