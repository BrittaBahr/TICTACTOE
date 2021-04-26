using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Network.SerializerDeserializer
{
    public class SerializerDeserializer<T>
    {
        public static byte[] Serialize(T value)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            byte[] buffer;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, value);
                memoryStream.Seek(0, SeekOrigin.Begin);
                if (memoryStream.Length >= int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("the variable was too long to put it into an array");
                }

                memoryStream.Flush();
                buffer = new byte[memoryStream.Length];
                memoryStream.Read(buffer, 0, buffer.Length);
                memoryStream.Close();
            }

            return buffer;
        }

        public static T Deserialize(byte[] buffer)
        {
            T value;
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                memoryStream.Seek(0, SeekOrigin.Begin);
                value = (T)formatter.Deserialize(memoryStream);
                memoryStream.Close();
            }

            return value;
        }
    }
}
