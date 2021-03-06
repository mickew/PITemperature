﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PiTemperature.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using PiTemperature.Repositories;

namespace PiTemperature.Meters
{
    public class Temperature
    {
        private const string DEVISE_DIR = "/sys/bus/w1/devices/";
        private const string MASTER_FILE = DEVISE_DIR + "w1_bus_master1/w1_master_slaves";
        private const string SLAVE_FILE = "w1_slave";

        private Timer tempTimer { get; set; }
        private IHubConnectionContext<dynamic> Clients { get; set; }
        private List<TempsensorOldTemp> tempsensors;
        private ISensorRepository _sensorRepository;

        public List<TempSensor> TempSensors { get { return tempsensors.ToList<TempSensor>(); } }

        public Temperature(IHubContext<TempHub> hubContext, ISensorRepository sensorRepository)
        {
            _sensorRepository = sensorRepository;
            tempsensors = new List<TempsensorOldTemp>();
            GetTempSensors();
            UpdateNames();
            Clients = hubContext.Clients;
            CheckAllTemps();
            tempTimer = new Timer(TimerCallback, null, 0, 5000);
        }

        public TempSensor GetTempSensor(string sensor)
        {
            return TempSensors.Where(c => c.Sensor == sensor).Select(c => new TempSensor(c.Sensor) { Name = c.Name, Temp = c.Temp }).FirstOrDefault();
        }

        public void RefreshClients()
        {
            Clients.All.broadcastTempSensors(TempSensors);
        }

        public void RescanTempSensors()
        {
            tempTimer.Dispose();
            try
            {
                GetTempSensors();
                UpdateNames();
                CheckAllTemps();
            }
            finally
            {
                tempTimer = new Timer(TimerCallback, null, 0, 5000);
            }
        }

        private void UpdateNames()
        {
            foreach (var item in tempsensors)
            {
                var sensor = _sensorRepository.Get(item.Sensor);
                if (sensor != null)
                {
                    item.Name = sensor.Name;
                }
            }
        }

        private void GetTempSensors()
        {
            tempsensors.Clear();
            if (File.Exists(MASTER_FILE))
            {
                using (FileStream fs = new FileStream(MASTER_FILE, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        int i = 0;
                        while (sr.Peek() >= 0)
                        {
                            i++;
                            string str;
                            str = sr.ReadLine();
                            if (string.Compare(str, "not found.") != 0)
                                tempsensors.Add(new TempsensorOldTemp(str) { Name = string.Format("TempSensor {0}", i) });
                        }
                    }
                }
            }
        }

        private void TimerCallback(Object o)
        {
            CheckAllTemps();
            foreach (var item in tempsensors)
            {
                if (Math.Abs(item.Oldtemp - item.Temp) > 0.2)
                {
                    item.Oldtemp = item.Temp;
                    Clients.All.broadcastTemperature(new TempSensor(item.Sensor) { Temp = item.Temp, Name = item.Name });
                }
            }
        }

        private void CheckAllTemps()
        {
            foreach (var tempSensor in tempsensors)
            {
                string path = string.Format("{0}{1}/{2}", DEVISE_DIR, tempSensor.Sensor, SLAVE_FILE);
                if (File.Exists(path))
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            var w1slavetext = sr.ReadToEnd();
                            string temptext = w1slavetext.Split(new string[] { "t=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                            double temp;
                            if (double.TryParse(temptext, out temp))
                            {
                                tempSensor.Temp = temp / 1000;
                            }
                        }
                    }
                }
                else
                {
                    tempSensor.Temp = (tempSensor.Temp == tempSensor.MaxValue) ? tempSensor.MinValue : tempSensor.MaxValue;
                }
            }
        }
    }
}
