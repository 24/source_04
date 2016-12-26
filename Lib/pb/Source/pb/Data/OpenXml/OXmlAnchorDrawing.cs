using DocumentFormat.OpenXml;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
//using A = DocumentFormat.OpenXml.Drawing;
//using DW2010 = DocumentFormat.OpenXml.Office2010.Word.Drawing;

namespace pb.Data.OpenXml
{
    // Anchor for Floating DrawingML Object, <wp:anchor>
    // possible child : SimplePosition <wp:simplePos>, HorizontalPosition <wp:positionH>, VerticalPosition <wp:positionV>, Extent <wp:extent>, EffectExtent <wp:effectExtent>,
    //   WrapNone <wp:wrapNone>, WrapSquare <wp:wrapSquare>, WrapTight <wp:wrapTight>, WrapThrough <wp:wrapThrough>, WrapTopBottom <wp:wrapTopAndBottom>,
    //   DocProperties <wp:docPr>, NonVisualGraphicFrameDrawingProperties <wp:cNvGraphicFramePr>,
    //   Graphic <a:graphic>,
    //   RelativeWidth <wp14:sizeRelH>, RelativeHeight <wp14:sizeRelV>
    public class OXmlAnchorDrawing
    {
        private string _embeddedReference = null;
        private uint _id = 0;
        private string _name;
        private OXmlPictureElement _pictureElement = null;
        //private string _title;
        //private string _description;
        //private long _width;
        //private long _height;

        //private int _rotation = 0;                                                                     // Graphic
        //private bool _horizontalFlip = false;                                                          // Graphic
        //private bool _verticalFlip = false;                                                            // Graphic
        //private EnumValue<A.BlipCompressionValues> _compressionState = A.BlipCompressionValues.Print;  // Graphic
        //private EnumValue<A.ShapeTypeValues> _presetShape = A.ShapeTypeValues.Rectangle;               // Graphic

        //private uint _distanceFromTop = 0;                           // <wp:anchor distT> Distance From Text on Top Edge
        //private uint _distanceFromBottom = 0;                        // <wp:anchor distB> Distance From Text on Bottom Edge
        //private uint _distanceFromLeft = 0;                          // <wp:anchor distL> Distance From Text on Left Edge
        //private uint _distanceFromRight = 0;                         // <wp:anchor distR> Distance From Text on Right Edge
        //private uint _relativePosition = 0;                            // <wp:anchor relativeHeight> Relative Z-Ordering Position
        //private bool _behindDoc = false;                             // <wp:anchor behindDoc> Display Behind Document Text
        //private bool _layoutInCell = false;                          // <wp:anchor layoutInCell> Layout In Table Cell
        //private bool _allowOverlap = false;                          // <wp:anchor allowOverlap> Allow Objects to Overlap
        //private bool _locked = false;                                // <wp:anchor locked> Lock Anchor
        //private bool _hidden = false;                                // <wp:anchor hidden> Hidden
        //private string _editId = null;                               // <wp:anchor wp14:editId> editId (hexBinary value)
        //private string _anchorId = null;                             // <wp:anchor wp14:anchorId> anchorId (hexBinary value)
        //private bool _simplePos = false;                             // <wp:anchor simplePos> Page Positioning
        //private DW.SimplePosition _simplePosition = null;            // child <wp:simplePos x y> Simple Positioning Coordinates
        //private DW.HorizontalPosition _horizontalPosition = null;    // child <wp:positionH> Horizontal Positioning
        //private DW.VerticalPosition _verticalPosition = null;        // child <wp:positionV> Vertical Positioning
        ////private DW.EffectExtent _effectExtent = null;                // child <wp:effectExtent> Object Extents Including Effects
        //private OXmlAnchorWrap _wrap = null;                        // child WrapNone <wp:wrapNone>, WrapSquare <wp:wrapSquare>, WrapTight <wp:wrapTight>, WrapThrough <wp:wrapThrough>, WrapTopBottom <wp:wrapTopAndBottom>,
        //private DW2010.RelativeHeight _relativeHeight = null;        // child <wp14:sizeRelV> RelativeHeight (Office2010 or above)
        //private DW2010.RelativeWidth _relativeWidth = null;          // child <wp14:sizeRelH> RelativeWidth (Office2010 or above)
        ////private OpenXmlElement _wrapElement = null;

        //public int Rotation { get { return _rotation; } set { _rotation = value; } }
        //public bool HorizontalFlip { get { return _horizontalFlip; } set { _horizontalFlip = value; } }
        //public bool VerticalFlip { get { return _verticalFlip; } set { _verticalFlip = value; } }
        //public EnumValue<A.BlipCompressionValues> CompressionState { get { return _compressionState; } set { _compressionState = value; } }
        //public EnumValue<A.ShapeTypeValues> PresetShape { get { return _presetShape; } set { _presetShape = value; } }

        //public uint DistanceFromTop { get { return _distanceFromTop; } set { _distanceFromTop = value; } }
        //public uint DistanceFromBottom { get { return _distanceFromBottom; } set { _distanceFromBottom = value; } }
        //public uint DistanceFromLeft { get { return _distanceFromLeft; } set { _distanceFromLeft = value; } }
        //public uint DistanceFromRight { get { return _distanceFromRight; } set { _distanceFromRight = value; } }
        //public uint RelativePosition { get { return _relativePosition; } set { _relativePosition = value; } }
        //public bool BehindDoc { get { return _behindDoc; } set { _behindDoc = value; } }
        //public bool LayoutInCell { get { return _layoutInCell; } set { _layoutInCell = value; } }
        //public bool AllowOverlap { get { return _allowOverlap; } set { _allowOverlap = value; } }
        //public bool Locked { get { return _locked; } set { _locked = value; } }
        //public bool Hidden { get { return _hidden; } set { _hidden = value; } }
        //public string EditId { get { return _editId; } set { _editId = value; } }
        //public string AnchorId { get { return _anchorId; } set { _anchorId = value; } }
        //public OXmlAnchorWrap Wrap { get { return _wrap; } set { _wrap = value; } }

        //public AnchorDrawing(string embeddedReference, uint id, string name, long width, long height, string title = null, string description = null)
        public OXmlAnchorDrawing(string embeddedReference, uint id, string name, OXmlPictureElement pictureElement)
        {
            _embeddedReference = embeddedReference;
            _id = id;
            _name = name;
            _pictureElement = pictureElement;
            //_title = title;
            //_description = description;
            //_width = width;
            //_height = height;
            //UnsetSimplePosition();
        }

        public DW.Anchor Create()
        {
            int width;
            int height;
            zimg.CalculateImageWidthHeight(_pictureElement.File, _pictureElement.Width, _pictureElement.Height, out width, out height);
            long emuWidth = OXmlTools.PixelToEmus(width);
            long emuHeight = OXmlTools.PixelToEmus(height);

            //DW.Anchor anchor = new DW.Anchor()
            //{
            //    DistanceFromTop = _distanceFromTop,
            //    DistanceFromBottom = _distanceFromBottom,
            //    DistanceFromLeft = _distanceFromLeft,
            //    DistanceFromRight = _distanceFromRight,
            //    RelativeHeight = _relativePosition,
            //    BehindDoc = _behindDoc,
            //    LayoutInCell = _layoutInCell,
            //    AllowOverlap = _allowOverlap,
            //    Locked = _locked,
            //    Hidden = _hidden
            //};

            DW.Anchor anchor = new DW.Anchor();

            OXmlAnchorPictureDrawing pictureDrawing = (OXmlAnchorPictureDrawing)_pictureElement.PictureDrawing;

            if (pictureDrawing.AnchorId != null)
                anchor.AnchorId = pictureDrawing.AnchorId;
            if (pictureDrawing.EditId != null)
                anchor.EditId = pictureDrawing.EditId;

            if (pictureDrawing.DistanceFromTop != null)
                anchor.DistanceFromTop = (uint)pictureDrawing.DistanceFromTop;
            if (pictureDrawing.DistanceFromBottom != null)
                anchor.DistanceFromBottom = (uint)pictureDrawing.DistanceFromBottom;
            if (pictureDrawing.DistanceFromLeft != null)
                anchor.DistanceFromLeft = (uint)pictureDrawing.DistanceFromLeft;
            if (pictureDrawing.DistanceFromRight != null)
                anchor.DistanceFromRight = (uint)pictureDrawing.DistanceFromRight;

            OXmlHorizontalPosition oxmlHorizontalPosition = pictureDrawing.HorizontalPosition;
            DW.HorizontalPosition horizontalPosition = new DW.HorizontalPosition();
            horizontalPosition.RelativeFrom = oxmlHorizontalPosition.RelativeFrom;
            if (oxmlHorizontalPosition.PositionOffset != null)
                horizontalPosition.PositionOffset = new DW.PositionOffset(OXmlTools.PixelToEmus((int)oxmlHorizontalPosition.PositionOffset).ToString());
            if (oxmlHorizontalPosition.HorizontalAlignment != null)
                horizontalPosition.HorizontalAlignment = new DW.HorizontalAlignment(oxmlHorizontalPosition.HorizontalAlignment);
            anchor.HorizontalPosition = horizontalPosition;

            OXmlVerticalPosition oxmlVerticalPosition = pictureDrawing.VerticalPosition;
            DW.VerticalPosition verticalPosition = new DW.VerticalPosition();
            verticalPosition.RelativeFrom = oxmlVerticalPosition.RelativeFrom;
            if (oxmlVerticalPosition.PositionOffset != null)
                verticalPosition.PositionOffset = new DW.PositionOffset(OXmlTools.PixelToEmus((int)oxmlVerticalPosition.PositionOffset).ToString());
            if (oxmlVerticalPosition.VerticalAlignment != null)
                verticalPosition.VerticalAlignment = new DW.VerticalAlignment(oxmlVerticalPosition.VerticalAlignment);
            anchor.VerticalPosition = verticalPosition;

            if (pictureDrawing.EffectExtent != null)
                anchor.EffectExtent = pictureDrawing.EffectExtent.ToEffectExtent();

            // mandatory
            anchor.AllowOverlap = pictureDrawing.AllowOverlap;
            // mandatory
            anchor.BehindDoc = pictureDrawing.BehindDoc;
            // not mandatory
            anchor.Hidden = pictureDrawing.Hidden;
            // mandatory
            anchor.LayoutInCell = pictureDrawing.LayoutInCell;
            // mandatory
            anchor.Locked = pictureDrawing.Locked;
            // mandatory
            anchor.RelativeHeight = (uint)pictureDrawing.RelativeHeight;

            // warning <wp:anchor simplePos> and <wp:simplePos x="0" y="0"/> are mandatory
            if (pictureDrawing.SimplePosition != null)
            {
                anchor.SimplePos = true;
                // Simple Positioning Coordinates, <wp:simplePos x="0" y="0"/>
                anchor.SimplePosition = new DW.SimplePosition { X = pictureDrawing.SimplePosition.X, Y = pictureDrawing.SimplePosition.Y };
            }
            else
            {
                anchor.SimplePos = false;
                anchor.SimplePosition = new DW.SimplePosition { X = 0, Y = 0 };
            }

            // Inline Drawing Object Extents
            //   Cx : Extent Length ???
            //   Cy : Extent Width ???
            // <wp:extent cx="3576638" cy="3570696"/>
            anchor.Extent = new DW.Extent() { Cx = emuWidth, Cy = emuHeight };


            //if (_horizontalPosition != null)
            //    // Horizontal Positioning
            //    // <wp:positionH relativeFrom="margin">
            //    //   <wp:posOffset>-38099</wp:posOffset>
            //    // </wp:positionH>
            //    anchor.HorizontalPosition = _horizontalPosition;

            //if (_verticalPosition != null)
            //    // Vertical Positioning
            //    // <wp:positionV relativeFrom="paragraph">
            //    //   <wp:posOffset>723900</wp:posOffset>
            //    // </wp:positionV>
            //    anchor.VerticalPosition = _verticalPosition;


            // Object Extents Including Effects
            //   BottomEdge : Additional Extent on Bottom Edge (b)
            //   LeftEdge   : Additional Extent on Left Edge (l)
            //   RightEdge  : Additional Extent on Right Edge (r)
            //   TopEdge    : Additional Extent on Top Edge (t)
            // <wp:effectExtent b="0" l="0" r="0" t="0"/>
            //if (_effectExtent == null)
            //    _effectExtent = new DW.EffectExtent();
            // <wp:effectExtent> is not mandatory
            //if (_effectExtent != null)
            //    anchor.EffectExtent = _effectExtent;

            //if (_wrapElement == null)
            //    SetWrapNone();
            //anchor.AppendChild(_wrapElement);
            anchor.AppendChild(CreateWrap(pictureDrawing.Wrap));

            // <wp:docPr> Non-Visual Properties, possible child : HyperlinkOnClick <a:hlinkClick> HyperlinkOnHover <a:hlinkHover> NonVisualDrawingPropertiesExtensionList <a:extLst>
            // <wp:docPr id="1" name="Picture1" title="" descr=""/>
            // Title = _title
            anchor.AppendChild(new DW.DocProperties() { Id = _id, Name = _name, Description = _pictureElement.Description });

            Graphic graphic = new Graphic(_embeddedReference, emuWidth, emuHeight);
            graphic.Rotation = _pictureElement.Rotation;
            graphic.HorizontalFlip = _pictureElement.HorizontalFlip;
            graphic.VerticalFlip = _pictureElement.VerticalFlip;
            graphic.CompressionState = _pictureElement.CompressionState;
            graphic.PresetShape = _pictureElement.PresetShape;
            anchor.AppendChild(graphic.Create());

            //if (_relativeHeight != null)
            //    // <wp14:sizeRelV> RelativeHeight (Office2010 or above), 
            //    anchor.AppendChild(_relativeHeight);
            //if (_relativeWidth != null)
            //    // <wp14:sizeRelH> RelativeWidth (Office2010 or above), possible child : PercentageWidth <wp14:pctWidth>
            //    anchor.AppendChild(_relativeWidth);

            return anchor;
        }

        //public void SetSimplePosition(long x, long y)
        //{
        //    _simplePos = true;
        //    _simplePosition = new DW.SimplePosition() { X = x, Y = y };
        //}

        //public void UnsetSimplePosition()
        //{
        //    _simplePos = false;
        //    _simplePosition = new DW.SimplePosition() { X = 0, Y = 0 };
        //}

        //public void SetHorizontalPosition(EnumValue<DW.HorizontalRelativePositionValues> relativeFrom, long? positionOffset, string horizontalAlignment = null, string percentagePositionHeightOffset = null)
        //{
        //    // Horizontal Positioning
        //    // <wp:positionH relativeFrom="margin">
        //    //   <wp:posOffset>-38099</wp:posOffset>
        //    // </wp:positionH>
        //    // <wp:positionH relativeFrom="margin">
        //    //   <wp:align>left</wp:align>
        //    // </wp:positionH>
        //    // Horizontal Alignment (Ecma Office Open XML Part 1 - Fundamentals And Markup Language Reference.pdf - 20.4.3.1 page 3145)
        //    //   left, right, center, inside, outside

        //    _horizontalPosition = new DW.HorizontalPosition();
        //    // Horizontal Position Relative Base, <wp:positionH relativeFrom>
        //    // Margin - Page Margin ("margin"), Page - Page Edge ("page"), Column - Column ("column"), Character - Character ("character"), LeftMargin - Left Margin ("leftMargin"), RightMargin - Right Margin ("rightMargin")
        //    // InsideMargin - Inside Margin ("insideMargin"), OutsideMargin - Outside Margin ("outsideMargin")
        //    _horizontalPosition.RelativeFrom = relativeFrom;

        //    if (positionOffset != null)
        //        // Absolute Position Offset, <wp:posOffset>
        //        _horizontalPosition.PositionOffset = new DW.PositionOffset(positionOffset.ToString());

        //    if (horizontalAlignment != null)
        //        // Relative Horizontal Alignment, <wp:align>
        //        _horizontalPosition.HorizontalAlignment = new DW.HorizontalAlignment(horizontalAlignment);

        //    if (percentagePositionHeightOffset != null)
        //        // PercentagePositionHeightOffset, <wp14:pctPosHOffset>, available in Office2010 or above
        //        _horizontalPosition.PercentagePositionHeightOffset = new DW2010.PercentagePositionHeightOffset(percentagePositionHeightOffset);
        //}

        //public void SetVerticalPosition(EnumValue<DW.VerticalRelativePositionValues> relativeFrom, long? positionOffset, string verticalAlignment = null, string percentagePositionVerticalOffset = null)
        //{
        //    // <wp:positionV relativeFrom="paragraph">
        //    //   <wp:posOffset>723900</wp:posOffset>
        //    // </wp:positionV>
        //    // <wp:positionV relativeFrom="line">
        //    //   <wp:align>top</wp:align>
        //    // </wp:positionV>
        //    // Vertical Alignment (Ecma Office Open XML Part 1 - Fundamentals And Markup Language Reference.pdf - 20.4.3.2 page 3146)
        //    //   top, bottom, center, inside, outside

        //    // Vertical Positioning
        //    _verticalPosition = new DW.VerticalPosition();
        //    // Vertical Position Relative Base, <wp:positionV relativeFrom>
        //    _verticalPosition.RelativeFrom = relativeFrom;

        //    if (positionOffset != null)
        //        // PositionOffset, <wp:posOffset>
        //        _verticalPosition.PositionOffset = new DW.PositionOffset(positionOffset.ToString());

        //    if (verticalAlignment != null)
        //        // Relative Vertical Alignment, <wp:align>
        //        _verticalPosition.VerticalAlignment = new DW.VerticalAlignment(verticalAlignment);

        //    if (percentagePositionVerticalOffset != null)
        //        // PercentagePositionVerticalOffset <wp14:pctPosVOffset>, available in Office2010 or above
        //        _verticalPosition.PercentagePositionVerticalOffset = new DW2010.PercentagePositionVerticalOffset(percentagePositionVerticalOffset);
        //}

        //public void SetEffectExtent(long topEdge, long bottomEdge, long leftEdge, long rightEdge)
        //{
        //    _effectExtent = new DW.EffectExtent() { TopEdge = topEdge, BottomEdge = bottomEdge, LeftEdge = leftEdge, RightEdge = rightEdge };
        //}

        //public void SetRelativeWidth(EnumValue<DW2010.SizeRelativeHorizontallyValues> relativeFrom, int percentageWidth)
        //{
        //    _relativeWidth = new DW2010.RelativeWidth();
        //    // <wp14:sizeRelH relativeFrom>
        //    _relativeWidth.ObjectId = relativeFrom;
        //    // PercentageWidth, child <wp14:pctWidth>
        //    _relativeWidth.PercentageWidth = new DW2010.PercentageWidth(percentageWidth.ToString());
        //}

        //public void SetRelativeHeight(EnumValue<DW2010.SizeRelativeVerticallyValues> relativeFrom, int percentageHeight)
        //{
        //    _relativeHeight = new DW2010.RelativeHeight();
        //    // <wp14:sizeRelV relativeFrom>
        //    _relativeHeight.RelativeFrom = relativeFrom;
        //    // PercentageHeight, child <wp14:pctHeight>
        //    _relativeHeight.PercentageHeight = new DW2010.PercentageHeight(percentageHeight.ToString());
        //}

        //CreateWrap()
        //OpenXmlElement _wrapElement
        private OpenXmlElement CreateWrap(OXmlAnchorWrap wrap)
        {
            switch (wrap.WrapType)
            {
                case OXmlAnchorWrapType.WrapNone:
                    return CreateWrapNone();
                case OXmlAnchorWrapType.WrapSquare:
                    return CreateWrapSquare((OXmlAnchorWrapSquare)wrap);
                case OXmlAnchorWrapType.WrapThrough:
                    throw new PBException("missing wrapPolygon in zDocXAnchorWrapTight");
                    //return CreateWrapThrough((zDocXAnchorWrapTight)_wrap);
                case OXmlAnchorWrapType.WrapTight:
                    return CreateWrapTight((OXmlAnchorWrapTight)wrap);
                case OXmlAnchorWrapType.WrapTopAndBottom:
                    return CreateWrapTopAndBottom((OXmlAnchorWrapTopAndBottom)wrap);
                default:
                    throw new PBException($"unknow wrap type {wrap.WrapType}");
            }
        }

        //public void SetWrapNone()
        private OpenXmlElement CreateWrapNone()
        {
            // No Text Wrapping, <wp:wrapNone>
            //_wrapElement = new DW.WrapNone();
            return new DW.WrapNone();
        }

        //public void SetWrapSquare(EnumValue<DW.WrapTextValues> wrapText, uint distanceFromTop = 0, uint distanceFromBottom = 0, uint distanceFromLeft = 0, uint distanceFromRight = 0, DW.EffectExtent effectExtent = null)
        private OpenXmlElement CreateWrapSquare(OXmlAnchorWrapSquare wrap)
        {
            // <wp:wrapSquare wrapText="bothSides" distB="57150" distT="57150" distL="57150" distR="57150"/>
            // <wp:wrapSquare wrapText="bothSides"/>

            //Square Wrapping, <wp:wrapSquare>
            DW.WrapSquare wrapElement = new DW.WrapSquare();

            // Text Wrapping Location, <wp:wrapSquare wrapText>
            // BothSides - Both Sides ("bothSides"), Left - Left Side Only ("left"), Right - Right Side Only ("right"), Largest - Largest Side Only ("largest")
            //wrapElement.WrapText = wrapText;
            wrapElement.WrapText = wrap.WrapText;

            if (wrap.DistanceFromTop != null)
                // Distance From Text (Top), <wp:wrapSquare distT>
                wrapElement.DistanceFromTop = (uint)wrap.DistanceFromTop;
            if (wrap.DistanceFromBottom != null)
                // Distance From Text on Bottom Edge, <wp:wrapSquare distB>
                wrapElement.DistanceFromBottom = (uint)wrap.DistanceFromBottom;
            if (wrap.DistanceFromLeft != null)
                // Distance From Text on Left Edge, <wp:wrapSquare distL>
                wrapElement.DistanceFromLeft = (uint)wrap.DistanceFromLeft;
            if (wrap.DistanceFromRight != null)
                // Distance From Text on Right Edge, <wp:wrapSquare distR>
                wrapElement.DistanceFromRight = (uint)wrap.DistanceFromRight;

            if (wrap.EffectExtent != null)
                // Object Extents Including Effects <wp:effectExtent>
                //   BottomEdge : Additional Extent on Bottom Edge (b)
                //   LeftEdge   : Additional Extent on Left Edge (l)
                //   RightEdge  : Additional Extent on Right Edge (r)
                //   TopEdge    : Additional Extent on Top Edge (t)
                // <wp:effectExtent b="0" l="0" r="0" t="0"/>
                wrapElement.EffectExtent = wrap.EffectExtent.ToEffectExtent();

            //_wrapElement = wrapElement;
            return wrapElement;
        }

        //public void SetWrapThrough(EnumValue<DW.WrapTextValues> wrapText, DW.WrapPolygon wrapPolygon, uint distanceFromLeft = 0, uint distanceFromRight = 0)
        private OpenXmlElement CreateWrapThrough(OXmlAnchorWrapTight wrap)
        {
            // Through Wrapping, <wp:wrapThrough>
            DW.WrapThrough wrapElement = new DW.WrapThrough();

            // Text Wrapping Location, <wp:wrapTight wrapText>
            // BothSides - Both Sides ("bothSides"), Left - Left Side Only ("left"), Right - Right Side Only ("right"), Largest - Largest Side Only ("largest")
            wrapElement.WrapText = wrap.WrapText;

            // Tight Wrapping Extents Polygon, <wp:wrapPolygon>
            //wrapElement.WrapPolygon = wrapPolygon;

            if (wrap.DistanceFromLeft != null)
                // Distance From Text on Left Edge, <wp:wrapTight distL>
                wrapElement.DistanceFromLeft = (uint)wrap.DistanceFromLeft;
            if (wrap.DistanceFromRight != null)
                // Distance From Text on Right Edge, <wp:wrapTight distR>
                wrapElement.DistanceFromRight = (uint)wrap.DistanceFromRight;

            //_wrapElement = wrapElement;
            return wrapElement;
        }

        //public void SetWrapTight(EnumValue<DW.WrapTextValues> wrapText, DW.WrapPolygon wrapPolygon, uint distanceFromLeft = 0, uint distanceFromRight = 0)
        private OpenXmlElement CreateWrapTight(OXmlAnchorWrapTight wrap)
        {
            // Tight Wrapping, <wp:wrapTight>
            DW.WrapTight wrapElement = new DW.WrapTight();

            // Text Wrapping Location, <wp:wrapTight wrapText>
            // BothSides - Both Sides ("bothSides"), Left - Left Side Only ("left"), Right - Right Side Only ("right"), Largest - Largest Side Only ("largest")
            //wrapElement.WrapText = wrapText;
            wrapElement.WrapText = wrap.WrapText;

            if (wrap.DistanceFromLeft != null)
                // Distance From Text on Left Edge, <wp:wrapTight distL>
                wrapElement.DistanceFromLeft = (uint)wrap.DistanceFromLeft;
            if (wrap.DistanceFromRight != null)
                // Distance From Text on Right Edge, <wp:wrapTight distR>
                wrapElement.DistanceFromRight = (uint)wrap.DistanceFromRight;

            // Tight Wrapping Extents Polygon, <wp:wrapPolygon>
            //wrapElement.WrapPolygon = wrapPolygon;
            //wrapElement.WrapPolygon = CreateSquareWrapPolygon(wrap.SquareSize);
            wrapElement.WrapPolygon = wrap.WrapPolygon.ToWrapPolygon();

            //_wrapElement = wrapElement;
            return wrapElement;
        }

        //public void SetWrapTopAndBottom(uint distanceFromTop = 0, uint distanceFromBottom = 0, DW.EffectExtent effectExtent = null)
        private OpenXmlElement CreateWrapTopAndBottom(OXmlAnchorWrapTopAndBottom wrap)
        {
            // Top and Bottom Wrapping, <wp:wrapTopAndBottom>, possible child : EffectExtent <wp:effectExtent>
            DW.WrapTopBottom wrapElement = new DW.WrapTopBottom();

            // Distance From Text on Top Edge, <wp:wrapTopAndBottom distT>
            //wrapElement.DistanceFromTop = distanceFromTop;
            if (wrap.DistanceFromTop != null)
                wrapElement.DistanceFromTop = (uint)wrap.DistanceFromTop;

            // Distance From Text on Bottom Edge, <wp:wrapTopAndBottom distB>
            //wrapElement.DistanceFromBottom = distanceFromBottom;
            if (wrap.DistanceFromBottom != null)
                wrapElement.DistanceFromBottom = (uint)wrap.DistanceFromBottom;

            if (wrap.EffectExtent != null)
                // Wrapping Boundaries <wp:effectExtent>
                //wrapElement.EffectExtent = effectExtent;
                wrapElement.EffectExtent = wrap.EffectExtent.ToEffectExtent();

            //_wrapElement = wrapElement;
            return wrapElement;
        }

        //private static DW.WrapPolygon CreateSquareWrapPolygon(long size, long startPointX = 0, long startPointY = 0, bool edited = false)
        //{
        //    // Tight Wrapping Extents Polygon, <wp:wrapPolygon>, possible child : StartPoint <wp:start>, LineTo <wp:lineTo>
        //    // Edited : Wrapping Points Modified, <wp:wrapPolygon edited>
        //    DW.WrapPolygon polygon = new DW.WrapPolygon() { Edited = edited };
        //    // Wrapping Polygon Start, <wp:start x="0" y="0">
        //    polygon.StartPoint = new DW.StartPoint() { X = startPointX, Y = startPointY };
        //    long x = startPointX;
        //    long y = startPointY;
        //    // Wrapping Polygon Line End Position, <wp:lineTo x="0" y="0"/>
        //    y += size; polygon.AppendChild(new DW.LineTo() { X = x, Y = y });
        //    x += size; polygon.AppendChild(new DW.LineTo() { X = x, Y = y });
        //    y -= size; polygon.AppendChild(new DW.LineTo() { X = x, Y = y });
        //    x -= size; polygon.AppendChild(new DW.LineTo() { X = x, Y = y });
        //    return polygon;
        //}

        //public void SetWrapTopBottom(uint distanceFromTop = 0, uint distanceFromBottom = 0, DW.EffectExtent effectExtent = null)
        //{
        //    // Top and Bottom Wrapping, <wp:wrapTopAndBottom>
        //    DW.WrapTopBottom wrap = new DW.WrapTopBottom();

        //    if (distanceFromTop != 0)
        //        // Distance From Text (Top), <wp:wrapSquare distT>
        //        wrap.DistanceFromTop = distanceFromTop;
        //    if (distanceFromBottom != 0)
        //        // Distance From Text on Bottom Edge, <wp:wrapSquare distB>
        //        wrap.DistanceFromBottom = distanceFromBottom;

        //    if (effectExtent != null)
        //        // Object Extents Including Effects <wp:effectExtent>
        //        //   BottomEdge : Additional Extent on Bottom Edge (b)
        //        //   LeftEdge   : Additional Extent on Left Edge (l)
        //        //   RightEdge  : Additional Extent on Right Edge (r)
        //        //   TopEdge    : Additional Extent on Top Edge (t)
        //        // <wp:effectExtent b="0" l="0" r="0" t="0"/>
        //        wrap.EffectExtent = effectExtent;

        //    _wrapElement = wrap;
        //}
    }
}
