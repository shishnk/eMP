namespace eMP_2;

    public class GridFactoryTime : GridFactory {
        public override IGrid CreateGrid(GridTypes timeGridType, GridParameters gridParameters) {
            return timeGridType switch {
                GridTypes.TimeRegular => new TimeRegularGrid(gridParameters),

                GridTypes.TimeIrregular => new TimeIrregularGrid(gridParameters),

                GridTypes.TimeNested => new TimeNestedGrid(gridParameters),

                 _ => throw new ArgumentOutOfRangeException(nameof(timeGridType), $"This type of grid does not exist: {timeGridType}"),
            };
        }
    }