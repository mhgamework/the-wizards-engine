#include "StdAfx.h"
#include "DX11FontWrapper.h"


MHGameWork::TheWizards::DX11FontWrapper::DX11FontWrapper(SlimDX::Direct3D11::Device^ device)
{
	this->initialize(device,L"Arial");
}

MHGameWork::TheWizards::DX11FontWrapper::DX11FontWrapper(SlimDX::Direct3D11::Device^ device, System::String^ family)
{
	void* txt = (void*)Marshal::StringToHGlobalUni(family);
	this->initialize(device,(LPCWSTR) txt);
	Marshal::FreeHGlobal(System::IntPtr(txt));
}

void MHGameWork::TheWizards::DX11FontWrapper::initialize(SlimDX::Direct3D11::Device^ device,LPCWSTR family)
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

void MHGameWork::TheWizards::DX11FontWrapper::drawInternal(System::String^ str,float size,float x,float y, SlimDX::Color4 color, UINT flags)
{
	ID3D11DeviceContext* pImmediateContext = (ID3D11DeviceContext*)this->device->ImmediateContext->ComPointer.ToPointer();
	void* txt = (void*)Marshal::StringToHGlobalUni(str);

	//color = 0xff0099ff;
	color = SlimDX::Color4(color.Alpha, color.Blue, color.Green, color.Red); // convert to ABGR

	int colorArgb = color.ToArgb();

	pFontWrapper->DrawString(pImmediateContext, (WCHAR*)txt, size, x, y, colorArgb, 	FW1_NOGEOMETRYSHADER | flags );
	Marshal::FreeHGlobal(System::IntPtr(txt));
}

void MHGameWork::TheWizards::DX11FontWrapper::DrawRight(System::String^ str,float size,float x,float y, SlimDX::Color4 color)
{
	this->drawInternal(str,size,x,y,color,FW1_RIGHT);
}
void MHGameWork::TheWizards::DX11FontWrapper::Draw(System::String^ str,float size,float x,float y, SlimDX::Color4 color)
{
	this->drawInternal(str,size,x,y,color,0);
}