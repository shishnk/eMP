namespace eMP_2;

public class GridFactorySpace : GridFactory {
    public override Grid CreateGrid(GridType spaceGridType, GridParameters gridParameters) {
        return spaceGridType switch {
            GridType.SpaceRegular => new SpaceRegularGrid(gridParameters),

            GridType.SpaceIrregular => new SpaceIrregularGrid(gridParameters),

            GridType.SpaceNested => new SpaceNestedGrid(gridParameters),

            _ => throw new ArgumentOutOfRangeException(nameof(spaceGridType), $"This type of grid does not exist: {spaceGridType}"),
        };
    }
}