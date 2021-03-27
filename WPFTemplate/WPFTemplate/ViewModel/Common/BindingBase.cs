using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFTemplate.ViewModel.Common
{
    public abstract class BindingBase : ViewModelBase
    {
        #region Properties  
        /// <summary>  
        /// 显示名称  
        /// </summary>  
        public virtual string DisplayName { get; protected set; }

        private readonly Dictionary<string, object> _properties;
        #endregion

        #region Constructor  
        /// <summary>  
        /// 实例化一个BindingBase对象  
        /// </summary>  
        protected BindingBase()
        {
            var properties = GetType().GetProperties();
            _properties = (from p in properties
                           from DefaultValueAttribute d in p.GetCustomAttributes(typeof(DefaultValueAttribute), false)
                           select new KeyValuePair<string, object>(p.Name, d.Value)).ToDictionary(x => x.Key, x => x.Value);
        }
        #endregion

        #region Set And Get Property Methods
        /// <summary>
        /// 获取双向绑定值量
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual TValue GetPropertyValue<TValue>([CallerMemberName] string propertyName = null)
        {
            if(propertyName == null)
            {
                throw new ArgumentException(nameof(propertyName));
            }
            return _properties.TryGetValue(propertyName, out var value) ? (TValue)value : default(TValue);
        }

        protected virtual TValue GetNotNullPropertyValue<TValue>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentException(nameof(propertyName));
            }
            var res = _properties.TryGetValue(propertyName, out var value) ? (TValue)value : default(TValue);
            if(res is string && string.IsNullOrEmpty(res as string))
            {
                throw new ArgumentException(nameof(propertyName));
            }
            return res;
        }

        /// <summary>
        /// 设置双向绑定值量
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual bool SetPropertyValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentException(nameof(propertyName));
            }
            if (EqualityComparer<TValue>.Default.Equals(value, GetPropertyValue<TValue>(propertyName)))
            {
                return false;
            }
            _properties[propertyName] = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region Cleanup Methods
        public override void Cleanup()
        {
            base.Cleanup();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<string>(this);
        }
        #endregion
    }
}
