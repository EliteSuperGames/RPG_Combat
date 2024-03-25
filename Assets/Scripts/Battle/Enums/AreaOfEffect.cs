public enum AreaOfEffect
{
    /// <summary>
    /// Only targets the single target
    /// </summary>
    Single,

    /// <summary>
    /// Targets the same row (front row, back row)
    /// </summary>
    SameRow,

    /// <summary>
    /// Targets in a line. So it would hit the front and back row in a line
    /// </summary>
    Line,

    /// <summary>
    /// Targets the row and the 4 spaces around it
    /// </summary>
    Cross,

    /// <summary>
    /// Not sure what this will be yet. Maybe a block of 4 spaces? Or a cross shape
    /// </summary>
    Square,

    /// <summary>
    /// Targets all enemies or allies
    /// </summary>
    All
}
