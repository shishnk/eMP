namespace eMP_2;

public class TimeRegularGrid : RegularGrid {
    public override bool TimeDependent => true;

    public TimeRegularGrid(GridParameters gridParameters) : base(gridParameters) { }
}

public class TimeIrregularGrid : IrregularGrid {
    public override bool TimeDependent => true;

    public TimeIrregularGrid(GridParameters gridParameters) : base(gridParameters) { }
}

public class TimeNestedGrid : NestedGrid {
    public override bool TimeDependent => true;

    public TimeNestedGrid(GridParameters gridParameters) : base(gridParameters) { }
}