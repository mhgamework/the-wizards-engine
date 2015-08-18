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
		DX11FontWrapper(SlimDX::Direct3D11::Device^ device, System::String^ family);
		void Draw(System::String^ str,float size,float x,float y,SlimDX::Color4 color);
		void DrawRight(System::String^ str,float size,float x,float y,SlimDX::Color4 color);

	private:
		SlimDX::Direct3D11::Device^ device;
		IFW1FontWrapper* pFontWrapper;
		void initialize(SlimDX::Direct3D11::Device^ device,LPCWSTR family);
		void drawInternal(System::String^ str,float size,float x,float y,SlimDX::Color4 color, UINT flags);

	};

	}
}