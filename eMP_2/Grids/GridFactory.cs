namespace eMP_2;

public enum GridTypes {
    SpaceRegular,
    SpaceIrregular,
    SpaceNested,
    TimeRegular,
    TimeIrregular,
    TimeNested
}

public abstract class GridFactory {
    public abstract IGrid CreateGrid(GridTypes gridType, GridParameters gridParameters);
}