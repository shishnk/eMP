namespace eMP_2;
    public class GridFactoryTime : GridFactory {
        protected override Grid CreateGrid(GridType timeGridType) {
            return timeGridType switch {
                GridType.TimeRegular => new TimeRegularGrid(),

                GridType.TimeIrregular => new TimeIrregularGrid(),

                GridType.TimeNested => new TimeNestedGrid(),

                 _ => throw new ArgumentOutOfRangeException(nameof(timeGridType), $"This type of grid does not exist: {timeGridType}"),
            };
        }
    }