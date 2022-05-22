namespace eMP_3;

public class IrregularGrid : Grid {
    private readonly Point3D[] _points = default!;
    private readonly int[][] _elements = default!;
    private readonly int[][] _sides = default!;
    public override ImmutableArray<Point3D> Points => _points.ToImmutableArray();
    public override ImmutableArray<ImmutableArray<int>> Elements => _elements.Select(item => item.ToImmutableArray()).ToImmutableArray();
    public override ImmutableArray<ImmutableArray<int>> Sides => _sides.Select(item => item.ToImmutableArray()).ToImmutableArray();

    public IrregularGrid(GridParameters gridParameters) : base(gridParameters) {
        _points = new Point3D[(gridParameters.SplitsX + 1) * (gridParameters.SplitsY + 1) * (gridParameters.SplitsZ + 1)];
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

            ArgumentNullException.ThrowIfNull(gridParameters.K, $"{nameof(gridParameters.K)} cannot be null");

            if (gridParameters.K <= 0) {
                throw new Exception("The coefficient must be greater than 0");
            }

            double[] pointsX = new double[gridParameters.SplitsX + 1];
            double[] pointsY = new double[gridParameters.SplitsY + 1];
            double[] pointsZ = new double[gridParameters.SplitsZ + 1];

            pointsX[0] = gridParameters.IntervalX.LeftBorder;
            pointsY[0] = gridParameters.IntervalY.LeftBorder;
            pointsZ[0] = gridParameters.IntervalZ.LeftBorder;

            double hx, hy, hz;
            double sum = 0.0;

            for (int k = 0; k < gridParameters.SplitsX; k++) {
                sum += Math.Pow(gridParameters.K.Value, k);
            }

            hx = gridParameters.IntervalX.Lenght / sum;
            sum = 0.0;

            for (int k = 0; k < gridParameters.SplitsY; k++) {
                sum += Math.Pow(gridParameters.K.Value, k);
            }

            hy = gridParameters.IntervalY.Lenght / sum;
            sum = 0.0;

            for (int k = 0; k < gridParameters.SplitsZ; k++) {
                sum += Math.Pow(gridParameters.K.Value, k);
            }

            hz = gridParameters.IntervalZ.Lenght / sum;

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

            for (int i = 0; i < pointsZ.Length; i++) {
                for (int j = 0; j < pointsY.Length; j++) {
                    for (int k = 0; k < pointsX.Length; k++) {
                        _points[index++] = new(pointsX[k], pointsY[j], pointsZ[i]);
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