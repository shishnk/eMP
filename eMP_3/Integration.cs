namespace eMP_3;

public static class Integration {
    public static double GaussSegment(Func<Point3D, double> psiI, Func<Point3D, double> psiJ, Point3D firstPoint, Point3D secondPoint) {
        var quadratures = Quadratures.SegmentGaussOrder9();

        double hx = secondPoint.X - firstPoint.X;
        double hy = secondPoint.Y - firstPoint.Y;
        double hz = secondPoint.Z - firstPoint.Z;
        double result = 0.0;

        foreach (var qi in quadratures) {
            foreach (var qj in quadratures) {
                foreach (var qk in quadratures) {
                    Point3D point = new(((qi.Node * hx) + firstPoint.X + secondPoint.X) / 2.0,
                                      ((qj.Node * hy) + firstPoint.Y + secondPoint.Y) / 2.0,
                                      ((qk.Node * hz) + firstPoint.Z + secondPoint.Z) / 2.0);

                    result += psiI(point) * psiJ(point) * qi.Weight * qj.Weight * qk.Weight;
                }
            }
        }

        return result * hx * hy * hz / 8.0;
    }

    public static double GaussSegmentGrad(Func<Point3D, double> psiI, Func<Point3D, double> psiJ, Point3D firstPoint, Point3D secondPoint) {
        var quadratures = Quadratures.SegmentGaussOrder9();

        double hx = secondPoint.X - firstPoint.X;
        double hy = secondPoint.Y - firstPoint.Y;
        double hz = secondPoint.Z - firstPoint.Z;
        double result = 0.0;

        foreach (var qi in quadratures) {
            foreach (var qj in quadratures) {
                foreach (var qk in quadratures) {
                    Point3D point = new(((qi.Node * hx) + firstPoint.X + secondPoint.X) / 2.0,
                                      ((qj.Node * hy) + firstPoint.Y + secondPoint.Y) / 2.0,
                                      ((qk.Node * hz) + firstPoint.Z + secondPoint.Z) / 2.0);

                    result += DerivativeX(point, hx) * qi.Weight * qj.Weight * qk.Weight;
                }
            }
        }

        result *= hx * hy * hz / 8.0;

        foreach (var qi in quadratures) {
            foreach (var qj in quadratures) {
                foreach (var qk in quadratures) {
                    Point3D point = new(((qi.Node * hx) + firstPoint.X + secondPoint.X) / 2.0,
                                      ((qj.Node * hy) + firstPoint.Y + secondPoint.Y) / 2.0,
                                      ((qk.Node * hz) + firstPoint.Z + secondPoint.Z) / 2.0);

                    result += DerivativeY(point, hy) * qi.Weight * qj.Weight * qk.Weight;
                }
            }
        }

        result *= hx * hy * hz / 8.0;

        foreach (var qi in quadratures) {
            foreach (var qj in quadratures) {
                foreach (var qk in quadratures) {
                    Point3D point = new(((qi.Node * hx) + firstPoint.X + secondPoint.X) / 2.0,
                                      ((qj.Node * hy) + firstPoint.Y + secondPoint.Y) / 2.0,
                                      ((qk.Node * hz) + firstPoint.Z + secondPoint.Z) / 2.0);

                    result += DerivativeZ(point, hz) * qi.Weight * qj.Weight * qk.Weight;
                }
            }
        }

        return result * hx * hy * hz / 8.0;

        double DerivativeX(Point3D point, double h)
            => (psiI(point + (h, 0, 0)) - psiI(point - (h, 0, 0))) / (2 * h) *
               ((psiJ(point + (h, 0, 0)) - psiJ(point - (h, 0, 0))) / (2 * h));

        double DerivativeY(Point3D point, double h)
           => (psiI(point + (0, h, 0)) - psiI(point - (0, h, 0))) / (2 * h) *
              ((psiJ(point + (0, h, 0)) - psiJ(point - (0, h, 0))) / (2 * h));

        double DerivativeZ(Point3D point, double h)
           => (psiI(point + (0, 0, h)) - psiI(point - (0, 0, h))) / (2 * h) *
              ((psiJ(point + (0, 0, h)) - psiJ(point - (0, 0, h))) / (2 * h));
    }
}