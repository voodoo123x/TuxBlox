using System;
using System.Windows;
using Tuxblox.ViewModel;

namespace Tuxblox.View
{
    /// <summary>
    /// Interaction logic for BloxDialogView.xaml
    /// </summary>
    public partial class BloxDialogView : Window
    {
        public BloxDialogView(int height, int width, string header, string message, DialogButtons buttons = DialogButtons.One,
            Action onButtonOneClick = null, Action onButtonTwoClick = null)
        {
            InitializeComponent();

            if (DataContext is BloxDialogViewModel context)
            {
                context.Configure(header, message, buttons, onButtonOneClick, onButtonTwoClick);

                context.CloseAction = () => Close();
            }
        }
    }
}
