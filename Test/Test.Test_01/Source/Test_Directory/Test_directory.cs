Test_01();
Test_02();
Test_xmlSerialize_01();
Test_xmlDeserialize_01();
Test_xmlSerialize_02();
Test_xmlDeserialize_02();
Test_xmlSerialize_03();
Test_xmlDeserialize_03();

XmlSerialize(, "test_02_01.xml");
XmlSerialize(, "test_02_01.xml");
XmlSerialize(, "test_02_01.xml");
XmlSerialize(, "test_02_01.xml");
/********** result
***********/

XmlSerialize(new Test_01() { id = 1, name = "toto", addressType = AddressType.Bill | AddressType.Ship }, "test_01_01.xml");
/********** result
<?xml version="1.0" encoding="utf-8"?>
<Test_01 xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <id>1</id>
  <name>toto</name>
  <addressType>Ship Bill</addressType>
</Test_01>
***********/

XmlSerialize(new Test_02() { id = 1, name = "toto", test = new Test_01() { id = 2, name = "tata", addressType = AddressType.Bill | AddressType.Ship } },
  "test_02_01.xml");
/********** result
<?xml version="1.0" encoding="utf-8"?>
<Test_02 xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <id>1</id>
  <name>toto</name>
  <test>
    <id>2</id>
    <name>tata</name>
    <addressType>Ship Bill</addressType>
  </test>
</Test_02>
***********/

XmlSerialize(new Test_04() { id = 1, name = "toto", test = new Test_03() { id = 2, name = "tata", addressType = AddressType.Bill | AddressType.Ship } },
  "test_04_01.xml");
/********** result
<?xml version="1.0" encoding="utf-8"?>
<Test_04 xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" id="1" name="toto">
  <test id="2" name="tata" addressType="Ship Bill" />
</Test_04>
***********/

