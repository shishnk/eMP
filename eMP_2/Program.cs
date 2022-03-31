using eMP_2;

GridParameters? spaceGridParameters = GridParameters.ReadJson("spaceGrid.json");
GridParameters? timeGridParameters = GridParameters.ReadJson("timeGrid.json");

GridFactorySpace gridFactorySpace = new();
GridFactoryTime gridFactoryTime = new();

IGrid spaceGrid = gridFactorySpace.CreateGrid(GridTypes.SpaceRegular, spaceGridParameters!);
IGrid timeGrid = gridFactoryTime.CreateGrid(GridTypes.TimeRegular, timeGridParameters!);

MFE mfe = MFE.CreateBuilder().SetSpaceGrid(spaceGrid).SetTimeGrid(timeGrid).SetTest(new FirstTest());
mfe.Compute();