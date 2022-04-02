using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torimochi.Configuration;
using Zenject;

namespace Torimochi.Installers
{
    public class TorimochiPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (!PluginConfig.Instance.Enable) {
                return;
            }
            this.Container.BindInterfacesAndSelfTo<TorimochiController>().AsCached().NonLazy();
        }
    }
}
