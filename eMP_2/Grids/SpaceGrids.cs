namespace eMP_2;

public class SpaceRegularGrid : RegularGrid {
    public override bool TimeDependent => false;

    public SpaceRegularGrid(GridParameters gridParameters) : base(gridParameters) { }
}

public class SpaceIrregularGrid : IrregularGrid {
    public override bool TimeDependent => false;

    public SpaceIrregularGrid(GridParameters gridParameters) : base(gridParameters) { }
}

public class SpaceNestedGrid : NestedGrid {
    public override bool TimeDependent => false;

    public SpaceNestedGrid(GridParameters gridParameters) : base(gridParameters) { }
}