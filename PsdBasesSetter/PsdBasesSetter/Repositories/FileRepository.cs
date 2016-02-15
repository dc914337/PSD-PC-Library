﻿using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using PsdBasesSetter.Crypto;
using PsdBasesSetter.Repositories.Objects;
using PsdBasesSetter.Repositories.Serializers;

namespace PsdBasesSetter.Repositories
{
    public class FileRepository : INotifyPropertyChanged, IRepository
    {
        public Base Base { get; set; }

        private byte[] EncryptionKey { get; set; }

        private string _path;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }
        

        public DateTime LastUpdated { get; private set; }
        
        public FileRepository()
        {
            //EncryptionKey = null
        }
        public FileRepository(byte[] encryptionKey)
        {
            EncryptionKey = encryptionKey;
            LastUpdated = DateTime.Now;
        }


        private bool MapData()
        {
            Base = new Base();
            byte[] dataBytes = ReadAllBytes();

            string dataStr;
            if (EncryptionKey != null)//decrypt
            {
                CryptoBase crypto = new CryptoBase(EncryptionKey);
                if (dataBytes.Length < CryptoBase.IVLength)
                    return false;
                if (!crypto.DecryptAll(dataBytes, out dataStr))
                    return false;
            }
            else
            {
                if (dataBytes.Length == 0)
                    return false;
                dataStr = Encoding.ASCII.GetString(dataBytes);
            }

            Base = Serializer.Deserialize(dataStr);
            return true;
        }



        private byte[] ReadAllBytes()
        {
            try
            {
                byte[] data;
                using (FileStream fsStream = new FileStream(Path, FileMode.Open))
                {
                    data = new byte[fsStream.Length];
                    fsStream.Read(data, 0, data.Length);
                }
                return data;
            }
            catch (Exception ex)
            {

                return new byte[0];
            }

        }


        public bool Create(string path)
        {
            Base = new Base();
            return SaveAs(path);
        }


        public bool SaveAs(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            Path = path;
            WriteChanges();
            return MapData();
        }

        public ConnectResult Connect(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return ConnectResult.WrongPath;

            Path = path;
            if (!MapData())
            {
                Path = null;
                return ConnectResult.AccessError;
            }

            WriteChanges();

            return ConnectResult.Success;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool WriteChanges()
        {
            CryptoBase crypto = new CryptoBase(EncryptionKey);

            string xmlToSerialize = Serializer.Serialize(Base);

            byte[] toWrite;

            if (EncryptionKey != null)
            {
                toWrite = crypto.EncryptAll(xmlToSerialize);
            }
            else
            {
                toWrite = Encoding.ASCII.GetBytes(xmlToSerialize);
            }


            try
            {
                using (FileStream fsStream = new FileStream(Path, FileMode.Create))
                {
                    fsStream.Write(toWrite, 0, toWrite.Length);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            LastUpdated = DateTime.Now;
            return true;
        }

    }

    public enum ConnectResult
    {
        AccessError,  //no permissions, wrong pass or broken file
        Success,
        WrongPath
    }
}
