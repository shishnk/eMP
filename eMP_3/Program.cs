using eMP_3;

GridFactory gridFactory = new();
var grid = gridFactory.CreateGrid(GridTypes.Regular, GridParameters.ReadJson("grid.jsonc")!.Value);

#region Итерационный метод
FEM fem1 = FEM.CreateBuilder().SetSpaceGrid(grid).SetTest(new Test1()).SetSolverSLAE(new BCGSTABLU(1000, 1E-14));
fem1.Compute();
#endregion

#region Прямой метод
FEM fem2 = FEM.CreateBuilder().SetSpaceGrid(grid).SetTest(new Test1()).SetDecomposer(new DecomposerLDU());
fem2.Compute();
#endregion