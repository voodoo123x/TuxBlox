using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;

namespace Tuxblox.ViewModel
{
    /// <summary>
    /// This class contains properties that the View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class BloxDialogViewModel : ViewModelBase
    {
        private string _DialogHeader;
        private string _DialogMessage;

        private DialogButtons _Buttons;
        private Action _OnButtonOneAction;
        private Action _OnButtonTwoAction;
        private Action _CloseAction;

        /// <summary>
        /// Gets or sets the DialogHeader property.
        /// </summary>
        public string DialogHeader
        {
            get { return _DialogHeader; }

            set
            {
                if (_DialogHeader != value)
                {
                    Set(ref _DialogHeader, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the DialogMessage property.
        /// </summary>
        public string DialogMessage
        {
            get { return _DialogMessage; }

            set
            {
                if (_DialogMessage != value)
                {
                    Set(ref _DialogMessage, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Buttons property.
        /// </summary>
        public DialogButtons Buttons
        {
            get { return _Buttons; }

            set
            {
                if (_Buttons != value)
                {
                    Set(ref _Buttons, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the OnButtonOneAction property.
        /// </summary>
        public Action OnButtonOneAction
        {
            get { return _OnButtonOneAction; }

            set
            {
                if (_OnButtonOneAction != value)
                {
                    Set(ref _OnButtonOneAction, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the OnButtonTwoAction property.
        /// </summary>
        public Action OnButtonTwoAction
        {
            get { return _OnButtonTwoAction; }

            set
            {
                if (_OnButtonTwoAction != value)
                {
                    Set(ref _OnButtonTwoAction, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the CloseAction property.
        /// </summary>
        public Action CloseAction
        {
            get { return _CloseAction; }

            set
            {
                if (_CloseAction != value)
                {
                    Set(ref _CloseAction, value);
                }
            }
        }

        /// <summary>
        /// Configures the BloxDialogViewModel.
        /// </summary>
        public void Configure(string header, string message, DialogButtons buttons, Action onButtonOneClick, Action onButtonTwoClick)
        {
            DialogHeader = header;
            DialogMessage = message;

            OnButtonOneAction = onButtonOneClick;
            OnButtonTwoAction = onButtonTwoClick;
        }

        #region Commands

        private RelayCommand _ButtonOneCommand;
        public RelayCommand ButtonOneCommand
        {
            get
            {
                if (_ButtonOneCommand == null)
                {
                    _ButtonOneCommand = new RelayCommand(ExecButtonOne);
                }

                return _ButtonOneCommand;
            }
        }

        private void ExecButtonOne()
        {
            if (OnButtonOneAction != null)
            {
                OnButtonOneAction.Invoke();
            }

            CloseAction.Invoke();
        }

        private RelayCommand _ButtonTwoCommand;
        public RelayCommand ButtonTwoCommand
        {
            get
            {
                if (_ButtonTwoCommand == null)
                {
                    _ButtonTwoCommand = new RelayCommand(ExecButtonOne);
                }

                return _ButtonTwoCommand;
            }
        }

        private void ExecButtonTwo()
        {
            if (OnButtonTwoAction != null)
            {
                OnButtonTwoAction.Invoke();
            }

            CloseAction.Invoke();
        }

        #endregion
    }

    public enum DialogButtons
    {
        One,
        Two
    }
}