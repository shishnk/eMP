﻿using eMP_2;

GridParameters? spaceGridParameters = GridParameters.ReadJson("spaceGrid.json");
GridParameters? timeGridParameters = GridParameters.ReadJson("timeGrid.json");

GridFactorySpace gridFactorySpace = new();
GridFactoryTime gridFactoryTime = new();

IGrid spaceGrid = gridFactorySpace.CreateGrid(GridTypes.SpaceRegular, spaceGridParameters!);
IGrid timeGrid = gridFactoryTime.CreateGrid(GridTypes.TimeRegular, timeGridParameters!);

MFE mfe = MFE.CreateBuilder().SetSpaceGrid(spaceGrid).SetTimeGrid(timeGrid).SetTest(new Test1())
.SetDecomposer(new DecomposerLU()).SetMethod((NonLinearSolverTypes.SimpleIteration, 1000, 1E-14));
mfe.Compute();