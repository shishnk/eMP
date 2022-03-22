namespace eMF_1;

public class GridFactory
{
    public Grid CreateGrid(GridType gridType, string path)
    {
        return gridType switch
        {
            GridType.Regular => new RegularGrid(path),

            GridType.Irregular => new IrregularGrid(path),

            GridType.Nested => new NestedGrid(path),

            _ => throw new ArgumentOutOfRangeException(nameof(gridType),
            $"This type of grid does not exist: {gridType}")
        };
    }
}