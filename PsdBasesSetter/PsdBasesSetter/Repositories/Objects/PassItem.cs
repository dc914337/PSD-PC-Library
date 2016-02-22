using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using PsdBasesSetter.Repositories.Serializers;
using System.Collections.Generic;

namespace PsdBasesSetter.Repositories.Objects
{
    [Serializable]
    [DataContract]
    public class PassItem
    {
        const string PASSWORD_KEY = "Password";

        public PassItem()
        {
        }


        public PassItem(String UUID, String[] tags, Strings strings)
        {
            Id = null;
            this.UUID = UUID;
            this.Tags = tags;
            this.Strings = strings.GetCopy();
            TranslatePasswordFromStrings();
        }

        public PassItem(ushort? id, String UUID, String[] tags, Strings strings, byte[] password)
        {
            Id = id;
            this.UUID = UUID;
            Tags = tags;
            Strings = strings.GetCopy();
            TranslatePasswordFromStrings();
            Pass = password;
        }


        private void TranslatePasswordFromStrings()
        {
            if (Strings.ContainsKey(PASSWORD_KEY))
            {
                SetPassFromString(Strings[PASSWORD_KEY]);
                Strings.Remove(PASSWORD_KEY);
            }
        }


        public void SetPassFromString(String pass)
        {
            Pass = Encoding.ASCII.GetBytes(pass);
        }

        public byte[] GetBytes()
        {
            return Pass;
        }

        public string Title
        {
            get
            {
                string val;
                return Strings.TryGetValue("Title", out val) ? val : String.Empty;
            }
            set
            {
                Strings.TryAdd("Title", value);
            }
        }

        public string Login
        {
            get
            {
                string val;
                return Strings.TryGetValue("Login", out val) ? val : String.Empty;
            }
            set
            {
                Strings.TryAdd("Login", value);
            }
        }

        public string Description
        {
            get
            {
                string val;
                return Strings.TryGetValue("Description", out val) ? val : String.Empty;
            }
            set
            {
                Strings.TryAdd("Description", value);
            }
        }

        public bool EnterWithLogin
        {
            get
            {
                string val;
                bool getVal = Strings.TryGetValue("EnterWithLogin", out val);
                if (!getVal)
                    return false;
                bool res;
                return bool.TryParse(val, out res) ? res : false;
            }
            set
            {
                Strings.TryAdd("EnterWithLogin", value.ToString());
            }
        }




        [DataMember]
        public String UUID { get; set; }
        [DataMember]
        public String[] Tags { get; set; }
        [DataMember]
        public Strings Strings { get; set; } = new Strings();

        [DataMember]
        public ushort? Id { get; set; }


        [DataMember]
        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] Pass { get; set; } = new byte[0];


        public PassItem GetCopy()
        {
            return new PassItem(Id, UUID, Tags, Strings, Pass);
        }

        public override string ToString()
        {
            return Title;
        }

        public void RestoreCopy(PassItem backup)
        {
            this.Id = backup.Id;
            this.UUID = backup.UUID;
            this.Tags = backup.Tags;
            this.Strings = backup.Strings.GetCopy();
            this.Pass = backup.Pass;
        }
    }
}
