using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFTemplate.ViewModel.Common
{
    public class HViewModelBase : BindingBase
    {
        #region Commands
        public ICommand UnLoadCommand { get; set; }
        #endregion


        #region Constructors
        public HViewModelBase()
        {
            UnLoadCommand = new RelayCommand(Cleanup);
        }
        #endregion

        #region Cleanup Methods
        /// <summary>
        /// 关闭页面View时同时解注释消息传递机制
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<string>(this);
        }
        #endregion
    }
}
