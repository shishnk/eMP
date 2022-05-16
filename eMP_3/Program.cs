using eMP_3;

GridFactory gridFactory = new();
var grid = gridFactory.CreateGrid(GridTypes.Regular, GridParameters.ReadJson("grid.jsonc")!.Value);

FEM fem = FEM.CreateBuilder().SetSpaceGrid(grid).SetTest(new Test1()).SetSolverSLAE(new LOS(1000, 1E-14));
fem.Compute();