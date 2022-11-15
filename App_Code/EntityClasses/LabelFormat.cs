public class LabelFormat
{
    /// <summary>
    ///     ''' Numerical Id of the label format
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public int Id { get; set; }
    /// <summary>
    ///     ''' Name of the label format (e.g. Avery L7163)
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public string Name { get; set; }
    /// <summary>
    ///     ''' Description of label format (e.g. A4 Sheet of 99.1 x 38.1mm address labels)
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public string Description { get; set; }
    /// <summary>
    ///     ''' Width of page in millimeters
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double PageWidth { get; set; }
    /// <summary>
    ///     ''' Height of page in millimeters
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double PageHeight { get; set; }
    /// <summary>
    ///     ''' Margin between top of page and top of first label in millimeters
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double TopMargin { get; set; }
    /// <summary>
    ///     ''' Margin between left of page and left of first label in millimeters
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double LeftMargin { get; set; }
    /// <summary>
    ///     ''' Width of individual label in millimeters
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double LabelWidth { get; set; }
    /// <summary>
    ///     ''' Height of individual label in millimeters
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double LabelHeight { get; set; }
    /// <summary>
    ///     ''' Padding on the left of an individual label, creates space between label edge and start of content
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double LabelPaddingLeft { get; set; }
    /// <summary>
    ///     ''' Padding on the Right of an individual label, creates space between label edge and end of content
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double LabelPaddingRight { get; set; }
    /// <summary>
    ///     ''' Padding on the top of an individual label, creates space between label edge and start of content
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double LabelPaddingTop { get; set; }
    /// <summary>
    ///     ''' Padding on the Bottom of an individual label, creates space between label edge and end of content
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double LabelPaddingBottom { get; set; }
    /// <summary>
    ///     ''' Distance between top of one label and top of label below it in millimeters
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double VerticalPitch { get; set; }
    /// <summary>
    ///     ''' Distance between left of one label and left of label to the right of it in millimeters
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public double HorizontalPitch { get; set; }
    /// <summary>
    ///     ''' Number of labels going across the page
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public int ColumnCount { get; set; }
    /// <summary>
    ///     ''' Number of labels going down the page
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public int RowCount { get; set; }

    /// <summary>
    ///     ''' Instantiate a new label sheet format definition
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public LabelFormat()
    {
    }

    /// <summary>
    ///     ''' Instantiate a new label sheet format definition
    ///     ''' </summary>
    ///     ''' <param name="Id">Numerical Id of the label format</param>
    ///     ''' <param name="Name">Name of the label format (e.g. Avery L7163)</param>
    ///     ''' <param name="Description">Description of label format (e.g. A4 Sheet of 99.1 x 38.1mm address labels)</param>
    ///     ''' <param name="PageWidth">Width of page in millimeters</param>
    ///     ''' <param name="PageHeight">Height of page in millimeters</param>
    ///     ''' <param name="TopMargin">Margin between top of page and top of first label in millimeters</param>
    ///     ''' <param name="LeftMargin">Margin between left of page and left of first label in millimeters</param>
    ///     ''' <param name="LabelWidth">Width of individual label in millimeters</param>
    ///     ''' <param name="LabelHeight">Height of individual label in millimeters</param>
    ///     ''' <param name="VerticalPitch">Distance between top of one label and top of label below it in millimeters</param>
    ///     ''' <param name="HorizontalPitch">Distance between left of one label and left of label to the right of it in millimeters</param>
    ///     ''' <param name="ColumnCount">Number of labels going across the page</param>
    ///     ''' <param name="RowCount">Number of labels going down the page</param>
    ///     ''' <param name="LabelPaddingLeft">Padding on the left of an individual label, creates space between label edge and start of content</param>
    ///     ''' <param name="LabelPaddingRight">Padding on the Right of an individual label, creates space between label edge and end of content</param>
    ///     ''' <param name="LabelPaddingTop">Padding on the top of an individual label, creates space between label edge and start of content</param>
    ///     ''' <param name="LabelPaddingBottom">Padding on the Bottom of an individual label, creates space between label edge and end of content</param>
    ///     ''' <remarks></remarks>
    public LabelFormat(int Id, string Name, string Description, double PageWidth, double PageHeight, double TopMargin, double LeftMargin, double LabelWidth, double LabelHeight, double VerticalPitch, double HorizontalPitch, int ColumnCount, int RowCount, double LabelPaddingLeft = 0.0, double LabelPaddingRight = 0.0, double LabelPaddingTop = 0.0, double LabelPaddingBottom = 0.0)
    {
        this.Id = Id;
        this.Name = Name;
        this.Description = Description;
        this.PageWidth = PageWidth;
        this.PageHeight = PageHeight;
        this.TopMargin = TopMargin;
        this.LeftMargin = LeftMargin;
        this.LabelWidth = LabelWidth;
        this.LabelHeight = LabelHeight;
        this.VerticalPitch = VerticalPitch;
        this.HorizontalPitch = HorizontalPitch;
        this.ColumnCount = ColumnCount;
        this.RowCount = RowCount;
        this.LabelPaddingLeft = LabelPaddingLeft;
        this.LabelPaddingRight = LabelPaddingRight;
        this.LabelPaddingTop = LabelPaddingTop;
        this.LabelPaddingBottom = LabelPaddingBottom;
    }
}
