using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace bykIFv1
{
    [Serializable]
    public class TrackItem : IComparable<TrackItem>, ISerializable
    {
        private List<Point> _items;
        private string _name;
        private DateTime _createTime;
        private string _description;

        public TrackItem(string name, DateTime createTime)
        {
            this._name = name;
            this._createTime = createTime;
            this._description = string.Empty;

            this._items = new List<Point>();
        }

        public TrackItem(SerializationInfo info, StreamingContext context)
        {
            this._name = info.GetString("name");
            this._createTime = info.GetDateTime("createTime");
            this._description = info.GetString("description");

            this._items = info.GetValue("items", typeof(List<Point>)) as List<Point>;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", this._name);
            info.AddValue("createTime", this._createTime);
            info.AddValue("description", this._description);
            info.AddValue("items", this._items);
        }


        public void Normalize()
        {
            // ソートしておきますよ
            _items.Sort();

            // 名前がないならつければいいじゃない
            if (0 >= this._name.Length)
            {
                this._name = string.Format("{0}({1})", _createTime.ToString("yyyy/MM/dd"), _items.Count);
            }
        }

        public List<Point> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public DateTime CreateTime
        {
            get
            {
                return _createTime;
            }
            set
            {
                _createTime = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }


        public override string ToString()
        {
            return _name;
        }

        public int CompareTo(TrackItem other)
        {
            return _createTime.CompareTo(other._createTime);
        }
    }
}
