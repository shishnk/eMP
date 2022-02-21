using eMF_1;

GridFactory gridFactory = new();
// MFD mfd = new(gridFactory.CreateGrid(GridType.Irregular, "grid/grid(irregular).txt"), "boundaries.txt");
MFD mfd = new(gridFactory.CreateGrid(GridType.Regular, "grid/grid(regular).txt"), "boundaries.txt");
// mfd.SetTest(new FirstTest());
mfd.SetTest(new SecondTest());
// mfd.SetTest(new ThirdTest());
// mfd.SetTest(new FourthTest());
mfd.SetMethodSolvingSLAE(new GaussSeidel(1000, 1E-14, 1.22));
mfd.Compute();

foreach (var item in mfd.Weights)
    Console.WriteLine(item);