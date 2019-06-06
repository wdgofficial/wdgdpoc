// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common.interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace WDG.Wallet.Win.Common
{
    public class BootStrapService : InstanceBase<BootStrapService>
    {
        public BootStrapService()
        {
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            CompositionContainer container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        /// <summary>
        /// load all page instance where base on IPage
        /// </summary>
        [ImportMany(typeof(IPage))]
        public List<IPage> Pages { get; set; }

        [ImportMany(typeof(IInvoke))]
        public List<IInvoke> Invokes { get; set; }

        [Import(typeof(IShell))]
        public IShell Shell { get; set; }

        /// <summary>
        /// find page by PageName
        /// this method will not create new objects 
        /// </summary>
        /// <param name="name">PageName</param>
        /// <returns></returns>
        public Page GetPage(string name)
        {
            if (name == null || Pages == null)
                return null;

            var page = Pages.FirstOrDefault(x => x.GetPageName() == name);
            if (page == null)
                return null;

            return page.GetCurrentPage();
        }

        public void Invoke<T>(string invokeName,T @params)
        {
            var invoke = Invokes.FirstOrDefault(x => x.GetInvokeName() == invokeName);
            if (invoke != null)
                invoke.Invoke(@params);
        }

        public void Invoke(string invokeName)
        {
            var invoke = Invokes.FirstOrDefault(x => x.GetInvokeName() == invokeName);
            if (invoke != null)
                invoke.Invoke<object>(null);
        }
    }
}
