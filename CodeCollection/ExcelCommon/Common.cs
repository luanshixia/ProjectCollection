using System;
using System.Collections.Generic;
using System.Linq;

//
// The Excel common exchange format model definitions
// ==================================================

namespace C1.Web.Api.Excel
{
    /// <summary>
    /// The Excel workbook.
    /// </summary>
    public class Workbook
    {
        /// <summary>
        /// The worksheet collection of the workbook.
        /// </summary>
        public List<Worksheet> Sheets { get; set; }
    }

    /// <summary>
    /// The Excel worksheet.
    /// </summary>
    public class Worksheet
    {
        /// <summary>
        /// The name of the worksheet.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The cells data in the worksheet.
        /// </summary>
        public List<List<string>> Data { get; set; }
        /// <summary>
        /// The default styles of the worksheet.
        /// </summary>
        public Style GlobalStyle { get; set; }
        /// <summary>
        /// The range based style settings.
        /// </summary>
        public List<RangeStyle> RangeStyles { get; set; }
        /// <summary>
        /// The row collection.
        /// </summary>
        public List<Row> Rows { get; set; }
        /// <summary>
        /// The column collection.
        /// </summary>
        public List<Column> Colunms { get; set; }
        /// <summary>
        /// The default height of rows.
        /// </summary>
        public int? DefaultRowHeight { get; set; }
        /// <summary>
        /// The default width of columns.
        /// </summary>
        public int? DefaultColumnWidth { get; set; }
    }

    /// <summary>
    /// The coordinates of a cell in a worksheet.
    /// </summary>
    public class CellPosition
    {
        /// <summary>
        /// The vertical coordinate.
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// The horizontal coordinate.
        /// </summary>
        public int Col { get; set; }
    }

    /// <summary>
    /// A rectangular area in a worksheet denominated by a top-left cell and a bottom-right cell.
    /// </summary>
    public class Range
    {
        /// <summary>
        /// The top-left cell position.
        /// </summary>
        public CellPosition From { get; set; }
        /// <summary>
        /// The bottom-right cell position.
        /// </summary>
        public CellPosition To { get; set; }
    }

    /// <summary>
    /// A column in a worksheet.
    /// </summary>
    public class Column
    {
        /// <summary>
        /// The width of the column.
        /// </summary>
        public int? Width { get; set; }
        /// <summary>
        /// The style of the column.
        /// </summary>
        public Style Style { get; set; }
    }

    /// <summary>
    /// A row in a worksheet.
    /// </summary>
    public class Row
    {
        /// <summary>
        /// The height of the row.
        /// </summary>
        public int? Height { get; set; }
        /// <summary>
        /// The style of the row.
        /// </summary>
        public Style Style { get; set; }
    }

    /// <summary>
    /// The appearance attributes of certain cells.
    /// </summary>
    public class Style
    {
        /// <summary>
        /// Data type.
        /// </summary>
        public NumberFormat? NumberFormat { get; set; }
        /// <summary>
        /// Font of text.
        /// </summary>
        public Font Font { get; set; }
        /// <summary>
        /// Horizontal alignment of cell content.
        /// </summary>
        public HAlign? HAlign { get; set; }
        /// <summary>
        /// Vertical alignment of cell content.
        /// </summary>
        public VAlign? VAlign { get; set; }
        /// <summary>
        /// Text indent.
        /// </summary>
        public int? Indent { get; set; }
        /// <summary>
        /// Rotation angle of text.
        /// </summary>
        public int? TextRotation { get; set; }
        /// <summary>
        /// Direction of text flow.
        /// </summary>
        public TextDirection? TextDirection { get; set; }
        /// <summary>
        /// Cell fill.
        /// </summary>
        public Fill Fill { get; set; }
        /// <summary>
        /// Cell border.
        /// </summary>
        public Border Border { get; set; }
        /// <summary>
        /// Allow text wrap.
        /// </summary>
        public bool? WrapText { get; set; }
        /// <summary>
        /// Allow text shrink to fit cell size.
        /// </summary>
        public bool? ShrinkToFit { get; set; }
        /// <summary>
        /// Whether to merge the range to a single cell.
        /// </summary>
        public bool? MergedCells { get; set; }
        /// <summary>
        /// Whether to lock the cells.
        /// </summary>
        public bool? Locked { get; set; }
        /// <summary>
        /// Whether to hide the cells.
        /// </summary>
        public bool? Hidden { get; set; }
    }

    /// <summary>
    /// Styles applied to a range.
    /// </summary>
    public class RangeStyle
    {
        /// <summary>
        /// The range to apply the styles to.
        /// </summary>
        public Range Range { get; set; }
        /// <summary>
        /// The styles to apply to the range.
        /// </summary>
        public Style Style { get; set; }
    }

    /// <summary>
    /// Font styles.
    /// </summary>
    public class Font
    {
        /// <summary>
        /// Typeface name.
        /// </summary>
        public string Family { get; set; }
        /// <summary>
        /// Font size.
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// Whether to make the text bold.
        /// </summary>
        public bool? Bold { get; set; }
        /// <summary>
        /// Whether to make the text italic.
        /// </summary>
        public bool? Italic { get; set; }
        /// <summary>
        /// Underscore setting.
        /// </summary>
        public Underline? Underline { get; set; }
        /// <summary>
        /// Whether to show strikethrough.
        /// </summary>
        public bool? Strikethrough { get; set; }
        /// <summary>
        /// Whether to make the text superscript.
        /// </summary>
        public bool? Superscript { get; set; }
        /// <summary>
        /// Whether to make the text subscript.
        /// </summary>
        public bool? Subscript { get; set; }
        /// <summary>
        /// Text color.
        /// </summary>
        public string Color { get; set; }
    }

    /// <summary>
    /// Border styles.
    /// </summary>
    public class Border
    {
        /// <summary>
        /// Border color
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// Border linetype.
        /// </summary>
        public BorderStyle? Style { get; set; }
        /// <summary>
        /// Border position.
        /// </summary>
        public BorderPosition? Position { get; set; }
    }

    /// <summary>
    /// Cell fill styles.
    /// </summary>
    public class Fill
    {
        /// <summary>
        /// Fill color.
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// Fill pattern.
        /// </summary>
        public PatternStyle? Pattern { get; set; }
    }

    /// <summary>
    /// The data type of the content in a cell.
    /// </summary>
    public enum NumberFormat
    {
        General,
        Number,
        Currency,
        Accounting,
        Date,
        Time,
        Percentage,
        Fraction,
        Scientific,
        Text,
        Special,
        Custom
    }

    /// <summary>
    /// Horizontal alignment options.
    /// </summary>
    public enum HAlign
    {
        General,
        Left,
        Center,
        Right,
        Fill,
        Justify,
        CenterAcrossSelection,
        Distributed
    }

    /// <summary>
    /// Vertical alignment options.
    /// </summary>
    public enum VAlign
    {
        Top,
        Center,
        Bottom,
        Justify,
        Distributed
    }

    /// <summary>
    /// The direction of text flow.
    /// </summary>
    public enum TextDirection
    {
        Context,
        LeftToRight,
        RightToLeft
    }

    /// <summary>
    /// The type of text underscore.
    /// </summary>
    public enum Underline
    {
        None,
        Single,
        Double,
        SingleAccounting,
        DoubleAccounting
    }

    /// <summary>
    /// The position of the cell borders.
    /// </summary>
    [Flags]
    public enum BorderPosition
    {
        None = 0x00,
        Bottom = 0x01,
        Top = 0x02,
        Left = 0x04,
        Right = 0x08,
        Outline = Bottom | Top | Left | Right,
        Inside = 0x10,
        All = Outline | Inside
    }

    /// <summary>
    /// The style of the cell borders.
    /// </summary>
    public enum BorderStyle
    {
        None = 0,
        Hair = 1,
        Dotted = 2,
        DashDot = 3,
        Thin = 4,
        DashDotDot = 5,
        Dashed = 6,
        MediumDashDotDot = 7,
        MediumDashed = 8,
        MediumDashDot = 9,
        Thick = 10,
        Medium = 11,
        Double = 12
    }

    /// <summary>
    /// The pattern type of the cell fills.
    /// </summary>
    public enum PatternStyle
    {
        None = 0,
        Solid = 1,
        DarkGray = 2,
        MediumGray = 3,
        LightGray = 4,
        Gray125 = 5,
        Gray0625 = 6,
        DarkVertical = 7,
        DarkHorizontal = 8,
        DarkDown = 9,
        DarkUp = 10,
        DarkGrid = 11,
        DarkTrellis = 12,
        LightVertical = 13,
        LightHorizontal = 14,
        LightDown = 15,
        LightUp = 16,
        LightGrid = 17,
        LightTrellis = 18,
    }
}