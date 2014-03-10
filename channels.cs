using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DirectShowLib;
namespace uTuner
{
  //  [DataContractAttribute]
    [SerializableAttribute]
    public class ChannelList: List<Channel> {

        public ChannelList Fork() {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, this);
            m.Position = 0;
            return b.Deserialize(m) as ChannelList;
        }

        public Channel GetChannelByID(int aID) {
            return this.Find(delegate(Channel c)
            {
                return (c.ID == aID);
                    
            });


        }

        public Channel GetChannelByFreq(int freq) {
            return this.Find(delegate(Channel c)
            {
                return (c.Freq == freq);
                    
            });


        }

        public void Merge(ChannelList chanList, bool isOverwrite = false) {
            foreach (Channel c in chanList)
            {
                Merge(c, isOverwrite);

                
            }
            int i = 0;
            while (i < this.Count)
            {
                var c = this[i];
                var chan = chanList.GetChannelByID(c.ID);
                if (chan == null)
                {
                    RemoveAt(i);
                    continue;
                }
                i++;

            }
        }
        public void Merge(Channel chan, bool isOverwrite = false)
        {
            
            var existedC = GetChannelByID(chan.ID);
            if (existedC == null)
                Add(chan);
            else
                if (isOverwrite)            
                    existedC.Copy(chan);
                

            

            
        }
        public new void Sort() {
            this.Sort(ChannelsCompare);
        }

        private int ChannelsCompare(Channel aChannel1, Channel aChannel2) {
            if (aChannel1 == null || aChannel2 == null)
                return 0;
            if (aChannel1 == aChannel2)
                return 0;
            if (aChannel1.Position > aChannel2.Position)
                return 1;
            else
                return -1;
            
        }

        public void Delete(int channelID) {
            foreach (Channel c in this)
                if (c.ID == channelID)
                {
                    this.Remove(c);
                    return;
                }
            
        }

        public Channel GetChannelByPos(int pos) {
            return this.Find(delegate(Channel c)
            {
                return (c.Position == pos);
                    
            });


        }

        public Channel NextChannel(Channel chan) {
            for (int i = 0; i < Count; i++)
            {
                var c = this[i];
                if (c.ID == chan.ID && i != Count - 1)
                    return this[i + 1];
                if (c.ID == chan.ID && i == Count - 1)
                    return this[0];
            }
            return null;
        }

        public Channel PrevChannel(Channel chan) {
            for (int i = 0; i < Count; i++)
            {
                var c = this[i];
                if (c.ID == chan.ID && i != 0)
                    return this[i - 1];
                if (c.ID == chan.ID && i == 0)
                    return this[Count -1];

             
            }
            return null;
        }

    }
    [DataContractAttribute]
    [SerializableAttribute]
    public class Channel
    {

        public Channel(string name, int freq) {
            Name = name;
            Freq = freq;
        }

        public Channel() {
           
        }

        public void Copy(Channel chan) {
            ID = chan.ID;
            Name = chan.Name;
            Position = chan.Position;
            Freq = chan.Freq;
            VideoStandard = chan.VideoStandard;
            AspectRatio = chan.AspectRatio;
            Zoom = chan.Zoom;
        }

        [DataMember]
        public int Freq { get; set; }
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Position { get; set; }
        [DataMember]
        public int Zoom { get; set; }
        [DataMember]
        public float AspectRatio { get; set; }
        [DataMember]
        public AnalogVideoStandard VideoStandard { get; set; }
    }
}
