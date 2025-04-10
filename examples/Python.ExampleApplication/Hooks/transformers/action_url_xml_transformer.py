from xml.etree import ElementTree
from tableau_migration import (
    IPublishableWorkbook,
    XmlContentTransformerBase
)

class ActionUrlXmlTransformer(XmlContentTransformerBase[IPublishableWorkbook]):
    
    def needs_xml_transforming(self, ctx: IPublishableWorkbook) -> bool:
        # Returning false prevents the transform method from running.
        # Implementing this method potentially allows workbooks to migrate
        # without loading the file into memory, improving migration speed.
        return True

    def transform(self, ctx: IPublishableWorkbook, xml: ElementTree.Element) -> None:
        # Changes to the XML are saved back to the workbook file before publishing.
        for action_link in xml.findall("actions/*/link"):
            action_link.set("expression", action_link.get("expression").replace("127.0.0.1", "testserver"))
