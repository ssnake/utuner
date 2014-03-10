using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;

using System.IO;
using System.IO.IsolatedStorage;

using System.Runtime.Serialization;

using System.Security.Permissions;

namespace uTuner
{

    public class Serializer<T> : System.Object
    {

        /// just for example
        private void Serialize2XML(string filename, Object obj)
        {
            XmlSerializer mySerializer = new XmlSerializer(obj.GetType());
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            StreamWriter myWriter = new StreamWriter(new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, myStore));
            mySerializer.Serialize(myWriter, obj);
            myWriter.Close();



        }

        public void Serialize2JSON(T obj, string filename)
        {
            string str = "";

            str = Serialize2JSON(obj);
            
           
            var myWriter = new StreamWriter(filename);
            myWriter.Write(str);
            myWriter.Close();
        }

        /// just for example
        private bool XML2Serialize(string filename, ref Object obj)
        {
            var ret = false;
            XmlSerializer mySerializer = new XmlSerializer(obj.GetType());
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (myStore.FileExists(filename))
            {


                StreamReader myReader = new StreamReader(new IsolatedStorageFileStream(filename, FileMode.Open, myStore));

                obj = mySerializer.Deserialize(myReader);
                myReader.Close();
                ret = true;
            }
            else
                obj =null;// new obj.GetType().;

            return ret;

        }

        public T JSON2Serialize(string filename, bool isFilename)
        {
            if (!isFilename)
                return JSON2Serialize(filename);
            
            try
            {
                
                
                if (File.Exists(filename))
                {


                    StreamReader myReader = new StreamReader(filename);
                    string str = "";
                    str = myReader.ReadToEnd();
                   
                    var ret = (T) JSON2Serialize(str);
                    myReader.Close();
                    return ret;
                }
                else
                    return default(T);


            }
            catch
            {

                return default(T); ;
            }
            

        }

        public T JSON2Serialize(string json)
        {

            try
            {
                var mySerializer = new DataContractJsonSerializer(typeof(T));
                




                var s = new MemoryStream();
                var buf = Encoding.UTF8.GetBytes(json);
                s.Write(buf, 0, buf.Length);
                s.Position = 0;
                var obj = (T) mySerializer.ReadObject(s);
                s.Close();
                return obj;



            }
            catch
            {
              
                return default(T);;
            }
            

        }

        public string Serialize2JSON(T obj)
        {
            var mySerializer = new DataContractJsonSerializer(obj.GetType());




            var memStream = new MemoryStream();

            
            mySerializer.WriteObject(memStream, obj);
            memStream.Position = 0;
            byte[] buf = memStream.ToArray();
            string s = Encoding.UTF8.GetString(buf, 0, buf.Length);
            return s;



        }
        [DataMember]
        public int NewProperty { get; set; }

    }
}
