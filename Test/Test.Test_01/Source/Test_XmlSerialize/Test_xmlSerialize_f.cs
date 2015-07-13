using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using pb;
using pb.Compiler;

namespace Test_xmlSerialize
{

    // La sérialisation XML avec .NET  http://tlevesque.developpez.com/dotnet/xml-serialization/
    //   Personnaliser la sérialisation avec les attributs de contrôle :
    //     [XmlIgnore]               : le champ n'est pas sérialisé
    //     [XmlElement("Rue")]       : change le nom de la balise xml du champ
    //     [XmlArray("Adresses")]    : change le nom de la balise xml pour une collection
    //     [XmlArrayItem("Adresse")] : change le nom de la balise xml pour les éléments d'une collection
    //     [XmlRoot("Personne")]     : change le nom de la balise xml root pour une classe
    //     [XmlAttribute("Id")]      : sérialise en tant qu'attribut
    //     [XmlAttribute()]          : idem
    //     [XmlEnum("Livraison")]    : change le nom de la balise xml de l'enum

    static partial class w
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _rs = RunSource.CurrentRunSource;

        public static void Init()
        {
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

        public static void Test_02()
        {
            _tr.WriteLine("Test_02");
            Person person = new Person() { Id = 1, FirstName = "toto", LastName = "tata" };
            person.zView();
        }

        public static void Test_xmlSerialize_01()
        {
            _tr.WriteLine("Test_xmlSerialize_01");
            Test_01 test = new Test_01() { id = 1, name = "toto", addressType = AddressType.Bill | AddressType.Ship };
            XmlSerializer xs = new XmlSerializer(typeof(Test_01));
            using (StreamWriter wr = new StreamWriter("test_01_01.xml"))
            {
                xs.Serialize(wr, test);
            }
        }

        public static void Test_xmlDeserialize_01()
        {
            _tr.WriteLine("Test_xmlDeserialize_01");
            XmlSerializer xs = new XmlSerializer(typeof(Test_01));
            using (StreamReader rd = new StreamReader("test_01_01.xml"))
            {
                Test_01 test = xs.Deserialize(rd) as Test_01;
                test.zView();
            }
        }

        public static void Test_xmlSerialize_02()
        {
            _tr.WriteLine("Test_xmlSerialize_02");
            Test_01 test = new Test_01() { id = 1, name = "toto" };
            XmlSerialize(test, "test_01_02.xml");
        }

        public static void Test_xmlDeserialize_02()
        {
            _tr.WriteLine("Test_xmlDeserialize_02");
            XmlDeserialize<Test_01>("test_01_01.xml").zView();
        }

        public static void Test_xmlSerialize_03()
        {
            _tr.WriteLine("Test_xmlSerialize_03");
            Test_02 test = new Test_02() { id = 1, name = "toto", test = new Test_01() { id = 2, name = "tata" } };
            XmlSerialize(test, "test_02_01.xml");
            test.zView();
        }

        public static void Test_xmlDeserialize_03()
        {
            _tr.WriteLine("Test_xmlDeserialize_03");
            XmlDeserialize<Test_02>("test_02_01.xml").zView();
        }

        public static EnumTest_01 Get_EnumTest_01()
        {
            EnumTest_01 enumTest = new EnumTest_01();
            enumTest.id = 999;
            enumTest.name = "enum999";
            enumTest.Add(new Test_01() { id = 1, name = "tata", addressType = AddressType.None });
            enumTest.Add(new Test_01() { id = 2, name = "toto", addressType = AddressType.Bill });
            enumTest.Add(new Test_01() { id = 3, name = "tutu", addressType = AddressType.Ship });
            enumTest.Add(new Test_01() { id = 4, name = "titi", addressType = AddressType.None });
            enumTest.Add(new Test_01() { id = 5, name = "zaza", addressType = AddressType.Bill });
            enumTest.Add(new Test_01() { id = 6, name = "zozo", addressType = AddressType.Ship });
            return enumTest;
        }

        public static void XmlSerialize<T>(T value, string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (StreamWriter wr = new StreamWriter(file))
            {
                xs.Serialize(wr, value);
            }
        }

        public static T XmlDeserialize<T>(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (StreamReader sr = new StreamReader(file))
            {
                return (T)xs.Deserialize(sr);
            }
        }

    }

    [Flags]
    public enum AddressType
    {
        None = 0,
        Ship = 1,
        Bill = 2
    }

    public class Test_01
    {
        public int id;
        public string name;
        public AddressType addressType;
    }

    public class Test_02
    {
        public int id;
        public string name;
        public Test_01 test;
    }

    public class Test_03
    {
        [XmlAttribute()]
        public int id;
        [XmlAttribute()]
        public string name;
        [XmlAttribute()]
        public AddressType addressType;
    }

    public class Test_04
    {
        [XmlAttribute()]
        public int id;
        [XmlAttribute()]
        public string name;
        public Test_03 test;
    }

    public class Test_05 : IXmlSerializable
    {
        public int id;
        public string name;
        public Test_01 test;

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("id", this.id.ToString());
            writer.WriteElementString("name", this.name.ToString());

            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            XmlAttributes attr = new XmlAttributes();
            attr.XmlRoot = new XmlRootAttribute("test");
            overrides.Add(typeof(Test_01), attr);
            XmlSerializer xs = new XmlSerializer(typeof(Test_01), overrides);
            xs.Serialize(writer, this.test);
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }

    public class EnumTest_01 : IEnumerable<Test_01>, IEnumerator<Test_01>
    {
        public int id;
        public string name;
        private List<Test_01> _list;
        private IEnumerator<Test_01> _enum = null;

        public EnumTest_01()
        {
            _list = new List<Test_01>();
        }

        public void Dispose()
        {
        }

        public void Add(Test_01 test)
        {
            _list.Add(test);
        }

        public IEnumerator<Test_01> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public Test_01 Current
        {
            get { return _enum.Current; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _enum.Current; }
        }

        public bool MoveNext()
        {
            if (_enum == null)
                _enum = _list.GetEnumerator();
            return _enum.MoveNext();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class EnumTest_02 : IEnumerable<Test_01>, IEnumerator<Test_01>, IXmlSerializable
    {
        public int id;
        public string name;
        private List<Test_01> _list;
        private IEnumerator<Test_01> _enum = null;

        public EnumTest_02()
        {
            _list = new List<Test_01>();
        }

        public void Dispose()
        {
        }

        public void Add(Test_01 test)
        {
            _list.Add(test);
        }

        public IEnumerator<Test_01> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public Test_01 Current
        {
            get { return _enum.Current; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _enum.Current; }
        }

        public bool MoveNext()
        {
            if (_enum == null)
                _enum = _list.GetEnumerator();
            return _enum.MoveNext();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
