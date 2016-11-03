using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DW2010 = DocumentFormat.OpenXml.Office2010.Word.Drawing;

//namespace Test.Test_OpenXml
namespace pb.Data.OpenXml
{
    // Anchor for Floating DrawingML Object, <wp:anchor>
    // possible child : SimplePosition <wp:simplePos>, HorizontalPosition <wp:positionH>, VerticalPosition <wp:positionV>, Extent <wp:extent>, EffectExtent <wp:effectExtent>,
    //   WrapNone <wp:wrapNone>, WrapSquare <wp:wrapSquare>, WrapTight <wp:wrapTight>, WrapThrough <wp:wrapThrough>, WrapTopBottom <wp:wrapTopAndBottom>,
    //   DocProperties <wp:docPr>, NonVisualGraphicFrameDrawingProperties <wp:cNvGraphicFramePr>,
    //   Graphic <a:graphic>,
    //   RelativeWidth <wp14:sizeRelH>, RelativeHeight <wp14:sizeRelV>
    public class AnchorDrawing
    {
        private string _embeddedReference = null;
        private uint _id = 0;
        private string _name;
        private string _title;
        private string _description;
        private long _width;
        private long _height;

        private int _rotation = 0;                                                                     // Graphic
        private bool _horizontalFlip = false;                                                          // Graphic
        private bool _verticalFlip = false;                                                            // Graphic
        private EnumValue<A.BlipCompressionValues> _compressionState = A.BlipCompressionValues.Print;  // Graphic
        private EnumValue<A.ShapeTypeValues> _presetShape = A.ShapeTypeValues.Rectangle;               // Graphic

        private uint _distanceFromTop = 0;                           // <wp:anchor distT> Distance From Text on Top Edge
        private uint _distanceFromBottom = 0;                        // <wp:anchor distB> Distance From Text on Bottom Edge
        private uint _distanceFromLeft = 0;                          // <wp:anchor distL> Distance From Text on Left Edge
        private uint _distanceFromRight = 0;                         // <wp:anchor distR> Distance From Text on Right Edge
        private uint _relativePosition = 0;                            // <wp:anchor relativeHeight> Relative Z-Ordering Position
        private bool _behindDoc = false;                             // <wp:anchor behindDoc> Display Behind Document Text
        private bool _layoutInCell = false;                          // <wp:anchor layoutInCell> Layout In Table Cell
        private bool _allowOverlap = false;                          // <wp:anchor allowOverlap> Allow Objects to Overlap
        private bool _locked = false;                                // <wp:anchor locked> Lock Anchor
        private bool _hidden = false;                                // <wp:anchor hidden> Hidden
        private string _editId = null;                               // <wp:anchor wp14:editId> editId (hexBinary value)
        private string _anchorId = null;                             // <wp:anchor wp14:anchorId> anchorId (hexBinary value)
        private bool _simplePos = false;                             // <wp:anchor simplePos> Page Positioning
        private DW.SimplePosition _simplePosition = null;            // child <wp:simplePos x y> Simple Positioning Coordinates
        private DW.HorizontalPosition _horizontalPosition = null;    // child <wp:positionH> Horizontal Positioning
        private DW.VerticalPosition _verticalPosition = null;        // child <wp:positionV> Vertical Positioning
        private DW.EffectExtent _effectExtent = null;                // child <wp:effectExtent> Object Extents Including Effects
        private DW2010.RelativeHeight _relativeHeight = null;        // child <wp14:sizeRelV> RelativeHeight (Office2010 or above)
        private DW2010.RelativeWidth _relativeWidth = null;          // child <wp14:sizeRelH> RelativeWidth (Office2010 or above)
        private OpenXmlElement _wrap = null;                         // child WrapNone <wp:wrapNone>, WrapSquare <wp:wrapSquare>, WrapTight <wp:wrapTight>, WrapThrough <wp:wrapThrough>, WrapTopBottom <wp:wrapTopAndBottom>,

        public int Rotation { get { return _rotation; } set { _rotation = value; } }
        public bool HorizontalFlip { get { return _horizontalFlip; } set { _horizontalFlip = value; } }
        public bool VerticalFlip { get { return _verticalFlip; } set { _verticalFlip = value; } }
        public EnumValue<A.BlipCompressionValues> CompressionState { get { return _compressionState; } set { _compressionState = value; } }
        public EnumValue<A.ShapeTypeValues> PresetShape { get { return _presetShape; } set { _presetShape = value; } }

        public uint DistanceFromTop { get { return _distanceFromTop; } set { _distanceFromTop = value; } }
        public uint DistanceFromBottom { get { return _distanceFromBottom; } set { _distanceFromBottom = value; } }
        public uint DistanceFromLeft { get { return _distanceFromLeft; } set { _distanceFromLeft = value; } }
        public uint DistanceFromRight { get { return _distanceFromRight; } set { _distanceFromRight = value; } }
        public uint RelativePosition { get { return _relativePosition; } set { _relativePosition = value; } }
        public bool BehindDoc { get { return _behindDoc; } set { _behindDoc = value; } }
        public bool LayoutInCell { get { return _layoutInCell; } set { _layoutInCell = value; } }
        public bool AllowOverlap { get { return _allowOverlap; } set { _allowOverlap = value; } }
        public bool Locked { get { return _locked; } set { _locked = value; } }
        public bool Hidden { get { return _hidden; } set { _hidden = value; } }
        public string EditId { get { return _editId; } set { _editId = value; } }
        public string AnchorId { get { return _anchorId; } set { _anchorId = value; } }

        public AnchorDrawing(string embeddedReference, uint id, string name, long width, long height, string title = null, string description = null)
        {
            _embeddedReference = embeddedReference;
            _id = id;
            _name = name;
            _title = title;
            _description = description;
            _width = width;
            _height = height;
            UnsetSimplePosition();
        }

        public DW.Anchor Create()
        {
            DW.Anchor anchor = new DW.Anchor()
            {
                DistanceFromTop = _distanceFromTop,
                DistanceFromBottom = _distanceFromBottom,
                DistanceFromLeft = _distanceFromLeft,
                DistanceFromRight = _distanceFromRight,
                RelativeHeight = _relativePosition,
                BehindDoc = _behindDoc,
                LayoutInCell = _layoutInCell,
                AllowOverlap = _allowOverlap,
                Locked = _locked,
                Hidden = _hidden
            };

            if (_editId != null)
                anchor.EditId = _editId;
            if (_anchorId != null)
                anchor.AnchorId = _anchorId;

            // warning <wp:anchor simplePos> and <wp:simplePos x="0" y="0"/> are mandatory
            anchor.SimplePos = _simplePos;
            // Simple Positioning Coordinates
            // <wp:simplePos x="0" y="0"/>
            anchor.SimplePosition = _simplePosition;

            if (_horizontalPosition != null)
                // Horizontal Positioning
                // <wp:positionH relativeFrom="margin">
                //   <wp:posOffset>-38099</wp:posOffset>
                // </wp:positionH>
                anchor.HorizontalPosition = _horizontalPosition;

            if (_verticalPosition != null)
                // Vertical Positioning
                // <wp:positionV relativeFrom="paragraph">
                //   <wp:posOffset>723900</wp:posOffset>
                // </wp:positionV>
                anchor.VerticalPosition = _verticalPosition;

            // Inline Drawing Object Extents
            //   Cx : Extent Length ???
            //   Cy : Extent Width ???
            // <wp:extent cx="3576638" cy="3570696"/>
            anchor.Extent = new DW.Extent() { Cx = _width, Cy = _height };

            // Object Extents Including Effects
            //   BottomEdge : Additional Extent on Bottom Edge (b)
            //   LeftEdge   : Additional Extent on Left Edge (l)
            //   RightEdge  : Additional Extent on Right Edge (r)
            //   TopEdge    : Additional Extent on Top Edge (t)
            // <wp:effectExtent b="0" l="0" r="0" t="0"/>
            //if (_effectExtent == null)
            //    _effectExtent = new DW.EffectExtent();
            // <wp:effectExtent> is not mandatory
            if (_effectExtent != null)
                anchor.EffectExtent = _effectExtent;

            if (_wrap == null)
                SetWrapNone();
            anchor.AppendChild(_wrap);

            // <wp:docPr> Non-Visual Properties, possible child : HyperlinkOnClick <a:hlinkClick> HyperlinkOnHover <a:hlinkHover> NonVisualDrawingPropertiesExtensionList <a:extLst>
            // <wp:docPr id="1" name="Picture1" title="" descr=""/>
            anchor.AppendChild(new DW.DocProperties() { Id = _id, Name = _name, Title = _title, Description = _description });

            Graphic graphic = new Graphic(_embeddedReference, _width, _height);
            graphic.Rotation = _rotation;
            graphic.HorizontalFlip = _horizontalFlip;
            graphic.VerticalFlip = _verticalFlip;
            graphic.CompressionState = _compressionState;
            graphic.PresetShape = _presetShape;
            anchor.AppendChild(graphic.Create());

            if (_relativeHeight != null)
                // <wp14:sizeRelV> RelativeHeight (Office2010 or above), 
                anchor.AppendChild(_relativeHeight);
            if (_relativeWidth != null)
                // <wp14:sizeRelH> RelativeWidth (Office2010 or above), possible child : PercentageWidth <wp14:pctWidth>
                anchor.AppendChild(_relativeWidth);

            return anchor;
        }

        public void SetSimplePosition(long x, long y)
        {
            _simplePos = true;
            _simplePosition = new DW.SimplePosition() { X = x, Y = y };
        }

        public void UnsetSimplePosition()
        {
            _simplePos = false;
            _simplePosition = new DW.SimplePosition() { X = 0, Y = 0 };
        }

        public void SetHorizontalPosition(EnumValue<DW.HorizontalRelativePositionValues> relativeFrom, long positionOffset, string horizontalAlignment = null, string percentagePositionHeightOffset = null)
        {
            // Horizontal Positioning
            // <wp:positionH relativeFrom="margin">
            //   <wp:posOffset>-38099</wp:posOffset>
            // </wp:positionH>

            _horizontalPosition = new DW.HorizontalPosition();
            // Horizontal Position Relative Base, <wp:positionH relativeFrom>
            // Margin - Page Margin ("margin"), Page - Page Edge ("page"), Column - Column ("column"), Character - Character ("character"), LeftMargin - Left Margin ("leftMargin"), RightMargin - Right Margin ("rightMargin")
            // InsideMargin - Inside Margin ("insideMargin"), OutsideMargin - Outside Margin ("outsideMargin")
            _horizontalPosition.RelativeFrom = relativeFrom;
            // Absolute Position Offset, <wp:posOffset>
            _horizontalPosition.PositionOffset = new DW.PositionOffset(positionOffset.ToString());
            if (horizontalAlignment != null)
                // Relative Horizontal Alignment, <wp:align>
                _horizontalPosition.HorizontalAlignment = new DW.HorizontalAlignment(horizontalAlignment);
            if (percentagePositionHeightOffset != null)
                // PercentagePositionHeightOffset, <wp14:pctPosHOffset>, available in Office2010 or above
                _horizontalPosition.PercentagePositionHeightOffset = new DW2010.PercentagePositionHeightOffset(percentagePositionHeightOffset);
        }

        public void SetVerticalPosition(EnumValue<DW.VerticalRelativePositionValues> relativeFrom, long positionOffset, string verticalAlignment = null, string percentagePositionVerticalOffset = null)
        {
            // <wp:positionV relativeFrom="paragraph">
            //   <wp:posOffset>723900</wp:posOffset>
            // </wp:positionV>

            // Vertical Positioning
            _verticalPosition = new DW.VerticalPosition();
            // Vertical Position Relative Base, <wp:positionV relativeFrom>
            _verticalPosition.RelativeFrom = relativeFrom;
            // PositionOffset, <wp:posOffset>
            _verticalPosition.PositionOffset = new DW.PositionOffset(positionOffset.ToString());
            if (verticalAlignment != null)
                // Relative Vertical Alignment, <wp:align>
                _verticalPosition.VerticalAlignment = new DW.VerticalAlignment(verticalAlignment);
            if (percentagePositionVerticalOffset != null)
                // PercentagePositionVerticalOffset <wp14:pctPosVOffset>, available in Office2010 or above
                _verticalPosition.PercentagePositionVerticalOffset = new DW2010.PercentagePositionVerticalOffset(percentagePositionVerticalOffset);
        }

        public void SetEffectExtent(long topEdge, long bottomEdge, long leftEdge, long rightEdge)
        {
            _effectExtent = new DW.EffectExtent() { TopEdge = topEdge, BottomEdge = bottomEdge, LeftEdge = leftEdge, RightEdge = rightEdge };
        }

        public void SetRelativeWidth(EnumValue<DW2010.SizeRelativeHorizontallyValues> relativeFrom, int percentageWidth)
        {
            _relativeWidth = new DW2010.RelativeWidth();
            // <wp14:sizeRelH relativeFrom>
            _relativeWidth.ObjectId = relativeFrom;
            // PercentageWidth, child <wp14:pctWidth>
            _relativeWidth.PercentageWidth = new DW2010.PercentageWidth(percentageWidth.ToString());
        }

        public void SetRelativeHeight(EnumValue<DW2010.SizeRelativeVerticallyValues> relativeFrom, int percentageHeight)
        {
            _relativeHeight = new DW2010.RelativeHeight();
            // <wp14:sizeRelV relativeFrom>
            _relativeHeight.RelativeFrom = relativeFrom;
            // PercentageHeight, child <wp14:pctHeight>
            _relativeHeight.PercentageHeight = new DW2010.PercentageHeight(percentageHeight.ToString());
        }

        public void SetWrapNone()
        {
            // No Text Wrapping, <wp:wrapNone>
            _wrap = new DW.WrapNone();
        }

        public void SetWrapSquare(EnumValue<DW.WrapTextValues> wrapText, uint distanceFromTop = 0, uint distanceFromBottom = 0, uint distanceFromLeft = 0, uint distanceFromRight = 0, DW.EffectExtent effectExtent = null)
        {
            // <wp:wrapSquare wrapText="bothSides" distB="57150" distT="57150" distL="57150" distR="57150"/>
            // <wp:wrapSquare wrapText="bothSides"/>

            //Square Wrapping, <wp:wrapSquare>
            DW.WrapSquare wrap = new DW.WrapSquare();

            // Text Wrapping Location, <wp:wrapSquare wrapText>
            // BothSides - Both Sides ("bothSides"), Left - Left Side Only ("left"), Right - Right Side Only ("right"), Largest - Largest Side Only ("largest")
            wrap.WrapText = wrapText;

            if (distanceFromTop != 0)
                // Distance From Text (Top), <wp:wrapSquare distT>
                wrap.DistanceFromTop = distanceFromTop;
            if (distanceFromBottom != 0)
                // Distance From Text on Bottom Edge, <wp:wrapSquare distB>
                wrap.DistanceFromBottom = distanceFromBottom;
            if (distanceFromLeft != 0)
                // Distance From Text on Left Edge, <wp:wrapSquare distL>
                wrap.DistanceFromLeft = distanceFromLeft;
            if (distanceFromRight != 0)
                // Distance From Text on Right Edge, <wp:wrapSquare distR>
                wrap.DistanceFromRight = distanceFromRight;

            if (effectExtent != null)
                // Object Extents Including Effects <wp:effectExtent>
                //   BottomEdge : Additional Extent on Bottom Edge (b)
                //   LeftEdge   : Additional Extent on Left Edge (l)
                //   RightEdge  : Additional Extent on Right Edge (r)
                //   TopEdge    : Additional Extent on Top Edge (t)
                // <wp:effectExtent b="0" l="0" r="0" t="0"/>
                wrap.EffectExtent = effectExtent;

            _wrap = wrap;
        }

        public void SetWrapTight(EnumValue<DW.WrapTextValues> wrapText, DW.WrapPolygon wrapPolygon, uint distanceFromLeft = 0, uint distanceFromRight = 0)
        {
            // Tight Wrapping, <wp:wrapTight>
            DW.WrapTight wrap = new DW.WrapTight();

            // Text Wrapping Location, <wp:wrapTight wrapText>
            // BothSides - Both Sides ("bothSides"), Left - Left Side Only ("left"), Right - Right Side Only ("right"), Largest - Largest Side Only ("largest")
            wrap.WrapText = wrapText;

            // Tight Wrapping Extents Polygon, <wp:wrapPolygon>
            wrap.WrapPolygon = wrapPolygon;

            if (distanceFromLeft != 0)
                // Distance From Text on Left Edge, <wp:wrapTight distL>
                wrap.DistanceFromLeft = distanceFromLeft;
            if (distanceFromRight != 0)
                // Distance From Text on Right Edge, <wp:wrapTight distR>
                wrap.DistanceFromRight = distanceFromRight;

            _wrap = wrap;
        }

        public void SetWrapTopAndBottom(uint distanceFromTop = 0, uint distanceFromBottom = 0, DW.EffectExtent effectExtent = null)
        {
            // Top and Bottom Wrapping, <wp:wrapTopAndBottom>, possible child : EffectExtent <wp:effectExtent>
            DW.WrapTopBottom wrap = new DW.WrapTopBottom();

            // Distance From Text on Top Edge, <wp:wrapTopAndBottom distT>
            wrap.DistanceFromTop = distanceFromTop;
            // Distance From Text on Bottom Edge, <wp:wrapTopAndBottom distB>
            wrap.DistanceFromBottom = distanceFromBottom;

            if (effectExtent != null)
                // Wrapping Boundaries <wp:effectExtent>
                wrap.EffectExtent = effectExtent;

            _wrap = wrap;
        }

        public static DW.WrapPolygon CreateSquareWrapPolygon(long size, long startPointX = 0, long startPointY = 0, bool edited = false)
        {
            // Tight Wrapping Extents Polygon, <wp:wrapPolygon>, possible child : StartPoint <wp:start>, LineTo <wp:lineTo>
            // Edited : Wrapping Points Modified, <wp:wrapPolygon edited>
            DW.WrapPolygon polygon = new DW.WrapPolygon() { Edited = edited };
            // Wrapping Polygon Start, <wp:start x="0" y="0">
            polygon.StartPoint = new DW.StartPoint() { X = startPointX, Y = startPointY };
            long x = startPointX;
            long y = startPointY;
            // Wrapping Polygon Line End Position, <wp:lineTo x="0" y="0"/>
            y += size; polygon.AppendChild(new DW.LineTo() { X = x, Y = y });
            x += size; polygon.AppendChild(new DW.LineTo() { X = x, Y = y });
            y -= size; polygon.AppendChild(new DW.LineTo() { X = x, Y = y });
            x -= size; polygon.AppendChild(new DW.LineTo() { X = x, Y = y });
            return polygon;
        }

        public void SetWrapThrough(EnumValue<DW.WrapTextValues> wrapText, DW.WrapPolygon wrapPolygon, uint distanceFromLeft = 0, uint distanceFromRight = 0)
        {
            // Through Wrapping, <wp:wrapThrough>
            DW.WrapThrough wrap = new DW.WrapThrough();

            // Text Wrapping Location, <wp:wrapTight wrapText>
            // BothSides - Both Sides ("bothSides"), Left - Left Side Only ("left"), Right - Right Side Only ("right"), Largest - Largest Side Only ("largest")
            wrap.WrapText = wrapText;

            // Tight Wrapping Extents Polygon, <wp:wrapPolygon>
            wrap.WrapPolygon = wrapPolygon;

            if (distanceFromLeft != 0)
                // Distance From Text on Left Edge, <wp:wrapTight distL>
                wrap.DistanceFromLeft = distanceFromLeft;
            if (distanceFromRight != 0)
                // Distance From Text on Right Edge, <wp:wrapTight distR>
                wrap.DistanceFromRight = distanceFromRight;

            _wrap = wrap;
        }

        public void SetWrapTopBottom(uint distanceFromTop = 0, uint distanceFromBottom = 0, DW.EffectExtent effectExtent = null)
        {
            // Top and Bottom Wrapping, <wp:wrapTopAndBottom>
            DW.WrapTopBottom wrap = new DW.WrapTopBottom();

            if (distanceFromTop != 0)
                // Distance From Text (Top), <wp:wrapSquare distT>
                wrap.DistanceFromTop = distanceFromTop;
            if (distanceFromBottom != 0)
                // Distance From Text on Bottom Edge, <wp:wrapSquare distB>
                wrap.DistanceFromBottom = distanceFromBottom;

            if (effectExtent != null)
                // Object Extents Including Effects <wp:effectExtent>
                //   BottomEdge : Additional Extent on Bottom Edge (b)
                //   LeftEdge   : Additional Extent on Left Edge (l)
                //   RightEdge  : Additional Extent on Right Edge (r)
                //   TopEdge    : Additional Extent on Top Edge (t)
                // <wp:effectExtent b="0" l="0" r="0" t="0"/>
                wrap.EffectExtent = effectExtent;

            _wrap = wrap;
        }

        public DW.WrapPolygon CreateWrapPolygon()
        {
            throw new PBException("not implemented");
        }
    }
}
