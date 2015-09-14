// This is the main DLL file.

#include "stdafx.h"

#include "VoxelEngine.Native.h"
#include "dualcontouring.aewallin\octree.hpp"

int VoxelEngineNative::Wrapper::Sum(int a, int b)
{
	return a+b;
}

void VoxelEngineNative::Wrapper::DCTest()
{
	//std::cout << " input file: " << argv[1] << "\n";
	float simplify_threshold = -1;
	char* inputFile ( "C:\\_Speedy\\the-wizards-github\\VoxelEngine.Native\\dualcontouring.aewallin\\mechanic.dcf");
	char* outputFile ( "C:\\_Speedy\\the-wizards-github\\VoxelEngine.Native\\dualcontouring.aewallin\\mechanic.ply");

	Octree* mytree = new Octree( inputFile, simplify_threshold ) ;

	/*if (vm.count("nointer")) {
		std::cout << "Intersection-free algorithm! [Ju et al. 2006] \n";
		mytree->genContourNoInter2( argv[2] ) ;
	} else {
		std::cout << "Original algorithm! [Ju et al. 2002] \n";*/
		mytree->genContour( outputFile ) ;
	//}

	/*if (vm.count("test")) {
	printf("Running intersection test... \n") ;
	int num = Intersection::testIntersection( argv[2], argv[3] ); // Pairwise intersection test - may take a while
	printf("%d intersections found!\n", num) ;
	}*/
}