using eMF_1;

GridFactory gridFactory = new();
// MFD mfd = new(gridFactory.CreateGrid(GridType.Irregular, "grid/grid(irregular).txt"), "boundaries.txt");
MFD mfd = new(gridFactory.CreateGrid(GridType.Nested, "grid/grid(nested).txt"), "boundaries.txt");
// MFD mfd = new(gridFactory.CreateGrid(GridType.Regular, "grid/grid(regular).txt"), "boundaries.txt");
// mfd.SetTest(new Test());
// mfd.SetTest(new FirstTest());  // $x$, $\lambda = 1$, $\gamma = 0$
mfd.SetTest(new SecondTest()); // $x^2 - y$, $\lambda = 0.5$, $\gamma = 1$ 
// mfd.SetTest(new ThirdTest());  // $3x^3 + 2y^3$, 1 area: $\lambda = \gamma = 0.5 $, 2 area : $\lambda = \gamma = 2$
// mfd.SetTest(new FourthTest());    // $\ln{(x+y)}$, $\lambda = 1, \gamma = 1$
// mfd.SetTest(new FifthTest());  // $4x^4$, $\lambda = 1$, $\gamma = 0$
// mfd.SetTest(new SixthTest());  // $4x^4 + 2y^4$, $\lambda = 1$, $\gamma = 0$
// mfd.SetTest(new SeventhTest());// $e^x + y$, $\lambda = 1$, $\gamma = 1$
// mfd.SetTest(new EighthTest()); // $x^3 + y$, $\lambda = 1$, $\gamma = 0$
mfd.SetMethodSolvingSLAE(new GaussSeidel(1000, 1E-14, 1.22));
mfd.Compute();