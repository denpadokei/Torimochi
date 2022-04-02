using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace Torimochi.Installers
{
    public class TorimochiPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<TorimochiController>().AsCached().NonLazy();
        }
    }
}
