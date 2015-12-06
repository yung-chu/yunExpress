using System;
using System.Collections.Generic;

namespace LighTake.Infrastructure.Data.Entity
{
    /// <summary>
    /// 表示一个数据实体
    /// </summary>
    [Serializable]
    public class EntityBase : INotifyPropertyChanged
    {
        private readonly IDictionary<string, object> _changedProperties;

        /// <summary>
        /// 值已改变的属性集合
        /// </summary>
        public IDictionary<string, object> ChangedProperties
        {
            get
            {
                return _changedProperties;
            }
        }

        public EntityBase()
            : this(true)
        {

        }

        public EntityBase(bool notifyPropertyChange)
        {
            _changedProperties = new Dictionary<string, object>();

            if (notifyPropertyChange)
            {
                PropertyChanged += OnEntityPropertyChanged;
            }
        }

        #region Event

        protected virtual void OnEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == this)
            {
                lock (_changedProperties)
                {
                    if (_changedProperties.ContainsKey(e.PropertyName))
                    {
                        _changedProperties[e.PropertyName] = e.NewValue;
                    }
                    else
                    {
                        _changedProperties.Add(e.PropertyName, e.NewValue);
                    }
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    /// <summary>
    /// 定义当属性值发生改变时通知事件接口
    /// </summary>
    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// 属性值改变事件委托
    /// </summary>
    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    /// <summary>
    /// 属性值改变事件参数
    /// </summary>
    public class PropertyChangedEventArgs : EventArgs
    {
        public string PropertyName
        {
            get;
            set;
        }

        public object OldValue
        {
            get;
            set;
        }

        public object NewValue
        {
            get;
            set;
        }
    }
}
