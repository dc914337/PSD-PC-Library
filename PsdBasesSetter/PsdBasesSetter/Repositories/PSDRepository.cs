﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using PsdBasesSetter.Device.Hid;
using PsdBasesSetter.Repositories.Objects;

namespace PsdBasesSetter.Repositories
{
    public class PSDRepository : IRepository
    {
        public PSDDevice Psd;
        public Base Base { get; private set; }

        public DateTime LastUpdated { get; set; }
        public byte[] LoginPass { get; set; }

        public String Name { get; set; }

        public PSDRepository(byte[] loginPass)
        {
            LoginPass = loginPass;
        }

        public enum SetPsdResult
        {
            NotConnected,
            WrongPassword,
            Connected
        }

        public SetPsdResult Connect(PSDDevice psdDevice)
        {
            if (psdDevice == null)
                return SetPsdResult.NotConnected;

            var connected = psdDevice.Connect();
            if (connected)
            {
                Psd = psdDevice;
                Name = Psd.ToString();
                Base = new Base();
                if (!Psd.Login(LoginPass))
                    return SetPsdResult.WrongPassword;
            }
            else
                Psd = null;
            return connected ? SetPsdResult.Connected : SetPsdResult.NotConnected;
        }

        public bool WriteChanges()
        {
            if (!Psd.Login(LoginPass))
                return false;
            Psd.WriteKeys(Base.BTKey, Base.HBTKey);
            var psdConverted = GetPreparedPasswords(Base.PassGroup.ToList());
            int wrote = Psd.WritePasswords(psdConverted);

            var result = wrote == psdConverted.Count();
            if (result)
                LastUpdated = DateTime.Now;
            return result;
        }


        //We think that here passwords are without spaces(no empty indexes). But it's not. We need either resort passwords or allow empty indexes
        private List<byte[]> GetPreparedPasswords(IEnumerable<PassItem> passes)
        {
            List<byte[]> psdConverted = new List<byte[]>();
            foreach (var pass in passes)
            {
                psdConverted.Add(pass.Pass);
            }
            return psdConverted;
        }

        public bool Reset()
        {
            return Psd.Reset(LoginPass);
        }
    }
}
