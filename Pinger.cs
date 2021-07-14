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

        async Task pinger(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var rs = httpClient_.GetStringAsync("index.html").Result;
                    //Thread.Sleep(900000);
                    //Thread.Sleep(10000);

                    //when the cancellation is requested, it should cancel the delay task too, by doing this the task can be stopped immediately after the stop is hit.
                    //If thread.sleep() is used to delay , there is no way to stop the task immediately, because it blocks the thread so we have to wait til it's awake.
                    await Task.Delay(900000, ct);
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
                cts_.Cancel();
                //while (TaskStatus.RanToCompletion != pinger_.Status) { }
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
