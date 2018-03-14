using LifeGame.InteractionRequestEx;
using LifeGame.Views;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace LifeGame.PrismTriggerActions
{
    public class ShowSettingsAction : PopupWindowActionBase
    {
        protected override Window CreateWindow(INotification notification)
        {
            return new Settings()
            {
                Confirmation = notification as SettingsConfirmatinon,
            };
        }

        protected override void ApplyNotificationToWindow(Window window, INotification notification)
        {
            var confirmation = notification as SettingsConfirmatinon;
            if (confirmation == null) return;

            window.Title = confirmation.Title;
        }
    }
}
