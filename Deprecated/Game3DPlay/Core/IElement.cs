using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core
{
    public interface IElement : IDisposable
    {



        ISpelObject Parent
        { get; }

        IElementContainer Container { get; }

        void LinkToContainer();
		void Unlink();

        bool AcceptContainer(IElementContainer IEC);

        void AcceptParent(ISpelObject nParent);



        //public bool Enabled
        //{
        //    get
        //    {

        //        return _enabled;
        //    }
        //    set { _enabled = value; }
        //}

        void Initialize();

        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        void LoadGraphicsContent(bool loadAllContent);


        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        void UnloadGraphicsContent(bool unloadAllContent);



		


    }
}
