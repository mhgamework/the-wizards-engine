#include "StdAfx.h"
#include <iostream>

#include <iostream>
#include <string>
#include <vcclr.h>

#include <dae.h>
#include <dom\domCOLLADA.h>
#include <dom\domLibrary_geometries.h>


#include "ColladaFile.h"

using namespace std;
using namespace System;

void MarshalString ( String ^ s, string& os ) {
   using namespace Runtime::InteropServices;
   const char* chars = 
      (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
   os = chars;
   Marshal::FreeHGlobal(IntPtr((void*)chars));
}

void MarshalString ( String ^ s, wstring& os ) {
   using namespace Runtime::InteropServices;
   const wchar_t* chars = 
      (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
   os = chars;
   Marshal::FreeHGlobal(IntPtr((void*)chars));
}


MHGameWork::TheWizards::Collada::ColladaFile::ColladaFile(System::String^ filename)
{
	DAE dae;

	string filenameNative;
	MarshalString(filename,filenameNative);

	daeString str; //= dae.get();
	daeElement* root =  dae.open(filenameNative);
	if (!root) {
		
		return;
	}

	domCOLLADA* domRoot = (domCOLLADA*)root;


	daeTArray< domLibrary_geometriesRef> lijst = domRoot->getLibrary_geometries_array();




	/*System::Windows::Forms::MessageBox::Show(
	libGeom.getCount().ToString()
	);*/


}
