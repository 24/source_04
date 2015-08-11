using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using pb.IO;

// Embedding and Hiding Files in PDF Documents http://blog.didierstevens.com/2009/07/01/embedding-and-hiding-files-in-pdf-documents/

// from PDFReference.pdf http://partners.adobe.com/public/developer/en/pdf/PDFReference.pdf
// PDF Reference and Adobe Extensions to the PDF Specification http://www.adobe.com/devnet/pdf/pdf_reference.html
// PDF reference 1.7 http://wwwimages.adobe.com/www.adobe.com/content/dam/Adobe/en/devnet/pdf/pdfs/PDF32000_2008.pdf
//
// TABLE 3.20 Compatibility operators page 95
// operands                operator   description
// —                       BX         (PDF 1.1) Begin a compatibility section. Unrecognized operators (along with their operands)
//                                    will be ignored without error until the balancing EX operator is encountered.
// —                       EX         (PDF 1.1) End a compatibility section begun by a balancing BX operator.
// TABLE 3.20 Compatibility operators page 95
// operands                operator   postscript                 description
// —                       BX                                    (PDF 1.1) Begin compatibility section
// —                       EX                                    (PDF 1.1) End compatibility section
//
// TABLE 4.7 Graphics state operators page 156
// operands                operator   description
// —                       q          Save the current graphics state on the graphics state stack (see “Graphics State Stack” on page 152).
// —                       Q          Restore the graphics state by removing the most recently saved state from the stack and making it the current state (see “Graphics State Stack” on page 152).
// a b c d e f             cm         Modify the current transformation matrix (CTM) by concatenating the specified matrix (see Section 4.2.1, “Coordinate Spaces”).
//                                    Although the operands specify a matrix, they are written as six separate numbers, not as an array.
// lineWidth               w          Set the line width in the graphics state (see “Line Width” on page 152).
// lineCap                 J          Set the line cap style in the graphics state (see “Line Cap Style” on page 153).
// lineJoin                j          Set the line join style in the graphics state (see “Line Join Style” on page 153).
// miterLimit              M          Set the miter limit in the graphics state (see “Miter Limit” on page 153).
// dashArray dashPhase     d          Set the line dash pattern in the graphics state (see “Line Dash Pattern” on page 155).
// intent                  ri         (PDF 1.1) Set the color rendering intent in the graphics state (see “Rendering Intents” on page 197).
// flatness                i          Set the flatness tolerance in the graphics state (see Section 6.5.1, “Flatness Tolerance”).
//                                    flatness is a number in the range 0 to 100; a value of 0 specifies the output device’s default flatness tolerance.
// dictName                gs         (PDF 1.2) Set the specified parameters in the graphics state. dictName is the name of a graphics state parameter dictionary
//                                    in the ExtGState subdictionary of the current resource dictionary (see the next section).
// TABLE 4.7 Graphics state operators page 156
// operands                operator   postscript                 description
// —                       q          gsave                      Save graphics state
// —                       Q          grestore                   Restore graphics state
// a b c d e f             cm         concat                     Concatenate matrix to current transformation matrix
// lineWidth               w          setlinewidth               Set line width
// lineCap                 J          setlinecap                 Set line cap style
// lineJoin                j          setlinejoin                Set line join style
// miterLimit              M          setmiterlimit              Set miter limit
// dashArray dashPhase     d          setdash                    Set line dash pattern
// intent                  ri                                    Set color rendering intent
// flatness                i          setflat                    Set flatness tolerance
// dictName                gs                                    (PDF 1.2) Set parameters from graphics state parameter dictionary
//
// TABLE 4.9 Path construction operators page 163
// operands                operator   description
// x y                     m          Begin a new subpath by moving the current point to coordinates (x, y), omitting any connecting line segment.
//                                    If the previous path construction operator in the current path was also m, the new m overrides it;
//                                    no vestige of the previous m operation remains in the path.
// x y                     l          (lowercase L) Append a straight line segment from the current point to the point (x, y). The new current point is (x, y).
// x1 y1 x2 y2 x3 y3       c          Append a cubic Bézier curve to the current path. The curve extends from the current point to the point (x3, y3), using (x1, y1) and
//                                    (x2, y2) as the Bézier control points (see “Cubic Bézier Curves,” below). The new current point is (x3, y3).
// x2 y2 x3 y3             v          Append a cubic Bézier curve to the current path. The curve extends from the current point to the point (x3, y3), using the current point
//                                    and (x2, y2) as the Bézier control points (see “Cubic Bézier Curves,” below). The new current point is (x3, y3).
// x1 y1 x3 y3             y          Append a cubic Bézier curve to the current path. The curve extends from the current point to the point (x3, y3), using (x1, y1) and (x3, y3)
//                                    as the Bézier control points (see “Cubic Bézier Curves,” below). The new current point is (x3, y3).
// —                       h          Close the current subpath by appending a straight line segment from the current point to the starting point of the subpath.
//                                    This operator terminates the current subpath; appending another segment to the current path will begin a new subpath, even if the new
//                                    segment begins at the endpoint reached by the h operation. If the current subpath is already closed, h does nothing
// x y width height        re         Append a rectangle to the current path as a complete subpath, with lower-left corner (x, y) and dimensions width and height in user space.
// TABLE 4.9 Path construction operators page 163
// operands                operator   postscript                 description
// x y                     m          moveto                     Begin new subpath
// x y                     l          lineto                     Append straight line segment to path
// x1 y1 x2 y2 x3 y3       c          curveto                    Append curved segment to path (three control points)
// x2 y2 x3 y3             v          curveto                    Append curved segment to path (initial point replicated)
// x1 y1 x3 y3             y          curveto                    Append curved segment to path (final point replicated)
// —                       h          closepath                  Close subpath
// x y width height        re                                    Append rectangle to path
//
// TABLE 4.10 Path-painting operators page 167
// operands                operator   description
// —                       S          Stroke the path.
// —                       s          Close and stroke the path. This operator has the same effect as the sequence h S.
// —                       f          Fill the path, using the nonzero winding number rule to determine the region to fill (see “Nonzero Winding Number Rule” on page 169).
// —                       F          Equivalent to f; included only for compatibility. Although applications that read PDF files must be able to accept this operator,
//                                    those that generate PDF files should use f instead.
// —                       f*         Fill the path, using the even-odd rule to determine the region to fill (see “Even-Odd Rule” on page 170).
// —                       B          Fill and then stroke the path, using the nonzero winding number rule to determine the region to fill.
//                                    This produces the same result as constructing two identical path objects, painting the first with f and the second with S.
//                                    Note, however, that the filling and stroking portions of the operation consult different values of several graphics
//                                    state parameters, such as the current color. See also “Special Path-Painting Considerations” on page 462.
// —                       B*         Fill and then stroke the path, using the even-odd rule to determine the region to fill.
//                                    This operator produces the same result as B, except that the path is filled as if with f* instead of f. See also “Special Path-Painting Considerations” on page 462.
// —                       b          Close, fill, and then stroke the path, using the nonzero winding number rule to determine the region to fill.
//                                    This operator has the same effect as the sequence h B. See also “Special Path-Painting Considerations” on page 462.
// —                       b*         Close, fill, and then stroke the path, using the even-odd rule to determine the region to fill. This operator has the same effect as the sequence h B*.
//                                    See also “Special Path-Painting Considerations” on page 462.
// —                       n          End the path object without filling or stroking it. This operator is a “path-painting no-op,”
//                                    used primarily for the side effect of changing the current clipping path (see Section 4.4.3, “Clipping Path Operators”).
// TABLE 4.10 Path-painting operators page 167
// operands                operator   postscript                 description
// —                       S          stroke                     Stroke path
// —                       s          closepath, stroke          Close and stroke path
// —                       f          fill                       Fill path using nonzero winding number rule
// —                       F          fill                       Fill path using nonzero winding number rule (obsolete)
// —                       f*         eofill                     Fill path using even-odd rule
// —                       B          fill, stroke               Fill and stroke path using nonzero winding number rule
// —                       B*         eofill, stroke             Fill and stroke path using even-odd rule
// —                       b          closepath, fill, stroke    Close, fill, and stroke path using nonzero winding number rule
// —                       b*         closepath, eofill, stroke  Close, fill, and stroke path using even-odd rule
// —                       n                                     End path without filling or stroking
//
// TABLE 4.11 Clipping path operators page 172
// operands                operator   description
// —                       W          Modify the current clipping path by intersecting it with the current path, using the nonzero winding number rule to determine which regions lie inside the clipping path.
// —                       W*         Modify the current clipping path by intersecting it with the current path, using the even-odd rule to determine which regions lie inside the clipping path.
// TABLE 4.11 Clipping path operators page 172
// operands                operator   postscript                 description
// —                       W          clip                       Set clipping path using nonzero winding number rule
// —                       W*         eoclip                     Set clipping path using even-odd rule
//
// TABLE 4.21 Color operators page 216
// operands                operator   description
// name                    CS         (PDF 1.1) Set the current color space to use for stroking operations. The operand name must be a name object.
//                                    If the color space is one that can be specified by a name and no additional parameters (DeviceGray, DeviceRGB, DeviceCMYK, and certain cases of Pattern),
//                                    the name may be specified directly. Otherwise, it must be a name defined in the ColorSpace subdictionary of the current resource dictionary (see Section 3.7.2, “Resource Dictionaries”);
//                                    the associated value is an array describing the color space (see Section 4.5.2, “Color Space Families”).
// name                    cs         (PDF 1.1) Same as CS, but for nonstroking operations.
// c1...cn                 SC         (PDF 1.1) Set the color to use for stroking operations in a device, CIE-based (other than ICCBased), or Indexed color space. The number of operands required
//                                    and their interpretation depends on the current stroking color space
// c1...cn                 SCN        (PDF 1.2) Same as SC, but also supports Pattern, Separation, DeviceN, and
// c1...cn name            SCN        ICCBased color spaces.
// c1...cn                 sc         (PDF 1.1) Same as SC, but for nonstroking operations.
// c1...cn                 scn        (PDF 1.2) Same as SCN, but for nonstroking operations.
// c1...cn name            scn
// gray                    G          Set the stroking color space to DeviceGray (or the DefaultGray color space; see “Default Color Spaces” on page 194)
//                                    and set the gray level to use for stroking operations. gray is a number between 0.0 (black) and 1.0 (white).
// gray                    g          Same as G, but for nonstroking operations.
// r g b                   RG         Set the stroking color space to DeviceRGB (or the DefaultRGB color space; see “Default Color Spaces” on page 194) and set the color to use for stroking operations.
//                                    Each operand must be a number between 0.0 (minimum intensity) and 1.0 (maximum intensity).
// r g b                   rg         Same as RG, but for nonstroking operations.
// c m y k                 K          Set the stroking color space to DeviceCMYK (or the DefaultCMYK color space; see “Default Color Spaces” on page 194) and set the color to use for stroking operations.
//                                    Each operand must be a number between 0.0 (zero concentration) and 1.0 (maximum concentration).
//                                    The behavior of this operator is affected by the overprint mode (see Section 4.5.6, “Overprint Control”).
// c m y k                 k          Same as K, but for nonstroking operations.
// TABLE 4.21 Color operators page 216
// operands                operator   postscript                 description
// name                    CS         setcolorspace              (PDF 1.1) Set color space for stroking operations
// name                    cs         setcolorspace              (PDF 1.1) Set color space for nonstroking operations
// c1...cn                 SC         setcolor                   (PDF 1.1) Set color for stroking operations
// c1...cn                 SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
// c1...cn name            SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
// c1...cn                 sc         setcolor                   (PDF 1.1) Set color for nonstroking operations
// c1...cn                 scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
// c1...cn name            scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
// gray                    G          setgray                    Set gray level for stroking operations
// gray                    g          setgray                    Set gray level for nonstroking operations
// r g b                   RG         setrgbcolor                Set RGB color for stroking operations
// r g b                   rg         setrgbcolor                Set RGB color for nonstroking operations
// c m y k                 K          setcmykcolor               Set CMYK color for stroking operations
// c m y k                 k          setcmykcolor               Set CMYK color for nonstroking operations
//
// TABLE 4.24 Shading operator page 232
// operands                operator   description
// name                    sh         (PDF 1.3) Paint the shape and color shading described by a shading dictionary, subject to the current clipping path. The current color in the graphics state is neither
//                                    used nor altered. The effect is different from that of painting a path using a shading pattern as the current color.
// TABLE 4.24 Shading operator page 232
// operands                operator   postscript                 description
//                         sh         shfill                     (PDF 1.3) Paint area defined by shading pattern
//
// TABLE 4.34 XObject operator page 261
// operands                operator   description
// name                    Do         Paint the specified XObject. The operand name must appear as a key in the XObject subdictionary of the current resource dictionary (see Section 3.7.2, “Resource Dictionaries”);
//                                    the associated value must be a stream whose Type entry, if present, is XObject. The effect of Do depends on the value of the XObject’s Subtype entry,
//                                    which may be Image (see Section 4.8.4, “Image Dictionaries”), Form (Section 4.9, “Form XObjects”), or PS (Section 4.10, “Post-Script XObjects”).
// TABLE 4.34 XObject operator page 261
// operands                operator   postscript                 description
//                         Do                                    Invoke named XObject
//
// TABLE 4.38 Inline image operators page 278
// operands                operator   description
// —                       BI         Begin an inline image object.
// —                       ID         Begin the image data for an inline image object.
// —                       EI         End an inline image object.
// TABLE 4.38 Inline image operators page 278
// operands                operator   postscript                 description
// —                       BI                                    Begin inline image object
// —                       ID                                    Begin inline image data
// —                       EI                                    End inline image object
//
// TABLE 4.39 Entries in an inline image object page 279
// full name               abbreviation
// BitsPerComponent        BPC
// ColorSpace              CS
// Decode                  D
// DecodeParms             DP
// Filter                  F
// Height                  H
// ImageMask               IM
// Intent (PDF 1.1)        No abbreviation
// Interpolate             I (uppercase I)
// Width                   W
// DeviceGray              G
// DeviceRGB               RGB
// DeviceCMYK              CMYK
// Indexed                 I (uppercase I)
// ASCIIHexDecode          AHx
// ASCII85Decode           A85
// LZWDecode               LZW
// FlateDecode (PDF 1.2)   Fl (uppercase F, lowercase L)
// RunLengthDecode         RL
// CCITTFaxDecode          CCF
// DCTDecode               DCT
//
// TABLE 5.2 Text state operators page 302
// operands                operator   description
// charSpace               Tc         Set the character spacing, Tc , to charSpace, which is a number expressed in unscaled text space units. Character spacing is used by the Tj, TJ, and ' operators. Initial value: 0.
// wordSpace               Tw         Set the word spacing, Tw, to wordSpace, which is a number expressed in unscaled text space units. Word spacing is used by the Tj, TJ, and ' operators. Initial value: 0.
// scale                   Tz         Set the horizontal scaling, Th, to (scale ÷ 100). scale is a number specifying the percentage of the normal width. Initial value: 100 (normal width).
// leading                 TL         Set the text leading, Tl , to leading, which is a number expressed in unscaled text space units. Text leading is used only by the T*, ', and " operators. Initial value: 0.
// font size               Tf         Set the text font, Tf , to font and the text font size, Tfs , to size. font is the name of a font resource in the Font subdictionary of the current resource dictionary; size is
//                                    a number representing a scale factor. There is no initial value for either font or size; they must be specified explicitly using Tf before any text is shown.
// render                  Tr         Set the text rendering mode, Tmode, to render, which is an integer. Initial value: 0.
// rise                    Ts         Set the text rise, Trise , to rise, which is a number expressed in unscaled text space units. Initial value: 0.
// TABLE 5.2 Text state operators page 302
// operands                operator   postscript                 description
// charSpace               Tc                                    Set character spacing
// wordSpace               Tw                                    Set word spacing
// scale                   Tz                                    Set horizontal text scaling
// leading                 TL                                    Set text leading
// font size               Tf         selectfont                 Set text font and size
// render                  Tr                                    Set text rendering mode
// rise                    Ts                                    Set text rise
//
// TABLE 5.4 Text object operators page 308
// operands                operator   description
// —                       BT         Begin a text object, initializing the text matrix, Tm, and the text line matrix, Tlm, to the identity matrix. Text objects cannot be nested; a second BT cannot appear before an ET.
// —                       ET         End a text object, discarding the text matrix.
// TABLE 5.4 Text object operators page 308
// operands                operator   postscript                 description
// —                       BT                                    Begin text object
// —                       ET                                    End text object
//
// TABLE 5.5 Text-positioning operators page 310
// operands                operator   description
// tx ty                   Td         Move to the start of the next line, offset from the start of the current line by (tx , ty ).
//                                    tx and ty are numbers expressed in unscaled text space units. More precisely, this operator performs the following assignments:
// tx ty                   TD         Move to the start of the next line, offset from the start of the current line by (tx , ty ).
//                                    As a side effect, this operator sets the leading parameter in the text state. This operator has the same effect as the code
//                                    −ty TL, tx ty Td
// a b c d e f             Tm         Set the text matrix, Tm, and the text line matrix, Tlm, as follows:
//                                    The operands are all numbers, and the initial value for Tm and Tlm is the identity matrix, [1 0 0 1 0 0].
//                                    Although the operands specify a matrix, they are passed to Tm as six separate numbers, not as an array.
//                                    The matrix specified by the operands is not concatenated onto the current text matrix, but replaces it.
// —                       T*         Move to the start of the next line. This operator has the same effect as the code
//                                    0 Tl Td where Tl is the current leading parameter in the text state.
// TABLE 5.5 Text-positioning operators page 310
// operands                operator   postscript                 description
// tx ty                   Td                                    Move text position
// tx ty                   TD                                    Move text position and set leading
// a b c d e f             Tm                                    Set text matrix and text line matrix
// —                       T*                                    Move to start of next text line
//
// TABLE 5.6 Text-showing operators page 311
// operands                operator   description
// string                  Tj         Show a text string.
// string                  '          Move to the next line and show a text string. This operator has the same effect as the code
//                                    T* string Tj
// aw ac string            "          Move to the next line and show a text string, using aw as the word spacing and ac as the character spacing (setting the corresponding parameters in the text state).
//                                    aw and ac are numbers expressed in unscaled text space units. This operator has the same effect as the code
//                                    aw Tw ac Tc string '
// array                   TJ         Show one or more text strings, allowing individual glyph positioning (see implementation note 40 in Appendix H).
//                                    Each element of array can be a string or a number. If the element is a string, this operator shows the string. If it is a number,
//                                    the operator adjusts the text position by that amount; that is, it translates the text matrix, Tm. The number is expressed in thousandths of a unit of text
//                                    space (see Section 5.3.3, “Text Space Details,” and implementation note 41 in Appendix H). This amount is subtracted from the current horizontal or vertical
//                                    coordinate, depending on the writing mode. In the default coordinate system, a positive adjustment has the effect of moving the next glyph painted either to the
//                                    left or down by the given amount. Figure 5.11 shows an example of the effect of passing offsets to TJ.
// TABLE 5.6 Text-showing operators page 311
// operands                operator   postscript                 description
// string                  Tj         show                       Show text
// string                  '                                     Move to next line and show text
// aw ac string            "                                     Set word and character spacing, move to next line, and show text
// array                   TJ                                    Show text, allowing individual glyph positioning
//
// TABLE 5.10 Type 3 font operators page 326
// operands                operator   description
// wx wy                   d0         Set width information for the glyph and declare that the glyph description specifies both its shape and its color. (Note that this operator name ends in the digit 0.)
//                                    wx specifies the horizontal displacement in the glyph coordinate system; it must be consistent with the corresponding width in the font’s Widths array. wy must be 0 (see Section 5.1.3, “Glyph Positioning and Metrics”).
//                                    This operator is permitted only in a content stream appearing in a Type 3 font’s CharProcs dictionary. It is typically used only if the glyph description executes operators to set the color explicitly.
// wx wy llx lly urx ury   d1         Set width and bounding box information for the glyph and declare that the glyph description specifies only shape, not color. (Note that this operator name ends in the digit 1.)
//                                    wx specifies the horizontal displacement in the glyph coordinate system; it must be consistent with the corresponding width in the font’s Widths array.
//                                    wy must be 0 (see Section 5.1.3, “Glyph Positioning and Metrics”).
//                                    llx and lly are the coordinates of the lower-left corner, and urx and ury the upper-right corner, of the glyph bounding box.
//                                    The glyph bounding box is the smallest rectangle, oriented with the axes of the glyph coordinate system,
//                                    that completely encloses all marks placed on the page as a result of executing the glyph’s description. The declared bounding box must be
//                                    correct—in other words, sufficiently large to enclose the entire glyph. If any marks fall outside this bounding box, the result is unpredictable.
//                                    A glyph description that begins with the d1 operator should not execute any operators that set the color (or other color-related parameters) in
//                                    the graphics state; any use of such operators will be ignored. The glyph description is executed solely to determine the glyph’s shape; its color is
//                                    determined by the graphics state in effect each time this glyph is painted by a text-showing operator. For the same reason, the glyph description
//                                    may not include an image; however, an image mask is acceptable, since it merely defines a region of the page to be painted with the current color.
//                                    This operator is permitted only in a content stream appearing in a Type 3 font’s CharProcs dictionary.
// TABLE 5.10 Type 3 font operators page 326
// operands                operator   postscript                 description
// wx wy                   d0         setcharwidth               Set glyph width in Type 3 font
// wx wy llx lly urx ury   d1         setcachedevice             Set glyph width and bounding box in Type 3 font
//
// TABLE 9.8 Marked-content operators page 584
// operands                operator   description
// tag                     MP         Designate a marked-content point. tag is a name object indicating the role or significance of the point.
// tag properties          DP         Designate a marked-content point with an associated property list.
//                                    tag is a name object indicating the role or significance of the point; properties is either an inline dictionary containing the property list or a name object
//                                    associated with it in the Properties subdictionary of the current resource dictionary (see Section 9.5.1, “Property Lists”).
// tag                     BMC        Begin a marked-content sequence terminated by a balancing EMC operator.
//                                    tag is a name object indicating the role or significance of the sequence.
// tag properties          BDC        Begin a marked-content sequence with an associated property list, terminated by a balancing EMC operator.
//                                    tag is a name object indicating the role or significance of the sequence;
//                                    properties is either an inline dictionary containing the property list or a name object associated with it in the Properties subdictionary of the current resource dictionary (see Section 9.5.1, “Property Lists”).
// —                       EMC        End a marked-content sequence begun by a BMC or BDC operator.
// TABLE 9.8 Marked-content operators page 584
// operands                operator   postscript                 description
// tag                     MP                                    (PDF 1.2) Define marked-content point
// tag properties          DP                                    (PDF 1.2) Define marked-content point with property list
// tag                     BMC                                   (PDF 1.2) Begin marked-content sequence
// tag properties          BDC                                   (PDF 1.2) Begin marked-content sequence with property list
// —                       EMC                                   (PDF 1.2) End marked-content sequence
//
// ***********************         all operators         ***********************
// operands                operator   description
// aw ac string            "          Move to the next line and show a text string, using aw as the word spacing and ac as the character spacing (setting the corresponding parameters in the text state).
// string                  '          Move to the next line and show a text string. This operator has the same effect as the code
// —                       b          Close, fill, and then stroke the path, using the nonzero winding number rule to determine the region to fill.
// —                       B          Fill and then stroke the path, using the nonzero winding number rule to determine the region to fill.
// —                       b*         Close, fill, and then stroke the path, using the even-odd rule to determine the region to fill. This operator has the same effect as the sequence h B*.
// —                       B*         Fill and then stroke the path, using the even-odd rule to determine the region to fill.
// tag properties          BDC        Begin a marked-content sequence with an associated property list, terminated by a balancing EMC operator.
// —                       BI         Begin an inline image object.
// tag                     BMC        Begin a marked-content sequence terminated by a balancing EMC operator.
// —                       BT         Begin a text object, initializing the text matrix, Tm, and the text line matrix, Tlm, to the identity matrix. Text objects cannot be nested; a second BT cannot appear before an ET.
// —                       BX         (PDF 1.1) Begin a compatibility section. Unrecognized operators (along with their operands)
// x1 y1 x2 y2 x3 y3       c          Append a cubic Bézier curve to the current path. The curve extends from the current point to the point (x3, y3), using (x1, y1) and
// a b c d e f             cm         Modify the current transformation matrix (CTM) by concatenating the specified matrix (see Section 4.2.1, “Coordinate Spaces”).
// name                    cs         (PDF 1.1) Same as CS, but for nonstroking operations.
// name                    CS         (PDF 1.1) Set the current color space to use for stroking operations. The operand name must be a name object.
// dashArray dashPhase     d          Set the line dash pattern in the graphics state (see “Line Dash Pattern” on page 155).
// wx wy                   d0         Set width information for the glyph and declare that the glyph description specifies both its shape and its color. (Note that this operator name ends in the digit 0.)
// wx wy llx lly urx ury   d1         Set width and bounding box information for the glyph and declare that the glyph description specifies only shape, not color. (Note that this operator name ends in the digit 1.)
// name                    Do         Paint the specified XObject. The operand name must appear as a key in the XObject subdictionary of the current resource dictionary (see Section 3.7.2, “Resource Dictionaries”);
// tag properties          DP         Designate a marked-content point with an associated property list.
// —                       EI         End an inline image object.
// —                       EMC        End a marked-content sequence begun by a BMC or BDC operator.
// —                       ET         End a text object, discarding the text matrix.
// —                       EX         (PDF 1.1) End a compatibility section begun by a balancing BX operator.
// —                       F          Equivalent to f; included only for compatibility. Although applications that read PDF files must be able to accept this operator,
// —                       f          Fill the path, using the nonzero winding number rule to determine the region to fill (see “Nonzero Winding Number Rule” on page 169).
// —                       f*         Fill the path, using the even-odd rule to determine the region to fill (see “Even-Odd Rule” on page 170).
// gray                    g          Same as G, but for nonstroking operations.
// gray                    G          Set the stroking color space to DeviceGray (or the DefaultGray color space; see “Default Color Spaces” on page 194)
// dictName                gs         (PDF 1.2) Set the specified parameters in the graphics state. dictName is the name of a graphics state parameter dictionary
// —                       h          Close the current subpath by appending a straight line segment from the current point to the starting point of the subpath.
// flatness                i          Set the flatness tolerance in the graphics state (see Section 6.5.1, “Flatness Tolerance”).
// —                       ID         Begin the image data for an inline image object.
// lineCap                 J          Set the line cap style in the graphics state (see “Line Cap Style” on page 153).
// lineJoin                j          Set the line join style in the graphics state (see “Line Join Style” on page 153).
// c m y k                 k          Same as K, but for nonstroking operations.
// c m y k                 K          Set the stroking color space to DeviceCMYK (or the DefaultCMYK color space; see “Default Color Spaces” on page 194) and set the color to use for stroking operations.
// x y                     l          (lowercase L) Append a straight line segment from the current point to the point (x, y). The new current point is (x, y).
// x y                     m          Begin a new subpath by moving the current point to coordinates (x, y), omitting any connecting line segment.
// miterLimit              M          Set the miter limit in the graphics state (see “Miter Limit” on page 153).
// tag                     MP         Designate a marked-content point. tag is a name object indicating the role or significance of the point.
// —                       n          End the path object without filling or stroking it. This operator is a “path-painting no-op,”
// —                       Q          Restore the graphics state by removing the most recently saved state from the stack and making it the current state (see “Graphics State Stack” on page 152).
// —                       q          Save the current graphics state on the graphics state stack (see “Graphics State Stack” on page 152).
// x y width height        re         Append a rectangle to the current path as a complete subpath, with lower-left corner (x, y) and dimensions width and height in user space.
// r g b                   rg         Same as RG, but for nonstroking operations.
// r g b                   RG         Set the stroking color space to DeviceRGB (or the DefaultRGB color space; see “Default Color Spaces” on page 194) and set the color to use for stroking operations.
// intent                  ri         (PDF 1.1) Set the color rendering intent in the graphics state (see “Rendering Intents” on page 197).
// —                       s          Close and stroke the path. This operator has the same effect as the sequence h S.
// —                       S          Stroke the path.
// c1...cn                 sc         (PDF 1.1) Same as SC, but for nonstroking operations.
// c1...cn                 SC         (PDF 1.1) Set the color to use for stroking operations in a device, CIE-based (other than ICCBased), or Indexed color space. The number of operands required
// c1...cn name            scn
// c1...cn                 SCN        (PDF 1.2) Same as SC, but also supports Pattern, Separation, DeviceN, and
// c1...cn                 scn        (PDF 1.2) Same as SCN, but for nonstroking operations.
// c1...cn name            SCN        ICCBased color spaces.
// name                    sh         (PDF 1.3) Paint the shape and color shading described by a shading dictionary, subject to the current clipping path. The current color in the graphics state is neither
// —                       T*         Move to the start of the next line. This operator has the same effect as the code
// charSpace               Tc         Set the character spacing, Tc , to charSpace, which is a number expressed in unscaled text space units. Character spacing is used by the Tj, TJ, and ' operators. Initial value: 0.
// tx ty                   Td         Move to the start of the next line, offset from the start of the current line by (tx , ty ).
// tx ty                   TD         Move to the start of the next line, offset from the start of the current line by (tx , ty ).
// font size               Tf         Set the text font, Tf , to font and the text font size, Tfs , to size. font is the name of a font resource in the Font subdictionary of the current resource dictionary; size is
// string                  Tj         Show a text string.
// array                   TJ         Show one or more text strings, allowing individual glyph positioning (see implementation note 40 in Appendix H).
// leading                 TL         Set the text leading, Tl , to leading, which is a number expressed in unscaled text space units. Text leading is used only by the T*, ', and " operators. Initial value: 0.
// a b c d e f             Tm         Set the text matrix, Tm, and the text line matrix, Tlm, as follows:
// render                  Tr         Set the text rendering mode, Tmode, to render, which is an integer. Initial value: 0.
// rise                    Ts         Set the text rise, Trise , to rise, which is a number expressed in unscaled text space units. Initial value: 0.
// wordSpace               Tw         Set the word spacing, Tw, to wordSpace, which is a number expressed in unscaled text space units. Word spacing is used by the Tj, TJ, and ' operators. Initial value: 0.
// scale                   Tz         Set the horizontal scaling, Th, to (scale ÷ 100). scale is a number specifying the percentage of the normal width. Initial value: 100 (normal width).
// x2 y2 x3 y3             v          Append a cubic Bézier curve to the current path. The curve extends from the current point to the point (x3, y3), using the current point
// —                       W          Modify the current clipping path by intersecting it with the current path, using the nonzero winding number rule to determine which regions lie inside the clipping path.
// lineWidth               w          Set the line width in the graphics state (see “Line Width” on page 152).
// —                       W*         Modify the current clipping path by intersecting it with the current path, using the even-odd rule to determine which regions lie inside the clipping path.
// x1 y1 x3 y3             y          Append a cubic Bézier curve to the current path. The curve extends from the current point to the point (x3, y3), using (x1, y1) and (x3, y3)
//
// APPENDIX A - Operator Summary page 699
// operator   postscript                 description                                                                           table  page
// b          closepath, fill, stroke    Close, fill, and stroke path using nonzero winding number rule                        4.10   167
// B          fill, stroke               Fill and stroke path using nonzero winding number rule                                4.10   167
// b*         closepath, eofill, stroke  Close, fill, and stroke path using even-odd rule                                      4.10   167
// B*         eofill, stroke             Fill and stroke path using even-odd rule                                              4.10   167
// BDC                                   (PDF 1.2) Begin marked-content sequence with property list                            9.8    584
// BI                                    Begin inline image object                                                             4.38   278
// BMC                                   (PDF 1.2) Begin marked-content sequence                                               9.8    584
// BT                                    Begin text object                                                                     5.4    308
// BX                                    (PDF 1.1) Begin compatibility section                                                 3.20    95
// c          curveto                    Append curved segment to path (three control points)                                  4.9    163
// cm         concat                     Concatenate matrix to current transformation matrix                                   4.7    156
// CS         setcolorspace              (PDF 1.1) Set color space for stroking operations                                     4.21   216
// cs         setcolorspace              (PDF 1.1) Set color space for nonstroking operations                                  4.21   217
// d          setdash                    Set line dash pattern                                                                 4.7    156
// d0         setcharwidth               Set glyph width in Type 3 font                                                        5.10   326
// d1         setcachedevice             Set glyph width and bounding box in Type 3 font                                       5.10   326
// Do                                    Invoke named XObject                                                                  4.34   261
// DP                                    (PDF 1.2) Define marked-content point with property list                              9.8    584
// EI                                    End inline image object                                                               4.38   278
// EMC                                   (PDF 1.2) End marked-content sequence                                                 9.8    584
// ET                                    End text object                                                                       5.4    308
// EX                                    (PDF 1.1) End compatibility section                                                   3.20    95
// f          fill                       Fill path using nonzero winding number rule                                           4.10   167
// F          fill                       Fill path using nonzero winding number rule (obsolete)                                4.10   167
// f*         eofill                     Fill path using even-odd rule                                                         4.10   167
// G          setgray                    Set gray level for stroking operations                                                4.21   217
// g          setgray                    Set gray level for nonstroking operations                                             4.21   217
// gs                                    (PDF 1.2) Set parameters from graphics state parameter dictionary                     4.7    156
// h          closepath                  Close subpath                                                                         4.9    163
// i          setflat                    Set flatness tolerance                                                                4.7    156
// ID                                    Begin inline image data                                                               4.38   278
// j          setlinejoin                Set line join style                                                                   4.7    156
// J          setlinecap                 Set line cap style                                                                    4.7    156
// K          setcmykcolor               Set CMYK color for stroking operations                                                4.21   218
// k          setcmykcolor               Set CMYK color for nonstroking operations                                             4.21   218
// l          lineto                     Append straight line segment to path                                                  4.9    163
// m          moveto                     Begin new subpath                                                                     4.9    163
// M          setmiterlimit              Set miter limit                                                                       4.7    156
// MP                                    (PDF 1.2) Define marked-content point                                                 9.8    584
// n                                     End path without filling or stroking                                                  4.10   167
// q          gsave                      Save graphics state                                                                   4.7    156
// Q          grestore                   Restore graphics state                                                                4.7    156
// re                                    Append rectangle to path                                                              4.9    164
// RG         setrgbcolor                Set RGB color for stroking operations                                                 4.21   217
// rg         setrgbcolor                Set RGB color for nonstroking operations                                              4.21   217
// ri                                    Set color rendering intent                                                            4.7    156
// s          closepath, stroke          Close and stroke path                                                                 4.10   167
// S          stroke                     Stroke path                                                                           4.10   167
// SC         setcolor                   (PDF 1.1) Set color for stroking operations                                           4.21   217
// sc         setcolor                   (PDF 1.1) Set color for nonstroking operations                                        4.21   217
// SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)       4.21   217
// scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)    4.21   217
// sh         shfill                     (PDF 1.3) Paint area defined by shading pattern                                       4.24   232
// T*                                    Move to start of next text line                                                       5.5    310
// Tc                                    Set character spacing                                                                 5.2    302
// Td                                    Move text position                                                                    5.5    310
// TD                                    Move text position and set leading                                                    5.5    310
// Tf         selectfont                 Set text font and size                                                                5.2    302
// Tj         show                       Show text                                                                             5.6    311
// TJ                                    Show text, allowing individual glyph positioning                                      5.6    311
// TL                                    Set text leading                                                                      5.2    302
// Tm                                    Set text matrix and text line matrix                                                  5.5    310
// Tr                                    Set text rendering mode                                                               5.2    302
// Ts                                    Set text rise                                                                         5.2    302
// Tw                                    Set word spacing                                                                      5.2    302
// Tz                                    Set horizontal text scaling                                                           5.2    302
// v          curveto                    Append curved segment to path (initial point replicated)                              4.9    163
// w          setlinewidth               Set line width                                                                        4.7    156
// W          clip                       Set clipping path using nonzero winding number rule                                   4.11   172
// W*         eoclip                     Set clipping path using even-odd rule                                                 4.11   172
// y          curveto                    Append curved segment to path (final point replicated)                                4.9    163
// '                                     Move to next line and show text                                                       5.6    311
// "                                     Set word and character spacing, move to next line, and show text                      5.6    311
//
// ***********************         all operators 2       ***********************
// TABLE 3.20 Compatibility operators page 95
// operands                operator   postscript                 description
// —                       BX                                    (PDF 1.1) Begin compatibility section
// —                       EX                                    (PDF 1.1) End compatibility section
// TABLE 4.7 Graphics state operators page 156
// operands                operator   postscript                 description
// —                       q          gsave                      Save graphics state
// —                       Q          grestore                   Restore graphics state
// a b c d e f             cm         concat                     Concatenate matrix to current transformation matrix
// lineWidth               w          setlinewidth               Set line width
// lineCap                 J          setlinecap                 Set line cap style
// lineJoin                j          setlinejoin                Set line join style
// miterLimit              M          setmiterlimit              Set miter limit
// dashArray dashPhase     d          setdash                    Set line dash pattern
// intent                  ri                                    Set color rendering intent
// flatness                i          setflat                    Set flatness tolerance
// dictName                gs                                    (PDF 1.2) Set parameters from graphics state parameter dictionary
// TABLE 4.9 Path construction operators page 163
// operands                operator   postscript                 description
// x y                     m          moveto                     Begin new subpath
// x y                     l          lineto                     Append straight line segment to path
// x1 y1 x2 y2 x3 y3       c          curveto                    Append curved segment to path (three control points)
// x2 y2 x3 y3             v          curveto                    Append curved segment to path (initial point replicated)
// x1 y1 x3 y3             y          curveto                    Append curved segment to path (final point replicated)
// —                       h          closepath                  Close subpath
// x y width height        re                                    Append rectangle to path
// TABLE 4.10 Path-painting operators page 167
// operands                operator   postscript                 description
// —                       S          stroke                     Stroke path
// —                       s          closepath, stroke          Close and stroke path
// —                       f          fill                       Fill path using nonzero winding number rule
// —                       F          fill                       Fill path using nonzero winding number rule (obsolete)
// —                       f*         eofill                     Fill path using even-odd rule
// —                       B          fill, stroke               Fill and stroke path using nonzero winding number rule
// —                       B*         eofill, stroke             Fill and stroke path using even-odd rule
// —                       b          closepath, fill, stroke    Close, fill, and stroke path using nonzero winding number rule
// —                       b*         closepath, eofill, stroke  Close, fill, and stroke path using even-odd rule
// —                       n                                     End path without filling or stroking
// TABLE 4.11 Clipping path operators page 172
// operands                operator   postscript                 description
// —                       W          clip                       Set clipping path using nonzero winding number rule
// —                       W*         eoclip                     Set clipping path using even-odd rule
// TABLE 4.21 Color operators page 216
// operands                operator   postscript                 description
// name                    CS         setcolorspace              (PDF 1.1) Set color space for stroking operations
// name                    cs         setcolorspace              (PDF 1.1) Set color space for nonstroking operations
// c1...cn                 SC         setcolor                   (PDF 1.1) Set color for stroking operations
// c1...cn                 SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
// c1...cn name            SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
// c1...cn                 sc         setcolor                   (PDF 1.1) Set color for nonstroking operations
// c1...cn                 scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
// c1...cn name            scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
// gray                    G          setgray                    Set gray level for stroking operations
// gray                    g          setgray                    Set gray level for nonstroking operations
// r g b                   RG         setrgbcolor                Set RGB color for stroking operations
// r g b                   rg         setrgbcolor                Set RGB color for nonstroking operations
// c m y k                 K          setcmykcolor               Set CMYK color for stroking operations
// c m y k                 k          setcmykcolor               Set CMYK color for nonstroking operations
// TABLE 4.24 Shading operator page 232
// operands                operator   postscript                 description
//                         sh         shfill                     (PDF 1.3) Paint area defined by shading pattern
// TABLE 4.34 XObject operator page 261
// operands                operator   postscript                 description
//                         Do                                    Invoke named XObject
// TABLE 4.38 Inline image operators page 278
// operands                operator   postscript                 description
// —                       BI                                    Begin inline image object
// —                       ID                                    Begin inline image data
// —                       EI                                    End inline image object
// TABLE 5.2 Text state operators page 302
// operands                operator   postscript                 description
// charSpace               Tc                                    Set character spacing
// wordSpace               Tw                                    Set word spacing
// scale                   Tz                                    Set horizontal text scaling
// leading                 TL                                    Set text leading
// font size               Tf         selectfont                 Set text font and size
// render                  Tr                                    Set text rendering mode
// rise                    Ts                                    Set text rise
// TABLE 5.4 Text object operators page 308
// operands                operator   postscript                 description
// —                       BT                                    Begin text object
// —                       ET                                    End text object
// TABLE 5.5 Text-positioning operators page 310
// operands                operator   postscript                 description
// tx ty                   Td                                    Move text position
// tx ty                   TD                                    Move text position and set leading
// a b c d e f             Tm                                    Set text matrix and text line matrix
// —                       T*                                    Move to start of next text line
// TABLE 5.6 Text-showing operators page 311
// operands                operator   postscript                 description
// string                  Tj         show                       Show text
// string                  '                                     Move to next line and show text
// aw ac string            "                                     Set word and character spacing, move to next line, and show text
// array                   TJ                                    Show text, allowing individual glyph positioning
// TABLE 5.10 Type 3 font operators page 326
// operands                operator   postscript                 description
// wx wy                   d0         setcharwidth               Set glyph width in Type 3 font
// wx wy llx lly urx ury   d1         setcachedevice             Set glyph width and bounding box in Type 3 font
// TABLE 9.8 Marked-content operators page 584
// operands                operator   postscript                 description
// tag                     MP                                    (PDF 1.2) Define marked-content point
// tag properties          DP                                    (PDF 1.2) Define marked-content point with property list
// tag                     BMC                                   (PDF 1.2) Begin marked-content sequence
// tag properties          BDC                                   (PDF 1.2) Begin marked-content sequence with property list
// —                       EMC                                   (PDF 1.2) End marked-content sequence

//namespace PB_Pdf
namespace pb.Pdf
{
    public enum PdfOpe
    {
        Unknow,
                                                                // TABLE 3.20 Compatibility operators page 95
                                                                // operands                operator   postscript                 description
        Compatibility_BeginCompatibility,                       // —                       BX                                    (PDF 1.1) Begin compatibility section
        Compatibility_EndCompatibility,                         // —                       EX                                    (PDF 1.1) End compatibility section
                                                                // TABLE 4.7 Graphics state operators page 156
                                                                // operands                operator   postscript                 description
        Graphics_SaveGraphicsState,                             // —                       q          gsave                      Save graphics state
        Graphics_RestoreGraphicsState,                          // —                       Q          grestore                   Restore graphics state
        Graphics_ConcatenateMatrix,                             // a b c d e f             cm         concat                     Concatenate matrix to current transformation matrix
        Graphics_SetLineWidth,                                  // lineWidth               w          setlinewidth               Set line width
        Graphics_SetLineCap,                                    // lineCap                 J          setlinecap                 Set line cap style
        Graphics_SetLineJoin,                                   // lineJoin                j          setlinejoin                Set line join style
        Graphics_SetMiterLimit,                                 // miterLimit              M          setmiterlimit              Set miter limit
        Graphics_SetLineDash,                                   // dashArray dashPhase     d          setdash                    Set line dash pattern
        Graphics_SetColorRenderingIntent,                       // intent                  ri                                    Set color rendering intent
        Graphics_SetFlatnessTolerance,                          // flatness                i          setflat                    Set flatness tolerance
        Graphics_SetParameters,                                 // dictName                gs                                    (PDF 1.2) Set parameters from graphics state parameter dictionary
                                                                // TABLE 4.9 Path construction operators page 163
                                                                // operands                operator   postscript                 description
        Path_moveto,                                            // x y                     m          moveto                     Begin new subpath
        Path_lineto,                                            // x y                     l          lineto                     Append straight line segment to path
        Path_curvetoThreeControl,                               // x1 y1 x2 y2 x3 y3       c          curveto                    Append curved segment to path (three control points)
        Path_curvetoInitialPoint,                               // x2 y2 x3 y3             v          curveto                    Append curved segment to path (initial point replicated)
        Path_curvetoFinalPoint,                                 // x1 y1 x3 y3             y          curveto                    Append curved segment to path (final point replicated)
        Path_closepath,                                         // —                       h          closepath                  Close subpath
        Path_rectangle,                                         // x y width height        re                                    Append rectangle to path
                                                                // TABLE 4.10 Path-painting operators page 167
                                                                // operands                operator   postscript                 description
        Path_Stroke,                                            // —                       S          stroke                     Stroke path
        Path_Close,                                             // —                       s          closepath, stroke          Close and stroke path
        Path_Fill,                                              // —                       f          fill                       Fill path using nonzero winding number rule
        Path_FillOld,                                           // —                       F          fill                       Fill path using nonzero winding number rule (obsolete)
        Path_FillEvenOdd,                                       // —                       f*         eofill                     Fill path using even-odd rule
        Path_FillStroke,                                        // —                       B          fill, stroke               Fill and stroke path using nonzero winding number rule
        Path_FillStrokeEvenOdd,                                 // —                       B*         eofill, stroke             Fill and stroke path using even-odd rule
        Path_CloseFill,                                         // —                       b          closepath, fill, stroke    Close, fill, and stroke path using nonzero winding number rule
        Path_CloseFillEvenOdd,                                  // —                       b*         closepath, eofill, stroke  Close, fill, and stroke path using even-odd rule
        Path_End,                                               // —                       n                                     End path without filling or stroking
                                                                // TABLE 4.11 Clipping path operators page 172
                                                                // operands                operator   postscript                 description
        Clipping_Clip,                                          // —                       W          clip                       Set clipping path using nonzero winding number rule
        Clipping_ClipEvenOdd,                                   // —                       W*         eoclip                     Set clipping path using even-odd rule
                                                                // TABLE 4.21 Color operators page 216
                                                                // operands                operator   postscript                 description
        Color_SetColorSpaceStroking,                            // name                    CS         setcolorspace              (PDF 1.1) Set color space for stroking operations
        Color_SetColorSpaceNonStroking,                         // name                    cs         setcolorspace              (PDF 1.1) Set color space for nonstroking operations
        Color_ColorStroking,                                    // c1...cn                 SC         setcolor                   (PDF 1.1) Set color for stroking operations
        Color_ColorStroking2,                                   // c1...cn                 SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
                                                                // c1...cn name            SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
        Color_ColorNonStroking,                                 // c1...cn                 sc         setcolor                   (PDF 1.1) Set color for nonstroking operations
        Color_ColorNonStroking2,                                // c1...cn                 scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
                                                                // c1...cn name            scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
        Color_SetGrayStroking,                                  // gray                    G          setgray                    Set gray level for stroking operations
        Color_SetGrayNonStroking,                               // gray                    g          setgray                    Set gray level for nonstroking operations
        Color_SetRGBColorStroking,                              // r g b                   RG         setrgbcolor                Set RGB color for stroking operations
        Color_SetRGBColorNonStroking,                           // r g b                   rg         setrgbcolor                Set RGB color for nonstroking operations
        Color_SetCMYKColorStroking,                             // c m y k                 K          setcmykcolor               Set CMYK color for stroking operations
        Color_SetCMYKColorNonStroking,                          // c m y k                 k          setcmykcolor               Set CMYK color for nonstroking operations
                                                                // TABLE 4.24 Shading operator page 232
                                                                // operands                operator   postscript                 description
        Shading_Fill,                                           //                         sh         shfill                     (PDF 1.3) Paint area defined by shading pattern
                                                                // TABLE 4.34 XObject operator page 261
                                                                // operands                operator   postscript                 description
        XObject_Do,                                             //                         Do                                    Invoke named XObject
                                                                // TABLE 4.38 Inline image operators page 278
                                                                // operands                operator   postscript                 description
        Image_Begin,                                            // —                       BI                                    Begin inline image object
                                                                // —                       ID                                    Begin inline image data
                                                                // —                       EI                                    End inline image object
                                                                // TABLE 5.2 Text state operators page 302
                                                                // operands                operator   postscript                 description
        TextState_SetCharacterSpacing,                          // charSpace               Tc                                    Set character spacing
        TextState_SetWordSpacing,                               // wordSpace               Tw                                    Set word spacing
        TextState_SetScale,                                     // scale                   Tz                                    Set horizontal text scaling
        TextState_SetLeading,                                   // leading                 TL                                    Set text leading
        TextState_SetFont,                                      // font size               Tf         selectfont                 Set text font and size
        TextState_SetRendering,                                 // render                  Tr                                    Set text rendering mode
        TextState_SetRise,                                      // rise                    Ts                                    Set text rise
                                                                // TABLE 5.4 Text object operators page 308
                                                                // operands                operator   postscript                 description
        TextObject_Begin,                                       // —                       BT                                    Begin text object
        TextObject_End,                                         // —                       ET                                    End text object
                                                                // TABLE 5.5 Text-positioning operators page 310
                                                                // operands                operator   postscript                 description
        TextPosition_Move,                                      // tx ty                   Td                                    Move text position
        TextPosition_MoveLeading,                               // tx ty                   TD                                    Move text position and set leading
        TextPosition_Matrix,                                    // a b c d e f             Tm                                    Set text matrix and text line matrix
        TextPosition_NextLine,                                  // —                       T*                                    Move to start of next text line
                                                                // TABLE 5.6 Text-showing operators page 311
                                                                // operands                operator   postscript                 description
        TextShow_Show,                                          // string                  Tj         show                       Show text
        TextShow_ShowNextLine,                                  // string                  '                                     Move to next line and show text
        TextShow_ShowSpacingNextLine,                           // aw ac string            "                                     Set word and character spacing, move to next line, and show text
        TextShow_ShowGlyph,                                     // array                   TJ                                    Show text, allowing individual glyph positioning
                                                                // TABLE 5.10 Type 3 font operators page 326
                                                                // operands                operator   postscript                 description
        Font_Width,                                             // wx wy                   d0         setcharwidth               Set glyph width in Type 3 font
        Font_WidthBounding,                                     // wx wy llx lly urx ury   d1         setcachedevice             Set glyph width and bounding box in Type 3 font
                                                                // TABLE 9.8 Marked-content operators page 584
                                                                // operands                operator   postscript                 description
        MarkedContent_Define,                                   // tag                     MP                                    (PDF 1.2) Define marked-content point
        MarkedContent_DefineProperty,                           // tag properties          DP                                    (PDF 1.2) Define marked-content point with property list
        MarkedContent_Begin,                                    // tag                     BMC                                   (PDF 1.2) Begin marked-content sequence
        MarkedContent_BeginProperty,                            // tag properties          BDC                                   (PDF 1.2) Begin marked-content sequence with property list
        MarkedContent_End                                       // —                       EMC                                   (PDF 1.2) End marked-content sequence
    }

    public interface IPdfInstruction
    {
        PdfOpe ope { get; set; }
        string opeString { get; set; }
        void Export(Writer w);
    }

    public class PdfInstruction : IPdfInstruction
    {
        private PdfOpe gOpe;
        private string gOpeString;
        public string prm;

        public PdfInstruction(PdfOpe ope, string opeString, string prm)
        {
            gOpe = ope;
            gOpeString = opeString;
            this.prm = prm;
        }

        public PdfOpe ope { get { return gOpe; } set { gOpe = value; } }
        public string opeString { get { return gOpeString; } set { gOpeString = value; } }
        public override string ToString()
        {
            if (prm != null)
                return string.Format("{0} {1}", gOpe, prm);
            else
                return gOpe.ToString();
        }
        public void Export(Writer w)
        {
            w.WriteLine(this.ToString());
        }
    }

    public class PdfInstructionInilineImage : IPdfInstruction
    {
        private PdfOpe gOpe;
        private string gOpeString;
        public PdfValueObject prm = null;
        public byte[] gStream = null;
        public byte[] gDeflatedStream = null;

        public PdfInstructionInilineImage()
        {
            gOpe = PdfOpe.Image_Begin;
            gOpeString = "BI";
        }

        public PdfOpe ope { get { return gOpe; } set { gOpe = value; } }
        public string opeString { get { return gOpeString; } set { gOpeString = value; } }
        public byte[] stream { get { return gStream; } set { gStream = value; } }
        public byte[] deflatedStream { get { return DeflateStream(); } }
        private byte[] DeflateStream()
        {
            if (gDeflatedStream == null && gStream != null)
            {
                if (prm != null && prm.isObject() && prm["F"] != null && prm["F"].value.valueName == "Fl")
                    gDeflatedStream = PdfReader.FlateDecode(gStream);
                else
                    gDeflatedStream = gStream;
            }
            return gDeflatedStream;
        }
        public override string ToString()
        {
            if (prm != null)
                return string.Format("{0} {1}", gOpe, prm);
            else
                return gOpe.ToString();
        }
        public void Export(Writer w)
        {
            w.WriteLine(gOpe.ToString());
            if (prm != null)
            {
                w.WriteLine(prm.ToString());
            }
            w.WriteLine("Image_Data");
            byte[] data = DeflateStream();
            int i = 0;
            foreach (byte b in data)
            {
                if (i % 16 == 0)
                {
                    if (i != 0) w.WriteLine();
                    w.Write(i.zToHex());
                }
                w.Write(" {0}", b.zToHex());
                i++;
            }
            if (i != 0) w.WriteLine();
            w.WriteLine("Image_End");
        }
    }

    public class PdfDataTxtReader
    {
        private Reader gr = null;
        private Regex grgOpe = new Regex("^([a-zA-Z_]+) *", RegexOptions.Compiled);
        private Regex grgEmpty = new Regex(@"^ *$", RegexOptions.Compiled);
        //000000B0 C4 00 17 00 00 03 01 00 00 00 00 00 00 00 00 00
        private Regex grgBytes = new Regex("^[0-9a-f]+(:? +([0-9a-f]+))+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public TraceDelegate Trace = null;

        public PdfDataTxtReader(string file, bool opeTxt = false, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, Encoding encoding = null)
        {
            gr = new Reader(file, mode, access, share, encoding);
        }

        private void trace(string msg, params object[] prm)
        {
            if (Trace == null) return;
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Trace(msg);
        }

        public static IPdfInstruction[] PdfReadAll(byte[] data)
        {
            PdfDataReader pdr = new PdfDataReader(data);
            return pdr.ReadAll();
        }

        public IPdfInstruction[] ReadAll()
        {
            List<IPdfInstruction> instructions = new List<IPdfInstruction>();
            while (true)
            {
                IPdfInstruction instruction = Read();
                if (instruction == null) break;
                trace("read {0}", instruction.opeString);
                instructions.Add(instruction);
            }
            return instructions.ToArray();
        }

        public IPdfInstruction Read()
        {
            Match match;
            string s;
            while (true)
            {
                s = gr.ReadLine();
                if (s == null) return null;
                match = grgOpe.Match(s);
                if (match.Success) break;
                match = grgEmpty.Match(s);
                if (!match.Success) throw new PBException("error wrong instruction \"{0}\" line {1}", s, gr.LineNumber);
            }
            s = s.Substring(match.Length);
            PdfOpe ope;
            ope = GetPdfOpeTxt(match.Groups[1].Value);
            if (ope == PdfOpe.Image_Begin)
            {
                if (s != "") throw new PBException("error bad BI instruction \"{0}\" line {1}", s, gr.LineNumber);
                return ReadInlineImageTxt();
            }
            else
            {
                if (s == "") s = null;
                return new PdfInstruction(ope, match.Groups[1].Value, s);
            }
        }

        public PdfOpe GetPdfOpeTxt(string opeString)
        {
            switch (opeString)
            {
                //                operands                operator   postscript                 description
                case "Compatibility_BeginCompatibility":         // —                       BX                                    (PDF 1.1) Begin compatibility section
                    return PdfOpe.Compatibility_BeginCompatibility;
                case "Compatibility_EndCompatibility":           // —                       EX                                    (PDF 1.1) End compatibility section
                    return PdfOpe.Compatibility_EndCompatibility;
                // TABLE 4.7 Graphics state operators page 156
                case "Graphics_SaveGraphicsState":               // —                       q          gsave                      Save graphics state
                    return PdfOpe.Graphics_SaveGraphicsState;
                case "Graphics_RestoreGraphicsState":            // —                       Q          grestore                   Restore graphics state
                    return PdfOpe.Graphics_RestoreGraphicsState;
                case "Graphics_ConcatenateMatrix":               // a b c d e f             cm         concat                     Concatenate matrix to current transformation matrix
                    return PdfOpe.Graphics_ConcatenateMatrix;
                case "Graphics_SetLineWidth":                    // lineWidth               w          setlinewidth               Set line width
                    return PdfOpe.Graphics_SetLineWidth;
                case "Graphics_SetLineCap":                      // lineCap                 J          setlinecap                 Set line cap style
                    return PdfOpe.Graphics_SetLineCap;
                case "Graphics_SetLineJoin":                     // lineJoin                j          setlinejoin                Set line join style
                    return PdfOpe.Graphics_SetLineJoin;
                case "Graphics_SetMiterLimit":                   // miterLimit              M          setmiterlimit              Set miter limit
                    return PdfOpe.Graphics_SetMiterLimit;
                case "Graphics_SetLineDash":                     // dashArray dashPhase     d          setdash                    Set line dash pattern
                    return PdfOpe.Graphics_SetLineDash;
                case "Graphics_SetColorRenderingIntent":         // intent                  ri                                    Set color rendering intent
                    return PdfOpe.Graphics_SetColorRenderingIntent;
                case "Graphics_SetFlatnessTolerance":            // flatness                i          setflat                    Set flatness tolerance
                    return PdfOpe.Graphics_SetFlatnessTolerance;
                case "Graphics_SetParameters":                   // dictName                gs                                    (PDF 1.2) Set parameters from graphics state parameter dictionary
                    return PdfOpe.Graphics_SetParameters;
                // TABLE 4.9 Path construction operators page 163
                case "Path_moveto":                              // x y                     m          moveto                     Begin new subpath
                    return PdfOpe.Path_moveto;
                case "Path_lineto":                              // x y                     l          lineto                     Append straight line segment to path
                    return PdfOpe.Path_lineto;
                case "Path_curvetoThreeControl":                 // x1 y1 x2 y2 x3 y3       c          curveto                    Append curved segment to path (three control points)
                    return PdfOpe.Path_curvetoThreeControl;
                case "Path_curvetoInitialPoint":                 // x2 y2 x3 y3             v          curveto                    Append curved segment to path (initial point replicated)
                    return PdfOpe.Path_curvetoInitialPoint;
                case "Path_curvetoFinalPoint":                   // x1 y1 x3 y3             y          curveto                    Append curved segment to path (final point replicated)
                    return PdfOpe.Path_curvetoFinalPoint;
                case "Path_closepath":                           // —                       h          closepath                  Close subpath
                    return PdfOpe.Path_closepath;
                case "Path_rectangle":                           // x y width height        re                                    Append rectangle to path
                    return PdfOpe.Path_rectangle;
                // TABLE 4.10 Path-painting operators page 167
                case "Path_Stroke":                              // —                       S          stroke                     Stroke path
                    return PdfOpe.Path_Stroke;
                case "Path_Close":                               // —                       s          closepath, stroke          Close and stroke path
                    return PdfOpe.Path_Close;
                case "Path_Fill":                                // —                       f          fill                       Fill path using nonzero winding number rule
                    return PdfOpe.Path_Fill;
                case "Path_FillOld":                             // —                       F          fill                       Fill path using nonzero winding number rule (obsolete)
                    return PdfOpe.Path_FillOld;
                case "Path_FillEvenOdd*":     //                 —                       f*         eofill                     Fill path using even-odd rule
                    return PdfOpe.Path_FillEvenOdd;
                case "Path_FillStroke":                          // —                       B          fill, stroke               Fill and stroke path using nonzero winding number rule
                    return PdfOpe.Path_FillStroke;
                case "Path_FillStrokeEvenOdd":                   // —                       B*         eofill, stroke             Fill and stroke path using even-odd rule
                    return PdfOpe.Path_FillStrokeEvenOdd;
                case "Path_CloseFill":                           // —                       b          closepath, fill, stroke    Close, fill, and stroke path using nonzero winding number rule
                    return PdfOpe.Path_CloseFill;
                case "Path_CloseFillEvenOdd":                    //                                                                                           —                       b*         closepath, eofill, stroke  Close, fill, and stroke path using even-odd rule
                    return PdfOpe.Path_CloseFillEvenOdd;
                case "Path_End":                                 // —                       n                                     End path without filling or stroking
                    return PdfOpe.Path_End;
                // TABLE 4.11 Clipping path operators page 172
                case "Clipping_Clip":                            // —                       W          clip                       Set clipping path using nonzero winding number rule
                    return PdfOpe.Clipping_Clip;
                case "Clipping_ClipEvenOdd":                     //                                                                                           —                       W*         eoclip                     Set clipping path using even-odd rule
                    return PdfOpe.Clipping_ClipEvenOdd;
                // TABLE 4.21 Color operators page 216
                case "Color_SetColorSpaceStroking":              // name                    CS         setcolorspace              (PDF 1.1) Set color space for stroking operations
                    return PdfOpe.Color_SetColorSpaceStroking;
                case "Color_SetColorSpaceNonStroking":           // name                    cs         setcolorspace              (PDF 1.1) Set color space for nonstroking operations
                    return PdfOpe.Color_SetColorSpaceNonStroking;
                case "Color_ColorStroking":                      // c1...cn                 SC         setcolor                   (PDF 1.1) Set color for stroking operations
                    return PdfOpe.Color_ColorStroking;
                // c1...cn                 SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
                case "Color_ColorStroking2":                     // c1...cn name            SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
                    return PdfOpe.Color_ColorStroking2;
                case "Color_ColorNonStroking":                   // c1...cn                 sc         setcolor                   (PDF 1.1) Set color for nonstroking operations
                    return PdfOpe.Color_ColorNonStroking;
                // c1...cn                 scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
                case "Color_ColorNonStroking2":                  // c1...cn name            scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
                    return PdfOpe.Color_ColorNonStroking2;
                case "Color_SetGrayStroking":                    // gray                    G          setgray                    Set gray level for stroking operations
                    return PdfOpe.Color_SetGrayStroking;
                case "Color_SetGrayNonStroking":                 // gray                    g          setgray                    Set gray level for nonstroking operations
                    return PdfOpe.Color_SetGrayNonStroking;
                case "Color_SetRGBColorStroking":                // r g b                   RG         setrgbcolor                Set RGB color for stroking operations
                    return PdfOpe.Color_SetRGBColorStroking;
                case "Color_SetRGBColorNonStroking":             // r g b                   rg         setrgbcolor                Set RGB color for nonstroking operations
                    return PdfOpe.Color_SetRGBColorNonStroking;
                case "Color_SetCMYKColorStroking":               // c m y k                 K          setcmykcolor               Set CMYK color for stroking operations
                    return PdfOpe.Color_SetCMYKColorStroking;
                case "Color_SetCMYKColorNonStroking":            // c m y k                 k          setcmykcolor               Set CMYK color for nonstroking operations
                    return PdfOpe.Color_SetCMYKColorNonStroking;
                // TABLE 4.24 Shading operator page 232
                case "Shading_Fill":                             //                         sh         shfill                     (PDF 1.3) Paint area defined by shading pattern
                    return PdfOpe.Shading_Fill;
                // TABLE 4.34 XObject operator page 261escription
                case "XObject_Do":                               //                         Do                                    Invoke named XObject
                    return PdfOpe.XObject_Do;
                // TABLE 4.38 Inline image operators page 278
                // —                       BI                                    Begin inline image object
                // —                       ID                                    Begin inline image data
                case "Image_Begin":                              // —                       EI                                    End inline image object
                    return PdfOpe.Image_Begin;
                // TABLE 5.2 Text state operators page 302
                case "TextState_SetCharacterSpacing":            // charSpace               Tc                                    Set character spacing
                    return PdfOpe.TextState_SetCharacterSpacing;
                case "TextState_SetWordSpacing":                 // wordSpace               Tw                                    Set word spacing
                    return PdfOpe.TextState_SetWordSpacing;
                case "TextState_SetScale":                       // scale                   Tz                                    Set horizontal text scaling
                    return PdfOpe.TextState_SetScale;
                case "TextState_SetLeading":                     // leading                 TL                                    Set text leading
                    return PdfOpe.TextState_SetLeading;
                case "TextState_SetFont":                        // font size               Tf         selectfont                 Set text font and size
                    return PdfOpe.TextState_SetFont;
                case "TextState_SetRendering":                   // render                  Tr                                    Set text rendering mode
                    return PdfOpe.TextState_SetRendering;
                case "TextState_SetRise":                        // rise                    Ts                                    Set text rise
                    return PdfOpe.TextState_SetRise;
                // TABLE 5.4 Text object operators page 308
                case "TextObject_Begin":                         // —                       BT                                    Begin text object
                    return PdfOpe.TextObject_Begin;
                case "TextObject_End":                           // —                       ET                                    End text object
                    return PdfOpe.TextObject_End;
                // TABLE 5.5 Text-positioning operators page 310
                case "TextPosition_Move":                        // tx ty                   Td                                    Move text position
                    return PdfOpe.TextPosition_Move;
                case "TextPosition_MoveLeading":                 // tx ty                   TD                                    Move text position and set leading
                    return PdfOpe.TextPosition_MoveLeading;
                case "TextPosition_Matrix":                      // a b c d e f             Tm                                    Set text matrix and text line matrix
                    return PdfOpe.TextPosition_Matrix;
                case "TextPosition_NextLine":                    //                                                                                           —                       T*                                    Move to start of next text line
                    return PdfOpe.TextPosition_NextLine;
                // TABLE 5.6 Text-showing operators page 311
                case "TextShow_Show":                            // string                  Tj         show                       Show text
                    return PdfOpe.TextShow_Show;
                case "TextShow_ShowNextLine":                    // string                  '                                     Move to next line and show text
                    return PdfOpe.TextShow_ShowNextLine;
                case "TextShow_ShowSpacingNextLine":             // aw ac string            "                                     Set word and character spacing, move to next line, and show text
                    return PdfOpe.TextShow_ShowSpacingNextLine;
                case "TextShow_ShowGlyph":                       // array                   TJ                                    Show text, allowing individual glyph positioning
                    return PdfOpe.TextShow_ShowGlyph;
                // TABLE 5.10 Type 3 font operators page 326
                case "Font_Width":                               // wx wy                   d0         setcharwidth               Set glyph width in Type 3 font
                    return PdfOpe.Font_Width;
                case "Font_WidthBounding":                       // wx wy llx lly urx ury   d1         setcachedevice             Set glyph width and bounding box in Type 3 font
                    return PdfOpe.Font_WidthBounding;
                // TABLE 9.8 Marked-content operators page 584
                case "MarkedContent_Define":                     // tag                     MP                                    (PDF 1.2) Define marked-content point
                    return PdfOpe.MarkedContent_Define;
                case "MarkedContent_DefineProperty":             // tag properties          DP                                    (PDF 1.2) Define marked-content point with property list
                    return PdfOpe.MarkedContent_DefineProperty;
                case "MarkedContent_Begin":                      // tag                     BMC                                   (PDF 1.2) Begin marked-content sequence
                    return PdfOpe.MarkedContent_Begin;
                case "MarkedContent_BeginProperty":              // tag properties          BDC                                   (PDF 1.2) Begin marked-content sequence with property list
                    return PdfOpe.MarkedContent_BeginProperty;
                case "MarkedContent_End":                        // —                       EMC                                   (PDF 1.2) End marked-content sequence
                    return PdfOpe.MarkedContent_End;
                default:
                    throw new PBException("error unknow operator \"{0}\"", opeString);
                //return PdfOpe.Unknow;
            }
        }

        public PdfInstructionInilineImage ReadInlineImageTxt()
        {
            PdfInstructionInilineImage instruction = new PdfInstructionInilineImage();
            PdfObjectReader por = new PdfObjectReader(new PdfStreamReader(gr));
            PdfValueObject prm = new PdfValueObject();
            while (true)
            {
                string s = gr.ReadLine();
                if (s == null) throw new PBException("error reading inline image Image_Data not found");
                if (s == "Image_Data") break;
                PdfNValue value = por.ReadPdfNValue(ref s);
                prm[value.name] = value;
                if (s != "") throw new PBException("error reading inline image wrong parameter \"{0}\"", s);
            }
            instruction.prm = prm;
            //instruction.stream = gr.ReadBytesUntil("\nEI\n");
            List<byte> buffer = new List<byte>();
            while (true)
            {
                string s = gr.ReadLine();
                if (s == null) throw new PBException("error reading inline image Image_End not found");
                if (s == "Image_End") break;
            }
            instruction.stream = buffer.ToArray();
            return instruction;
        }
    }

    public class PdfDataReader
    {
        private Reader gr = null;
        private Regex grgOpe = new Regex(" *([a-zA-Z\\*]+|'|\")$", RegexOptions.Compiled | RegexOptions.RightToLeft);
        private Regex grgEmpty = new Regex(@"^ *$", RegexOptions.Compiled);
        public TraceDelegate Trace = null;

        public PdfDataReader(byte[] data)
        {
            gr = new Reader(data);
        }

        public PdfDataReader(string file, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, Encoding encoding = null)
        {
            gr = new Reader(file, mode, access, share, encoding);
        }

        private void trace(string msg, params object[] prm)
        {
            if (Trace == null) return;
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Trace(msg);
        }

        public static IPdfInstruction[] PdfReadAll(byte[] data)
        {
            PdfDataReader pdr = new PdfDataReader(data);
            return pdr.ReadAll();
        }

        public IPdfInstruction[] ReadAll()
        {
            List<IPdfInstruction> instructions = new List<IPdfInstruction>();
            while (true)
            {
                IPdfInstruction instruction = Read();
                if (instruction == null) break;
                trace("read {0}", instruction.opeString);
                instructions.Add(instruction);
            }
            return instructions.ToArray();
        }

        public IPdfInstruction Read()
        {
            Match match;
            string s;
            while (true)
            {
                s = gr.ReadLine();
                if (s == null) return null;
                match = grgOpe.Match(s);
                if (match.Success) break;
                match = grgEmpty.Match(s);
                if (!match.Success) throw new PBException("error wrong instruction \"{0}\" line {1}", s, gr.LineNumber);
            }
            s = s.Substring(0, s.Length - match.Length);
            PdfOpe ope;
            ope = GetPdfOpe(match.Groups[1].Value);
            if (ope == PdfOpe.Image_Begin)
            {
                if (s != "") throw new PBException("error bad BI instruction \"{0}\" line {1}", s, gr.LineNumber);
                return ReadInlineImage();
            }
            else
            {
                if (s == "") s = null;
                return new PdfInstruction(ope, match.Groups[1].Value, s);
            }
        }

        public PdfOpe GetPdfOpe(string opeString)
        {
            switch (opeString)
            {
                //                operands                operator   postscript                 description
                case "BX":     // —                       BX                                    (PDF 1.1) Begin compatibility section
                    return PdfOpe.Compatibility_BeginCompatibility;
                case "EX":     // —                       EX                                    (PDF 1.1) End compatibility section
                    return PdfOpe.Compatibility_EndCompatibility;
                // TABLE 4.7 Graphics state operators page 156
                case "q":      // —                       q          gsave                      Save graphics state
                    return PdfOpe.Graphics_SaveGraphicsState;
                case "Q":      // —                       Q          grestore                   Restore graphics state
                    return PdfOpe.Graphics_RestoreGraphicsState;
                case "cm":     // a b c d e f             cm         concat                     Concatenate matrix to current transformation matrix
                    return PdfOpe.Graphics_ConcatenateMatrix;
                case "w":      // lineWidth               w          setlinewidth               Set line width
                    return PdfOpe.Graphics_SetLineWidth;
                case "J":      // lineCap                 J          setlinecap                 Set line cap style
                    return PdfOpe.Graphics_SetLineCap;
                case "j":      // lineJoin                j          setlinejoin                Set line join style
                    return PdfOpe.Graphics_SetLineJoin;
                case "M":      // miterLimit              M          setmiterlimit              Set miter limit
                    return PdfOpe.Graphics_SetMiterLimit;
                case "d":      // dashArray dashPhase     d          setdash                    Set line dash pattern
                    return PdfOpe.Graphics_SetLineDash;
                case "ri":     // intent                  ri                                    Set color rendering intent
                    return PdfOpe.Graphics_SetColorRenderingIntent;
                case "i":      // flatness                i          setflat                    Set flatness tolerance
                    return PdfOpe.Graphics_SetFlatnessTolerance;
                case "gs":     // dictName                gs                                    (PDF 1.2) Set parameters from graphics state parameter dictionary
                    return PdfOpe.Graphics_SetParameters;
                // TABLE 4.9 Path construction operators page 163
                case "m":      // x y                     m          moveto                     Begin new subpath
                    return PdfOpe.Path_moveto;
                case "l":      // x y                     l          lineto                     Append straight line segment to path
                    return PdfOpe.Path_lineto;
                case "c":      // x1 y1 x2 y2 x3 y3       c          curveto                    Append curved segment to path (three control points)
                    return PdfOpe.Path_curvetoThreeControl;
                case "v":      // x2 y2 x3 y3             v          curveto                    Append curved segment to path (initial point replicated)
                    return PdfOpe.Path_curvetoInitialPoint;
                case "y":      // x1 y1 x3 y3             y          curveto                    Append curved segment to path (final point replicated)
                    return PdfOpe.Path_curvetoFinalPoint;
                case "h":      // —                       h          closepath                  Close subpath
                    return PdfOpe.Path_closepath;
                case "re":     // x y width height        re                                    Append rectangle to path
                    return PdfOpe.Path_rectangle;
                // TABLE 4.10 Path-painting operators page 167
                case "S":      // —                       S          stroke                     Stroke path
                    return PdfOpe.Path_Stroke;
                case "s":      // —                       s          closepath, stroke          Close and stroke path
                    return PdfOpe.Path_Close;
                case "f":      // —                       f          fill                       Fill path using nonzero winding number rule
                    return PdfOpe.Path_Fill;
                case "F":      // —                       F          fill                       Fill path using nonzero winding number rule (obsolete)
                    return PdfOpe.Path_FillOld;
                case "f*":     //                                                                                           —                       f*         eofill                     Fill path using even-odd rule
                    return PdfOpe.Path_FillEvenOdd;
                case "B":      // —                       B          fill, stroke               Fill and stroke path using nonzero winding number rule
                    return PdfOpe.Path_FillStroke;
                case "B*":     //                                                                                           —                       B*         eofill, stroke             Fill and stroke path using even-odd rule
                    return PdfOpe.Path_FillStrokeEvenOdd;
                case "b":      // —                       b          closepath, fill, stroke    Close, fill, and stroke path using nonzero winding number rule
                    return PdfOpe.Path_CloseFill;
                case "b*":     //                                                                                           —                       b*         closepath, eofill, stroke  Close, fill, and stroke path using even-odd rule
                    return PdfOpe.Path_CloseFillEvenOdd;
                case "n":      // —                       n                                     End path without filling or stroking
                    return PdfOpe.Path_End;
                // TABLE 4.11 Clipping path operators page 172
                case "W":      // —                       W          clip                       Set clipping path using nonzero winding number rule
                    return PdfOpe.Clipping_Clip;
                case "W*":     //                                                                                           —                       W*         eoclip                     Set clipping path using even-odd rule
                    return PdfOpe.Clipping_ClipEvenOdd;
                // TABLE 4.21 Color operators page 216
                case "CS":     // name                    CS         setcolorspace              (PDF 1.1) Set color space for stroking operations
                    return PdfOpe.Color_SetColorSpaceStroking;
                case "cs":     // name                    cs         setcolorspace              (PDF 1.1) Set color space for nonstroking operations
                    return PdfOpe.Color_SetColorSpaceNonStroking;
                case "SC":     // c1...cn                 SC         setcolor                   (PDF 1.1) Set color for stroking operations
                    return PdfOpe.Color_ColorStroking;
                               // c1...cn                 SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
                case "SCN":    // c1...cn name            SCN        setcolor                   (PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)
                    return PdfOpe.Color_ColorStroking2;
                case "sc":     // c1...cn                 sc         setcolor                   (PDF 1.1) Set color for nonstroking operations
                    return PdfOpe.Color_ColorNonStroking;
                               // c1...cn                 scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
                case "scn":    // c1...cn name            scn        setcolor                   (PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)
                    return PdfOpe.Color_ColorNonStroking2;
                case "G":      // gray                    G          setgray                    Set gray level for stroking operations
                    return PdfOpe.Color_SetGrayStroking;
                case "g":      // gray                    g          setgray                    Set gray level for nonstroking operations
                    return PdfOpe.Color_SetGrayNonStroking;
                case "RG":     // r g b                   RG         setrgbcolor                Set RGB color for stroking operations
                    return PdfOpe.Color_SetRGBColorStroking;
                case "rg":     // r g b                   rg         setrgbcolor                Set RGB color for nonstroking operations
                    return PdfOpe.Color_SetRGBColorNonStroking;
                case "K":      // c m y k                 K          setcmykcolor               Set CMYK color for stroking operations
                    return PdfOpe.Color_SetCMYKColorStroking;
                case "k":      // c m y k                 k          setcmykcolor               Set CMYK color for nonstroking operations
                    return PdfOpe.Color_SetCMYKColorNonStroking;
                // TABLE 4.24 Shading operator page 232
                case "sh":     //                         sh         shfill                     (PDF 1.3) Paint area defined by shading pattern
                    return PdfOpe.Shading_Fill;
                // TABLE 4.34 XObject operator page 261escription
                case "Do":     //                         Do                                    Invoke named XObject
                    return PdfOpe.XObject_Do;
                // TABLE 4.38 Inline image operators page 278
                               // —                       BI                                    Begin inline image object
                               // —                       ID                                    Begin inline image data
                case "BI":     // —                       EI                                    End inline image object
                    return PdfOpe.Image_Begin;
                // TABLE 5.2 Text state operators page 302
                case "Tc":     // charSpace               Tc                                    Set character spacing
                    return PdfOpe.TextState_SetCharacterSpacing;
                case "Tw":     // wordSpace               Tw                                    Set word spacing
                    return PdfOpe.TextState_SetWordSpacing;
                case "Tz":     // scale                   Tz                                    Set horizontal text scaling
                    return PdfOpe.TextState_SetScale;
                case "TL":     // leading                 TL                                    Set text leading
                    return PdfOpe.TextState_SetLeading;
                case "Tf":     // font size               Tf         selectfont                 Set text font and size
                    return PdfOpe.TextState_SetFont;
                case "Tr":     // render                  Tr                                    Set text rendering mode
                    return PdfOpe.TextState_SetRendering;
                case "Ts":     // rise                    Ts                                    Set text rise
                    return PdfOpe.TextState_SetRise;
                // TABLE 5.4 Text object operators page 308
                case "BT":     // —                       BT                                    Begin text object
                    return PdfOpe.TextObject_Begin;
                case "ET":     // —                       ET                                    End text object
                    return PdfOpe.TextObject_End;
                // TABLE 5.5 Text-positioning operators page 310
                case "Td":     // tx ty                   Td                                    Move text position
                    return PdfOpe.TextPosition_Move;
                case "TD":     // tx ty                   TD                                    Move text position and set leading
                    return PdfOpe.TextPosition_MoveLeading;
                case "Tm":     // a b c d e f             Tm                                    Set text matrix and text line matrix
                    return PdfOpe.TextPosition_Matrix;
                case "T*":     //                                                                                           —                       T*                                    Move to start of next text line
                    return PdfOpe.TextPosition_NextLine;
                // TABLE 5.6 Text-showing operators page 311
                case "Tj":     // string                  Tj         show                       Show text
                    return PdfOpe.TextShow_Show;
                case "'":      // string                  '                                     Move to next line and show text
                    return PdfOpe.TextShow_ShowNextLine;
                case "\"":     // aw ac string            "                                     Set word and character spacing, move to next line, and show text
                    return PdfOpe.TextShow_ShowSpacingNextLine;
                case "TJ":     // array                   TJ                                    Show text, allowing individual glyph positioning
                    return PdfOpe.TextShow_ShowGlyph;
                // TABLE 5.10 Type 3 font operators page 326
                case "d0":     // wx wy                   d0         setcharwidth               Set glyph width in Type 3 font
                    return PdfOpe.Font_Width;
                case "d1":     // wx wy llx lly urx ury   d1         setcachedevice             Set glyph width and bounding box in Type 3 font
                    return PdfOpe.Font_WidthBounding;
                // TABLE 9.8 Marked-content operators page 584
                case "MP":     // tag                     MP                                    (PDF 1.2) Define marked-content point
                    return PdfOpe.MarkedContent_Define;
                case "DP":     // tag properties          DP                                    (PDF 1.2) Define marked-content point with property list
                    return PdfOpe.MarkedContent_DefineProperty;
                case "BMC":    // tag                     BMC                                   (PDF 1.2) Begin marked-content sequence
                    return PdfOpe.MarkedContent_Begin;
                case "BDC":    // tag properties          BDC                                   (PDF 1.2) Begin marked-content sequence with property list
                    return PdfOpe.MarkedContent_BeginProperty;
                case "EMC":    // —                       EMC                                   (PDF 1.2) End marked-content sequence
                    return PdfOpe.MarkedContent_End;
                default:
                    throw new PBException("error unknow operator \"{0}\"", opeString);
                    //return PdfOpe.Unknow;
            }
        }

        public PdfInstructionInilineImage ReadInlineImage()
        {
            PdfInstructionInilineImage instruction = new PdfInstructionInilineImage();
            PdfObjectReader por = new PdfObjectReader(new PdfStreamReader(gr));
            PdfValueObject prm = new PdfValueObject();
            while (true)
            {
                string s = gr.ReadLine();
                if (s == null) throw new PBException("error reading inline image ID not found");
                if (s == "ID") break;
                PdfNValue value = por.ReadPdfNValue(ref s);
                prm[value.name] = value;
                if (s != "") throw new PBException("error reading inline image wrong parameter \"{0}\"", s);
            }
            instruction.prm = prm;
            instruction.stream = gr.ReadBytesUntil("\nEI\n");
            return instruction;
        }
    }
}
