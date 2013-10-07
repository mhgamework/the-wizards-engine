#pragma once
#include "_Dependencies\Headers\FW1FontWrapper.h";
using namespace System::Runtime::InteropServices;

/**
 * Uses the http://fw1.codeplex.com/ font wrapper to render fonts
 */


namespace MHGameWork
{
	namespace TheWizards
	{

	public ref class DX11FontWrapper
	{
	public:
		DX11FontWrapper(SlimDX::Direct3D11::Device^ device);
		void Draw(System::String^ str,float size,int x,int y,SlimDX::Color4 color);

	private:
		SlimDX::Direct3D11::Device^ device;
		IFW1FontWrapper* pFontWrapper;
	};

	}
}