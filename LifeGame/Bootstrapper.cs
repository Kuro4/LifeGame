using LifeGame.Views;
using System.Windows;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Prism.Ioc;

namespace LifeGame
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            //moduleCatalog.AddModule(typeof(CellModule));
        }
    }
}
