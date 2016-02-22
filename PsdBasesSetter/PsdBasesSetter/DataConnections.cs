using System;
using System.Linq;
using PsdBasesSetter.Crypto;
using PsdBasesSetter.Device.Hid;
using PsdBasesSetter.Repositories;
using PsdBasesSetter.Repositories.Objects;

namespace PsdBasesSetter
{
    public class DataConnections
    {
        private const int BtKeyLength = 32;
        private const int HBtKeyLength = 32;

        public FileRepository PcBase { get; set; }
        public FileRepository PhoneBase { get; private set; }
        public PSDRepository PsdBase { get; private set; }

        public Base MainBase { get; set; }

        public String UserPass
        {
            set
            {
                _userPasses = new BasePasswords(value);
            }
        }

        private BasePasswords _userPasses;

        public PassGroup RootGroup => PcBase.Base.PassGroup;

        public DateTime LastUpdate
        {
            get
            {
                var allDates = new DateTime[] {
                    PcBase?.LastUpdated ?? DateTime.MaxValue,
                    PhoneBase?.LastUpdated ?? DateTime.MaxValue,
                    PsdBase?.LastUpdated ?? DateTime.MaxValue };
                return allDates.Min();
            }
        }

        public bool TryCreateAndSetPcBase(String path)
        {
            if (_userPasses == null)
                return false;

            var newPcBase = new FileRepository(_userPasses.BasePassword);
            var connectResult = newPcBase.Create(path);
            if (!newPcBase.Create(path))
                return false;
            return TrySetPcBase(path);
        }


        public bool TrySetPcBase(String path)
        {
            if (_userPasses == null)
                return false;

            var newPcBase = new FileRepository(_userPasses.BasePassword);
            var connectResult = newPcBase.Connect(path);
            if (connectResult == ConnectResult.WrongPath)
                return false;

            if (connectResult == ConnectResult.Success)
                PcBase = newPcBase;

            if (connectResult == ConnectResult.Success)
            {
                MainBase = PcBase.Base;
                return true;
            }

            return false;
        }

        public bool TryCreateAndSetPhoneBase(String path)
        {
            if (_userPasses == null)
                return false;

            var newPhoneBase = new FileRepository(_userPasses.PhonePassword);
            var connectResult = newPhoneBase.Connect(path);
            if (!newPhoneBase.Create(path))
                return false;
            return TrySetPhoneBase(path);
        }

        public bool TrySetPhoneBase(String path)
        {
            if (_userPasses == null)
                return false;

            var newPhoneBase = new FileRepository(_userPasses.PhonePassword);
            var connectResult = newPhoneBase.Connect(path);
            if (connectResult == ConnectResult.WrongPath)
                return false;

            if (connectResult == ConnectResult.Success)
                PhoneBase = newPhoneBase;

            return connectResult == ConnectResult.Success;
        }



        public PSDRepository.SetPsdResult TrySetPsdBase(PSDDevice newDevice)
        {
            /* if (_userPasses == null)
                 return false;*/

            var newPsdBase = new PSDRepository(_userPasses.PsdLoginPass);
            var res = newPsdBase.Connect(newDevice);
            if (res == PSDRepository.SetPsdResult.Connected ||
                res == PSDRepository.SetPsdResult.WrongPassword)
                PsdBase = newPsdBase;
            return res;
        }


        public WriteAllResult WriteAll()
        {
            UpdateAll();

            if (PcBase != null && !PcBase.WriteChanges())
                return WriteAllResult.FailedPc;

            if (PhoneBase != null && !PhoneBase.WriteChanges())
                return WriteAllResult.FailedPhone;

            if (PsdBase != null && !PsdBase.WriteChanges())
                return WriteAllResult.FailedPsd;

            return WriteAllResult.Success;
        }


        private void UpdateAll()
        {
            byte[] hBtKEy = KeyGenerator.GenerateByteKey(HBtKeyLength);
            byte[] btKey = KeyGenerator.GenerateByteKey(BtKeyLength);

            if (PhoneBase != null && PsdBase != null)
            {
                PhoneBase.Base.PassGroup.Passwords = new PasswordList();
                PsdBase.Base.PassGroup.Passwords = new PasswordList();
                ushort passId = 0;
                foreach (var passItem in MainBase.PassGroup)
                {
                    passItem.Id = passId++;
                    var dPass = new DividedPassword(passItem);
                    var phonePass = passItem.GetCopy();
                    phonePass.Pass = dPass?.Part1;
                    var psdPass = passItem.GetCopy();
                    psdPass.Pass = dPass?.Part2;

                    PhoneBase.Base.PassGroup.Passwords.Add((ushort)passItem.Id, phonePass);
                    PsdBase.Base.PassGroup.Passwords.Add((ushort)passItem.Id, psdPass);
                }
                PhoneBase.Base.BTKey = btKey;
                PhoneBase.Base.HBTKey = hBtKEy;
                PsdBase.Base.BTKey = btKey;
                PsdBase.Base.HBTKey = hBtKEy;
            }

        }
    }


    public enum WriteAllResult
    {
        Success,
        FailedPc,
        FailedPhone,
        FailedPsd
    }
}
