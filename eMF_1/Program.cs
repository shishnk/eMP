using eMF_1;

GridFactory gridFactory = new();
// MFD mfd = new(gridFactory.CreateGrid(GridType.Irregular, "grid/grid(irregular).txt"));
MFD mfd = new(gridFactory.CreateGrid(GridType.Regular, "grid/grid(regular).txt"));
mfd.SetTest(new FirstTest());
mfd.SetMethodSolvingSLAE(new GaussSeidel(1000, 1E-14));
mfd.Compute();