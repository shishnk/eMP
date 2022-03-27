using eMP_2;

GridParameters? timeGridParameters;
GridParameters? spaceGridParameters;

try {
    var sr = new StreamReader("timeGrid.json");
    using (sr) {
        timeGridParameters = JsonConvert.DeserializeObject<GridParameters>(sr.ReadToEnd());
    }
    sr = new StreamReader("spaceGrid.json");
    using (sr) {
        spaceGridParameters = JsonConvert.DeserializeObject<GridParameters>(sr.ReadToEnd());
    }

    GridFactorySpace gridFactorySpace = new();
    GridFactoryTime gridFactoryTime = new();
    var grid1 =gridFactorySpace.CreateGrid(GridType.SpaceIrregular, spaceGridParameters!);
    var grid2 = gridFactoryTime.CreateGrid(GridType.TimeIrregular, timeGridParameters!);
    
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
}

Console.WriteLine();