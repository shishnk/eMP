namespace eMP_2;

    public class GridFactoryTime : GridFactory {
        public override Grid CreateGrid(GridType timeGridType, GridParameters gridParameters) {
            return timeGridType switch {
                GridType.TimeRegular => new TimeRegularGrid(gridParameters),

                GridType.TimeIrregular => new TimeIrregularGrid(gridParameters),

                GridType.TimeNested => new TimeNestedGrid(gridParameters),

                 _ => throw new ArgumentOutOfRangeException(nameof(timeGridType), $"This type of grid does not exist: {timeGridType}"),
            };
        }
    }