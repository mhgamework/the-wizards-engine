using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core
{
    public interface ISpelObject : IDisposable
    {
        BaseHoofdObject HoofdObj { get; }
        ISpelObject Parent { get;}

        bool Enabled { get; set; }
        bool ParentEnabled { get; set; }

        void SetParentEnabledRecursive(bool nParentEnabled);


        ISpelObject GetChild(int index);
        void AddChild(SpelObject nSpO);
		void RemoveChild(SpelObject nSpO);

        Element GetElement(int index);
        void AddElement(Element IE);
        T GetElement<T>() where T : Element;
        void LinkElement(Element IE);

        void Initialize();

		///// <summary>
		///// Load your graphics content.  If loadAllContent is true, you should
		///// load content from both ResourceManagementMode pools.  Otherwise, just
		///// load ResourceManagementMode.Manual content.
		///// </summary>
		///// <param name="loadAllContent">Which type of content to load.</param>
		//void LoadGraphicsContent(bool loadAllContent);


		///// <summary>
		///// Unload your graphics content.  If unloadAllContent is true, you should
		///// unload content from both ResourceManagementMode pools.  Otherwise, just
		///// unload ResourceManagementMode.Manual content.  Manual content will get
		///// Disposed by the GraphicsDevice during a Reset.
		///// </summary>
		///// <param name="unloadAllContent">Which type of content to unload.</param>
		//void UnloadGraphicsContent(bool unloadAllContent);


		void AddIElementContainer(IElementContainer IEC);
    }
}
