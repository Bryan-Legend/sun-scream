﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using ProtoBuf;
using System.IO;

namespace Server
{
    [DataContract]
    class UpdatePacket
    {
        [DataMember(Order = 3)]
        public int ControlingEntityId { get; set; }

        [DataMember(Order = 1)]
        public List<Entity> Entites { get; set; }

        [DataMember(Order = 2)]
        public List<Message> Messages { get; set; }

        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);

                var bytes = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(bytes, 0, (int)stream.Length);

                //output.AppendText("\nSerialized to: " + BitConverter.ToString(bytes));
                //output.ScrollToCaret();

                return bytes;
            }
        }
    }
}