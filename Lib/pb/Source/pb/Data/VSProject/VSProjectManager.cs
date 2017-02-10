using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

// project visual studio 2015 v14.0 :
//
//   source :
//     <ItemGroup>
//       <Compile Include="Source\PlayListReader.cs" />
//     </ItemGroup>
//
//   source link :
//     <ItemGroup>
//       <Compile Include="..\..\..\Lib\pb\Source\pb\_pb\Application.cs">
//         <Link>SourceLnk\Lib\pb\_pb\Application.cs</Link>
//       </Compile>
//     </ItemGroup>
//
//   file :
//     <ItemGroup>
//       <None Include="tv.run.cs" />
//     </ItemGroup>
//
//   framework reference :
//     <ItemGroup>
//       <Reference Include="System.Data" />
//     </ItemGroup>
//
//   dll reference :
//     <ItemGroup>
//       <Reference Include="CenterCLR.NamingFormatter">
//         <HintPath>..\..\..\library\CenterCLR.NamingFormatter\CenterCLR.NamingFormatter.1.1.0\net35\CenterCLR.NamingFormatter.dll</HintPath>
//       </Reference>
//     </ItemGroup>
//
//   project reference :
//     <ItemGroup>
//       <ProjectReference Include="..\..\RunSource\v2\runsource.command\runsource.v2.command.csproj">
//         <Project>{12d4a2fb-84e4-4067-8655-a019e2cf7b5f}</Project>
//         <Name>runsource.v2.command</Name>
//       </ProjectReference>
//     </ItemGroup>

namespace pb.Data.VSProject
{
    public class VSSource
    {
        public string File;
        public string Link;
    }

    public class VSReference
    {
        public string Name;   // from Include for framework reference and dll reference, from Name for project reference
        public string File;   // from Include for project reference, from HintPath for dll reference
        public bool ProjectReference;
        public string ProjectId;
        public XElement Element;
    }

    public class VSProjectManager
    {
        private string _projectFile = null;
        private XDocument _projectDocument = null;
        private XNamespace _projectNamespace = null;
        private XElement _projectSourceItemGroup = null;
        private XElement _projectReferenceItemGroup = null;
        private bool _projectModified = false;

        public VSProjectManager(string projectFile)
        {
            _projectFile = projectFile;
        }

        public string ProjectFile { get { return _projectFile; } }
        public bool ProjectModified { get { return _projectModified; } }

        private void Load()
        {
            if (_projectDocument == null)
            {
                _projectDocument = XDocument.Load(_projectFile);
                _projectNamespace = _projectDocument.Root.GetDefaultNamespace();
                //Trace.WriteLine($"root default namespace {_vsProjectDocument.Root.GetDefaultNamespace()}");
            }
        }

        public void Save()
        {
            _projectDocument.Save(_projectFile);
        }

        public IEnumerable<VSSource> GetSources()
        {
            //   source :
            //     <ItemGroup>
            //       <Compile Include="Source\PlayListReader.cs" />
            //     </ItemGroup>
            //
            //   source link :
            //     <ItemGroup>
            //       <Compile Include="..\..\..\Lib\pb\Source\pb\_pb\Application.cs">
            //         <Link>SourceLnk\Lib\pb\_pb\Application.cs</Link>
            //       </Compile>
            //     </ItemGroup>

            //foreach (XElement compile in _projectDocument.Root.Elements(_projectNamespace + "ItemGroup").SelectMany(xe => xe.Elements(_projectNamespace + "Compile")))
            foreach (XElement compile in GetSourcesElements())
            {
                yield return new VSSource { File = compile.Attribute("Include").Value, Link = compile.Element(_projectNamespace + "Link")?.Value };
            }
        }

        public void AddSource(string relativePath)
        {
            if (_projectSourceItemGroup == null)
            {
                Trace.WriteLine("  add source ItemGroup");
                _projectSourceItemGroup = new XElement(_projectNamespace + "ItemGroup");
                _projectDocument.Root.Add(_projectSourceItemGroup);
            }
            Trace.WriteLine($"  add source \"{relativePath}\"");
            _projectSourceItemGroup.Add(new XElement(_projectNamespace + "Compile", new XAttribute("Include", relativePath)));
            _projectModified = true;
        }

        public void RemoveSource(string relativePath)
        {
            bool removed = false;
            foreach (XElement compile in GetSourcesElements())
            {
                if (compile.Element(_projectNamespace + "Link") == null && compile.Attribute("Include").Value == relativePath)
                {
                    Trace.WriteLine($"  remove source \"{relativePath}\"");
                    compile.Remove();
                    _projectModified = true;
                    removed = true;
                    break;
                }
            }
            if (!removed)
                Trace.WriteLine($"  warning can't remove source, source not found \"{relativePath}\"");
        }

        public void AddSourceLink(string relativePath, string link)
        {
            if (_projectSourceItemGroup == null)
            {
                Trace.WriteLine("  add source ItemGroup");
                _projectSourceItemGroup = new XElement(_projectNamespace + "ItemGroup");
                _projectDocument.Root.Add(_projectSourceItemGroup);
            }
            Trace.WriteLine($"  add source link \"{relativePath}\"");
            _projectSourceItemGroup.Add(new XElement(_projectNamespace + "Compile", new XAttribute("Include", relativePath), new XElement(_projectNamespace + "Link", new XText(link))));
            _projectModified = true;
        }

        public void RemoveSourceLink(string relativePath)
        {
            bool removed = false;
            foreach (XElement compile in GetSourcesElements())
            {
                if (compile.Element(_projectNamespace + "Link") != null && compile.Attribute("Include").Value == relativePath)
                {
                    Trace.WriteLine($"  remove source link \"{relativePath}\"");
                    compile.Remove();
                    _projectModified = true;
                    removed = true;
                    break;
                }
            }
            if (!removed)
                Trace.WriteLine($"  warning can't remove source link, source link not found \"{relativePath}\"");
        }

        private IEnumerable<XElement> GetSourcesElements()
        {
            Load();
            foreach (XElement compile in _projectDocument.Root.Elements(_projectNamespace + "ItemGroup").SelectMany(xe => xe.Elements(_projectNamespace + "Compile")))
            {
                //XElement link = compile.Element(_projectNamespace + "Link");
                //if (link != null)
                //    yield return compile;
                yield return compile;
            }
        }

        public IEnumerable<VSReference> GetReferences()
        {
            //   framework reference :
            //     <ItemGroup>
            //       <Reference Include="System.Data" />
            //     </ItemGroup>
            //
            //   dll reference :
            //     <ItemGroup>
            //       <Reference Include="CenterCLR.NamingFormatter">
            //         <HintPath>..\..\..\library\CenterCLR.NamingFormatter\CenterCLR.NamingFormatter.1.1.0\net35\CenterCLR.NamingFormatter.dll</HintPath>
            //       </Reference>
            //     </ItemGroup>
            //
            //   project reference :
            //     <ItemGroup>
            //       <ProjectReference Include="..\..\RunSource\v2\runsource.command\runsource.v2.command.csproj">
            //         <Project>{12d4a2fb-84e4-4067-8655-a019e2cf7b5f}</Project>
            //         <Name>runsource.v2.command</Name>
            //       </ProjectReference>
            //     </ItemGroup>

            Load();
            foreach (XElement reference in _projectDocument.Root.Elements(_projectNamespace + "ItemGroup").SelectMany(xe => xe.Elements(_projectNamespace + "Reference")))
                yield return new VSReference { Name = GetReferenceName(reference.Attribute("Include").Value), File = reference.Element(_projectNamespace + "HintPath")?.Value, Element = reference };
            foreach (XElement reference in _projectDocument.Root.Elements(_projectNamespace + "ItemGroup").SelectMany(xe => xe.Elements(_projectNamespace + "ProjectReference")))
                yield return new VSReference { Name = reference.Element(_projectNamespace + "Name")?.Value, File = reference.Attribute("Include").Value, ProjectReference = true,
                    ProjectId = reference.Element(_projectNamespace + "Project")?.Value, Element = reference };
        }

        private static string GetReferenceName(string name)
        {
            // "System.Collections.Immutable, Version=1.1.37.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL"
            // return "System.Collections.Immutable"
            int i = name.IndexOf(',');
            if (i != -1)
                name = name.Substring(0, i);
            return name;
        }

        public void AddReference(string name, string relativePath)
        {
            if (_projectReferenceItemGroup == null)
            {
                Trace.WriteLine("  add reference ItemGroup");
                _projectReferenceItemGroup = new XElement(_projectNamespace + "ItemGroup");
                _projectDocument.Root.Add(_projectReferenceItemGroup);
            }
            Trace.WriteLine($"  add reference \"{name}\"");
            XElement reference = new XElement(_projectNamespace + "Reference", new XAttribute("Include", name));
            if (relativePath != null)
                reference.Add(new XElement(_projectNamespace + "HintPath", new XText(relativePath)));
            _projectReferenceItemGroup.Add(reference);
            _projectModified = true;
        }

        public void RemoveAssemblyReference(string name)
        {
            bool removed = false;
            foreach (VSReference reference in GetReferences())
            {
                if (reference.Name == name)
                {
                    Trace.WriteLine($"  remove reference \"{name}\"");
                    reference.Element.Remove();
                    _projectModified = true;
                    removed = true;
                    break;
                }
            }
            if (!removed)
                Trace.WriteLine($"  warning can't remove reference, reference not found \"{name}\"");
        }
    }
}
