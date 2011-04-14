// ColladaWrapper.h : main header file for the ColladaWrapper DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CColladaWrapperApp
// See ColladaWrapper.cpp for the implementation of this class
//

class CColladaWrapperApp : public CWinApp
{
public:
	CColladaWrapperApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
