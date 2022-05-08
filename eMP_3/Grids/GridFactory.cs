namespace eMP_3;

public enum GridTypes {
    Regular,
    Irregular
}

public interface IFactory {
    public Grid CreateGrid(GridTypes gridType, GridParameters gridParameters);
}

public class GridFactory : IFactory {
    public Grid CreateGrid(GridTypes gridType, GridParameters gridParameters) {
        return gridType switch {
            GridTypes.Regular => new RegularGrid(gridParameters),
            GridTypes.Irregular => new IrregularGrid(gridParameters),

            _ => throw new InvalidEnumArgumentException($"This type of grid does not exist: {nameof(gridType)}")
        };
    }
}