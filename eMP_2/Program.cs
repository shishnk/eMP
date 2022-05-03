using eMP_2;

GridFactorySpace gridFactorySpace = new();
GridFactoryTime gridFactoryTime = new();

IGrid spaceGrid = gridFactorySpace.CreateGrid(GridTypes.SpaceRegular, GridParameters.ReadJson("spaceGrid.json")!);
IGrid timeGrid = gridFactoryTime.CreateGrid(GridTypes.TimeRegular, GridParameters.ReadJson("timeGrid.json")!);

MFE mfe = MFE.CreateBuilder().SetSpaceGrid(spaceGrid).SetTimeGrid(timeGrid).SetTest(new Test2())
.SetDecomposer(new DecomposerLU()).SetMethod((NonLinearSolverTypes.Newton, 1000, 1E-14));
mfe.Compute();