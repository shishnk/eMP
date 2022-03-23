namespace eMP_2;
    public class GridFactorySpace : GridFactory {
        protected override Grid CreateGrid(GridType spaceGridType) {
            return spaceGridType switch {
                GridType.SpaceRegular => new SpaceRegularGrid(),

                GridType.SpaceIrregular => new SpaceIrregularGrid(),

                GridType.SpaceNested => new SpaceNestedGrid(),

                 _ => throw new ArgumentOutOfRangeException(nameof(spaceGridType), $"This type of grid does not exist: {spaceGridType}"),
            };
        }
    }