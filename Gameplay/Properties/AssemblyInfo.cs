using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MHGameWork.TheWizards.Data;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gameplay")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Gameplay")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3a17e389-837d-4fec-ab1f-742a0af91114")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]




//[assembly: TWProfile(TWProfileAttribute.NameType.Class, AttributeTargetTypes = "MHGameWork.TheWizards.*", AttributeTargetMembers = "Simulate")]
#if PROFILE
[assembly: TWProfile(AttributeTargetTypes = "MHGameWork.TheWizards.*", AttributeTargetMembers = "*")]
#endif
[assembly: TWProfile(AttributeTargetTypes = "MHGameWork.TheWizards.RTSTestCase1.Magic.*", AttributeTargetMembers = "*")]
