namespace eMP_2;

public class GridFactorySpace : GridFactory {
    public override IGrid CreateGrid(GridTypes spaceGridType, GridParameters gridParameters) {
        return spaceGridType switch {
            GridTypes.SpaceRegular => new SpaceRegularGrid(gridParameters),

            GridTypes.SpaceIrregular => new SpaceIrregularGrid(gridParameters),

            GridTypes.SpaceNested => new SpaceNestedGrid(gridParameters),

            _ => throw new ArgumentOutOfRangeException(nameof(spaceGridType), $"This type of grid does not exist: {spaceGridType}"),
        };
    }
}