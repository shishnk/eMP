namespace eMP_2;

public enum GridType {
    SpaceRegular,
    SpaceIrregular,
    SpaceNested,
    TimeRegular,
    TimeIrregular,
    TimeNested
}

public abstract class GridFactory {
    public abstract Grid CreateGrid(GridType gridType, GridParameters gridParameters);
}