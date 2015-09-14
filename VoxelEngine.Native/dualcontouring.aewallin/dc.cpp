/*
  Copyright (C) 2011 Tao Ju

  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public License
  (LGPL) as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.

  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public
  License along with this library; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/
#include "stdafx.h"

//#include "octree.hpp"
//#include "PLYReader.hpp"
//#include "PLYWriter.hpp"
//#include "intersection.hpp"
//
//#include <math.h>
//#include <iostream>
//
//#include <boost/program_options.hpp>
//namespace po = boost::program_options;
//
////#define ALLOW_INTERSECTION
//
///*	Parameters
// *	argv[1]:	name of input file (.dcf format)
// *	argv[2]:	name of output file (.ply format)
// *	argv[3]:	(OPTIONAL) name of secondary output file (.ply format) 
// *              when using dual contouring, storing self-intersecting triangles.
//*/
//
//int main( int args, char* argv[] )
//{
//	// Declare the supported options.
//	po::options_description desc("Allowed options");
//	desc.add_options()
//		("help", "produce help message")
//		("simplify", po::value<float>(), "set simplify threshold (float)")
//		("nointer", "use intersection-free algorithm")
//		("test", "run intersection test")
//	;
//
//	po::variables_map vm;
//	po::store(po::parse_command_line(args, argv, desc), vm);
//	po::notify(vm);    
//
//	if (vm.count("help")) {
//		std::cout << desc << "\n";
//		return 1;
//	}
//	float simplify_threshold = -1;
//	if (vm.count("simplify")) {
//		simplify_threshold = vm["simplify"].as<float>();
//		std::cout << "Simplify threshold set: " << simplify_threshold << "\n";
//	} else {
//		std::cout << "Simplify not set.\n";
//	}
//
//	// Read input file
//	std::cout << " input file: " << argv[1] << "\n";
//	Octree* mytree = new Octree( argv[1], simplify_threshold ) ;
//
//	if (vm.count("nointer")) {
//		std::cout << "Intersection-free algorithm! [Ju et al. 2006] \n";
//		mytree->genContourNoInter2( argv[2] ) ;
//	} else {
//		std::cout << "Original algorithm! [Ju et al. 2002] \n";
//		mytree->genContour( argv[2] ) ;
//	}
//	
//	if (vm.count("test")) {
//		printf("Running intersection test... \n") ;
//		int num = Intersection::testIntersection( argv[2], argv[3] ); // Pairwise intersection test - may take a while
//		printf("%d intersections found!\n", num) ;
//	}
//}
//
