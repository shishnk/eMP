namespace eMP_3;

public class RegularGrid : Grid {
    private readonly Point3D[] _points = default!;
    private readonly int[][] _elements = default!;
    private readonly int[][] _sides = default!;
    public override ImmutableArray<Point3D> Points => _points.ToImmutableArray();
    public override ImmutableArray<ImmutableArray<int>> Elements => _elements.Select(item => item.ToImmutableArray()).ToImmutableArray();
    public override ImmutableArray<ImmutableArray<int>> Sides => _sides.Select(item => item.ToImmutableArray()).ToImmutableArray();

    public RegularGrid(GridParameters gridParameters) : base (gridParameters)  {
        _points = new Point3D[(gridParameters.SplitsX + 1) * (gridParameters.SplitsY + 1) * (gridParameters.SplitsZ + 1)];
        _elements = new double[gridParameters.SplitsX * gridParameters.SplitsY * gridParameters.SplitsZ].Select(_ => new int[8]).ToArray();
        _sides = new int[6][];
        _sides[0] = new int[(gridParameters.SplitsX + 1) * (gridParameters.SplitsZ + 1)]; // front
        _sides[1] = new int[(gridParameters.SplitsX + 1) * (gridParameters.SplitsZ + 1)]; // back
        _sides[2] = new int[(gridParameters.SplitsY + 1) * (gridParameters.SplitsZ + 1)]; // left
        _sides[3] = new int[(gridParameters.SplitsY + 1) * (gridParameters.SplitsZ + 1)]; // right
        _sides[4] = new int[(gridParameters.SplitsX + 1) * (gridParameters.SplitsY + 1)]; // bottom
        _sides[5] = new int[(gridParameters.SplitsX + 1) * (gridParameters.SplitsY + 1)]; // top
        Build(gridParameters);
    }

    private void Build(GridParameters gridParameters) {
        try {
            if (gridParameters.SplitsX < 1 || gridParameters.SplitsY < 1 || gridParameters.SplitsZ < 1) {
                throw new Exception("The number of splits must be greater than or equal to 1");
            }

            double hx = gridParameters.IntervalX.Lenght / gridParameters.SplitsX;
            double hy = gridParameters.IntervalY.Lenght / gridParameters.SplitsY;
            double hz = gridParameters.IntervalZ.Lenght / gridParameters.SplitsZ;

            double[] pointsX = new double[gridParameters.SplitsX + 1];
            double[] pointsY = new double[gridParameters.SplitsY + 1];
            double[] pointsZ = new double[gridParameters.SplitsZ + 1];

            pointsX[0] = gridParameters.IntervalX.LeftBorder;
            pointsY[0] = gridParameters.IntervalY.LeftBorder;
            pointsZ[0] = gridParameters.IntervalZ.LeftBorder;

            for (int i = 1; i < pointsX.Length; i++) {
                pointsX[i] = pointsX[i - 1] + hx;
            }

            for (int i = 1; i < pointsY.Length; i++) {
                pointsY[i] = pointsY[i - 1] + hy;
            }

            for (int i = 1; i < pointsZ.Length; i++) {
                pointsZ[i] = pointsZ[i - 1] + hz;
            }

            int index = 0;

            for (int k = 0; k < pointsZ.Length; k++) {
                for (int j = 0; j < pointsY.Length; j++) {
                    for (int i = 0; i < pointsX.Length; i++) {
                        _points[index++] = new(pointsX[i], pointsY[j], pointsZ[k]);
                    }
                }
            }

            // k по z, j по y, i по x

            int nx = pointsX.Length;
            int ny = pointsY.Length;
            int nz = pointsZ.Length;

            int Nx = pointsX.Length - 1;
            int Ny = pointsY.Length - 1;
            int Nz = pointsZ.Length - 1;

            index = 0;

            for (int k = 0; k < Nz; k++) {
                for (int j = 0; j < Ny; j++) {
                    for (int i = 0; i < Nx; i++) {
                        _elements[index][0] = i + (j * nx) + (k * nx * ny);
                        _elements[index][1] = i + 1 + (j * nx) + (k * nx * ny);
                        _elements[index][2] = i + ((j + 1) * nx) + (k * nx * ny);
                        _elements[index][3] = i + 1 + ((j + 1) * nx) + (k * nx * ny);
                        _elements[index][4] = i + (j * nx) + ((k + 1) * nx * ny);
                        _elements[index][5] = i + 1 + (j * nx) + ((k + 1) * nx * ny);
                        _elements[index][6] = i + ((j + 1) * nx) + ((k + 1) * nx * ny);
                        _elements[index++][7] = i + 1 + ((j + 1) * nx) + ((k + 1) * nx * ny);
                    }
                }
            }

            // front and back
            for (int k = 0; k < nz; k++) {
                for (int i = 0; i < nx; i++) {
                    _sides[0][i + (k * nx)] = i + 0 + (k * nx * ny);
                    _sides[1][i + (k * nx)] = i + (nx * (ny - 1)) + (k * nx * ny);
                }
            }

            // left and right
            for (int k = 0; k < nz; k++) {
                for (int j = 0; j < ny; j++) {
                    _sides[2][j + (k * ny)] = 0 + (j * nx) + (k * nx * ny);
                    _sides[3][j + (k * ny)] = nx - 1 + (j * nx) + (k * nx * ny);
                }
            }

            // bottom and top
            for (int j = 0; j < ny; j++) {
                for (int i = 0; i < nx; i++) {
                    _sides[4][i + (j * nx)] = i + (j * nx) + 0;
                    _sides[5][i + (j * nx)] = i + (j * nx) + (nx * ny * (nz - 1));
                }
            }

            // var res = _points.Except(_internalPoints);

            WritePoints();
        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }

    private void WritePoints() {
        var sw = new StreamWriter("points.txt");
        using (sw) {
            for (int i = 0; i < _points.Length; i++) {
                sw.WriteLine(_points[i]);
            }
        }
    }
}