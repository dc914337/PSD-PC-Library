﻿using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PsdBasesSetter.Repositories.Serializers;

namespace PsdBasesSetter.Repositories.Objects
{

    [Serializable]
    public class Base
    {
        [DataMember]
        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] BTKey { get; set; }

        [DataMember]
        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] HBTKey { get; set; }

        [DataMember]
        public PassGroup PassGroup { get; set; } = new PassGroup();

        public Base()
        {
            PassGroup = new PassGroup();
        }
    }
}
