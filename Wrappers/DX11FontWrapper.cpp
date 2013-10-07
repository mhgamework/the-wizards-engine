#include "StdAfx.h"
#include "DX11FontWrapper.h"


MHGameWork::TheWizards::DX11FontWrapper::DX11FontWrapper(SlimDX::Direct3D11::Device^ device)
{
	this->device = device;

	IFW1Factory *pFW1Factory;
	FW1CreateFactory(FW1_VERSION, &pFW1Factory);
	ID3D11Device* dev = (ID3D11Device*)device->ComPointer.ToPointer();



	IFW1FontWrapper* pw;

	pFW1Factory->CreateFontWrapper(dev, L"Arial", &pw); 
	pFW1Factory->Release();

	this->pFontWrapper = pw;
}

void MHGameWork::TheWizards::DX11FontWrapper::Draw(System::String^ str,float size,int x,int y, SlimDX::Color4 color)
{
	ID3D11DeviceContext* pImmediateContext = (ID3D11DeviceContext*)this->device->ImmediateContext->ComPointer.ToPointer();
	void* txt = (void*)Marshal::StringToHGlobalUni(str);

	//color = 0xff0099ff;
	color = SlimDX::Color4(color.Alpha, color.Blue, color.Green, color.Red); // convert to ABGR

	int colorArgb = color.ToArgb();

	pFontWrapper->DrawString(pImmediateContext, (WCHAR*)txt, size, x, y, colorArgb, 	FW1_NOGEOMETRYSHADER);
	Marshal::FreeHGlobal(System::IntPtr(txt));
}