using CG.Web.MegaApiClient;
using pb.Data.Mongo;
using pb.IO;
using System;
using System.Collections.Generic;

// MegaClient.Login() bad password : API response: ResourceNotExists (CG.Web.MegaApiClient.ApiException)
// CG.Web.MegaApiClient.MegaApiClient.GetNodes() ??? : API response: BadSessionId (CG.Web.MegaApiClient.ApiException)

namespace pb.Web.Data
{
    public class MegaNode
    {
        public string Id = null;
        public INode Node = null;
        public string Name = null;
        public string Path = null;
        //[BsonIgnore]
        public MegaDirectoryNode Parent = null;

        //public string GetName()
        //{
        //    if (Node.Name == null)
        //    {
        //        switch (Node.Type)
        //        {
        //            case NodeType.Root:
        //                return "Root";
        //            case NodeType.Inbox:
        //                return "Inbox";
        //            case NodeType.Trash:
        //                return "Trash";
        //            default:
        //                throw new PBException($"wrong node type {Node.Type}");
        //        }
        //    }
        //    else
        //        return Node.Name;
        //}
    }

    public class MegaDirectoryNode : MegaNode
    {
        public List<MegaNode> Childs = new List<MegaNode>();
    }

    //public class MegaNode
    //{
    //    public string Id;
    //    public string Name;
    //    public string Path;
    //    public NodeType Type;
    //    public DateTime LastModificationDate;
    //    public long Size;
    //    public string Owner;
    //    public string ParentId;

    //    public string GetName()
    //    {
    //        if (Name == null)
    //        {
    //            switch (Type)
    //            {
    //                case NodeType.Root:
    //                    return "Root";
    //                case NodeType.Inbox:
    //                    return "Inbox";
    //                case NodeType.Trash:
    //                    return "Trash";
    //                default:
    //                    throw new PBException($"wrong node type {Type}");
    //            }
    //        }
    //        else
    //            return Name;
    //    }

    //    public static MegaNode Create(INode node, string path = null)
    //    {
    //        return new MegaNode { Id = node.Id, Name = node.Name, Type = node.Type, LastModificationDate = node.LastModificationDate, Size = node.Size, Owner = node.Owner, ParentId = node.ParentId, Path = path };
    //    }
    //}

    [Flags]
    public enum NodesOptions
    {
        None      = 0,
        File      = 0x0001,
        Directory = 0x0002,
        Recursive = 0x0004,
        Default   = File | Directory
    }

    public class MegaClient
    {
        private string _email = null;
        private string _password = null;
        private string _nodesCacheFile = null;
        private bool _refreshNodesCache = false;
        private MegaApiClient _client = null;
        //private IEnumerable<INode> _nodes = null;
        //private Dictionary<string, List<INode>> _nodes = null;
        //Dictionary<string, List<MegaNode>> _megaNodes = null;
        //Dictionary<string, Dictionary<string, MegaNode>> _megaNodes = null;
        //Dictionary<string, Dictionary<string, List<MegaNode>>> _megaNodes = null;
        private Dictionary<string, MegaDirectoryNode> _megaNodes = null;
        private int _dictionaryNodes1_nodeWithUnknowParent = 0;
        private int _dictionaryNodes1_nodeAlreadyCreated = 0;
        private MegaDirectoryNode _root = null;
        private MegaDirectoryNode _inbox = null;
        private MegaDirectoryNode _trash = null;
        private int _nodeCount = 0;
        private int _fileNodeCount = 0;
        private int _directoryNodeCount = 0;

        public string Email { get { return _email; } set { _email = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public string NodesCacheFile { get { return _nodesCacheFile; } set { _nodesCacheFile = value; } }
        public bool RefreshNodesCache { get { return _refreshNodesCache; } set { _refreshNodesCache = value; } }
        public int NodeCount { get { return _nodeCount; } }
        public int FileNodeCount { get { return _fileNodeCount; } }
        public int DirectoryNodeCount { get { return _directoryNodeCount; } }

        public void Login()
        {
            _client = new MegaApiClient();
            _client.Login(_email, _password);
        }

        //private class MegaNodeEnum
        //{
        //    public MegaNode Node;
        //    public string Path;
        //    public IEnumerator<INode> Childs;
        //}

        //public void LoadMegaNodes(string file)
        //{
        //    _megaNodes = zMongo.ReadFileAs<Dictionary<string, Dictionary<string, List<MegaNode>>>>(file);
        //}

        //public void SaveMegaNodes(string file)
        //{
        //    GetMegaNodes().zSave(file, jsonIndent: true);
        //}

        //public void UploadFile(string file, string megaDirectory, Action<double> progression = null)
        //{
        //    string filepath = "filetoupload";
        //    using (var stream = new ProgressionStream(new FileStream(file, FileMode.Open), progression))
        //    {
        //        var parent = _client.GetNodes().FirstOrDefault(x => x.Type == NodeType.Root);
        //        _client.Upload(stream, Path.GetFileName(filepath), parent);
        //    }
        //}

        //public void DownloadWithProgression()
        //{
        //    using (var fileStream = new FileStream(filePath, FileMode.Create))
        //    {
        //        using (var downloadStream = new ProgressionStream(client.Download(node), this.PrintProgression))
        //        {
        //            downloadStream.CopyTo(fileStream);
        //        }
        //    }
        //}

        //private MegaNode GetDirectoryNode(string directory)
        //{
        //    if (directory != "/" && directory.EndsWith("/"))
        //        directory = directory.TrimEnd('/');
        //    if (_megaNodes.ContainsKey(directory))
        //        return _megaNodes[directory];
        //}

        //public IEnumerable<MegaNode> GetNodes(string path = "/")
        //{
        //    GetMegaNodes();
        //    bool directory = false;
        //    string path2 = path;
        //    if (path != "/" && path.EndsWith("/"))
        //    {
        //        //path2 = path.Substring(0, path.Length - 1);
        //        path2 = path.TrimEnd('/');
        //        directory = true;
        //    }
        //    if (!_megaNodes.ContainsKey(path2))
        //    {
        //        // search a file
        //        bool foundFile = false;
        //        if (!directory)
        //        {
        //            int i = path.LastIndexOf('/');
        //            if (i > 0)
        //            {
        //                string path3 = path.Substring(0, i);
        //                if (_megaNodes.ContainsKey(path3))
        //                {
        //                    var childs = _megaNodes[path3];
        //                    string file = path.Substring(i + 1);
        //                    if (childs.ContainsKey(file))
        //                    {
        //                        foundFile = true;
        //                        foreach (MegaNode node in childs[file])
        //                            yield return node;
        //                    }
        //                }
        //            }
        //        }
        //        if (!foundFile)
        //            Trace.WriteLine($"file not found \"{path}\"");
        //        yield break;
        //    }
        //    Stack<IEnumerator<MegaNode>> stack = new Stack<IEnumerator<MegaNode>>();
        //    stack.Push(_megaNodes[path2].Values.SelectMany(nodes => nodes).GetEnumerator());
        //    while (stack.Count > 0)
        //    {
        //        IEnumerator<MegaNode> enumerator = stack.Peek();
        //        if (enumerator.MoveNext())
        //        {
        //            MegaNode megaNode = enumerator.Current;
        //            yield return megaNode;
        //            if ((megaNode.Type == NodeType.Directory || megaNode.Type == NodeType.Root || megaNode.Type == NodeType.Inbox || megaNode.Type == NodeType.Trash) && _megaNodes.ContainsKey(megaNode.Path))
        //                stack.Push(_megaNodes[megaNode.Path].Values.SelectMany(nodes => nodes).GetEnumerator());
        //        }
        //        else
        //            stack.Pop();
        //    }
        //}

        public IEnumerable<MegaNode> GetNodes(string directory = null, NodesOptions options = NodesOptions.Default)
        {
            GetMegaNodes();
            //bool directory = false;
            //string path2 = directory;
            if (directory == null)
                directory = "/";
            if (directory != "/" && directory.EndsWith("/"))
                directory = directory.TrimEnd('/');
            if (!_megaNodes.ContainsKey(directory))
            {
                Trace.WriteLine($"directory not found \"{directory}\"");
                yield break;
            }
            bool returnFile = (options & NodesOptions.File) == NodesOptions.File;
            bool returnDirectory = (options & NodesOptions.Directory) == NodesOptions.Directory;
            bool recursive = (options & NodesOptions.Recursive) == NodesOptions.Recursive;
            Stack<IEnumerator<MegaNode>> stack = new Stack<IEnumerator<MegaNode>>();
            IEnumerator<MegaNode> enumerator = _megaNodes[directory].Childs.GetEnumerator();
            stack.Push(enumerator);
            while (true)
            {
                if (enumerator.MoveNext())
                {
                    MegaNode megaNode = enumerator.Current;
                    if (megaNode.Node.Type != NodeType.File)
                    {
                        if (returnDirectory)
                            yield return megaNode;
                        if (recursive)
                        {
                            enumerator = ((MegaDirectoryNode)megaNode).Childs.GetEnumerator();
                            stack.Push(enumerator);
                        }
                    }
                    else if (returnFile)
                        yield return megaNode;
                }
                else
                {
                    stack.Pop();
                    if (stack.Count == 0)
                        break;
                    enumerator = stack.Peek();
                }
            }
        }

        //public IEnumerable<MegaNode> EnumerateNodes()
        //{
        //    Dictionary<string, List<INode>> nodes = GetDictionaryNodes();
        //    Stack<MegaNodeEnum> stack = new Stack<MegaNodeEnum>();
        //    stack.Push(new MegaNodeEnum { Childs = nodes[""].GetEnumerator() });
        //    while (stack.Count > 0)
        //    {
        //        MegaNodeEnum nodeEnum = stack.Peek();
        //        if (nodeEnum.Childs.MoveNext())
        //        {
        //            INode node = nodeEnum.Childs.Current;
        //            MegaNode megaNode = MegaNode.Create(node, GetPath(nodeEnum.Node, node));
        //            yield return megaNode;
        //            if ((node.Type == NodeType.Directory || node.Type == NodeType.Root || node.Type == NodeType.Inbox || node.Type == NodeType.Trash) && nodes.ContainsKey(node.Id))
        //                stack.Push(new MegaNodeEnum { Node = megaNode, Childs = nodes[node.Id].GetEnumerator() });
        //        }
        //        else
        //            stack.Pop();
        //    }
        //}

        //private static string GetPath(MegaNode parent, INode node)
        //{
        //    if (parent != null)
        //        return parent.Path + "/" + node.Name;
        //    else
        //    {
        //        switch (node.Type)
        //        {
        //            case NodeType.Root:
        //                return "/Root";
        //            case NodeType.Inbox:
        //                return "/Inbox";
        //            case NodeType.Trash:
        //                return "/Trash";
        //            default:
        //                throw new PBException($"wrong node type {node.Type}");
        //        }
        //    }
        //}

        //public IEnumerable<MegaNode> EnumerateNodes()
        //{
        //    INode node = GetRoot();
        //    MegaNode megaNode = MegaNode.Create(node, "/Root");
        //    yield return megaNode;
        //    Stack<MegaNodeEnum> stack = new Stack<MegaNodeEnum>();
        //    stack.Push(new MegaNodeEnum { Node = megaNode, Childs = _client.GetNodes(node).GetEnumerator() });
        //    while (stack.Count > 0)
        //    {
        //        MegaNodeEnum nodeEnum = stack.Peek();
        //        if (nodeEnum.Childs.MoveNext())
        //        {
        //            node = nodeEnum.Childs.Current;
        //            megaNode = MegaNode.Create(node, nodeEnum.Node.Path + "/" + node.Name);
        //            yield return megaNode;
        //            if (node.Type == NodeType.Directory)
        //                stack.Push(new MegaNodeEnum { Node = megaNode, Childs = _client.GetNodes(node).GetEnumerator() });
        //        }
        //        else
        //            stack.Pop();
        //    }
        //}

        public IEnumerable<INode> _GetNodes()
        {
            if (!_refreshNodesCache && _nodesCacheFile != null && zFile.Exists(_nodesCacheFile))
            {
                return zMongo.BsonRead<INode>(_nodesCacheFile);
            }
            else
            {
                IEnumerable<INode> nodes = _client.GetNodes();
                if (_nodesCacheFile != null)
                    nodes.zSave(_nodesCacheFile, jsonIndent: true);
                return nodes;
            }
        }

        public Dictionary<string, MegaDirectoryNode> GetMegaNodes()
        {
            if (_megaNodes == null)
            {
                //DateTime start = DateTime.Now;
                //IEnumerable<INode> nodes = _client.GetNodes();
                IEnumerable<INode> nodes = _GetNodes();
                //TimeSpan time = DateTime.Now - start;
                //Trace.WriteLine($"MegaClient.GetMegaNodes_v2() : _client.GetNodes() time {time}");

                //start = DateTime.Now;
                // Dictionary : one node by directory, key is mega id
                Dictionary<string, MegaDirectoryNode> megaNodes1 = GetMegaNodes1(nodes);
                //time = DateTime.Now - start;
                //Trace.WriteLine($"MegaClient.GetMegaNodes_v2() : GetDictionaryNodes1_v2() time {time}");
                //Trace.WriteLine($"MegaClient.GetMegaNodes_v2() : GetDictionaryNodes1_v2() megaNodes.Count {megaNodes1.Count} nodeCount {_nodeCount} _directoryNodeCount {_directoryNodeCount} _fileNodeCount {_fileNodeCount} nodeWithUnknowParent {_dictionaryNodes1_nodeWithUnknowParent} nodeAlreadyCreated {_dictionaryNodes1_nodeAlreadyCreated}");

                //start = DateTime.Now;
                // Dictionary : one node by directory, key is directory path
                _megaNodes = new Dictionary<string, MegaDirectoryNode>();

                MegaDirectoryNode root = new MegaDirectoryNode { Path = "/" };
                root.Childs.Add(_root);
                root.Childs.Add(_inbox);
                root.Childs.Add(_trash);
                _megaNodes.Add("/", root);

                // browse tree directory nodes
                //foreach (MegaDirectoryNode_v2 node in BrowseDirectoryNodes(megaNodes1))
                foreach (MegaNode node in BrowseNodes(megaNodes1))
                {
                    string path = null;
                    if (node.Parent != null)
                        path = node.Parent.Path;
                    //path += "/" + node.GetName();
                    path += "/" + node.Name;
                    node.Path = path;
                    if (node.Node.Type != NodeType.File)
                        _megaNodes.Add(path, (MegaDirectoryNode)node);
                }
                //time = DateTime.Now - start;
                //Trace.WriteLine($"MegaClient.GetMegaNodes_v2() : create _megaNodes_v2 time {time}");
            }
            return _megaNodes;
        }

        // not used
        //private IEnumerable<MegaDirectoryNode_v2> BrowseDirectoryNodes(Dictionary<string, MegaDirectoryNode_v2> megaNodes)
        //{
        //    Stack<IEnumerator<MegaNode_v2>> stack = new Stack<IEnumerator<MegaNode_v2>>();
        //    foreach (MegaDirectoryNode_v2 rootNode in new MegaDirectoryNode_v2[] { _root, _inbox, _trash })
        //    {
        //        yield return rootNode;
        //        IEnumerator<MegaNode_v2> enumerator = rootNode.Childs.GetEnumerator();
        //        stack.Push(enumerator);
        //        while (true)
        //        {
        //            if (enumerator.MoveNext())
        //            {
        //                MegaNode_v2 node = enumerator.Current;
        //                //yield return node;
        //                if (node.Node.Type == NodeType.Directory)
        //                {
        //                    MegaDirectoryNode_v2 directoryNode = (MegaDirectoryNode_v2)node;
        //                    yield return directoryNode;
        //                    enumerator = directoryNode.Childs.GetEnumerator();
        //                    stack.Push(enumerator);
        //                }
        //            }
        //            else
        //            {
        //                stack.Pop();
        //                if (stack.Count == 0)
        //                    break;
        //                enumerator = stack.Peek();
        //            }
        //        }
        //    }
        //}

        private IEnumerable<MegaNode> BrowseNodes(Dictionary<string, MegaDirectoryNode> megaNodes)
        {
            Stack<IEnumerator<MegaNode>> stack = new Stack<IEnumerator<MegaNode>>();
            foreach (MegaDirectoryNode rootNode in new MegaDirectoryNode[] { _root, _inbox, _trash })
            {
                yield return rootNode;
                IEnumerator<MegaNode> enumerator = rootNode.Childs.GetEnumerator();
                stack.Push(enumerator);
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        MegaNode node = enumerator.Current;
                        yield return node;
                        if (node.Node.Type == NodeType.Directory)
                        {
                            MegaDirectoryNode directoryNode = (MegaDirectoryNode)node;
                            //yield return directoryNode;
                            enumerator = directoryNode.Childs.GetEnumerator();
                            stack.Push(enumerator);
                        }
                    }
                    else
                    {
                        stack.Pop();
                        if (stack.Count == 0)
                            break;
                        enumerator = stack.Peek();
                    }
                }
            }
        }

        private Dictionary<string, MegaDirectoryNode> GetMegaNodes1(IEnumerable<INode> nodes)
        {
            // Dictionary : one node by directory, key is mega id
            Dictionary<string, MegaDirectoryNode> megaNodes = new Dictionary<string, MegaDirectoryNode>();
            _dictionaryNodes1_nodeWithUnknowParent = 0;
            _dictionaryNodes1_nodeAlreadyCreated = 0;
            _nodeCount = 0;
            _fileNodeCount = 0;
            _directoryNodeCount = 0;
            foreach (INode node in nodes)
            {
                _nodeCount++;

                //if (type == NodeType.Root)
                //    _root = node;
                //else if (type == NodeType.Inbox)
                //    _inbox = node;
                //else if (type == NodeType.Trash)
                //    _trash = node;

                // MegaNode_v2 : Id, Node, Path, Childs

                MegaDirectoryNode parentMegaNode = null;
                string parentId = node.ParentId;
                if (parentId != null && parentId != "")
                {
                    if (!megaNodes.TryGetValue(parentId, out parentMegaNode))
                    {
                        _dictionaryNodes1_nodeWithUnknowParent++;
                        parentMegaNode = new MegaDirectoryNode { Id = parentId };
                        megaNodes.Add(parentId, parentMegaNode);
                        //Trace.WriteLine($"nodeWithUnknowParent : Id {node.Id} Name {node.Name}");
                    }
                }

                MegaNode megaNode;
                NodeType type = node.Type;
                if (type != NodeType.File)
                {
                    string id = node.Id;
                    MegaDirectoryNode megaDirectoryNode;
                    if (!megaNodes.TryGetValue(id, out megaDirectoryNode))
                    {
                        megaDirectoryNode = new MegaDirectoryNode { Id = id, Node = node, Name = GetName(node) };
                        megaNodes.Add(id, megaDirectoryNode);
                    }
                    else
                    {
                        _dictionaryNodes1_nodeAlreadyCreated++;
                        if (megaDirectoryNode.Node != null)
                            throw new PBException($"node should be null");
                        megaDirectoryNode.Node = node;
                        megaDirectoryNode.Name = GetName(node);
                        //Trace.WriteLine($"nodeAlreadyCreated : Id {node.Id} Name {node.Name}");
                    }

                    if (type == NodeType.Root)
                        _root = megaDirectoryNode;
                    else if (type == NodeType.Inbox)
                        _inbox = megaDirectoryNode;
                    else if (type == NodeType.Trash)
                        _trash = megaDirectoryNode;

                    megaNode = megaDirectoryNode;
                    if (type == NodeType.Directory)
                        _directoryNodeCount++;
                }
                else
                {
                    megaNode = new MegaNode { Id = node.Id, Node = node, Name = GetName(node) };
                    _fileNodeCount++;
                }

                if (parentMegaNode != null)
                {
                    megaNode.Parent = parentMegaNode;
                    parentMegaNode.Childs.Add(megaNode);
                }
            }
            return megaNodes;
        }

        public string GetName(INode node)
        {
            if (node.Name == null)
            {
                switch (node.Type)
                {
                    case NodeType.Root:
                        return "Root";
                    case NodeType.Inbox:
                        return "Inbox";
                    case NodeType.Trash:
                        return "Trash";
                    default:
                        throw new PBException($"wrong node type {node.Type}");
                }
            }
            else
                return node.Name;
        }

        //private Dictionary<string, Dictionary<string, List<MegaNode>>> GetMegaNodes()
        //{
        //    if (_megaNodes == null)
        //    {
        //        Dictionary<string, List<INode>> nodes = GetDictionaryNodes();
        //        _megaNodes = new Dictionary<string, Dictionary<string, List<MegaNode>>>();
        //        Stack<MegaNodeEnum> stack = new Stack<MegaNodeEnum>();
        //        stack.Push(new MegaNodeEnum { Path = "/", Childs = nodes[""].GetEnumerator() });
        //        while (stack.Count > 0)
        //        {
        //            MegaNodeEnum nodeEnum = stack.Peek();
        //            if (nodeEnum.Childs.MoveNext())
        //            {
        //                INode node = nodeEnum.Childs.Current;
        //                //GetPath(nodeEnum.Node, node)
        //                //MegaNode megaNode = MegaNode.Create(node, nodeEnum.Path == "/" ? "/");
        //                MegaNode megaNode = MegaNode.Create(node);
        //                string parentPath = nodeEnum.Path;
        //                string name = megaNode.GetName();
        //                string path = "/" + name;
        //                megaNode.Path = parentPath == "/" ? path : parentPath + path;
        //                if (!_megaNodes.ContainsKey(parentPath))
        //                    _megaNodes.Add(parentPath, new Dictionary<string, List<MegaNode>>());
        //                Dictionary<string, List<MegaNode>> childMegaNodes = _megaNodes[parentPath];
        //                //if (childMegaNodes.ContainsKey(name))
        //                //    throw new PBException($"duplicate node \"{name}\" in \"{parentPath}\"");
        //                //childMegaNodes.Add(name, megaNode);
        //                if (!childMegaNodes.ContainsKey(name))
        //                    childMegaNodes.Add(name, new List<MegaNode>());
        //                childMegaNodes[name].Add(megaNode);
        //                if ((node.Type == NodeType.Directory || node.Type == NodeType.Root || node.Type == NodeType.Inbox || node.Type == NodeType.Trash) && nodes.ContainsKey(node.Id))
        //                    stack.Push(new MegaNodeEnum { Node = megaNode, Path = megaNode.Path, Childs = nodes[node.Id].GetEnumerator() });
        //            }
        //            else
        //                stack.Pop();
        //        }
        //    }
        //    return _megaNodes;
        //}

        //private Dictionary<string, List<INode>> GetDictionaryNodes()
        //{
        //    //if (_nodes == null)
        //    //{
        //        //IEnumerable<INode> nodes = _client.GetNodes();
        //        Dictionary<string, List<INode>> nodes = new Dictionary<string, List<INode>>();
        //        foreach (INode node in _client.GetNodes())
        //        {
        //            //if (node.Type == NodeType.Root)
        //            //    _root = node;
        //            //else if (node.Type == NodeType.Inbox)
        //            //    _inbox = node;
        //            //if (node.Type == NodeType.Trash)
        //            //    _trash = node;
        //            if (!nodes.ContainsKey(node.ParentId))
        //                nodes.Add(node.ParentId, new List<INode>());
        //            nodes[node.ParentId].Add(node);
        //        }
        //    //}
        //    return nodes;
        //}

        //private INode GetRoot()
        //{
        //    if (_root == null)
        //        _root = GetNodes().First(node => node.Type == NodeType.Root);
        //    return _root;
        //}
    }
}
