using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle( "KFF" )]
[assembly: AssemblyDescription( "Provides C# methods for reading and writing in the KFF v6.0." )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "" )]
[assembly: AssemblyProduct( "KFF v6.0" )]
[assembly: AssemblyCopyright( "Copyright © Katniss 2019" )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Ustawienie elementu ComVisible na wartość false sprawia, że typy w tym zestawie są niewidoczne
// dla składników COM. Jeśli potrzebny jest dostęp do typu w tym zestawie z
// COM, ustaw wartość true dla atrybutu ComVisible tego typu.
[assembly: ComVisible( false )]
// Następujący identyfikator GUID jest identyfikatorem biblioteki typów w przypadku udostępnienia tego projektu w modelu COM
[assembly: Guid( "58910457-ea71-4c83-8c1d-e0fa2d5806e1" )]

[assembly: AssemblyVersion( "1.1.1" )]
[assembly: AssemblyFileVersion( "1.1.1" )] // wersja produktu
[assembly: NeutralResourcesLanguage( "" )]

/*
 * 1.0   - Initial version.
 * 
 * 
 * 1.0.1 - Added ability to check amount of elements in the object at specified path.
 * 
 * 
 * 1.0.2 - Added ability to read kff formatted files from disk.
 *       - Added ability to check parameters of the root object of the serializer (currently child count, and type (tag/payload)).
 * 
 * 
 * 1.0.3 - Added KFFSerializer.Analyze(Path) method. It replaces the old way with MoveScope().
 * 
 * 
 * 1.0.4 - Added ability to check if the analysis was successful (tag/payload was found).
 *       - Empty strings can now be read as any array type by KFFSerializer.
 * 
 * 
 * 1.0.5 - Changed the analysis operation to use a new class structure called AnalysisData.
 * 
 * 
 * 1.1   - Added placeholders to paths.
 *       - Added filename support for easy feedback on malformed files.
 *       
 *       
 * 1.1.1 - Added paths to error messages.
 * 
 */