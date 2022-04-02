using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torimochi.VIews;
using Zenject;

namespace Torimochi.Installers
{
    public class TorimochiMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<TorimochiSettingTabViewController>().FromNewComponentAsViewController().AsCached().NonLazy();
        }
    }
}
