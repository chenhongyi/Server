using Model;
using Shared.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class InitHelpers
    {
        private static IWsSerializer pserializer;
        private static ProtobufWsSerializer mserializer;



        public static IWsSerializer GetPse()
        {
            if (pserializer != null)
            {
                return pserializer;
            }
            else
            {
                return pserializer = SerializerFactory.CreateSerializer();
            }

        }

        public static ProtobufWsSerializer GetMse()
        {
            if (mserializer != null)
            {
                return mserializer;
            }
            else
            {
                return mserializer = new ProtobufWsSerializer();
            }

        }
    }
}
