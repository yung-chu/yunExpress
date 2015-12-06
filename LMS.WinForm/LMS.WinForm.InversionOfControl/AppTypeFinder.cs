using System;
using System.Collections.Generic;
using System.Reflection;
using LighTake.Infrastructure.Common;


namespace LMS.WinForm.InversionOfControl
{
    /// <summary>
    /// Provides information about types in the current web application. 
    /// Optionally this class can look at all assemblies in the bin folder.
    /// </summary>
    public class AppTypeFinder : AppDomainTypeFinder
    {
        #region Fields

        private bool _ensureBinFolderAssembliesLoaded = true;
        private bool _binFolderAssembliesLoaded = false;

        #endregion

        #region Ctor

        public AppTypeFinder()
        {
            this._ensureBinFolderAssembliesLoaded = true;          
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets wether assemblies in the bin folder of the web application should be specificly checked for beeing loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.
        /// </summary>
        public bool EnsureBinFolderAssembliesLoaded
        {
            get { return _ensureBinFolderAssembliesLoaded; }
            set { _ensureBinFolderAssembliesLoaded = value; }
        }
        
        #endregion

        #region Methods

        public override IList<Assembly> GetAssemblies()
        {
            if (this.EnsureBinFolderAssembliesLoaded && !_binFolderAssembliesLoaded)
            {
                _binFolderAssembliesLoaded = true;
                string binPath =AppDomain.CurrentDomain.BaseDirectory;
                LoadMatchingAssemblies(binPath);
            }

            return base.GetAssemblies();
        }
        #endregion
    }
}
